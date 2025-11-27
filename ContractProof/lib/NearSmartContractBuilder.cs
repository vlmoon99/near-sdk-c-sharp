using System;
using System.Runtime.InteropServices;
using System.Text;

namespace ContractProof;

/// <summary>
/// Builder class for creating NEAR smart contracts with minimal boilerplate.
/// Provides helper methods for common contract operations.
/// </summary>
public unsafe static class NearSmartContractBuilder
{
    #region Return Operations

    /// <summary>
    /// Returns a string value from the contract.
    /// </summary>
    /// <param name="value">The string value to return.</param>
    public static void ReturnValue(string value)
    {
        byte[] utf8Bytes = Encoding.UTF8.GetBytes(value);
        fixed (byte* ptr = utf8Bytes)
        {
            NearSystemImports.ValueReturn((ulong)utf8Bytes.Length, (ulong)ptr);
        }
    }

    /// <summary>
    /// Returns raw bytes from the contract.
    /// </summary>
    /// <param name="value">The byte array to return.</param>
    public static void ReturnValue(byte[] value)
    {
        if (value == null || value.Length == 0)
        {
            value = Array.Empty<byte>();
        }
        
        fixed (byte* ptr = value)
        {
            NearSystemImports.ValueReturn((ulong)value.Length, (ulong)ptr);
        }
    }

    #endregion

    #region Logging Operations

    /// <summary>
    /// Logs a string message.
    /// </summary>
    /// <param name="message">The message to log.</param>
    public static void Log(string message)
    {
        NearBlockchainEnv.LogString(message);
    }

    /// <summary>
    /// Logs raw bytes.
    /// </summary>
    /// <param name="data">The data to log.</param>
    public static void Log(byte[] data)
    {
        string message = Encoding.UTF8.GetString(data);
        NearBlockchainEnv.LogString(message);
    }

    #endregion

    #region Input Operations

    /// <summary>
    /// Gets the raw input bytes from the contract call.
    /// </summary>
    /// <returns>The raw input bytes.</returns>
    /// <exception cref="Exception">Thrown when input reading fails.</exception>
    public static byte[] GetInputRaw()
    {
        var (data, _, error) = NearBlockchainEnv.ContractInputRaw();
        
        if (error != null)
        {
            throw error;
        }
        
        return data ?? Array.Empty<byte>();
    }

    /// <summary>
    /// Gets the input as a UTF-8 string. Useful for JSON input that you want to process as a string.
    /// </summary>
    /// <returns>The input decoded as UTF-8 string.</returns>
    public static string GetInputString()
    {
        byte[] data = GetInputRaw();
        
        if (data == null || data.Length == 0)
        {
            return string.Empty;
        }
        
        return Encoding.UTF8.GetString(data);
    }

    #endregion

    #region Error Handling

    /// <summary>
    /// Wraps a contract method with error handling.
    /// </summary>
    /// <param name="action">The action to execute.</param>
    /// <param name="logErrors">Whether to log errors when they occur.</param>
    public static void Execute(Action action, bool logErrors = true)
    {
        try
        {
            action();
        }
        catch (Exception ex)
        {
            if (logErrors)
            {
                Log($"Error: {ex.Message}");
            }
            throw;
        }
    }

    #endregion

    #region Common Contract Patterns

    /// <summary>
    /// Creates a simple log-only contract method.
    /// Executes with automatic error handling.
    /// </summary>
    /// <param name="message">The message to log.</param>
    public static void LogMethod(string message)
    {
        Execute(() => Log(message));
    }

    /// <summary>
    /// Creates a simple return-value contract method.
    /// Executes with automatic error handling.
    /// </summary>
    /// <param name="value">The value to return.</param>
    public static void ReturnMethod(string value)
    {
        Execute(() => ReturnValue(value));
    }

    /// <summary>
    /// Creates an echo method that returns the input.
    /// Executes with automatic error handling.
    /// </summary>
    public static void EchoMethod()
    {
        Execute(() =>
        {
            var input = GetInputString();
            ReturnValue(input);
        });
    }

    #endregion

    #region Context Information Helpers

    /// <summary>
    /// Gets the current account ID.
    /// </summary>
    /// <returns>The current account ID.</returns>
    public static string GetCurrentAccountId()
    {
        return NearBlockchainEnv.GetCurrentAccountId();
    }

    /// <summary>
    /// Gets the signer account ID.
    /// </summary>
    /// <returns>The signer account ID.</returns>
    public static string GetSignerAccountId()
    {
        return NearBlockchainEnv.GetSignerAccountID();
    }

    /// <summary>
    /// Gets the predecessor account ID.
    /// </summary>
    /// <returns>The predecessor account ID.</returns>
    public static string GetPredecessorAccountId()
    {
        return NearBlockchainEnv.GetPredecessorAccountID();
    }

    /// <summary>
    /// Gets the block timestamp in milliseconds.
    /// </summary>
    /// <returns>The block timestamp in milliseconds.</returns>
    public static ulong GetBlockTimeMs()
    {
        return (ulong)NearBlockchainEnv.GetBlockTimeMs();
    }

    #endregion

    #region Storage Helpers

    /// <summary>
    /// Writes a string value to storage.
    /// </summary>
    /// <param name="key">The storage key.</param>
    /// <param name="value">The string value to store.</param>
    public static void StorageWrite(string key, string value)
    {
        byte[] keyBytes = Encoding.UTF8.GetBytes(key);
        byte[] valueBytes = Encoding.UTF8.GetBytes(value);
        NearBlockchainEnv.StorageWrite(keyBytes, valueBytes);
    }

    /// <summary>
    /// Reads a string value from storage.
    /// </summary>
    /// <param name="key">The storage key.</param>
    /// <returns>The stored string value.</returns>
    public static string StorageRead(string key)
    {
        byte[] keyBytes = Encoding.UTF8.GetBytes(key);
        byte[] valueBytes = NearBlockchainEnv.StorageRead(keyBytes);
        return Encoding.UTF8.GetString(valueBytes);
    }

    /// <summary>
    /// Checks if a key exists in storage.
    /// </summary>
    /// <param name="key">The storage key.</param>
    /// <returns>True if the key exists, false otherwise.</returns>
    public static bool StorageHasKey(string key)
    {
        byte[] keyBytes = Encoding.UTF8.GetBytes(key);
        return NearBlockchainEnv.StorageHasKey(keyBytes);
    }

    #endregion
}