using System;
using System.Runtime.InteropServices;
using WindowsDesktop.Interop.Build10240;

namespace WindowsDesktop.Interop.Build22000
{
    [ComImport]
    [Guid("00000000-0000-0000-0000-000000000000") /* replace at runtime */]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IVirtualDesktopNotification
    {
        void VirtualDesktopCreated(IObjectArray p0, IVirtualDesktop pDesktop);

        void VirtualDesktopDestroyBegin(IObjectArray p0, IVirtualDesktop pDesktopDestroyed, IVirtualDesktop pDesktopFallback);

        void VirtualDesktopDestroyFailed(IObjectArray p0, IVirtualDesktop pDesktopDestroyed, IVirtualDesktop pDesktopFallback);

        void VirtualDesktopDestroyed(IObjectArray p0, IVirtualDesktop pDesktopDestroyed, IVirtualDesktop pDesktopFallback);

        void Proc7(int p0);

        void VirtualDesktopMoved(IObjectArray p0, IVirtualDesktop pDesktop, int nIndexFrom, int nIndexTo);

        void VirtualDesktopRenamed(IVirtualDesktop pDesktop, HString chName);

        void ViewVirtualDesktopChanged(IApplicationView pView);

        void CurrentVirtualDesktopChanged(IObjectArray p0, IVirtualDesktop pDesktopOld, IVirtualDesktop pDesktopNew);

        void VirtualDesktopWallpaperChanged(IVirtualDesktop pDesktop, HString chPath);
    }

    internal class VirtualDesktopNotification : VirtualDesktopNotificationService.EventListenerBase, IVirtualDesktopNotification
    {
        public void VirtualDesktopCreated(IObjectArray p0, IVirtualDesktop pDesktop)
        {
            this.CreatedCore(pDesktop);
        }

        public void VirtualDesktopDestroyBegin(IObjectArray p0, IVirtualDesktop pDesktopDestroyed, IVirtualDesktop pDesktopFallback)
        {
            this.DestroyBeginCore(pDesktopDestroyed, pDesktopFallback);
        }

        public void VirtualDesktopDestroyFailed(IObjectArray p0, IVirtualDesktop pDesktopDestroyed, IVirtualDesktop pDesktopFallback)
        {
            this.DestroyFailedCore(pDesktopDestroyed, pDesktopFallback);
        }

        public void VirtualDesktopDestroyed(IObjectArray p0, IVirtualDesktop pDesktopDestroyed, IVirtualDesktop pDesktopFallback)
        {
            this.DestroyedCore(pDesktopDestroyed, pDesktopFallback);
        }

        public void Proc7(int p0)
        {
        }

        public void VirtualDesktopMoved(IObjectArray p0, IVirtualDesktop pDesktop, int nIndexFrom, int nIndexTo)
        {
            this.MovedCore(p0, pDesktop, nIndexFrom, nIndexTo);
        }

        public void VirtualDesktopRenamed(IVirtualDesktop pDesktop, HString chName)
        {
            this.RenamedCore(pDesktop, chName);
        }

        public void ViewVirtualDesktopChanged(IApplicationView pView)
        {
            this.ViewChangedCore(pView);
        }

        public void CurrentVirtualDesktopChanged(IObjectArray p0, IVirtualDesktop pDesktopOld, IVirtualDesktop pDesktopNew)
        {
            this.CurrentChangedCore(pDesktopOld, pDesktopNew);
        }

        public void VirtualDesktopWallpaperChanged(IVirtualDesktop pDesktop, HString chPath)
        {
            this.WallpaperChangedCore(pDesktop, chPath);
        }
    }
}
