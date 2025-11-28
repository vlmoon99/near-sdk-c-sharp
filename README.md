# Building NEAR Smart Contracts with C# and .NET

This guide walks you through creating, compiling, and deploying smart contracts on the NEAR blockchain using C# and .NET 10.

## Prerequisites

Before starting, install these tools:

1. **Ubuntu x86/64 or ARM** (native or WSL)
2. **[.NET 10 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/10.0)**
3. **[WASI SDK 27](https://github.com/WebAssembly/wasi-sdk/releases)** - Download and extract (you'll use `clang++` from here for .NET to WASI compilation)
4. **wasm-tools & wat2wasm** - Install via Rust/Cargo or find pre-built binaries
5. **[NEAR CLI](https://docs.near.org/tools/near-cli#installation)**

## Steps

### 1. Create Empty Project

```bash
dotnet new console -n ContractProof
```

### 2. Configure csproj File

#### 2.1 Set Output Type

Change from:
```xml
<OutputType>Exe</OutputType>
```

to:
```xml
<OutputType>library</OutputType>
```

#### 2.2 Add Necessary Props for Small Size WASM Output

```xml
    <AllowUnsafeBlocks>true</AllowUnsafeBlocks>
    <DebugType>none</DebugType>
    <InvariantGlobalization>true</InvariantGlobalization>
    <StackTraceSupport>false</StackTraceSupport>
    <OptimizationPreference>Size</OptimizationPreference>
    <EnableTrimAnalyzer>true</EnableTrimAnalyzer>
    <ILLinkTreatWarningsAsErrors>false</ILLinkTreatWarningsAsErrors>
    <TrimmerRemoveSymbols>true</TrimmerRemoveSymbols>
    <DebuggerSupport>false</DebuggerSupport>
    <PublishTrimmed>true</PublishTrimmed>
    <SelfContained>true</SelfContained>
    <IlcGenerateMstatFile>true</IlcGenerateMstatFile>
    <IlcGenerateDgmlFile>true</IlcGenerateDgmlFile>
```

#### 2.3 Add Native AOT LLVM Compiler to the Project

```xml
  <ItemGroup>
    <PackageReference Include="Microsoft.DotNet.ILCompiler.LLVM" Version="10.0.0-*" />
    <PackageReference Include="runtime.$(NETCoreSdkPortableRuntimeIdentifier).Microsoft.DotNet.ILCompiler.LLVM" Version="10.0.0-*" />
  </ItemGroup>
```

#### 2.4 Add WASI Stubs

Add WASI stubs for the build system. If we don't have clean env imports in our WASM file, this file cannot be deployed on the NEAR Blockchain. Only NEAR Blockchain env imports can be in the WASM file. You can also use this tactic in other fields when you need to have clean env functions. Here we're defining our WIT (WASI Interface Type) for strict import and export for our clean WASM compilation.

```xml
  <ItemGroup>
    <NativeLibrary Include="stubs/stubs.o" />
    <CustomLinkerArg Include="-Wl,--component-type $(MSBuildProjectDirectory)/wasi_interfaces_type/smartcontract.wit" />
  </ItemGroup>
```

#### 2.5 Add nuget.config with Next Properties

```xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <packageSources>
    <clear />
    <add key="nuget" value="https://api.nuget.org/v3/index.json" />
    <add key="dotnet-experimental" value="https://pkgs.dev.azure.com/dnceng/public/_packaging/dotnet-experimental/nuget/v3/index.json" />
  </packageSources>
</configuration>
```


This was the initial setup of the project. In the future we will return to our csproj file for future optimizations of WASI build.

### 3. Write Smart Contract

We'll write a Smart Contract which we will later compile to the clean WASM target. If you're new to blockchain technology and smart contract development, I advise reading a bit about it on the NEAR Blockchain docs website:
- [Basics](https://docs.near.org/protocol/basics)
- [Smart Contract Development](https://docs.near.org/smart-contracts/what-is)

I'll try to make it simple with these few sentences for full understanding of what we'll write next:

#### 3.1 Smart Contract Basics

Each Smart Contract represents code which is stored on the blockchain nodes and other users can execute this code.

#### 3.2 Native Functions

Each Smart Contract has some sort of native functions (like native functions such as File System, Networking, Time, Cryptography on Linux, Windows, Mac OS) and you can use these blockchain native functions inside smart contract.

#### 3.3 What Blockchain Can Do Inside Smart Contract Functions

- Write/read data
- Call native blockchain cryptography functions
- Call other smart contracts
- Get block time
- Get tx information (who signs tx, who will receive it, how much gas user has to spend, how much deposit user attaches to the tx, what the input of the user)

### 4. Gas and Storage Costs

Each tx on a chain costs money (NEAR Tokens). In one word it's called Gas, which means how much computational resources blockchain node spends in order to execute your functions. Each data which is stored also costs tokens, which is 100kb per NEAR token. So taking this into account we need to write our functions small, efficient, for cheapest and fastest transaction.

After a little theoretical information we are able to get into writing our first smart contract code.

```csharp
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
```

### 5. Smart Contract Structure

#### 5.1 Basic Classes

These are the classes on top of which all things work in smart contract building, which include 3 levels:

```csharp
class NearSmartContractBuilder {} // Helpful class for building smart contract, level 3 in this example
class NearBlockchainEnv {}        // Implemented environment methods of the NEAR Blockchain, based on NearSystemImports
class NearSystemImports {}        // NEAR Blockchain System Method mappings, get system methods of the NEAR Blockchain which we can call inside our Smart Contract
```

#### 5.2 Smart Contract Structure

```csharp
public unsafe static class SmartContract
{
    public const string owner = "vlmoon.near";
    // ...other smart contract structure fields
    // Each field costs some storage to store, more you want to store the more you need to pay for your smart contract
}
```

#### 5.3 Smart Contract Functions

On chain we have 2 types of functions:

**View (read any data) - it's free for every client:**

```csharp
public unsafe static class SmartContract
{
    public const string owner = "vlmoon.near";
    
    [UnmanagedCallersOnly(EntryPoint = "returnowner")]
    public static void ReturnOwner() => NearSmartContractBuilder.ReturnMethod(owner);
}
```

**Write operation (any modifications of the Smart Contract storage)** which must be paid some sort of gas for the transaction plus for the storage which you will occupy. Some smart contracts can pay for the user, some smart contracts require attaching some sort of deposit in order to store something on a chain. In case when you delete something from smart contract, you will be able to withdraw your deposit token for storage from the smart contract.

```csharp
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
```

### 6. Build Code into Clean wasm-32-unknown-unknown Target

#### 6.1 Make Our Build Script Executable

```bash
chmod +x ./scripts/build_smart_contract.sh
```

#### 6.2 Run Build Script

```bash
./scripts/build_smart_contract.sh
```

#### 6.3 Inside Build Script

We have a few things which are related to our blockchain use case. If you're trying to create clean WASM you don't need to follow this script 1 to 1. In this script we unbundle our WASI module, take main module with code, change "-" to "_" in imports (it's the WASI limitation we can create env imports with "_"), transform it back from WAT to WASM and voilà, our clean WASM file is ready. We have our contract.wasm file which is ready to deploy on the chain.

### 7. Deploy Smart Contract

Before we're gonna deploy our smart contract on chain we must know a few things. For now NEAR blockchain on the mainnet and testnet does not support bulk memory WASM feature. It's the reason why right after deploying this smart contract.wasm on chain you will have a "Deserialization" error. It will be fixed in new releases very soon(1+ month), and for now for testing the idea of C# on a chain we will use localnet network (run NEAR Blockchain local node). In order to do it, start with the next steps:

#### 7.1 Install Rust

#### 7.2 Check Your RAM Availability

If less than 64GB, create swap 64GB, because on the last steps of building the NEAR Blockchain node you will have an error.

#### 7.3 Clone NEAR Core

```bash
git clone https://github.com/near/nearcore
```

#### 7.4 Build Debug or Release

```bash
cargo build
```

or

```bash
cargo build --release
```

#### 7.5 Run NEAR Node

Go to the bin folder and find neard bin:

```bash
cd target/debug/
```

This will init all necessary configs in order to run node locally:

```bash
./neard init
```

This command will run our local node. After we run our node for now we need just to check that all things run and we can shut it down for now, because we need to create some configuration:

```bash
./neard run
```

As we already downloaded and installed NEAR CLI in Prerequisites, so we have it installed and we can create our configurations.

We need to setup our CLI in order to be able to connect to our local node:

```bash
cd ~/.config/near-cli
```

```bash
nano config.toml
```

Add this, or if there will be some config, change it to your localhost - http://127.0.0.1:3030:

```toml
[network_connection.localnet]
network_name = "localnet"
rpc_url = "http://127.0.0.1:3030"
wallet_url = "https://app.mynearwallet.com/"
explorer_transaction_url = "https://explorer.near.org/transactions/"
```

#### 7.6 Create Keys for Smart Contract Manipulation

Create keys for smart contract manipulation (deploy, call functions, etc):

```bash
cd ~/.near/
```

In this folder we will have the file validator_key.json.

We will need to copy all data in new file called test.near.json plus we need to add a new key "private_key" with value of "secret_key". It will be necessary for CLI in order to sign tx. (You can find an example in ContractProof/near_creds/test.near.json, but there are keys from my local node, please take it into account). After we create this file we can transfer it to our smartcontract/test-wasm folder (take into account that there are already some keys with the same names - you will need to delete them before moving your keys).

#### 7.7 Run Our Local Node Again

```bash
./neard run
```

#### 7.8 Deploy Our Smart Contract on the Localchain

```bash
chmod +x ./scripts/deploy_smart_contract.sh
```

```bash
./scripts/deploy_smart_contract.sh
```

### 8. Test Smart Contract Using NEAR CLI

Make our test executable. Please make sure you have NEAR CLI installed and test.near.json keys were generated properly with secretKey key where your private key will be.

```bash
chmod +x ./scripts/test_smart_contract_functions_execution.sh
```

Execute our test. Inside tests we have read only (free) transactions, and payable transactions for read and write operations:

```bash
./scripts/test_smart_contract_functions_execution.sh
```