#!/usr/bin/env bash
set -e

near --teach-me contract deploy test.near use-file ./bin/Release/net10.0/wasi-wasm/publish/contract.wasm with-init-call helloworld json-args {} prepaid-gas '300.0 Tgas' attached-deposit '0 NEAR' network-config localnet sign-with-access-key-file test.near.json send
