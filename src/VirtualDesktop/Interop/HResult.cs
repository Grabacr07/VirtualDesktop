using System;
using System.Linq;

namespace WindowsDesktop.Interop;

internal enum HResult : uint
{
    // ReSharper disable IdentifierTypo
    TYPE_E_OUTOFBOUNDS = 0x80028CA1,
    TYPE_E_ELEMENTNOTFOUND = 0x8002802B,
    REGDB_E_CLASSNOTREG = 0x80040154,
    RPC_S_SERVER_UNAVAILABLE = 0x800706BA,
    // ReSharper restore IdentifierTypo
}

internal static class HResultExtensions
{
    public static bool Match(this Exception ex, params HResult[] hResult)
    {
        return hResult
            .Cast<uint>()
            .Any(x => (uint)ex.HResult == x);
    }
}
