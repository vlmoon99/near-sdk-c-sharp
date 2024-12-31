#!/bin/bash

cd ./smartcontract

../wasi-sdk-24.0-x86_64-linux/bin/clang++ stubs.cpp -o stubs.o -c
if [ $? -ne 0 ]; then
    echo "error in clang++"
    exit 1
fi

dotnet publish -r wasi-wasm /p:DebugType=none /p:InvariantGlobalization=true /p:OptimizationPreference=Size /p:StackTraceSupport=false
if [ $? -ne 0 ]; then
    echo "Error in dotnet publish"
    exit 1
fi
echo "Script completed successfully"

if [ $? -eq 0 ]; then
    mv ./bin/Release/net9.0/wasi-wasm/publish/smartcontract.wasm ./test-wasm/
else
    echo "Error in dotnet publish"
    exit 1
fi

cd ./test-wasm

wasm-tools component unbundle smartcontract.wasm --module-dir ./ -o unbundled-module0.wasm
if [ $? -ne 0 ]; then
    echo "Error in wasm-tools component unbundle"
    exit 1
fi


wasm2wat unbundled-module0.wasm -o test.wat
if [ $? -ne 0 ]; then
    echo "Error in wasm2wat"
    exit 1
fi


wat2wasm test.wat -o test.wasm
if [ $? -ne 0 ]; then
    echo "Error in wasm2wat"
    exit 1
fi

echo "Script completed successfully"