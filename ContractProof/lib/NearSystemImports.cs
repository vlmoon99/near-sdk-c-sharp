using System;
using System.Runtime.InteropServices;

namespace ContractProof;

public unsafe static class NearSystemImports
{
    // Register operations
    [WasmImportLinkage]
    [DllImport("env", EntryPoint = "read-register")]
    public static extern void ReadRegister(ulong registerId, ulong ptr);

    [WasmImportLinkage]
    [DllImport("env", EntryPoint = "register-len")]
    public static extern ulong RegisterLen(ulong registerId);

    [WasmImportLinkage]
    [DllImport("env", EntryPoint = "write-register")]
    public static extern void WriteRegister(ulong registerId, ulong dataLen, ulong dataPtr);

    // Storage operations
    [WasmImportLinkage]
    [DllImport("env", EntryPoint = "storage-write")]
    public static extern ulong StorageWrite(ulong keyLen, ulong keyPtr, ulong valueLen, ulong valuePtr, ulong registerId);

    [WasmImportLinkage]
    [DllImport("env", EntryPoint = "storage-read")]
    public static extern ulong StorageRead(ulong keyLen, ulong keyPtr, ulong registerId);

    [WasmImportLinkage]
    [DllImport("env", EntryPoint = "storage-remove")]
    public static extern ulong StorageRemove(ulong keyLen, ulong keyPtr, ulong registerId);

    [WasmImportLinkage]
    [DllImport("env", EntryPoint = "storage-has-key")]
    public static extern ulong StorageHasKey(ulong keyLen, ulong keyPtr);

    // Account operations
    [WasmImportLinkage]
    [DllImport("env", EntryPoint = "current-account-id")]
    public static extern void CurrentAccountId(ulong registerId);

    [WasmImportLinkage]
    [DllImport("env", EntryPoint = "signer-account-id")]
    public static extern void SignerAccountId(ulong registerId);

    [WasmImportLinkage]
    [DllImport("env", EntryPoint = "signer-account-pk")]
    public static extern void SignerAccountPk(ulong registerId);

    [WasmImportLinkage]
    [DllImport("env", EntryPoint = "predecessor-account-id")]
    public static extern void PredecessorAccountId(ulong registerId);

    // Input operations
    [WasmImportLinkage]
    [DllImport("env", EntryPoint = "input")]
    public static extern void Input(ulong registerId);

    // Block operations
    [WasmImportLinkage]
    [DllImport("env", EntryPoint = "block-index")]
    public static extern ulong BlockIndex();

    [WasmImportLinkage]
    [DllImport("env", EntryPoint = "block-timestamp")]
    public static extern ulong BlockTimestamp();

    [WasmImportLinkage]
    [DllImport("env", EntryPoint = "epoch-height")]
    public static extern ulong EpochHeight();

    // Storage and balance operations
    [WasmImportLinkage]
    [DllImport("env", EntryPoint = "storage-usage")]
    public static extern ulong StorageUsage();

    [WasmImportLinkage]
    [DllImport("env", EntryPoint = "account-balance")]
    public static extern void AccountBalance(ulong balancePtr);

    [WasmImportLinkage]
    [DllImport("env", EntryPoint = "account-locked-balance")]
    public static extern void AccountLockedBalance(ulong balancePtr);

    [WasmImportLinkage]
    [DllImport("env", EntryPoint = "attached-deposit")]
    public static extern void AttachedDeposit(ulong balancePtr);

    // Gas operations
    [WasmImportLinkage]
    [DllImport("env", EntryPoint = "prepaid-gas")]
    public static extern ulong PrepaidGas();

    [WasmImportLinkage]
    [DllImport("env", EntryPoint = "used-gas")]
    public static extern ulong UsedGas();

    // Random operations
    [WasmImportLinkage]
    [DllImport("env", EntryPoint = "random-seed")]
    public static extern void RandomSeed(ulong registerId);

    // Cryptographic hash operations
    [WasmImportLinkage]
    [DllImport("env", EntryPoint = "sha256")]
    public static extern void Sha256(ulong valueLen, ulong valuePtr, ulong registerId);

    [WasmImportLinkage]
    [DllImport("env", EntryPoint = "keccak256")]
    public static extern void Keccak256(ulong valueLen, ulong valuePtr, ulong registerId);

    [WasmImportLinkage]
    [DllImport("env", EntryPoint = "keccak512")]
    public static extern void Keccak512(ulong valueLen, ulong valuePtr, ulong registerId);

    [WasmImportLinkage]
    [DllImport("env", EntryPoint = "ripemd160")]
    public static extern void Ripemd160(ulong valueLen, ulong valuePtr, ulong registerId);

    // Cryptographic signature operations
    [WasmImportLinkage]
    [DllImport("env", EntryPoint = "ecrecover")]
    public static extern ulong Ecrecover(ulong hashLen, ulong hashPtr, ulong sigLen, ulong sigPtr, ulong v, ulong malleabilityFlag, ulong registerId);

    [WasmImportLinkage]
    [DllImport("env", EntryPoint = "ed25519-verify")]
    public static extern ulong Ed25519Verify(ulong sigLen, ulong sigPtr, ulong msgLen, ulong msgPtr, ulong pubKeyLen, ulong pubKeyPtr);

    // Alt BN128 operations
    [WasmImportLinkage]
    [DllImport("env", EntryPoint = "alt-bn128-g1-multiexp")]
    public static extern void AltBn128G1Multiexp(ulong valueLen, ulong valuePtr, ulong registerId);

    [WasmImportLinkage]
    [DllImport("env", EntryPoint = "alt-bn128-g1-sum")]
    public static extern void AltBn128G1Sum(ulong valueLen, ulong valuePtr, ulong registerId);

    [WasmImportLinkage]
    [DllImport("env", EntryPoint = "alt-bn128-pairing-check")]
    public static extern ulong AltBn128PairingCheck(ulong valueLen, ulong valuePtr);

    // Validator operations
    [WasmImportLinkage]
    [DllImport("env", EntryPoint = "validator-stake")]
    public static extern void ValidatorStake(ulong accountIdLen, ulong accountIdPtr, ulong stakePtr);

    [WasmImportLinkage]
    [DllImport("env", EntryPoint = "validator-total-stake")]
    public static extern void ValidatorTotalStake(ulong stakePtr);

    // Return and logging operations
    [WasmImportLinkage]
    [DllImport("env", EntryPoint = "value-return")]
    public static extern void ValueReturn(ulong valueLen, ulong valuePtr);

    [WasmImportLinkage]
    [DllImport("env", EntryPoint = "panic-utf8")]
    public static extern void PanicUtf8(ulong len, ulong ptr);

    [WasmImportLinkage]
    [DllImport("env", EntryPoint = "log-utf8")]
    public static extern void LogUtf8(ulong len, ulong ptr);

    [WasmImportLinkage]
    [DllImport("env", EntryPoint = "log-utf16")]
    public static extern void LogUtf16(ulong len, ulong ptr);

    // Promise operations
    [WasmImportLinkage]
    [DllImport("env", EntryPoint = "promise-create")]
    public static extern ulong PromiseCreate(ulong accountIdLen, ulong accountIdPtr, ulong functionNameLen, ulong functionNamePtr, ulong argumentsLen, ulong argumentsPtr, ulong amountPtr, ulong gas);

    [WasmImportLinkage]
    [DllImport("env", EntryPoint = "promise-then")]
    public static extern ulong PromiseThen(ulong promiseIndex, ulong accountIdLen, ulong accountIdPtr, ulong functionNameLen, ulong functionNamePtr, ulong argumentsLen, ulong argumentsPtr, ulong amountPtr, ulong gas);

    [WasmImportLinkage]
    [DllImport("env", EntryPoint = "promise-and")]
    public static extern ulong PromiseAnd(ulong promiseIdxPtr, ulong promiseIdxCount);

    // Promise batch operations
    [WasmImportLinkage]
    [DllImport("env", EntryPoint = "promise-batch-create")]
    public static extern ulong PromiseBatchCreate(ulong accountIdLen, ulong accountIdPtr);

    [WasmImportLinkage]
    [DllImport("env", EntryPoint = "promise-batch-then")]
    public static extern ulong PromiseBatchThen(ulong promiseIndex, ulong accountIdLen, ulong accountIdPtr);

    // Promise batch actions
    [WasmImportLinkage]
    [DllImport("env", EntryPoint = "promise-batch-action-create-account")]
    public static extern void PromiseBatchActionCreateAccount(ulong promiseIndex);

    [WasmImportLinkage]
    [DllImport("env", EntryPoint = "promise-batch-action-deploy-contract")]
    public static extern void PromiseBatchActionDeployContract(ulong promiseIndex, ulong codeLen, ulong codePtr);

    [WasmImportLinkage]
    [DllImport("env", EntryPoint = "promise-batch-action-function-call")]
    public static extern void PromiseBatchActionFunctionCall(ulong promiseIndex, ulong functionNameLen, ulong functionNamePtr, ulong argumentsLen, ulong argumentsPtr, ulong amountPtr, ulong gas);

    [WasmImportLinkage]
    [DllImport("env", EntryPoint = "promise-batch-action-function-call-weight")]
    public static extern void PromiseBatchActionFunctionCallWeight(ulong promiseIndex, ulong functionNameLen, ulong functionNamePtr, ulong argumentsLen, ulong argumentsPtr, ulong amountPtr, ulong gas, ulong weight);

    [WasmImportLinkage]
    [DllImport("env", EntryPoint = "promise-batch-action-transfer")]
    public static extern void PromiseBatchActionTransfer(ulong promiseIndex, ulong amountPtr);

    [WasmImportLinkage]
    [DllImport("env", EntryPoint = "promise-batch-action-stake")]
    public static extern void PromiseBatchActionStake(ulong promiseIndex, ulong amountPtr, ulong publicKeyLen, ulong publicKeyPtr);

    [WasmImportLinkage]
    [DllImport("env", EntryPoint = "promise-batch-action-add-key-with-full-access")]
    public static extern void PromiseBatchActionAddKeyWithFullAccess(ulong promiseIndex, ulong publicKeyLen, ulong publicKeyPtr, ulong nonce);

    [WasmImportLinkage]
    [DllImport("env", EntryPoint = "promise-batch-action-add-key-with-function-call")]
    public static extern void PromiseBatchActionAddKeyWithFunctionCall(ulong promiseIndex, ulong publicKeyLen, ulong publicKeyPtr, ulong nonce, ulong allowancePtr, ulong receiverIdLen, ulong receiverIdPtr, ulong functionNamesLen, ulong functionNamesPtr);

    [WasmImportLinkage]
    [DllImport("env", EntryPoint = "promise-batch-action-delete-key")]
    public static extern void PromiseBatchActionDeleteKey(ulong promiseIndex, ulong publicKeyLen, ulong publicKeyPtr);

    [WasmImportLinkage]
    [DllImport("env", EntryPoint = "promise-batch-action-delete-account")]
    public static extern void PromiseBatchActionDeleteAccount(ulong promiseIndex, ulong beneficiaryIdLen, ulong beneficiaryIdPtr);

    // Promise yield operations
    [WasmImportLinkage]
    [DllImport("env", EntryPoint = "promise-yield-create")]
    public static extern ulong PromiseYieldCreate(ulong functionNameLen, ulong functionNamePtr, ulong argumentsLen, ulong argumentsPtr, ulong gas, ulong gasWeight, ulong registerId);

    [WasmImportLinkage]
    [DllImport("env", EntryPoint = "promise-yield-resume")]
    public static extern uint PromiseYieldResume(ulong dataIdLen, ulong dataIdPtr, ulong payloadLen, ulong payloadPtr);

    // Promise result operations
    [WasmImportLinkage]
    [DllImport("env", EntryPoint = "promise-results-count")]
    public static extern ulong PromiseResultsCount();

    [WasmImportLinkage]
    [DllImport("env", EntryPoint = "promise-result")]
    public static extern ulong PromiseResult(ulong resultIdx, ulong registerId);

    [WasmImportLinkage]
    [DllImport("env", EntryPoint = "promise-return")]
    public static extern void PromiseReturn(ulong promiseId);
}