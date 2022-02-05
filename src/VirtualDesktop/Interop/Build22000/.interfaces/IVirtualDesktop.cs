using System;
using System.Runtime.InteropServices;
using WindowsDesktop.Interop.Build10240;

namespace WindowsDesktop.Interop.Build22000
{
    [ComImport]
    [Guid("00000000-0000-0000-0000-000000000000") /* replace at runtime */]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IVirtualDesktop
    {
        bool IsViewVisible(IApplicationView view);

        Guid GetID();

        IntPtr Proc5();

        HString GetName();

        HString GetWallpaperPath();
    }
}
