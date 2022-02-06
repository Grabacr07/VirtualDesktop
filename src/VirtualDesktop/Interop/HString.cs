using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using WinRT;

namespace WindowsDesktop.Interop;

// ## Note
// .NET 5 has removed WinRT support, so HString cannot marshal to System.String.
// Since marshalling with UnmanagedType.HString fails, use IntPtr to get the string via C#/WinRT MarshalString.
// 
// see also: https://github.com/microsoft/CsWinRT/blob/master/docs/interop.md

[StructLayout(LayoutKind.Sequential)]
public struct HString
{
    private readonly IntPtr _abi;

    internal HString(string str)
    {
        this._abi = MarshalString.GetAbi(MarshalString.CreateMarshaler(str));
    }
    
    public static implicit operator string(HString hStr)
        => MarshalString.FromAbi(hStr._abi);
}
