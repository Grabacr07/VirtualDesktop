using System;
using System.Collections.Generic;
using System.Linq;
using WinRT;

namespace WindowsDesktop.Interop;

internal static class HString
{
    public static string MarshalFromHString(this IntPtr hstr)
        => MarshalString.FromAbi(hstr);

    public static IntPtr MarshalToHString(this string str)
        => MarshalString.GetAbi(MarshalString.CreateMarshaler(str));
}
