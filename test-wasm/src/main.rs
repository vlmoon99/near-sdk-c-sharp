
#[tokio::main]
async fn main() -> anyhow::Result<()> {
    let worker: near_workspaces::Worker<near_workspaces::network::Sandbox> = near_workspaces::sandbox().await?;
    
    const NFT_WASM_FILEPATH: &str = "../smartcontract/test-wasm/test.wasm";
    let wasm = std::fs::read(NFT_WASM_FILEPATH)?;
    let contract = worker.dev_deploy(&wasm).await?;
    
    let outcome = contract
        .call("helloworld")
        .transact()
        .await?;

    println!("new_default_meta outcome: {:#?}", outcome);


    Ok(())
}
