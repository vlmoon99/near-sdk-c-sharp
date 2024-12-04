In order to make basic compilation use ->

```

cd smartcontract

dotnet publish -r wasi-wasm /p:DebugType=none /p:InvariantGlobalization=true /p:OptimizationPreference=Size /p:StackTraceSupport=false


home/vlmoon99/wasi/wasi-sdk-24.0-x86_64-linux/bin/clang++ stubs.cpp -o stubs.o -c

  (table (;0;) 735 735 funcref)
  (memory (;0;) 19 32768)
  (global (;0;) (mut i32) i32.const 1215344)
  (global (;1;) i32 i32.const 0)
  (global (;2;) i32 i32.const 145704)
  (global (;3;) i32 i32.const 145692)
  (global (;4;) i32 i32.const 164036)
  (export "memory" (memory 0))
  (export "_initialize" (func 10))
  (export "cabi_realloc" (func 2447))


  (table (;0;) 64 64 funcref)
  (memory (;0;) 17)
  (global (;0;) (mut i32) i32.const 1048576)
  (global (;1;) i32 i32.const 1072465)
  (global (;2;) i32 i32.const 1072480)
  (export "memory" (memory 0))
  (export "contract_source_metadata" (func 137))
  (export "set_status" (func 140))
  (export "get_status" (func 149))
  (export "__data_end" (global 1))
  (export "__heap_base" (global 2))


```