In order to make basic compilation use ->

```

cd smartcontract

dotnet publish -r browser-wasm /p:DebugType=none /p:InvariantGlobalization=true /p:OptimizationPreference=Size /p:StackTraceSupport=false

```