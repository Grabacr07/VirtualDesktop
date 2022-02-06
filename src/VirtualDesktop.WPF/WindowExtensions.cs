using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;

namespace WindowsDesktop;

public static class WindowExtensions
{
    /// <summary>
    /// Determines whether this window is on the current virtual desktop.
    /// </summary>
    public static bool IsCurrentVirtualDesktop(this Window window)
    {
        return VirtualDesktop.IsCurrentVirtualDesktop(window.GetHandle());
    }

    /// <summary>
    /// Returns the virtual desktop this window is located on.
    /// </summary>
    public static VirtualDesktop? GetCurrentDesktop(this Window window)
    {
        return VirtualDesktop.FromHwnd(window.GetHandle());
    }

    /// <summary>
    /// Moves a window to the specified virtual desktop.
    /// </summary>
    public static void MoveToDesktop(this Window window, VirtualDesktop virtualDesktop)
    {
        VirtualDesktop.MoveToDesktop(window.GetHandle(), virtualDesktop);
    }

    /// <summary>
    /// Switches to a virtual desktop and moves the specified window to that desktop.
    /// </summary>
    /// <param name="virtualDesktop">The virtual desktop to move the window to.</param>
    /// <param name="window">The window to move.</param>
    public static void SwitchAndMove(this VirtualDesktop virtualDesktop, Window window)
    {
        if (window.IsPinned() == false) window.MoveToDesktop(virtualDesktop);
        virtualDesktop.Switch();
    }

    /// <summary>
    /// Determines whether this window is pinned.
    /// </summary>
    /// <returns><see langword="true" /> if pinned, <see langword="false" /> otherwise.</returns>
    public static bool IsPinned(this Window window)
    {
        return VirtualDesktop.IsPinnedWindow(window.GetHandle());
    }

    /// <summary>
    /// Pins a window, showing it on all virtual desktops.
    /// </summary>
    /// <returns><see langword="true" /> if already pinned or successfully pinned, <see langword="false" /> otherwise (most of the time, the target window is not found or not ready).</returns>
    public static bool Pin(this Window window)
    {
        return VirtualDesktop.PinWindow(window.GetHandle());
    }

    /// <summary>
    /// Unpins a window.
    /// </summary>
    /// <returns><see langword="true" /> if already unpinned or successfully unpinned, <see langword="false" /> otherwise (most of the time, the target window is not found or not ready).</returns>
    public static bool Unpin(this Window window)
    {
        return VirtualDesktop.UnpinWindow(window.GetHandle());
    }

    /// <summary>
    /// Toggles a window between being pinned and unpinned.
    /// </summary>
    /// <returns><see langword="true" /> if successfully toggled, <see langword="false" /> otherwise (most of the time, the target window is not found or not ready).</returns>
    public static bool TogglePin(this Window window)
    {
        var handle = window.GetHandle();

        return VirtualDesktop.IsPinnedWindow(handle)
            ? VirtualDesktop.UnpinWindow(handle)
            : VirtualDesktop.PinWindow(handle);
    }

    /// <summary>
    /// Returns the window handle for this <see cref="Visual" />.
    /// </summary>
    public static IntPtr GetHandle(this Visual visual)
        => PresentationSource.FromVisual(visual) is HwndSource hwndSource
            ? hwndSource.Handle
            : throw new ArgumentException("Unable to get a window handle. Call it after the Window.SourceInitialized event is fired.", nameof(visual));
}
