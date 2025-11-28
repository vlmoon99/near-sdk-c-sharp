#!/usr/bin/env bash
set -e

echo "Test Return Value as-read-only"
near --teach-me contract call-function as-read-only test.near returnowner json-args {} network-config localnet now

echo "Test Return Value as-read-only"
near --teach-me contract call-function as-read-only test.near returnvalue json-args {} network-config localnet now

echo "Test Hello World Log as-read-only"
near --teach-me contract call-function as-read-only test.near helloworld json-args {} network-config localnet now

echo "Test Return Value Input as-read-only"
near --teach-me contract call-function as-read-only test.near returnvalueinput json-args '{"hello":"world"}' network-config localnet now

echo "Test Log Input as-read-only"
near --teach-me contract call-function as-read-only test.near loginput json-args '{"hello":"world"}' network-config localnet now

echo "Test Greet as-read-only"
near --teach-me contract call-function as-read-only test.near greet text-args 'Vlad' network-config localnet now

echo "Test Write Operation"
near --teach-me contract call-function as-transaction test.near store text-args 'TestData' prepaid-gas '100.0 Tgas' attached-deposit '0 NEAR' sign-as test.near network-config localnet sign-with-access-key-file ./near_creds/test.near.json send

echo "Test Read Operation"
near --teach-me contract call-function as-read-only test.near retrieve json-args {} network-config localnet now


