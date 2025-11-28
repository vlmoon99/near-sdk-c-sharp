In the folders **`Case1/`** and **`WasmCase1/`** you can find a simple example demonstrating how to build a system with:

* **One host**, which provides a set of host functions.
* **Multiple C# applications**, where each application (guest) can be one of many possible WASM modules.

The **`Case1/`** folder contains the host implementation, where we use **Wasmtime** to create the WASM environment.

The **`WasmCase1/`** folder contains our custom logic that depends on the host functions defined in **`Case1/`**.

---

## Build Pipeline Overview

To understand the detailed transformation and build steps, see the script:

```
WasmCase1/scripts/build_clean_wasm.sh
```

Important notes for producing a **clean WASM**:

1. You must use **WASI build (version 27)** — this is important right now.
2. You must provide the necessary **WASI stubs** for every imported `env` function.
3. You must define your WIT interface file:

```
WasmCase1/wasi_interfaces_type/interface.wit
```

This WIT file specifies:

* Which functions we import.
* The types of those functions.
* Which functions we export.
* The types of exported functions.

4. As the final step (“the cherry on the cake”), you must include the native AOT LLVM compiler.
   Add the correct `nuget.config` and insert the following lines into your `.csproj`:

```xml
<ItemGroup>
  <PackageReference Include="Microsoft.DotNet.ILCompiler.LLVM" Version="10.0.0-*" />
  <PackageReference Include="runtime.$(NETCoreSdkPortableRuntimeIdentifier).Microsoft.DotNet.ILCompiler.LLVM" Version="10.0.0-*" />
</ItemGroup>
```

---

## Additional Resources

For deeper understanding of how everything works under the hood, I recommend reading the native AOT LLVM source code:

**NativeAOT-LLVM repository:**
[https://github.com/dotnet/runtimelab/tree/feature/NativeAOT-LLVM](https://github.com/dotnet/runtimelab/tree/feature/NativeAOT-LLVM)

For support, I recommend joining the official Microsoft Discord:
[https://discord.gg/csharp](https://discord.gg/csharp)

Especially the **`allow-unsafe-blocks-NativeAOT-LLVM`** channel:
[https://discord.com/channels/143867839282020352/1141126727028985877](https://discord.com/channels/143867839282020352/1141126727028985877)

You can find more information about common issues there.

---

## Reducing Binary Size

If you want an even smaller WASM file than the ~700 KB I currently have, you will need to write your own **custom .NET runtime** and build it yourself.
This is difficult, but depending on your use case it may be suitable.

You can take inspiration from these projects:

1. **Building a self-contained game in C# under 8 KB**
   (This project also demonstrates the idea of a mini runtime for extremely small WASM files.)
   [https://migeel.sk/blog/2020/01/03/building-a-self-contained-game-in-csharp-under-8-kilobytes/](https://migeel.sk/blog/2020/01/03/building-a-self-contained-game-in-csharp-under-8-kilobytes/)
   [https://github.com/MichalStrehovsky/SeeSharpSnake](https://github.com/MichalStrehovsky/SeeSharpSnake)

2. **WASM fork of the same 8 KB game project**
   [https://github.com/yowl/SeeSharpSnake/tree/wasm4](https://github.com/yowl/SeeSharpSnake/tree/wasm4)

