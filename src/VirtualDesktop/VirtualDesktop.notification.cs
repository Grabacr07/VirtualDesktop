using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using WindowsDesktop.Interop.Proxy;
using WindowsDesktop.Utils;

namespace WindowsDesktop;

partial class VirtualDesktop
{
    private static readonly ConcurrentDictionary<IntPtr, ViewChangedListener> _viewChangedEventListeners = new();

    /// <summary>
    /// Occurs when a virtual desktop is created.
    /// </summary>
    /// <remarks>
    /// See <see cref="CurrentChanged"/> for details.
    /// </remarks>
    public static event EventHandler<VirtualDesktop>? Created;

    public static event EventHandler<VirtualDesktopDestroyEventArgs>? DestroyBegin;

    public static event EventHandler<VirtualDesktopDestroyEventArgs>? DestroyFailed;

    /// <summary>
    /// Occurs when a virtual desktop is destroyed.
    /// </summary>
    /// <remarks>
    /// See <see cref="CurrentChanged"/> for details.
    /// </remarks>
    public static event EventHandler<VirtualDesktopDestroyEventArgs>? Destroyed;

    /// <summary>
    /// Occurs when the current virtual desktop is changed.
    /// </summary>
    /// <remarks>
    /// The internal initialization is triggered by the call of a static property/method.<br/>
    /// Therefore, events are not fired just by subscribing to them.<br/>
    /// <br/>
    /// If you want to use only event subscription, the following code is recommended.<br/>
    /// <code>
    /// VirtualDesktop.Configuration();
    /// </code>
    /// </remarks>
    public static event EventHandler<VirtualDesktopChangedEventArgs>? CurrentChanged;

    /// <summary>
    /// Occurs when the virtual desktop is moved.
    /// </summary>
    /// <remarks>
    /// See <see cref="CurrentChanged"/> for details.
    /// </remarks>
    public static event EventHandler<VirtualDesktopMovedEventArgs>? Moved;

    /// <summary>
    /// Occurs when a virtual desktop is renamed.
    /// </summary>
    /// <remarks>
    /// See <see cref="CurrentChanged"/> for details.
    /// </remarks>
    public static event EventHandler<VirtualDesktopRenamedEventArgs>? Renamed;

    /// <summary>
    /// Occurs when a virtual desktop wallpaper is changed.
    /// </summary>
    /// <remarks>
    /// See <see cref="CurrentChanged"/> for details.
    /// </remarks>
    public static event EventHandler<VirtualDesktopWallpaperChangedEventArgs>? WallpaperChanged;

    /// <summary>
    /// Register a listener to receive changes in the application view.
    /// </summary>
    /// <param name="targetHwnd">The target window handle to receive events from. If specify <see cref="IntPtr.Zero"/>, all changes will be delivered.</param>
    /// <param name="action">Action to be performed.</param>
    /// <returns><see cref="IDisposable"/> instance for unsubscribing.</returns>
    public static IDisposable RegisterViewChanged(IntPtr targetHwnd, Action<IntPtr> action)
    {
        InitializeIfNeeded();

        var listener = _viewChangedEventListeners.GetOrAdd(targetHwnd, x => new ViewChangedListener(x));
        listener.Listeners.Add(action);

        return Disposable.Create(() => listener.Listeners.Remove(action));
    }

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
            => Moved?.Invoke(this, new VirtualDesktopMovedEventArgs(pDesktop, nIndexFrom, nIndexTo));

        public void ViewVirtualDesktopChanged(IApplicationView pView)
        {
            if (_viewChangedEventListeners.TryGetValue(IntPtr.Zero, out var all)) all.Call();
            if (_viewChangedEventListeners.TryGetValue(pView.GetThumbnailWindow(), out var listener)) listener.Call();
        }

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

    private class ViewChangedListener
    {
        private readonly IntPtr _targetHandle;

        public List<Action<IntPtr>> Listeners { get; } = new();

        public ViewChangedListener(IntPtr targetHandle)
        {
            this._targetHandle = targetHandle;
        }

        public void Call()
        {
            foreach (var listener in this.Listeners) listener(this._targetHandle);
        }
    }
}
