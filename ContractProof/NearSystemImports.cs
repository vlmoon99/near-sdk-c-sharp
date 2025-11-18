using System;
using System.Runtime.InteropServices;

namespace ContractProof;

public unsafe static class NearSystemImports
{
    // Register operations
    [WasmImportLinkage]
    [DllImport("env", EntryPoint = "read-register")]
    public static extern void ReadRegister(long registerId, long ptr);

    [WasmImportLinkage]
    [DllImport("env", EntryPoint = "register-len")]
    public static extern long RegisterLen(long registerId);

    [WasmImportLinkage]
    [DllImport("env", EntryPoint = "write-register")]
    public static extern void WriteRegister(long registerId, long dataLen, long dataPtr);

    // Storage operations
    [WasmImportLinkage]
    [DllImport("env", EntryPoint = "storage-write")]
    public static extern long StorageWrite(long keyLen, long keyPtr, long valueLen, long valuePtr, long registerId);

    [WasmImportLinkage]
    [DllImport("env", EntryPoint = "storage-read")]
    public static extern long StorageRead(long keyLen, long keyPtr, long registerId);

    [WasmImportLinkage]
    [DllImport("env", EntryPoint = "storage-remove")]
    public static extern long StorageRemove(long keyLen, long keyPtr, long registerId);

    [WasmImportLinkage]
    [DllImport("env", EntryPoint = "storage-has-key")]
    public static extern long StorageHasKey(long keyLen, long keyPtr);

    // Account operations
    [WasmImportLinkage]
    [DllImport("env", EntryPoint = "current-account-id")]
    public static extern void CurrentAccountId(long registerId);

    [WasmImportLinkage]
    [DllImport("env", EntryPoint = "signer-account-id")]
    public static extern void SignerAccountId(long registerId);

    [WasmImportLinkage]
    [DllImport("env", EntryPoint = "signer-account-pk")]
    public static extern void SignerAccountPk(long registerId);

    [WasmImportLinkage]
    [DllImport("env", EntryPoint = "predecessor-account-id")]
    public static extern void PredecessorAccountId(long registerId);

    // Input operations
    [WasmImportLinkage]
    [DllImport("env", EntryPoint = "input")]
    public static extern void Input(long registerId);

    // Block operations
    [WasmImportLinkage]
    [DllImport("env", EntryPoint = "block-index")]
    public static extern long BlockIndex();

    [WasmImportLinkage]
    [DllImport("env", EntryPoint = "block-timestamp")]
    public static extern long BlockTimestamp();

    [WasmImportLinkage]
    [DllImport("env", EntryPoint = "epoch-height")]
    public static extern long EpochHeight();

    // Storage and balance operations
    [WasmImportLinkage]
    [DllImport("env", EntryPoint = "storage-usage")]
    public static extern long StorageUsage();

    [WasmImportLinkage]
    [DllImport("env", EntryPoint = "account-balance")]
    public static extern void AccountBalance(long balancePtr);

    [WasmImportLinkage]
    [DllImport("env", EntryPoint = "account-locked-balance")]
    public static extern void AccountLockedBalance(long balancePtr);

    [WasmImportLinkage]
    [DllImport("env", EntryPoint = "attached-deposit")]
    public static extern void AttachedDeposit(long balancePtr);

    // Gas operations
    [WasmImportLinkage]
    [DllImport("env", EntryPoint = "prepaid-gas")]
    public static extern long PrepaidGas();

    [WasmImportLinkage]
    [DllImport("env", EntryPoint = "used-gas")]
    public static extern long UsedGas();

    // Random operations
    [WasmImportLinkage]
    [DllImport("env", EntryPoint = "random-seed")]
    public static extern void RandomSeed(long registerId);

    // Cryptographic hash operations
    [WasmImportLinkage]
    [DllImport("env", EntryPoint = "sha256")]
    public static extern void Sha256(long valueLen, long valuePtr, long registerId);

    [WasmImportLinkage]
    [DllImport("env", EntryPoint = "keccak256")]
    public static extern void Keccak256(long valueLen, long valuePtr, long registerId);

    [WasmImportLinkage]
    [DllImport("env", EntryPoint = "keccak512")]
    public static extern void Keccak512(long valueLen, long valuePtr, long registerId);

    [WasmImportLinkage]
    [DllImport("env", EntryPoint = "ripemd160")]
    public static extern void Ripemd160(long valueLen, long valuePtr, long registerId);

    // Cryptographic signature operations
    [WasmImportLinkage]
    [DllImport("env", EntryPoint = "ecrecover")]
    public static extern long Ecrecover(long hashLen, long hashPtr, long sigLen, long sigPtr, long v, long malleabilityFlag, long registerId);

    [WasmImportLinkage]
    [DllImport("env", EntryPoint = "ed25519-verify")]
    public static extern long Ed25519Verify(long sigLen, long sigPtr, long msgLen, long msgPtr, long pubKeyLen, long pubKeyPtr);

    // Alt BN128 operations
    [WasmImportLinkage]
    [DllImport("env", EntryPoint = "alt-bn128-g1-multiexp")]
    public static extern void AltBn128G1Multiexp(long valueLen, long valuePtr, long registerId);

    [WasmImportLinkage]
    [DllImport("env", EntryPoint = "alt-bn128-g1-sum")]
    public static extern void AltBn128G1Sum(long valueLen, long valuePtr, long registerId);

    [WasmImportLinkage]
    [DllImport("env", EntryPoint = "alt-bn128-pairing-check")]
    public static extern long AltBn128PairingCheck(long valueLen, long valuePtr);

    // Validator operations
    [WasmImportLinkage]
    [DllImport("env", EntryPoint = "validator-stake")]
    public static extern void ValidatorStake(long accountIdLen, long accountIdPtr, long stakePtr);

    [WasmImportLinkage]
    [DllImport("env", EntryPoint = "validator-total-stake")]
    public static extern void ValidatorTotalStake(long stakePtr);

    // Return and logging operations
    [WasmImportLinkage]
    [DllImport("env", EntryPoint = "value-return")]
    public static extern void ValueReturn(long valueLen, long valuePtr);

    [WasmImportLinkage]
    [DllImport("env", EntryPoint = "panic-utf8")]
    public static extern void PanicUtf8(long len, long ptr);

    [WasmImportLinkage]
    [DllImport("env", EntryPoint = "log-utf8")]
    public static extern void LogUtf8(long len, long ptr);

    [WasmImportLinkage]
    [DllImport("env", EntryPoint = "log-utf16")]
    public static extern void LogUtf16(long len, long ptr);

    // Promise operations
    [WasmImportLinkage]
    [DllImport("env", EntryPoint = "promise-create")]
    public static extern long PromiseCreate(long accountIdLen, long accountIdPtr, long functionNameLen, long functionNamePtr, long argumentsLen, long argumentsPtr, long amountPtr, long gas);

    [WasmImportLinkage]
    [DllImport("env", EntryPoint = "promise-then")]
    public static extern long PromiseThen(long promiseIndex, long accountIdLen, long accountIdPtr, long functionNameLen, long functionNamePtr, long argumentsLen, long argumentsPtr, long amountPtr, long gas);

    [WasmImportLinkage]
    [DllImport("env", EntryPoint = "promise-and")]
    public static extern long PromiseAnd(long promiseIdxPtr, long promiseIdxCount);

    // Promise batch operations
    [WasmImportLinkage]
    [DllImport("env", EntryPoint = "promise-batch-create")]
    public static extern long PromiseBatchCreate(long accountIdLen, long accountIdPtr);

    [WasmImportLinkage]
    [DllImport("env", EntryPoint = "promise-batch-then")]
    public static extern long PromiseBatchThen(long promiseIndex, long accountIdLen, long accountIdPtr);

    // Promise batch actions
    [WasmImportLinkage]
    [DllImport("env", EntryPoint = "promise-batch-action-create-account")]
    public static extern void PromiseBatchActionCreateAccount(long promiseIndex);

    [WasmImportLinkage]
    [DllImport("env", EntryPoint = "promise-batch-action-deploy-contract")]
    public static extern void PromiseBatchActionDeployContract(long promiseIndex, long codeLen, long codePtr);

    [WasmImportLinkage]
    [DllImport("env", EntryPoint = "promise-batch-action-function-call")]
    public static extern void PromiseBatchActionFunctionCall(long promiseIndex, long functionNameLen, long functionNamePtr, long argumentsLen, long argumentsPtr, long amountPtr, long gas);

    [WasmImportLinkage]
    [DllImport("env", EntryPoint = "promise-batch-action-function-call-weight")]
    public static extern void PromiseBatchActionFunctionCallWeight(long promiseIndex, long functionNameLen, long functionNamePtr, long argumentsLen, long argumentsPtr, long amountPtr, long gas, long weight);

    [WasmImportLinkage]
    [DllImport("env", EntryPoint = "promise-batch-action-transfer")]
    public static extern void PromiseBatchActionTransfer(long promiseIndex, long amountPtr);

    [WasmImportLinkage]
    [DllImport("env", EntryPoint = "promise-batch-action-stake")]
    public static extern void PromiseBatchActionStake(long promiseIndex, long amountPtr, long publicKeyLen, long publicKeyPtr);

    [WasmImportLinkage]
    [DllImport("env", EntryPoint = "promise-batch-action-add-key-with-full-access")]
    public static extern void PromiseBatchActionAddKeyWithFullAccess(long promiseIndex, long publicKeyLen, long publicKeyPtr, long nonce);

    [WasmImportLinkage]
    [DllImport("env", EntryPoint = "promise-batch-action-add-key-with-function-call")]
    public static extern void PromiseBatchActionAddKeyWithFunctionCall(long promiseIndex, long publicKeyLen, long publicKeyPtr, long nonce, long allowancePtr, long receiverIdLen, long receiverIdPtr, long functionNamesLen, long functionNamesPtr);

    [WasmImportLinkage]
    [DllImport("env", EntryPoint = "promise-batch-action-delete-key")]
    public static extern void PromiseBatchActionDeleteKey(long promiseIndex, long publicKeyLen, long publicKeyPtr);

    [WasmImportLinkage]
    [DllImport("env", EntryPoint = "promise-batch-action-delete-account")]
    public static extern void PromiseBatchActionDeleteAccount(long promiseIndex, long beneficiaryIdLen, long beneficiaryIdPtr);

    // Promise yield operations
    [WasmImportLinkage]
    [DllImport("env", EntryPoint = "promise-yield-create")]
    public static extern long PromiseYieldCreate(long functionNameLen, long functionNamePtr, long argumentsLen, long argumentsPtr, long gas, long gasWeight, long registerId);

    [WasmImportLinkage]
    [DllImport("env", EntryPoint = "promise-yield-resume")]
    public static extern uint PromiseYieldResume(long dataIdLen, long dataIdPtr, long payloadLen, long payloadPtr);

    // Promise result operations
    [WasmImportLinkage]
    [DllImport("env", EntryPoint = "promise-results-count")]
    public static extern long PromiseResultsCount();

    [WasmImportLinkage]
    [DllImport("env", EntryPoint = "promise-result")]
    public static extern long PromiseResult(long resultIdx, long registerId);

    [WasmImportLinkage]
    [DllImport("env", EntryPoint = "promise-return")]
    public static extern void PromiseReturn(long promiseId);
}