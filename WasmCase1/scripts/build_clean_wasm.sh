#!/usr/bin/env bash
set -e

echo "Current dir:"
pwd

echo "Build Wasm"
dotnet publish -r wasi-wasm


PUBLISH_DIR="./bin/Release/net10.0/wasi-wasm/publish"
echo "Changing directory to: $PUBLISH_DIR"
cd "$PUBLISH_DIR"

echo "After cd:"
pwd
ls -l

echo "Decouple Wasm"
wasm-tools component unbundle WasmCase1.wasm --module-dir ./ || true

echo "After unbundle:"
ls -l

echo "Transform unbundled module to WAT"
wasm2wat ./unbundled-module0.wasm -o ./test.wat || true

echo "After WAT transform:"
ls -l

echo "Create final optimized wasm version."

wat2wasm test.wat -o wasm_case_1.wasm

ls -l

cd - > /dev/null
echo "Done!"


