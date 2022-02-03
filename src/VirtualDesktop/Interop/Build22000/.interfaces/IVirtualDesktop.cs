using System;
using System.Runtime.InteropServices;
using WindowsDesktop.Interop.Build10240;

namespace WindowsDesktop.Interop.Build22000
{
    // ## Note
    // .NET 5 has removed WinRT support, so HString cannot marshal to string.
    // Since marshalling with UnmanagedType.HString fails, use IntPtr to extract the string via CsWinRT MarshalString.
    // 
    // see also: https://github.com/microsoft/CsWinRT/blob/master/docs/interop.md

    [ComImport]
    [Guid("00000000-0000-0000-0000-000000000000") /* replace at runtime */]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IVirtualDesktop
    {
        bool IsViewVisible(IApplicationView view);

        Guid GetID();

        IntPtr Proc5();

        IntPtr GetName();

        IntPtr GetWallpaperPath();
    }
}
