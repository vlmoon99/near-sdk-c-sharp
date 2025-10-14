using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using LLVMSharp.Interop;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Runtime.InteropServices;

// === Attributes ===
[AttributeUsage(AttributeTargets.Method)]
public class WasmExportAttribute : Attribute { public string Name { get; set; } = ""; }
[AttributeUsage(AttributeTargets.Method)]
public class WasmImportLinkageAttribute : Attribute { }
[AttributeUsage(AttributeTargets.Method)]
public class WasmImportAttribute : Attribute
{
    public string Module { get; set; } = "env";
    public string Name { get; set; } = "";
}

// === Models ===
public record ImportedMethod(string CSharpName, string Module, string Name, ITypeSymbol ReturnType, IList<IParameterSymbol> Parameters);
public record ExportedMethod(string CSharpName, string Name, ITypeSymbol ReturnType, IList<IParameterSymbol> Parameters, BlockSyntax Body, SemanticModel SemanticModel);

// === Roslyn Walker ===
public class WasmCollector : CSharpSyntaxWalker
{
    private readonly SemanticModel _semantic;
    public List<ImportedMethod> Imports { get; } = new();
    public List<ExportedMethod> Exports { get; } = new();
    public List<string> Errors { get; } = new();

    public WasmCollector(SemanticModel semantic) => _semantic = semantic;

    public override void VisitMethodDeclaration(MethodDeclarationSyntax node)
    {
        var sym = _semantic.GetDeclaredSymbol(node);
        if (sym == null) { Errors.Add(node.Identifier.Text); return; }

        // --- Imports ---
        var importAttr = sym.GetAttributes().FirstOrDefault(a =>
            a.AttributeClass?.Name == nameof(WasmImportLinkageAttribute) ||
            a.AttributeClass?.Name == "WasmImport"
        );
        if (importAttr != null)
        {
            string module = "env";
            string name = sym.Name;
            if (importAttr.AttributeClass?.Name == "WasmImport")
            {
                module = importAttr.NamedArguments.FirstOrDefault(na => na.Key == "Module").Value.Value?.ToString() ?? "env";
                name = importAttr.NamedArguments.FirstOrDefault(na => na.Key == "Name").Value.Value?.ToString() ?? sym.Name;
            }
            // Convert '-' to '_'
            name = name.Replace('-', '_');
            Imports.Add(new ImportedMethod(sym.Name, module, name, sym.ReturnType, sym.Parameters));
        }

        // --- Exports ---
        var exportAttr = sym.GetAttributes().FirstOrDefault(a =>
            a.AttributeClass?.Name == nameof(WasmExportAttribute) ||
            a.AttributeClass?.Name == "UnmanagedCallersOnlyAttribute"
        );
        if (exportAttr != null)
        {
            string name = exportAttr.NamedArguments.FirstOrDefault(na => na.Key == "EntryPoint").Value.Value?.ToString()
                          ?? exportAttr.NamedArguments.FirstOrDefault(na => na.Key == "Name").Value.Value?.ToString()
                          ?? sym.Name;

            BlockSyntax body;
            if (node.Body != null) body = node.Body;
            else if (node.ExpressionBody != null)
            {
                body = SyntaxFactory.Block(
                    sym.ReturnType.SpecialType == SpecialType.System_Void
                    ? SyntaxFactory.ExpressionStatement(node.ExpressionBody.Expression)
                    : SyntaxFactory.ReturnStatement(node.ExpressionBody.Expression)
                );
            }
            else
            {
                Errors.Add($"Exported method {sym.Name} has no body");
                return;
            }

            Exports.Add(new ExportedMethod(sym.Name, name, sym.ReturnType, sym.Parameters, body, _semantic));
        }

        base.VisitMethodDeclaration(node);
    }
}

// === Type Mapper ===
public class TypeMapper
{
    public LLVMTypeRef Map(ITypeSymbol t) => t.SpecialType switch
    {
        SpecialType.System_Void => LLVMTypeRef.Void,
        SpecialType.System_Boolean => LLVMTypeRef.Int1,
        SpecialType.System_SByte or SpecialType.System_Byte => LLVMTypeRef.Int8,
        SpecialType.System_Int16 or SpecialType.System_UInt16 => LLVMTypeRef.Int16,
        SpecialType.System_Int32 or SpecialType.System_UInt32 => LLVMTypeRef.Int32,
        SpecialType.System_Int64 or SpecialType.System_UInt64 => LLVMTypeRef.Int64,
        SpecialType.System_Single => LLVMTypeRef.Float,
        SpecialType.System_Double => LLVMTypeRef.Double,
        _ => throw new NotSupportedException(t.ToDisplayString())
    };
}

// === Expression Generator ===
public class ExpressionGenerator
{
    private readonly LLVMBuilderRef _builder;
    private readonly TypeMapper _mapper;
    private readonly SemanticModel _semantic;
    private readonly Dictionary<string, LLVMValueRef> _locals;
    private readonly Dictionary<string, LLVMValueRef> _parameters;
    private readonly Dictionary<string, LLVMValueRef> _functions;

    public ExpressionGenerator(LLVMBuilderRef builder, TypeMapper mapper, SemanticModel semantic,
        Dictionary<string, LLVMValueRef> locals, Dictionary<string, LLVMValueRef> parameters,
        Dictionary<string, LLVMValueRef> functions)
    {
        _builder = builder; _mapper = mapper; _semantic = semantic;
        _locals = locals; _parameters = parameters; _functions = functions;
    }

    public LLVMValueRef Generate(ExpressionSyntax expr) => expr switch
    {
        LiteralExpressionSyntax lit => GenerateLiteral(lit),
        BinaryExpressionSyntax bin => GenerateBinary(bin),
        IdentifierNameSyntax id => _parameters.TryGetValue(id.Identifier.Text, out var p) ? p : _builder.BuildLoad2(_locals[id.Identifier.Text].TypeOf.ElementType, _locals[id.Identifier.Text], id.Identifier.Text),
        InvocationExpressionSyntax inv => GenerateInvocation(inv),
        _ => throw new NotSupportedException(expr.GetType().Name)
    };

    private LLVMValueRef GenerateLiteral(LiteralExpressionSyntax lit)
    {
        var type = _mapper.Map(_semantic.GetTypeInfo(lit).Type!);
        if (lit.Token.Value is int i) return LLVMValueRef.CreateConstInt(type, (ulong)i, i < 0);
        if (lit.Token.Value is long l) return LLVMValueRef.CreateConstInt(type, (ulong)l, l < 0);
        if (lit.Token.Value is float f) return LLVMValueRef.CreateConstReal(type, f);
        if (lit.Token.Value is double d) return LLVMValueRef.CreateConstReal(type, d);
        if (lit.IsKind(SyntaxKind.TrueLiteralExpression)) return LLVMValueRef.CreateConstInt(LLVMTypeRef.Int1, 1, false);
        if (lit.IsKind(SyntaxKind.FalseLiteralExpression)) return LLVMValueRef.CreateConstInt(LLVMTypeRef.Int1, 0, false);
        throw new NotSupportedException();
    }

    private LLVMValueRef GenerateBinary(BinaryExpressionSyntax bin)
    {
        var l = Generate(bin.Left);
        var r = Generate(bin.Right);
        return bin.Kind() switch
        {
            SyntaxKind.AddExpression => _builder.BuildAdd(l, r, "add"),
            SyntaxKind.SubtractExpression => _builder.BuildSub(l, r, "sub"),
            SyntaxKind.MultiplyExpression => _builder.BuildMul(l, r, "mul"),
            SyntaxKind.DivideExpression => _builder.BuildSDiv(l, r, "div"),
            SyntaxKind.EqualsExpression => _builder.BuildICmp(LLVMIntPredicate.LLVMIntEQ, l, r, "eq"),
            SyntaxKind.NotEqualsExpression => _builder.BuildICmp(LLVMIntPredicate.LLVMIntNE, l, r, "ne"),
            SyntaxKind.LessThanExpression => _builder.BuildICmp(LLVMIntPredicate.LLVMIntSLT, l, r, "lt"),
            SyntaxKind.LessThanOrEqualExpression => _builder.BuildICmp(LLVMIntPredicate.LLVMIntSLE, l, r, "le"),
            SyntaxKind.GreaterThanExpression => _builder.BuildICmp(LLVMIntPredicate.LLVMIntSGT, l, r, "gt"),
            SyntaxKind.GreaterThanOrEqualExpression => _builder.BuildICmp(LLVMIntPredicate.LLVMIntSGE, l, r, "ge"),
            _ => throw new NotSupportedException(bin.Kind().ToString())
        };
    }

    private LLVMValueRef GenerateInvocation(InvocationExpressionSyntax inv)
    {
        var sym = _semantic.GetSymbolInfo(inv).Symbol as IMethodSymbol;
        if (sym == null) throw new Exception("Cannot resolve symbol");
        var func = _functions[sym.Name];
        var args = inv.ArgumentList.Arguments.Select(a => Generate(a.Expression)).ToArray();
        return _builder.BuildCall2(func.TypeOf, func, args, "");
    }
}

// === Statement Generator ===
public class StatementGenerator
{
    private readonly LLVMBuilderRef _builder;
    private readonly ExpressionGenerator _exprGen;
    private readonly TypeMapper _mapper;
    private readonly SemanticModel _semantic;
    private readonly Dictionary<string, LLVMValueRef> _locals;

    public StatementGenerator(LLVMBuilderRef builder, ExpressionGenerator exprGen, TypeMapper mapper,
        SemanticModel semantic, Dictionary<string, LLVMValueRef> locals)
    {
        _builder = builder; _exprGen = exprGen; _mapper = mapper; _semantic = semantic; _locals = locals;
    }

    public void Generate(StatementSyntax stmt)
    {
        switch (stmt)
        {
            case ReturnStatementSyntax ret:
                if (ret.Expression != null) _builder.BuildRet(_exprGen.Generate(ret.Expression));
                else _builder.BuildRetVoid();
                break;
            case ExpressionStatementSyntax expr: _exprGen.Generate(expr.Expression); break;
            case LocalDeclarationStatementSyntax local:
                foreach (var v in local.Declaration.Variables)
                {
                    var sym = _semantic.GetDeclaredSymbol(v) as ILocalSymbol;
                    var type = _mapper.Map(sym!.Type);
                    var alloca = _builder.BuildAlloca(type, v.Identifier.Text);
                    _locals[v.Identifier.Text] = alloca;
                    if (v.Initializer != null) _builder.BuildStore(_exprGen.Generate(v.Initializer.Value), alloca);
                }
                break;
            default: throw new NotSupportedException(stmt.GetType().Name);
        }
    }
}

// === Compiler ===
public class WasmCompiler
{
    private readonly TypeMapper _mapper = new();
    private readonly Dictionary<string, LLVMValueRef> _functions = new();

    public void Compile(string csFile, string outputLl)
    {
        var code = File.ReadAllText(csFile);
        var tree = CSharpSyntaxTree.ParseText(code);
        var comp = CSharpCompilation.Create("WasmModule")
            .AddReferences(
                MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(DllImportAttribute).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(UnmanagedCallersOnlyAttribute).Assembly.Location)
            )
            .AddSyntaxTrees(tree);

        var model = comp.GetSemanticModel(tree);
        var collector = new WasmCollector(model);
        collector.Visit(tree.GetRoot());

        if (collector.Errors.Any()) { collector.Errors.ForEach(Console.WriteLine); return; }
        Console.WriteLine($"Imports: {collector.Imports.Count}, Exports: {collector.Exports.Count}");

        LLVM.InitializeWebAssemblyTarget();
        LLVM.InitializeWebAssemblyTargetInfo();
        LLVM.InitializeWebAssemblyTargetMC();
        LLVM.InitializeWebAssemblyAsmPrinter();
        LLVM.InitializeWebAssemblyAsmParser();

        var module = LLVMModuleRef.CreateWithName("wasm_module");
        module.Target = "wasm32-unknown-unknown";
        module.DataLayout = "e-m:e-p:32:32-i64:64-n32:64-S128";
        var builder = module.Context.CreateBuilder();

        // --- Imports ---
        foreach (var import in collector.Imports)
        {
            var ret = _mapper.Map(import.ReturnType);
            var args = import.Parameters.Select(p => _mapper.Map(p.Type)).ToArray();
            var func = module.AddFunction(import.Name, LLVMTypeRef.CreateFunction(ret, args, false));
            func.Linkage = LLVMLinkage.LLVMExternalLinkage;
            _functions[import.CSharpName] = func;
        }

        // --- Exports ---
        foreach (var export in collector.Exports) GenerateFunction(module, builder, export);

        if (!module.TryVerify(LLVMVerifierFailureAction.LLVMPrintMessageAction, out var err))
            Console.WriteLine($"LLVM Verify Failed: {err}");

        module.PrintToFile(outputLl);
        Console.WriteLine($"LLVM IR saved: {outputLl}");

        builder.Dispose();
        module.Dispose();
    }

    private void GenerateFunction(LLVMModuleRef module, LLVMBuilderRef builder, ExportedMethod export)
    {
        var retType = _mapper.Map(export.ReturnType);
        var paramTypes = export.Parameters.Select(p => _mapper.Map(p.Type)).ToArray();
        var func = module.AddFunction(export.Name, LLVMTypeRef.CreateFunction(retType, paramTypes, false));
        func.Linkage = LLVMLinkage.LLVMExternalLinkage;
        _functions[export.CSharpName] = func;

        var entry = func.AppendBasicBlock("entry");
        builder.PositionAtEnd(entry);

        var parameters = new Dictionary<string, LLVMValueRef>();
        for (uint i = 0; i < export.Parameters.Count; i++)
        {
            var p = func.GetParam(i);
            p.Name = export.Parameters[(int)i].Name;
            parameters[p.Name] = p;
        }

        var locals = new Dictionary<string, LLVMValueRef>();
        var exprGen = new ExpressionGenerator(builder, _mapper, export.SemanticModel, locals, parameters, _functions);
        var stmtGen = new StatementGenerator(builder, exprGen, _mapper, export.SemanticModel, locals);

        foreach (var stmt in export.Body.Statements) stmtGen.Generate(stmt);

        if (!export.Body.Statements.Any() || export.Body.Statements.Last() is not ReturnStatementSyntax)
            if (export.ReturnType.SpecialType == SpecialType.System_Void)
                builder.BuildRetVoid();
    }
}

// === Main ===
class ProgramMain
{
    static void Main()
    {
        var compiler = new WasmCompiler();
        compiler.Compile("SmartContract.csn", "output.ll");
        Console.WriteLine("Next step: compile output.ll to WASM with llc + wasm-ld.");
    }
}
