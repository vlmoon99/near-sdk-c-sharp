1. Create patch in nearcore
`nearcore-contract-log.patch`

2. Compile it in docker (make docker-nearcore) or use local build

3. Run localnet:

neard init
neard run


4. Get validator key for test.near account:


sed 's/secret/private/'  <~/.near/validator_key.json >~/.near/test.near.json


5. Setup near CLI on localnet:


cat >>~/.config/near-cli/config.toml <<'EOF'

[network_connection.localnet]
network_name = "localnet"
rpc_url = "http://127.0.0.1:3030"
wallet_url = "https://app.mynearwallet.com/"
explorer_transaction_url = "https://explorer.near.org/transactions/"
EOF


6. Deploy smart contract and try to call it:


near contract deploy test.near use-file /mnt/test.wasm with-init-call new json-args {} prepaid-gas '100.0 Tgas' attached-deposit '0 NEAR' network-config localnet sign-with-access-key-file test.near.json send