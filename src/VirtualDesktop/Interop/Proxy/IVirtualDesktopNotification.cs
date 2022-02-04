namespace WindowsDesktop.Interop.Proxy;

[ComInterface]
public interface IVirtualDesktopNotification
{
    void VirtualDesktopCreated(IVirtualDesktop pDesktop);

    void VirtualDesktopDestroyBegin(IVirtualDesktop pDesktopDestroyed, IVirtualDesktop pDesktopFallback);

    void VirtualDesktopDestroyFailed(IVirtualDesktop pDesktopDestroyed, IVirtualDesktop pDesktopFallback);

    void VirtualDesktopDestroyed(IVirtualDesktop pDesktopDestroyed, IVirtualDesktop pDesktopFallback);

    void VirtualDesktopMoved(IVirtualDesktop pDesktop, int nIndexFrom, int nIndexTo);

    void VirtualDesktopRenamed(IVirtualDesktop pDesktop, string chName);

    void ViewVirtualDesktopChanged(IApplicationView pView);

    void CurrentVirtualDesktopChanged(IVirtualDesktop pDesktopOld, IVirtualDesktop pDesktopNew);

    void VirtualDesktopWallpaperChanged(IVirtualDesktop pDesktop, string chPath);
}
