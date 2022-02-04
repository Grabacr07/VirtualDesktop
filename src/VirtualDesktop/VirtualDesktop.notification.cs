using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using WindowsDesktop.Interop;

namespace WindowsDesktop;

partial class VirtualDesktop
{
    /// <summary>
    /// Occurs when a virtual desktop is created.
    /// </summary>
    public static event EventHandler<VirtualDesktop>? Created;

    public static event EventHandler<VirtualDesktopDestroyEventArgs>? DestroyBegin;

    public static event EventHandler<VirtualDesktopDestroyEventArgs>? DestroyFailed;

    /// <summary>
    /// Occurs when a virtual desktop is destroyed.
    /// </summary>
    public static event EventHandler<VirtualDesktopDestroyEventArgs>? Destroyed;

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static event EventHandler? ApplicationViewChanged;

    /// <summary>
    /// Occurs when the current virtual desktop is changed.
    /// </summary>
    public static event EventHandler<VirtualDesktopChangedEventArgs>? CurrentChanged;

    /// <summary>
    /// Occurs when a virtual desktop is renamed.
    /// </summary>
    public static event EventHandler<VirtualDesktopRenamedEventArgs>? Renamed;

    /// <summary>
    /// Occurs when a virtual desktop wallpaper is changed.
    /// </summary>
    public static event EventHandler<VirtualDesktopWallpaperChangedEventArgs>? WallpaperChanged;

    private class EventProxy : IVirtualDesktopNotification
    {
        public void VirtualDesktopCreated(IVirtualDesktop pDesktop)
            => Created?.Invoke(this, pDesktop.ToVirtualDesktop());

        public void VirtualDesktopDestroyBegin(IVirtualDesktop pDesktopDestroyed, IVirtualDesktop pDesktopFallback)
            => DestroyBegin?.Invoke(this, new VirtualDesktopDestroyEventArgs(pDesktopDestroyed, pDesktopFallback));

        public void VirtualDesktopDestroyFailed(IVirtualDesktop pDesktopDestroyed, IVirtualDesktop pDesktopFallback)
            => DestroyFailed?.Invoke(this, new VirtualDesktopDestroyEventArgs(pDesktopDestroyed, pDesktopFallback));

        public void VirtualDesktopDestroyed(IVirtualDesktop pDesktopDestroyed, IVirtualDesktop pDesktopFallback)
            => Destroyed?.Invoke(this, new VirtualDesktopDestroyEventArgs(pDesktopDestroyed, pDesktopFallback));

        public void VirtualDesktopMoved(IVirtualDesktop pDesktop, int nIndexFrom, int nIndexTo)
        {
        }

        public void ViewVirtualDesktopChanged(IApplicationView pView)
            => ApplicationViewChanged?.Invoke(this, EventArgs.Empty);

        public void CurrentVirtualDesktopChanged(IVirtualDesktop pDesktopOld, IVirtualDesktop pDesktopNew)
            => CurrentChanged?.Invoke(this, new VirtualDesktopChangedEventArgs(pDesktopOld, pDesktopNew));

        public void VirtualDesktopRenamed(IVirtualDesktop pDesktop, string chName)
        {
            var desktop = pDesktop.ToVirtualDesktop();
            desktop._name = chName;

            Renamed?.Invoke(this, new VirtualDesktopRenamedEventArgs(desktop, chName));
        }

        public void VirtualDesktopWallpaperChanged(IVirtualDesktop pDesktop, string chPath)
        {
            var desktop = pDesktop.ToVirtualDesktop();
            desktop._wallpaperPath = chPath;

            WallpaperChanged?.Invoke(this, new VirtualDesktopWallpaperChangedEventArgs(desktop, chPath));
        }
    }
}
