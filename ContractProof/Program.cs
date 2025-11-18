
using System;
using System.Runtime.InteropServices;

namespace ContractProof;
public unsafe static class SmartContract
{
    [UnmanagedCallersOnly(EntryPoint = "returnvalue")]
    public static unsafe void RetunValue()
    {
        NearSystemImports.Input(NearBlockchainEnv.AtomicOpRegister);

        Span<byte> utf8Bytes =
        [
            104, 101, 108, 108, 111, 32, 119, 111, 114, 108, 100
        ];

        fixed (byte* ptr = utf8Bytes)
        {
            NearSystemImports.ValueReturn(utf8Bytes.Length, (long)ptr);
        }
    }

    [UnmanagedCallersOnly(EntryPoint = "helloworld")]
    public static unsafe void HelloWorld()
    {
        NearSystemImports.Input(NearBlockchainEnv.AtomicOpRegister);

        Span<byte> utf8Bytes =
        [
            104, 101, 108, 108, 111, 32, 119, 111, 114, 108, 100
        ];

        fixed (byte* ptr = utf8Bytes)
        {
            NearSystemImports.LogUtf8(utf8Bytes.Length, (long)ptr);
        }
    }
}
