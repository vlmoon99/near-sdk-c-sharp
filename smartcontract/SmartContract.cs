using System;
using System.Runtime.InteropServices;
namespace smartcontract
{

    //All sys mports must be changed after compilation from "-" to "_" but for now we need to do this mannualy
    public unsafe static class WasmImports
    {
        [WasmImportLinkage]
        [DllImport("env", EntryPoint = "storage-read")]
        public static extern long StorageRead(long param1, long param2, long param3);

        [WasmImportLinkage]
        [DllImport("env", EntryPoint = "storage-write")]
        public static extern long StorageWrite(long param1, long param2, long param3, long param4, long param5);

        [WasmImportLinkage]
        [DllImport("env", EntryPoint = "storage-has_key")]
        public static extern long StorageHasKey(long param1, long param2);

        [WasmImportLinkage]
        [DllImport("env", EntryPoint = "read-register")]
        public static extern void ReadRegister(long param1, long param2);

        [WasmImportLinkage]
        [DllImport("env", EntryPoint = "value-return")]
        public static extern long ValueReturn(long param1, long param2);

        [WasmImportLinkage]
        [DllImport("env", EntryPoint = "log-utf8")]
        public static extern long LogUtf8(long param1, long param2);
    }

    public unsafe static class SmartContract
    {
        
        [UnmanagedCallersOnly(EntryPoint = "helloworld")]
        public unsafe static void HelloWorld()
        {
            Log();
        }

        public unsafe static void Log()
        {
            byte[] utf8Bytes = [104, 101, 108, 108, 111, 32, 119, 111, 114, 108, 100];
            unsafe
            {
                fixed (byte* ptr = utf8Bytes)
                {
                    int length = 11;
                    WasmImports.LogUtf8(length, (long)ptr);
                }
            }
        }
    }

}
