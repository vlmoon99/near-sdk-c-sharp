using System;
using System.Runtime.InteropServices;

namespace smartcontract
{
    //All sys imports must be changed after compilation from "-" to "_" but for now we need to do this manually
    public unsafe static class WasmImports
    {
        [WasmImportLinkage]
        [DllImport("env", EntryPoint = "input")]
        public static extern void Input(long registerId);

        [WasmImportLinkage]
        [DllImport("env", EntryPoint = "value-return")]
        public static extern void ValueReturn(long param1, long param2);

        [WasmImportLinkage]
        [DllImport("env", EntryPoint = "log-utf8")]
        public static extern void LogUtf8(long param1, long param2);
    }

    public unsafe static class SmartContract
    {
        private const long ATOMIC_OP_REGISTER = long.MaxValue - 2;

        [UnmanagedCallersOnly(EntryPoint = "returnvalue")]
        public unsafe static void RetunValue()
        {
            WasmImports.Input(ATOMIC_OP_REGISTER);
            
            Span<byte> utf8Bytes =
            [
                104, 101, 108, 108, 111, 32, 119, 111, 114, 108, 100 
            ];
            
            fixed (byte* ptr = utf8Bytes)
            {
                WasmImports.ValueReturn(11, (long)ptr);
            }
        }

        [UnmanagedCallersOnly(EntryPoint = "helloworld")]
        public unsafe static void HelloWorld()
        {
            WasmImports.Input(ATOMIC_OP_REGISTER);
            
            Span<byte> utf8Bytes =
            [
                104, 101, 108, 108, 111, 32, 119, 111, 114, 108, 100 
            ];
            
            fixed (byte* ptr = utf8Bytes)
            {
                WasmImports.LogUtf8(11, (long)ptr);
            }
        }

        [UnmanagedCallersOnly(EntryPoint = "returnvaluenative")]
        public unsafe static void RetunValueNative()
        {
            WasmImports.Input(ATOMIC_OP_REGISTER);
            
            byte* ptr = (byte*)NativeMemory.Alloc(11);
            
            ptr[0] = 104; ptr[1] = 101; ptr[2] = 108; ptr[3] = 108; ptr[4] = 111;
            ptr[5] = 32; ptr[6] = 119; ptr[7] = 111; ptr[8] = 114; ptr[9] = 108; ptr[10] = 100;
            
            WasmImports.ValueReturn(11, (long)ptr);
            
            NativeMemory.Free(ptr);
        }

        [UnmanagedCallersOnly(EntryPoint = "helloworldnative")]
        public unsafe static void HelloWorldNative()
        {
            WasmImports.Input(ATOMIC_OP_REGISTER);
            
            byte* ptr = (byte*)NativeMemory.Alloc(11);
            
            ptr[0] = 104; ptr[1] = 101; ptr[2] = 108; ptr[3] = 108; ptr[4] = 111;
            ptr[5] = 32; ptr[6] = 119; ptr[7] = 111; ptr[8] = 114; ptr[9] = 108; ptr[10] = 100;
            
            WasmImports.LogUtf8(11, (long)ptr);
            
            NativeMemory.Free(ptr);
        }
    }
}