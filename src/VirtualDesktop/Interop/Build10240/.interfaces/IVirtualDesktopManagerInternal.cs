using System;
using System.Runtime.InteropServices;
using Windows.Win32.UI.Shell.Common;

namespace WindowsDesktop.Interop.Build10240
{
    [ComImport]
    [Guid("00000000-0000-0000-0000-000000000000") /* replace at runtime */]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IVirtualDesktopManagerInternal
    {
        int GetCount();

        void MoveViewToDesktop(IApplicationView pView, IVirtualDesktop desktop);

        bool CanViewMoveDesktops(IApplicationView pView);

        IVirtualDesktop GetCurrentDesktop();

        IObjectArray GetDesktops();

        IVirtualDesktop GetAdjacentDesktop(IVirtualDesktop pDesktopReference, AdjacentDesktop uDirection);

        void SwitchDesktop(IVirtualDesktop desktop);

        IVirtualDesktop CreateDesktop();

        void RemoveDesktop(IVirtualDesktop pRemove, IVirtualDesktop pFallbackDesktop);

        IVirtualDesktop FindDesktop(in Guid desktopId);
    }
}
