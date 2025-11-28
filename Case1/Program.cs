using System;
using Wasmtime;

class Program
{
    static void Main()
    {
        using var engine = new Engine();
        using var module = Module.FromFile(engine, "wasm_case_1.wasm");
        using var linker = new Linker(engine);
        using var store = new Store(engine);

        linker.Define(
            "env",
            "write-something",
            Function.FromCallback<long, long, int>(store, (id, value) =>
            {
                Console.WriteLine($"[HOST] write-something called with id={id}, value={value}");
                return 1;
            })
        );

        linker.Define(
            "env",
            "read-something",
            Function.FromCallback<long, long>(store, (id) =>
            {
                Console.WriteLine($"[HOST] read-something called with id={id}");
                return 40 + id;
            })
        );

        var instance = linker.Instantiate(store, module);

        var readFunc = instance.GetFunction<long, long>("read");
        long result1 = readFunc(10);
        Console.WriteLine($"[HOST] WASM read(10) returned: {result1}");

        var writeFunc = instance.GetFunction<long, long, int>("write");
        int result2 = writeFunc(5, 777);
        Console.WriteLine($"[HOST] WASM write(5, 777) returned: {result2}");
    }
}
