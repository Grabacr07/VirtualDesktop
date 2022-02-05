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
    public static bool IsPinned(this Window window)
    {
        return VirtualDesktop.IsPinnedWindow(window.GetHandle());
    }

    /// <summary>
    /// Pins a window, showing it on all virtual desktops.
    /// </summary>
    public static void Pin(this Window window)
    {
        VirtualDesktop.PinWindow(window.GetHandle());
    }

    /// <summary>
    /// Unpins a window.
    /// </summary>
    public static void Unpin(this Window window)
    {
        VirtualDesktop.UnpinWindow(window.GetHandle());
    }

    /// <summary>
    /// Toggles a window between being pinned and unpinned.
    /// </summary>
    public static void TogglePin(this Window window)
    {
        var handle = window.GetHandle();

        if (VirtualDesktop.IsPinnedWindow(handle))
        {
            VirtualDesktop.UnpinWindow(handle);
        }
        else
        {
            VirtualDesktop.PinWindow(handle);
        }
    }

    /// <summary>
    /// Returns the window handle for this <see cref="Visual" />.
    /// </summary>
    public static IntPtr GetHandle(this Visual visual)
        => PresentationSource.FromVisual(visual) is HwndSource hwndSource
            ? hwndSource.Handle
            : throw new ArgumentException("Unable to get a window handle.", nameof(visual));
}
