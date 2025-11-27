using System;
using System.Runtime.InteropServices;
using System.Text;

namespace ContractProof;

public unsafe static class NearBlockchainEnv
{
    public const string RegisterExpectedErr = "Register was expected to have data because we just wrote it into it.";

    public const ulong AtomicOpRegister = ulong.MaxValue - 2;

    public const ulong EvictedRegister = ulong.MaxValue - 1;

    public const ulong DataIdRegister = 0;

    public static readonly byte[] StateKey = Encoding.UTF8.GetBytes("STATE");

    public const ulong MinAccountIDLen = 2;

    public const ulong MaxAccountIDLen = 64;

    // Error messages
    public const string ErrExpectedDataInRegister = "(REGISTER_ERROR): expected data in register, but found none";
    public const string ErrInvalidAccountID = "(ACCOUNT_ERROR): invalid account ID";
    public const string ErrKeyNotFound = "(STORAGE_ERROR): key not found";
    public const string ErrValueNotFound = "(STORAGE_ERROR): value not found";
    public const string ErrFailedToParseInput = "(INPUT_ERROR): failed to parse input";
    public const string ErrUnsupportedDataFormat = "(FORMAT_ERROR): unsupported data format";
    public const string ErrGettingAccountBalance = "(BALANCE_ERROR): error while getting account balance";
    public const string ErrGettingLockedAccountBalance = "(BALANCE_ERROR): error while getting locked account balance";
    public const string ErrGettingAttachedDeposit = "(DEPOSIT_ERROR): error while getting attached deposit";
    public const string ErrFailedToWriteValueInStorage = "(STORAGE_ERROR): failed to write value in the storage by provided key, result of operation is 0";
    public const string ErrKeyIsEmpty = "(STORAGE_ERROR): key is empty";
    public const string ErrFailedToReadKey = "(STORAGE_ERROR): failed to read the key";
    public const string ErrFailedToReadRegister = "(REGISTER_ERROR): failed to read register";
    public const string ErrCantRemoveDataByKey = "(STORAGE_ERROR): can't remove data by that key";
    public const string ErrFailedToReadEvictedRegister = "(REGISTER_ERROR): failed to read evicted register";
    public const string ErrStateNotFound = "(STATE_ERROR): state not found";
    public const string ErrFailedToWriteStateToStorage = "(STORAGE_ERROR): failed to write state to storage";
    public const string ErrInvalidInputHashAndSignatureEmpty = "(INPUT_ERROR): invalid input: hash and signature must not be empty";
    public const string PanicStrEcrecoverFailed = "(PANIC): Ecrecover failed";
    public const string ErrAccountIDMustNotBeEmpty = "(ACCOUNT_ERROR): account ID must not be empty";
    public const string ErrGettingValidatorStakeAmount = "(STAKE_ERROR): error while getting validator stake amount";
    public const string ErrGettingValidatorTotalStakeAmount = "(STAKE_ERROR): error while getting validator total stake amount";
    public const string ErrPromiseResult = "(PROMISE_ERROR): no promise results available";

    // Registers API

    /// <summary>
    /// Tries to execute the given method and reads the data from the register.
    /// </summary>
    /// <param name="method">The method to be executed.</param>
    /// <returns>The data read from the register.</returns>
    /// <exception cref="Exception">Thrown when the method execution or data reading fails.</exception>
    public static byte[] TryMethodIntoRegister(Action<ulong> method)
    {
        method(AtomicOpRegister);
        return ReadRegisterSafe(AtomicOpRegister);
    }

    /// <summary>
    /// Executes the given method and ensures the data is read from the register.
    /// </summary>
    /// <param name="method">The method to be executed.</param>
    /// <returns>The data read from the register.</returns>
    /// <exception cref="Exception">Thrown when the method execution or data reading fails.</exception>
    public static byte[] MethodIntoRegister(Action<ulong> method)
    {
        var data = TryMethodIntoRegister(method);
        if (data == null || data.Length == 0)
        {
            throw new Exception(ErrExpectedDataInRegister);
        }
        return data;
    }

    /// <summary>
    /// Reads the data from the specified register safely.
    /// </summary>
    /// <param name="registerId">The ID of the register to read from.</param>
    /// <returns>The data read from the register.</returns>
    /// <exception cref="Exception">Thrown when the register reading fails.</exception>
    public static byte[] ReadRegisterSafe(ulong registerId)
    {
        ulong length = NearSystemImports.RegisterLen(registerId);

        // Assert valid account id
        AssertValidAccountId(Encoding.UTF8.GetBytes(length.ToString()));

        if (length == 0)
        {
            throw new Exception(ErrExpectedDataInRegister);
        }

        byte[] buffer = new byte[length];
        fixed (byte* ptr = buffer)
        {
            NearSystemImports.ReadRegister(registerId, (ulong)ptr);
        }

        return buffer;
    }

    /// <summary>
    /// Writes the given data to the specified register safely.
    /// </summary>
    /// <param name="registerId">The ID of the register to write to.</param>
    /// <param name="data">The data to be written to the register.</param>
    public static void WriteRegisterSafe(ulong registerId, byte[] data)
    {
        if (data == null || data.Length == 0)
        {
            return;
        }

        fixed (byte* ptr = data)
        {
            NearSystemImports.WriteRegister(registerId, (ulong)data.Length, (ulong)ptr);
        }
    }

    // Storage API

    /// <summary>
    /// Writes the given value to the specified key in storage.
    /// </summary>
    /// <param name="key">The key to write the value to.</param>
    /// <param name="value">The value to write.</param>
    /// <returns>True if the value was successfully written, false otherwise.</returns>
    /// <exception cref="Exception">Thrown when the key or value is empty or if the write operation fails.</exception>
    public static bool StorageWrite(byte[] key, byte[] value)
    {
        if (key == null || key.Length == 0)
        {
            throw new Exception(ErrKeyNotFound);
        }

        if (value == null || value.Length == 0)
        {
            throw new Exception(ErrValueNotFound);
        }

        ulong keyLen = (ulong)key.Length;
        ulong valueLen = (ulong)value.Length;

        fixed (byte* keyPtr = key)
        fixed (byte* valuePtr = value)
        {
            return StorageWriteRecursive(keyLen, (ulong)keyPtr, valueLen, (ulong)valuePtr, 0);
        }
    }

    /// <summary>
    /// Attempts to write the value to the specified key in storage recursively.
    /// </summary>
    /// <param name="keyLen">The length of the key.</param>
    /// <param name="keyPtr">The pointer to the key.</param>
    /// <param name="valueLen">The length of the value.</param>
    /// <param name="valuePtr">The pointer to the value.</param>
    /// <param name="attempt">The current attempt number.</param>
    /// <returns>True if the value was successfully written, false otherwise.</returns>
    /// <exception cref="Exception">Thrown when the write operation fails after the allowed attempts.</exception>
    public static bool StorageWriteRecursive(ulong keyLen, ulong keyPtr, ulong valueLen, ulong valuePtr, int attempt)
    {
        ulong result = NearSystemImports.StorageWrite(keyLen, keyPtr, valueLen, valuePtr, EvictedRegister);

        if (result == 1)
        {
            return true;
        }

        if (result == 0 && attempt < 1)
        {
            return StorageWriteRecursive(keyLen, keyPtr, valueLen, valuePtr, attempt + 1);
        }

        throw new Exception(ErrFailedToWriteValueInStorage);
    }

    /// <summary>
    /// Reads the value associated with the given key from storage.
    /// </summary>
    /// <param name="key">The key to read the value for.</param>
    /// <returns>The value associated with the key.</returns>
    /// <exception cref="Exception">Thrown when the read operation fails.</exception>
    public static byte[] StorageRead(byte[] key)
    {
        if (key == null || key.Length == 0)
        {
            throw new Exception(ErrKeyIsEmpty);
        }

        ulong keyLen = (ulong)key.Length;
        ulong result;

        fixed (byte* keyPtr = key)
        {
            result = NearSystemImports.StorageRead(keyLen, (ulong)keyPtr, AtomicOpRegister);
        }

        if (result == 0)
        {
            throw new Exception(ErrFailedToReadKey);
        }

        byte[] value = ReadRegisterSafe(AtomicOpRegister);
        if (value == null)
        {
            throw new Exception(ErrFailedToReadRegister);
        }

        return value;
    }

    /// <summary>
    /// Removes the value associated with the given key from storage.
    /// </summary>
    /// <param name="key">The key to remove.</param>
    /// <returns>True if the value was successfully removed, false otherwise.</returns>
    /// <exception cref="Exception">Thrown when the key is empty or if the remove operation fails.</exception>
    public static bool StorageRemove(byte[] key)
    {
        if (key == null || key.Length == 0)
        {
            throw new Exception(ErrKeyIsEmpty);
        }

        ulong keyLen = (ulong)key.Length;
        ulong result;

        fixed (byte* keyPtr = key)
        {
            result = NearSystemImports.StorageRemove(keyLen, (ulong)keyPtr, EvictedRegister);
        }

        if (result == 0)
        {
            throw new Exception(ErrCantRemoveDataByKey);
        }

        return true;
    }

    /// <summary>
    /// Reads the value from the evicted register.
    /// </summary>
    /// <returns>The value read from the evicted register.</returns>
    /// <exception cref="Exception">Thrown when the read operation fails.</exception>
    public static byte[] StorageGetEvicted()
    {
        try
        {
            byte[] value = ReadRegisterSafe(EvictedRegister);
            return value;
        }
        catch (Exception ex)
        {
            throw new Exception($"{ErrFailedToReadEvictedRegister} {ex.Message}");
        }
    }

    /// <summary>
    /// Checks if the given key exists in storage.
    /// </summary>
    /// <param name="key">The key to check for existence.</param>
    /// <returns>True if the key exists, false otherwise.</returns>
    /// <exception cref="Exception">Thrown when the key is empty.</exception>
    public static bool StorageHasKey(byte[] key)
    {
        if (key == null || key.Length == 0)
        {
            throw new Exception(ErrKeyIsEmpty);
        }

        ulong keyLen = (ulong)key.Length;
        ulong result;

        fixed (byte* keyPtr = key)
        {
            result = NearSystemImports.StorageHasKey(keyLen, (ulong)keyPtr);
        }

        return result == 1;
    }

    /// <summary>
    /// Writes the given data to the state.
    /// </summary>
    /// <param name="data">The data to write.</param>
    /// <exception cref="Exception">Thrown when the write operation fails.</exception>
    public static void StateWrite(byte[] data)
    {
        ulong keyLen = (ulong)StateKey.Length;
        ulong valueLen = (ulong)data.Length;

        fixed (byte* keyPtr = StateKey)
        fixed (byte* valuePtr = data)
        {
            StorageWriteRecursive(keyLen, (ulong)keyPtr, valueLen, (ulong)valuePtr, 0);
        }
    }

    /// <summary>
    /// Reads the data from the state.
    /// </summary>
    /// <returns>The data read from the state.</returns>
    /// <exception cref="Exception">Thrown when the read operation fails.</exception>
    public static byte[] StateRead()
    {
        ulong keyLen = (ulong)StateKey.Length;
        ulong result;

        fixed (byte* keyPtr = StateKey)
        {
            result = NearSystemImports.StorageRead(keyLen, (ulong)keyPtr, EvictedRegister);
        }

        if (result == 0)
        {
            throw new Exception(ErrStateNotFound);
        }

        byte[] data = ReadRegisterSafe(EvictedRegister);
        if (data == null)
        {
            throw new Exception(ErrFailedToReadRegister);
        }

        return data;
    }

    /// <summary>
    /// Checks if the state exists.
    /// </summary>
    /// <returns>True if the state exists, false otherwise.</returns>
    public static bool StateExists()
    {
        ulong keyLen = (ulong)StateKey.Length;
        ulong result;

        fixed (byte* keyPtr = StateKey)
        {
            result = NearSystemImports.StorageHasKey(keyLen, (ulong)keyPtr);
        }

        return result == 1;
    }

    // Context API

    /// <summary>
    /// Checks if the provided account ID is valid.
    /// </summary>
    /// <param name="data">The account ID data to validate.</param>
    /// <returns>The valid account ID as a string.</returns>
    /// <exception cref="Exception">Thrown when the account ID is invalid.</exception>
    public static string AssertValidAccountId(byte[] data)
    {
        if (data == null || data.Length == 0)
        {
            throw new Exception(ErrInvalidAccountID);
        }
        return Encoding.UTF8.GetString(data);
    }

    /// <summary>
    /// Retrieves the current account ID.
    /// </summary>
    /// <returns>The current account ID.</returns>
    /// <exception cref="Exception">Thrown when the retrieval fails.</exception>
    public static string GetCurrentAccountId()
    {
        try
        {
            byte[] data = MethodIntoRegister(registerId => NearSystemImports.CurrentAccountId(registerId));
            return AssertValidAccountId(data);
        }
        catch (Exception ex)
        {
            LogString($"Error in GetCurrentAccountId: {ex.Message}");
            throw;
        }
    }

    /// <summary>
    /// Retrieves the signer account ID.
    /// </summary>
    /// <returns>The signer account ID.</returns>
    /// <exception cref="Exception">Thrown when the retrieval fails.</exception>
    public static string GetSignerAccountID()
    {
        try
        {
            byte[] data = MethodIntoRegister(registerId => NearSystemImports.SignerAccountId(registerId));
            return AssertValidAccountId(data);
        }
        catch (Exception ex)
        {
            LogString($"Error in GetSignerAccountID: {ex.Message}");
            throw;
        }
    }

    /// <summary>
    /// Retrieves the public key of the signer account.
    /// </summary>
    /// <returns>The public key of the signer account.</returns>
    /// <exception cref="Exception">Thrown when the retrieval fails.</exception>
    public static byte[] GetSignerAccountPK()
    {
        try
        {
            byte[] data = MethodIntoRegister(registerId => NearSystemImports.SignerAccountPk(registerId));
            return data;
        }
        catch (Exception ex)
        {
            LogString($"Error in GetSignerAccountPK: {ex.Message}");
            throw;
        }
    }

    /// <summary>
    /// Retrieves the predecessor account ID.
    /// </summary>
    /// <returns>The predecessor account ID.</returns>
    /// <exception cref="Exception">Thrown when the retrieval fails.</exception>
    public static string GetPredecessorAccountID()
    {
        try
        {
            byte[] data = MethodIntoRegister(registerId => NearSystemImports.PredecessorAccountId(registerId));
            return AssertValidAccountId(data);
        }
        catch (Exception ex)
        {
            LogString($"Error in GetPredecessorAccountID: {ex.Message}");
            throw;
        }
    }

    /// <summary>
    /// Retrieves the current block height.
    /// </summary>
    /// <returns>The current block height.</returns>
    public static ulong GetCurrentBlockHeight()
    {
        return NearSystemImports.BlockTimestamp();
    }

    /// <summary>
    /// Retrieves the block time in milliseconds.
    /// </summary>
    /// <returns>The block time in milliseconds.</returns>
    public static ulong GetBlockTimeMs()
    {
        return NearSystemImports.BlockTimestamp() / 1_000_000;
    }

    /// <summary>
    /// Retrieves the current epoch height.
    /// </summary>
    /// <returns>The current epoch height.</returns>
    public static ulong GetEpochHeight()
    {
        return NearSystemImports.EpochHeight();
    }

    /// <summary>
    /// Retrieves the storage usage.
    /// </summary>
    /// <returns>The storage usage.</returns>
    public static ulong GetStorageUsage()
    {
        return NearSystemImports.StorageUsage();
    }

    /// <summary>
    /// Detects the type of input data based on the provided key path.
    /// </summary>
    /// <param name="decodedData">The decoded data to analyze.</param>
    /// <param name="keyPath">The key path to locate the specific data element.</param>
    /// <returns>A tuple containing the detected value, type, and any error.</returns>
    public static (byte[] Value, string Type, Exception Error) DetectInputType(byte[] decodedData, params string[] keyPath)
    {
        // TODO: Implement JSON parsing logic similar to jsonparser.Get
        // This is a placeholder - you'll need to implement actual JSON parsing
        // using System.Text.Json or Newtonsoft.Json

        try
        {
            // Placeholder implementation
            // In actual implementation, parse JSON and detect type
            return (decodedData, "object", null);
        }
        catch (Exception ex)
        {

            LogString($"Error in GetContractInput: {ErrFailedToParseInput}");
            return (null, "unknown", ex);
        }
    }

    /// <summary>
    /// Retrieves the input data for the contract.
    /// </summary>
    /// <param name="options">Options specifying how to handle the input data.</param>
    /// <returns>A tuple containing the input data, type, and any error.</returns>
    public static (byte[] Data, string Type, Exception Error) ContractInput(ContractInputOptions options)
    {
        try
        {
            byte[] data = MethodIntoRegister(registerId => NearSystemImports.Input(registerId));

            if (options.IsRawBytes)
            {
                return (data, "rawBytes", null);
            }

            var (parsedData, detectedType, error) = DetectInputType(data);
            if (error != null)
            {
                LogString($"Failed to detect input type: {error.Message}");
                return (null, "", error);
            }

            return (parsedData, detectedType, null);
        }
        catch (Exception ex)
        {
            LogString($"Error in GetContractInput: {ex.Message}");
            return (null, "", ex);
        }
    }


    public static (byte[] Data, string Type, Exception Error) ContractInputRaw()
    {
        try
        {
            // Always load raw bytes from the register
            byte[] data = MethodIntoRegister(registerId => NearSystemImports.Input(registerId));

            // Return raw bytes without parsing, type is fixed as "rawBytes"
            return (data, "rawBytes", null);
        }
        catch (Exception ex)
        {
            LogString($"Error in ContractInputRaw: {ex.Message}");
            return (null, "", ex);
        }
    }


    // Economics API

    /// <summary>
    /// Retrieves the current account balance.
    /// </summary>
    /// <returns>The current account balance.</returns>
    /// <exception cref="Exception">Thrown when the retrieval fails.</exception>
    public static Uint128 GetAccountBalance()
    {
        byte[] data = new byte[16];
        fixed (byte* ptr = data)
        {
            NearSystemImports.AccountBalance((ulong)ptr);
        }

        try
        {
            Uint128 accountBalance = Uint128.LoadLE(data);
            return accountBalance;
        }
        catch (Exception)
        {
            throw new Exception(ErrGettingAccountBalance);
        }
    }

    /// <summary>
    /// Retrieves the locked balance of the account.
    /// </summary>
    /// <returns>The locked balance of the account.</returns>
    /// <exception cref="Exception">Thrown when the retrieval fails.</exception>
    public static Uint128 GetAccountLockedBalance()
    {
        byte[] data = new byte[16];
        fixed (byte* ptr = data)
        {
            NearSystemImports.AccountLockedBalance((ulong)ptr);
        }

        try
        {
            Uint128 accountBalance = Uint128.LoadLE(data);
            return accountBalance;
        }
        catch (Exception)
        {
            throw new Exception(ErrGettingLockedAccountBalance);
        }
    }

    /// <summary>
    /// Retrieves the attached deposit.
    /// </summary>
    /// <returns>The attached deposit.</returns>
    /// <exception cref="Exception">Thrown when the retrieval fails.</exception>
    public static Uint128 GetAttachedDeposit()
    {
        byte[] data = new byte[16];
        fixed (byte* ptr = data)
        {
            NearSystemImports.AttachedDeposit((ulong)ptr);
        }

        try
        {
            Uint128 attachedDeposit = Uint128.LoadLE(data);
            return attachedDeposit;
        }
        catch (Exception)
        {
            throw new Exception(ErrGettingAttachedDeposit);
        }
    }

    /// <summary>
    /// Retrieves the prepaid gas.
    /// </summary>
    /// <returns>The prepaid gas.</returns>
    public static NearGas GetPrepaidGas()
    {
        return new NearGas { Inner = NearSystemImports.PrepaidGas() };
    }

    /// <summary>
    /// Retrieves the used gas.
    /// </summary>
    /// <returns>The used gas.</returns>
    public static NearGas GetUsedGas()
    {
        return new NearGas { Inner = NearSystemImports.UsedGas() };
    }

    /// <summary>
    /// Logs a string message.
    /// </summary>
    /// <param name="message">The message to log.</param>
    public static void LogString(string message)
    {
        byte[] messageBytes = Encoding.UTF8.GetBytes(message);
        fixed (byte* ptr = messageBytes)
        {
            NearSystemImports.LogUtf8((ulong)messageBytes.Length, (ulong)ptr);
        }
    }
}



// Supporting types

/// <summary>
/// Options for contract input handling.
/// </summary>
public struct ContractInputOptions
{
    public bool IsRawBytes { get; set; }
}

/// <summary>
/// Represents a 128-bit unsigned integer.
/// </summary>
public struct Uint128
{
    public ulong Hi { get; set; }
    public ulong Lo { get; set; }

    /// <summary>
    /// Loads a Uint128 from a little-endian byte array.
    /// </summary>
    /// <param name="data">The byte array containing the data.</param>
    /// <returns>The loaded Uint128 value.</returns>
    public static Uint128 LoadLE(byte[] data)
    {
        if (data == null || data.Length != 16)
        {
            throw new ArgumentException("Data must be exactly 16 bytes");
        }

        ulong lo = BitConverter.ToUInt64(data, 0);
        ulong hi = BitConverter.ToUInt64(data, 8);

        return new Uint128 { Lo = lo, Hi = hi };
    }

    /// <summary>
    /// Stores this Uint128 as a little-endian byte array.
    /// </summary>
    /// <returns>The byte array representation.</returns>
    public byte[] StoreLE()
    {
        byte[] result = new byte[16];
        BitConverter.GetBytes(Lo).CopyTo(result, 0);
        BitConverter.GetBytes(Hi).CopyTo(result, 8);
        return result;
    }
}

/// <summary>
/// Represents NEAR gas units.
/// </summary>
public struct NearGas
{
    public ulong Inner { get; set; }
}