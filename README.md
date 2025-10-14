# If u want to contribute write me on :
```
Telegram : @vlmoon99 or @vlmoon77
Discord : @vlad_mykolaienko
Twitter : @vlmoon99
Gmail : vlmoon.near@gmail.com
```
# Main Goal Of This Project 
```
Create Proof of Concept "Smart Contract" compiled from C# for Near Blokchain  and call log("Hello World") on the blockchain side
```
# What already achived
```
1.Small size of wasm.
2.Clear "env" imports.
3.Pass serilazition.
4.Pass deserilazition on wasmtime runtime + enabled bulk memmory on the host machine.
5.Be able to call smart contract (but with unreachable code for now , and it's a main problem). 
```

# Understand the structure of the repository :
```
1. smartcontract folder - it's c# .net project which we compile to the wasm
2. test-wasm 
(Currenty we can't use it for test, because we cant enable bulk memmory support there, it's possible only inside nearcore aka Near Blockchain Node) 
- it's rust project where we use near_workspaces package in order to upload our wasm to the blockchain in emulation mode , but we will have all possible errors and return results as we do this on real blockchain.
```
# How to compile and test the smart contract (Simple using mocks and wasmtime)
```
1. Download and install : .net 9.0, wasi 24 or 25 version, rust

2. Go to the smartcontract folder : 

cd smartcontract

3. Create stubs for our wasm file using clang++ from wasi sdk (It's important to use clang++ from wasi folder) :

/wasi-sdk-24.0-x86_64-linux/bin/clang++ stubs.cpp -o stubs.o -c

We stubs our wasi import from env in order to pass serialization when smart contract is uploading on the Blockchain

4. Run build using nativeaot-llvm .net compiler and produce our optimized wasm file which is less than 1mb in size :

dotnet publish -r wasi-wasm /p:DebugType=none /p:InvariantGlobalization=true /p:OptimizationPreference=Size /p:StackTraceSupport=false


5.Unbundle our outcome "smartcontract.wasm" because Near Blockchain doesn't support wasi components , only clear wasm modules

wasm-tools component unbundle smartcontract.wasm --module-dir ./

This command will produce -> 

smartcontract.deps.json  smartcontract.wasm  unbundled-module0.wasm

unbundled-module0.wasm - it's our clean wasm component


6.But it's not the end, and the problem is - near blockchain has imports like "log_utf8" , but in .wit format where we provide our "imports" and "exports", we can't use underscore "_" , so now we can only transform our wasm component to wat , chage "-" to "_" mannualy and make wat2wasm conversion

wasm2wat unbundled-module0.wasm -o test.wat

Make hand transformation(in a future we will create an automatic script but now we can only make it by our hands)

wat2wasm test.wat -o test.wasm


7. In order to test wasm file in "lite mode" we can use near-sdk-c-sharp/smartcontract/test-wasm/testwasm.ipynb wherein we will use wasmtime runtime which is used in Near Blockchain , so we can mock our "env" methods
and see if there will be some errors, if there no errors - we can go to the advance testing and up and running our local Near Blockchain Node.


```
# How to compile and test the smart contract (Advance using Local Node)
```
1. Download and install : .net 9.0, wasi 24 or 25 version, rust

2. Go to the smartcontract folder : 

cd smartcontract

3. Create stubs for our wasm file using clang++ from wasi sdk (It's important to use clang++ from wasi folder) :

/wasi-sdk-24.0-x86_64-linux/bin/clang++ stubs.cpp -o stubs.o -c

We stubs our wasi import from env in order to pass serialization when smart contract is uploading on the Blockchain

4. Run build using nativeaot-llvm .net compiler and produce our optimized wasm file which is less than 1mb in size :

dotnet publish -r wasi-wasm /p:DebugType=none /p:InvariantGlobalization=true /p:OptimizationPreference=Size /p:StackTraceSupport=false


5.Unbundle our outcome "smartcontract.wasm" because Near Blockchain doesn't support wasi components , only clear wasm modules

wasm-tools component unbundle smartcontract.wasm --module-dir ./

This command will produce -> 

smartcontract.deps.json  smartcontract.wasm  unbundled-module0.wasm

unbundled-module0.wasm - it's our clean wasm component


6.But it's not the end, and the problem is - near blockchain has imports like "log_utf8" , but in .wit format where we provide our "imports" and "exports", we can't use underscore "_" , so now we can only transform our wasm component to wat , chage "-" to "_" mannualy and make wat2wasm conversion

wasm2wat unbundled-module0.wasm -o test.wat

Make hand transformation(in a future we will create an automatic script but now we can only make it by our hands)

wat2wasm test.wat -o test.wasm


7.Our wasm is ready for uploading it to the blockchain for testing, now we need to clone nearcore repositoty which is represents Near Blockchain Node

git clone https://github.com/near/nearcore

Last release was 2.4.0, so we need to make checkout on this version

git checkout 2.4.0

8.Go to the nearcore directory and change the settings in order to enable bulk memmory support + enable wasitime enviroment by default,
I created a git patch with all necessary changes in order to see all necessary logs and enable wasitime runtime and memmory bulk support

cd nearcore

git apply csharpsdk.patch

9.After we have all necessary changes insdie nearcore repo , we can start to build our Near Blockchain node, we will do it locally on our PC :

cargo build --release (Release) 
or 
cargo build (Debug)

In process of building this node you can have a lot of errors, but they can happen only in two way's - you dont have some necessary build dependecy (clang,make,build-essentials, etc) or you will not have enough memmory for compilation

If you dont have enough memmory for compilation - create swap with 40G of memmory, it help me on my laptop (24gb ram) , while on 128g ram workstation I build it without any swap.

10.We will need to initialize our node and make test run before we will upload our smartcontract there

cd /nearcore/target/release (If we build node in release mode)

or

cd /nearcore/target/debug  (If we build node in debug mode)

./neard init - this will init all necessary configs in order to run node localy 
./neard run - this cmd will run our local node



11.Now we are ready to start our node , but before it we need to create near acocunt using near rust cli.

Download and run Near CLI :

curl --proto '=https' --tlsv1.2 -LsSf https://github.com/near/near-cli-rs/releases/latest/download/near-cli-rs-installer.sh | sh

After we have CLI we need to setup our cli in order to be able to connect to our local node:

cd ~/.config/near-cli

nano config.toml

Add this , or if there will be some config - change it to your localhost - http://127.0.0.1:3030

[network_connection.localnet]
network_name = "localnet"
rpc_url = "http://127.0.0.1:3030"
wallet_url = "https://app.mynearwallet.com/"
explorer_transaction_url = "https://explorer.near.org/transactions/"


12.After we init our local node, setup cli in order to call this local node , we will need to take keys in order to sing tx and send it to the local node

cd ~/.near/

in this folder we will have the file validator_key.json

We will need to copy all data in new file call test.near.json + we need to add new key "secret_key" with value of private key , it will be necessary for cli in order to sign tx (You can find an example in smartcontract/test-wasm/test.near.json , but there a keys from my local node, pls take it into account), after we will create this file we can tranfer it to our smartcontract/test-wasm folder (take into account that there alredy some keys with the same names - you will need to delete it before moving your keys).


13. Test our wasm file

After we have all necessary files inside near-sdk-c-sharp/smartcontract/test-wasm, we can start to test it local 

near contract deploy test.near use-file ./test.wasm with-init-call helloworld json-args {} prepaid-gas '300.0 Tgas' attached-deposit '0 NEAR' network-config localnet sign-with-access-key-file test.near.json send

```
# Discover Near Blockchain SDK, API which can be helpful in creating SDK on C#
```
1. Near core m it's main blockchain node which will process our wasm file 

https://github.com/near/nearcore


2. Near SDK JS
Works in a way when JS -> C -> WASM

https://github.com/near/near-sdk-js/tree/develop

https://github.com/near/near-sdk-js/blob/develop/packages/near-sdk-js/builder/builder.c

3.Near SDK RS 

Compile it using native rust compiler

https://github.com/near/near-sdk-rs


```
