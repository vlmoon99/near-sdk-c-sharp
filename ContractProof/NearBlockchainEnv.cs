using System;
using System.Runtime.InteropServices;

//1. TODO: Think about how to use ulong instead of long, now it gives the compilation error

namespace ContractProof;

public unsafe static class NearBlockchainEnv
{
    public const string RegisterExpectedErr = "Register was expected to have data because we just wrote it into it.";

    public const long AtomicOpRegister = long.MaxValue - 2;

    public const long EvictedRegister = long.MaxValue - 1;

    public const long DataIdRegister = 0;

    public static readonly byte[] StateKey = System.Text.Encoding.UTF8.GetBytes("STATE");

    public const long MinAccountIDLen = 2;

    public const long MaxAccountIDLen = 64;

}