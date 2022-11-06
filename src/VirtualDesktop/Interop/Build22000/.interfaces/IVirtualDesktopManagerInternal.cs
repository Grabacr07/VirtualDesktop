using System;
using System.Runtime.InteropServices;
using WindowsDesktop.Interop.Build10240;

namespace WindowsDesktop.Interop.Build22000
{
    [ComImport]
    [Guid("00000000-0000-0000-0000-000000000000") /* replace at runtime */]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IVirtualDesktopManagerInternal
    {
        int GetCount(IntPtr hWndOrMon);

        void MoveViewToDesktop(IApplicationView pView, IVirtualDesktop desktop);

        bool CanViewMoveDesktops(IApplicationView pView);

        IVirtualDesktop GetCurrentDesktop(IntPtr hWndOrMon);

        IObjectArray GetAllCurrentDesktops();

        IObjectArray GetDesktops(IntPtr hWndOrMon);

        IVirtualDesktop GetAdjacentDesktop(IVirtualDesktop pDesktopReference, int uDirection);

        void SwitchDesktop(IntPtr hWndOrMon, IVirtualDesktop desktop);

        IVirtualDesktop CreateDesktop(IntPtr hWndOrMon);

        void MoveDesktop(IVirtualDesktop desktop, IntPtr hWndOrMon, int nIndex);

        void RemoveDesktop(IVirtualDesktop pRemove, IVirtualDesktop pFallbackDesktop);

        IVirtualDesktop FindDesktop(in Guid desktopId);

        void GetDesktopSwitchIncludeExcludeViews(IVirtualDesktop desktop, out IObjectArray o1, out IObjectArray o2);

        void SetDesktopName(IVirtualDesktop desktop, HString name);

        void SetDesktopWallpaper(IVirtualDesktop desktop, HString path);

        void UpdateWallpaperPathForAllDesktops(HString path);

        void CopyDesktopState(IApplicationView pView0, IApplicationView pView1);

        bool GetDesktopIsPerMonitor();

        void SetDesktopIsPerMonitor(bool state);
    }
}
