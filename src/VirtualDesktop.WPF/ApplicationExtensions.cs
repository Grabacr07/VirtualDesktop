using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace WindowsDesktop;

public static class ApplicationExtensions
{
    /// <summary>
    /// Determines whether this application is pinned.
    /// </summary>
    /// <returns><see langword="true" /> if pinned, <see langword="false" /> otherwise.</returns>
    public static bool IsPinned(this Application app)
    {
        return VirtualDesktop.TryGetAppUserModelId(app.GetWindowHandle(), out var appId)
            && VirtualDesktop.IsPinnedApplication(appId);
    }

    /// <summary>
    /// Pins an application, showing it on all virtual desktops.
    /// </summary>
    /// <returns><see langword="true" /> if already pinned or successfully pinned, <see langword="false" /> otherwise (most of the time, main window is not found).</returns>
    public static bool Pin(this Application app)
    {
        return VirtualDesktop.TryGetAppUserModelId(app.GetWindowHandle(), out var appId)
            && VirtualDesktop.PinApplication(appId);
    }

    /// <summary>
    /// Unpins an application.
    /// </summary>
    /// <returns><see langword="true" /> if already unpinned or successfully unpinned, <see langword="false" /> otherwise (most of the time, main window is not found).</returns>
    public static bool Unpin(this Application app)
    {
        return VirtualDesktop.TryGetAppUserModelId(app.GetWindowHandle(), out var appId)
            && VirtualDesktop.UnpinApplication(appId);
    }

    /// <summary>
    /// Toggles an application between being pinned and unpinned.
    /// </summary>
    /// <returns><see langword="true" /> if successfully toggled, <see langword="false" /> otherwise (most of the time, main window is not found).</returns>
    public static bool TogglePin(this Application app)
    {
        if (VirtualDesktop.TryGetAppUserModelId(app.GetWindowHandle(), out var appId) == false) return false;

        return VirtualDesktop.IsPinnedApplication(appId) 
            ? VirtualDesktop.UnpinApplication(appId) 
            : VirtualDesktop.PinApplication(appId);
    }

    private static IntPtr GetWindowHandle(this Application app)
        => app.MainWindow?.GetHandle()
            ?? throw new InvalidOperationException();
}
