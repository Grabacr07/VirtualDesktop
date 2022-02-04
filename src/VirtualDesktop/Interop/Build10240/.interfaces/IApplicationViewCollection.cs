using System;
using System.Runtime.InteropServices;

namespace WindowsDesktop.Interop.Build10240
{
    [ComImport]
    [Guid("00000000-0000-0000-0000-000000000000") /* replace at runtime */]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IApplicationViewCollection
    {
        IObjectArray GetViews();

        IObjectArray GetViewsByZOrder();

        IObjectArray GetViewsByAppUserModelId(string id);

        IApplicationView GetViewForHwnd(IntPtr hwnd);

        IApplicationView GetViewForApplication(object application);

        IApplicationView GetViewForAppUserModelId(string id);

        IntPtr GetViewInFocus();

        void RefreshCollection();

        int RegisterForApplicationViewChanges(object listener);

        int RegisterForApplicationViewPositionChanges(object listener);

        void UnregisterForApplicationViewChanges(int cookie);
    }
}
