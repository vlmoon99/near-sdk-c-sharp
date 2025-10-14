// using System;
// using System.Text;
// using System.IO;
// using LLVMSharp.Interop;

// public class SimpleExample
// {
//     public static unsafe void Main()
//     {
//         LLVM.InitializeWebAssemblyTarget();
//         LLVM.InitializeWebAssemblyTargetInfo();
//         LLVM.InitializeWebAssemblyTargetMC();
//         LLVM.InitializeWebAssemblyAsmPrinter();
//         LLVM.InitializeWebAssemblyAsmParser();

//         var module = LLVMModuleRef.CreateWithName("simple_module");

//         // Set WebAssembly target
//         module.Target = "wasm32-unknown-unknown";
//         module.DataLayout = "e-m:e-p:32:32-i64:64-n32:64-S128";

//         // Use the module's context for all types
//         var context = module.Context;
//         var i32Type = LLVMTypeRef.Int32;
//         var i64Type = LLVMTypeRef.Int64;
//         var voidType = LLVMTypeRef.Void;

//         // Declare the imported log_utf8 function from "env" module
//         // log_utf8(length: i64, ptr: i64) -> void
//         var logUtf8Type = LLVMTypeRef.CreateFunction(voidType, new[] { i64Type, i64Type }, false);
//         var logUtf8Func = module.AddFunction("log_utf8", logUtf8Type);
        
//         // Mark as imported from "env" module
//         logUtf8Func.Linkage = LLVMLinkage.LLVMExternalLinkage;

//         // Create our exported "helloworld" function
//         var helloWorldType = LLVMTypeRef.CreateFunction(voidType, Array.Empty<LLVMTypeRef>(), false);
//         var helloWorldFunc = module.AddFunction("helloworld", helloWorldType);
//         helloWorldFunc.Linkage = LLVMLinkage.LLVMExternalLinkage;

//         var entry = helloWorldFunc.AppendBasicBlock("entry");
//         var builder = context.CreateBuilder();
//         builder.PositionAtEnd(entry);

//         // Create the "hello world" string as a global constant
//         var helloString = "hello world";
//         var helloBytes = Encoding.UTF8.GetBytes(helloString);
        
//         // Create array type for the string
//         var arrayType = LLVMTypeRef.CreateArray(LLVMTypeRef.Int8, (uint)helloBytes.Length);
        
//         // Create constant array with the string bytes
//         var stringConstants = new LLVMValueRef[helloBytes.Length];
//         for (var i = 0; i < helloBytes.Length; i++)
//         {
//             stringConstants[i] = LLVMValueRef.CreateConstInt(LLVMTypeRef.Int8, helloBytes[i], false);
//         }
//         var constArray = LLVMValueRef.CreateConstArray(LLVMTypeRef.Int8, stringConstants);
        
//         // Create global variable for the string
//         var globalString = module.AddGlobal(arrayType, ".str");
//         globalString.Initializer = constArray;
//         globalString.Linkage = LLVMLinkage.LLVMPrivateLinkage;
//         globalString.IsGlobalConstant = true;
//         LLVM.SetUnnamedAddress(globalString, LLVMUnnamedAddr.LLVMGlobalUnnamedAddr);

//         // Get pointer to the string (GEP with two zero indices)
//         var zero = LLVMValueRef.CreateConstInt(i32Type, 0, false);
//         var stringPtr = builder.BuildInBoundsGEP2(
//             arrayType,
//             globalString,
//             new[] { zero, zero },
//             "str_ptr"
//         );

//         // Convert pointer to i64 for WebAssembly
//         var ptrAsI64 = builder.BuildPtrToInt(stringPtr, i64Type, "ptr_as_i64");
        
//         // Create length as i64
//         var length = LLVMValueRef.CreateConstInt(i64Type, (ulong)helloBytes.Length, false);

//         // Call log_utf8(length, ptr)
//         _ = builder.BuildCall2(
//             logUtf8Type,
//             logUtf8Func,
//             new[] { length, ptrAsI64 },
//             ""
//         );

//         // Return void
//         _ = builder.BuildRetVoid();

//         // Verify module
//         if (!module.TryVerify(LLVMVerifierFailureAction.LLVMPrintMessageAction, out var error))
//         {
//             Console.WriteLine($"Module verification failed: {error}");
//             return;
//         }

//         // Compute project root
//         var projectRoot = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "./"));
//         var llPath = Path.Combine(projectRoot, "output.ll");
//         var bcPath = Path.Combine(projectRoot, "output.bc");

//         // Write LLVM IR
//         try
//         {
//             module.PrintToFile(llPath);
//             Console.WriteLine($"LLVM IR written to: {llPath}");
            
//             // Also print to console for inspection
//             Console.WriteLine("\n=== Generated LLVM IR ===");
//             Console.WriteLine(module.PrintToString());
//         }
//         catch (Exception ex)
//         {
//             Console.WriteLine($"Error writing LLVM IR (.ll) file! {ex.Message}");
//         }

//         // Write bitcode
//         try
//         {
//             var result = module.WriteBitcodeToFile(bcPath);
//             Console.WriteLine($"LLVM bitcode written to: {bcPath} with result {result}");
//         }
//         catch (Exception ex)
//         {
//             Console.WriteLine($"Error writing bitcode: {ex.Message}");
//         }

//         // Cleanup
//         builder.Dispose();
//         module.Dispose();

//         Console.WriteLine("\nLLVM module 'simple_module' created successfully.");
//         Console.WriteLine("The 'helloworld' function should now be visible to the blockchain WASM machine.");
//         Console.WriteLine("It imports 'log_utf8' from the 'env' module as expected.");
//     }
// }