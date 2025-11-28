using System;
using System.Runtime.InteropServices;

namespace ContractProof;

public unsafe static class SmartContract
{
    public const string owner = "vlmoon.near";

    [UnmanagedCallersOnly(EntryPoint = "returnowner")]
    public static void ReturnOwner() => NearSmartContractBuilder.ReturnMethod(owner);

    [UnmanagedCallersOnly(EntryPoint = "returnvalue")]
    public static void ReturnValue() => NearSmartContractBuilder.ReturnMethod("hello world");

    [UnmanagedCallersOnly(EntryPoint = "helloworld")]
    public static void HelloWorld() => NearSmartContractBuilder.LogMethod("hello world");

    [UnmanagedCallersOnly(EntryPoint = "returnvalueinput")]
    public static void ReturnValueInput()
    {
        NearSmartContractBuilder.Execute(() =>
        {
            string input = NearSmartContractBuilder.GetInputString();
            NearSmartContractBuilder.ReturnValue(input);
        });
    }

    [UnmanagedCallersOnly(EntryPoint = "loginput")]
    public static void LogInput()
    {
        NearSmartContractBuilder.Execute(() =>
        {
            string input = NearSmartContractBuilder.GetInputString();
            NearSmartContractBuilder.Log($"Received input: {input}");
        });
    }

    [UnmanagedCallersOnly(EntryPoint = "greet")]
    public static void Greet()
    {
        NearSmartContractBuilder.Execute(() =>
        {
            string input = NearSmartContractBuilder.GetInputString();
            string response = $"Hello, {input}!";
            NearSmartContractBuilder.ReturnValue(response);
        });
    }

    [UnmanagedCallersOnly(EntryPoint = "store")]
    public static void Store()
    {
        NearSmartContractBuilder.Execute(() =>
        {
            string input = NearSmartContractBuilder.GetInputString();
            NearSmartContractBuilder.StorageWrite("mykey", input);
            NearSmartContractBuilder.Log($"Stored: {input}");
        });
    }

    [UnmanagedCallersOnly(EntryPoint = "retrieve")]
    public static void Retrieve()
    {
        NearSmartContractBuilder.Execute(() =>
        {
            string value = NearSmartContractBuilder.StorageRead("mykey");
            NearSmartContractBuilder.ReturnValue(value);
        });
    }
}