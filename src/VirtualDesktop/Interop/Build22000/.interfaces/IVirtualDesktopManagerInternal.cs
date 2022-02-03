using System;
using System.Runtime.InteropServices;
using Windows.Win32.UI.Shell.Common;
using WindowsDesktop.Interop.Build10240;

namespace WindowsDesktop.Interop.Build22000
{
    [ComImport]
    [Guid("00000000-0000-0000-0000-000000000000") /* replace at runtime */]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IVirtualDesktopManagerInternal
    {
        int GetCount(IntPtr hWndOrMon);

        void MoveViewToDesktop(IApplicationView pView, IVirtualDesktop desktop);

        bool CanViewMoveDesktops(IApplicationView pView);

        IVirtualDesktop GetCurrentDesktop(IntPtr hWndOrMon);

        IObjectArray GetDesktops(IntPtr hWndOrMon);

        IVirtualDesktop GetAdjacentDesktop(IVirtualDesktop pDesktopReference, AdjacentDesktop uDirection);

        void SwitchDesktop(IntPtr hWndOrMon, IVirtualDesktop desktop);

        IVirtualDesktop CreateDesktop(IntPtr hWndOrMon);

        void MoveDesktop(IVirtualDesktop desktop, IntPtr hWndOrMon, int nIndex);

        void RemoveDesktop(IVirtualDesktop pRemove, IVirtualDesktop pFallbackDesktop);

        IVirtualDesktop FindDesktop(in Guid desktopId);

        void GetDesktopSwitchIncludeExcludeViews(IVirtualDesktop desktop, out IObjectArray o1, out IObjectArray o2);

        void SetDesktopName(IVirtualDesktop desktop, IntPtr name);

        void SetDesktopWallpaper(IVirtualDesktop desktop, IntPtr path);

        void UpdateWallpaperPathForAllDesktops(IntPtr path);

        void CopyDesktopState(IApplicationView pView0, IApplicationView pView1);

        bool GetDesktopIsPerMonitor();

        void SetDesktopIsPerMonitor(bool state);
    }
}
