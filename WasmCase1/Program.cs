using System;
using System.Runtime.InteropServices;

namespace case1
{
    public unsafe static class WasmImports
    {
        [WasmImportLinkage]
        [DllImport("env", EntryPoint = "write-something")]
        public static extern bool WriteSomethingToTheHost(long id, long value);

        [WasmImportLinkage]
        [DllImport("env", EntryPoint = "read-something")]
        public static extern long ReadSomethingFromTheHost(long id);
    }

    public unsafe static class ExportFunctions
    {
        [UnmanagedCallersOnly(EntryPoint = "read")]
        public static long Read(long id)
        {
            long hostValue = WasmImports.ReadSomethingFromTheHost(id);

            long processed = hostValue * 2 + 10;

            return processed;
        }

        [UnmanagedCallersOnly(EntryPoint = "write")]
        public static bool Write(long id, long value)
        {
            long newValue = value + 123;

            bool result = WasmImports.WriteSomethingToTheHost(id, newValue);

            return result;
        }
    }
}
