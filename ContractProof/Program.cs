using System;
using System.Runtime.InteropServices;
using System.Text;

namespace ContractProof;

public unsafe static class SmartContract
{
    [UnmanagedCallersOnly(EntryPoint = "returnvalue")]
    public static unsafe void RetunValue()
    {
        var (data, type, error) = NearBlockchainEnv.ContractInputRaw();
        
        if (error != null)
        {
            NearBlockchainEnv.LogString($"Error reading input: {error.Message}");
            return;
        }

        string message = "hello world";
        byte[] utf8Bytes = Encoding.UTF8.GetBytes(message);

        fixed (byte* ptr = utf8Bytes)
        {
            NearSystemImports.ValueReturn(utf8Bytes.Length, (long)ptr);
        }
    }

    [UnmanagedCallersOnly(EntryPoint = "helloworld")]
    public static unsafe void HelloWorld()
    {
        var (data, type, error) = NearBlockchainEnv.ContractInputRaw();
        
        if (error != null)
        {
            NearBlockchainEnv.LogString($"Error reading input: {error.Message}");
            return;
        }

        string message = "hello world";
        byte[] utf8Bytes = Encoding.UTF8.GetBytes(message);

        fixed (byte* ptr = utf8Bytes)
        {
            NearSystemImports.LogUtf8(utf8Bytes.Length, (long)ptr);
        }
    }
}