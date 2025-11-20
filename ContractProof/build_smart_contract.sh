#!/usr/bin/env bash
set -e

echo "Build Wasm"
dotnet publish -r wasi-wasm

echo "Current dir:"
pwd

PUBLISH_DIR="./bin/Release/net10.0/wasi-wasm/publish"
echo "Changing directory to: $PUBLISH_DIR"
cd "$PUBLISH_DIR"

echo "After cd:"
pwd
ls -l

echo "Decouple Wasm"
wasm-tools component unbundle ContractProof.wasm --module-dir ./ || true

echo "After unbundle:"
ls -l

echo "Transform unbundled module to WAT"
wasm2wat ./unbundled-module0.wasm -o ./test.wat || true

echo "After WAT transform:"
ls -l

echo "Fixing import names inside test.wat..."

perl -pi -e '
    if (/^\s*\(import "env" "([^"]+)"/) {
        $name = $1;
        $fixed = $name;
        $fixed =~ s/-/_/g;
        s/\Q$name\E/$fixed/;
    }
' test.wat

echo "Done fixing imports."

echo "Create final Contract."

wat2wasm test.wat -o contract.wasm

cd - > /dev/null
echo "Done!"
