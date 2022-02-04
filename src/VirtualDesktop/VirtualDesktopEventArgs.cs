using System;
using WindowsDesktop.Interop.Proxy;

namespace WindowsDesktop;

/// <summary>
/// Provides data for the <see cref="VirtualDesktop.Renamed" /> event.
/// </summary>
public class VirtualDesktopRenamedEventArgs : EventArgs
{
    public VirtualDesktop Desktop { get; }

    public string Name { get; }

    public VirtualDesktopRenamedEventArgs(VirtualDesktop desktop, string name)
    {
        this.Desktop = desktop;
        this.Name = name;
    }
}

/// <summary>
/// Provides data for the <see cref="VirtualDesktop.WallpaperChanged" /> event.
/// </summary>
public class VirtualDesktopWallpaperChangedEventArgs : EventArgs
{
    public VirtualDesktop Desktop { get; }

    public string Path { get; }

    public VirtualDesktopWallpaperChangedEventArgs(VirtualDesktop desktop, string path)
    {
        this.Desktop = desktop;
        this.Path = path;
    }
}

/// <summary>
/// Provides data for the <see cref="VirtualDesktop.CurrentChanged" /> event.
/// </summary>
public class VirtualDesktopChangedEventArgs : EventArgs
{
    public VirtualDesktop OldDesktop { get; }

    public VirtualDesktop NewDesktop { get; }

    public VirtualDesktopChangedEventArgs(VirtualDesktop oldDesktop, VirtualDesktop newDesktop)
    {
        this.OldDesktop = oldDesktop;
        this.NewDesktop = newDesktop;
    }

    internal VirtualDesktopChangedEventArgs(IVirtualDesktop oldDesktop, IVirtualDesktop newDesktop)
        : this(oldDesktop.ToVirtualDesktop(), newDesktop.ToVirtualDesktop())
    {
    }
}

/// <summary>
/// Provides data for the <see cref="VirtualDesktop.CurrentChanged" /> event.
/// </summary>
public class VirtualDesktopMovedEventArgs : EventArgs
{
    public VirtualDesktop Desktop { get; }

    public int OldIndex { get; }

    public int NewIndex { get; }

    public VirtualDesktopMovedEventArgs(VirtualDesktop desktop, int oldIndex, int newIndex)
    {
        this.Desktop = desktop;
        this.OldIndex = oldIndex;
        this.NewIndex = newIndex;
    }

    internal VirtualDesktopMovedEventArgs(IVirtualDesktop desktop, int oldIndex, int newIndex)
        : this(desktop.ToVirtualDesktop(), oldIndex, newIndex)
    {
    }
}

/// <summary>
/// Provides data for the <see cref="VirtualDesktop.DestroyBegin" />, <see cref="VirtualDesktop.DestroyFailed" />, and <see cref="VirtualDesktop.Destroyed" /> events.
/// </summary>
public class VirtualDesktopDestroyEventArgs : EventArgs
{
    /// <summary>
    /// Gets the virtual desktop that was destroyed.
    /// </summary>
    public VirtualDesktop Destroyed { get; }

    /// <summary>
    /// Gets the virtual desktop to be displayed after <see cref="Destroyed" /> is destroyed.
    /// </summary>
    public VirtualDesktop Fallback { get; }

    public VirtualDesktopDestroyEventArgs(VirtualDesktop destroyed, VirtualDesktop fallback)
    {
        this.Destroyed = destroyed;
        this.Fallback = fallback;
    }

    internal VirtualDesktopDestroyEventArgs(IVirtualDesktop destroyed, IVirtualDesktop fallback)
        : this(destroyed.ToVirtualDesktop(), fallback.ToVirtualDesktop())
    {
    }
}
