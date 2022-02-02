using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using Windows.Win32;
using Windows.Win32.Foundation;
using WindowsDesktop.Interop;

namespace WindowsDesktop;

/// <summary>
/// Encapsulates a Windows 10/11 virtual desktop.
/// </summary>
[DebuggerDisplay("{Name} ({Id})")]
public partial class VirtualDesktop
{
    #region instance members

    /// <summary>
    /// Gets the unique identifier for this virtual desktop.
    /// </summary>
    public Guid Id { get; }

    /// <summary>
    /// Gets the name of this virtual desktop.
    /// </summary>
    public string Name
    {
        get => this._name;
        set
        {
            _provider.VirtualDesktopManagerInternal.SetDesktopName(this._source, value);
            this._name = value;
        }
    }

    /// <summary>
    /// Switches to this virtual desktop.
    /// </summary>
    public void Switch()
    {
        _provider.VirtualDesktopManagerInternal.SwitchDesktop(this._source);
    }

    /// <summary>
    /// Removes this virtual desktop and switches to an available one.
    /// </summary>
    /// <remarks>If this is the last virtual desktop, a new one will be created to switch to.</remarks>
    public void Remove()
    {
        var fallback = _provider.VirtualDesktopManagerInternal
            .GetDesktops()
            .Select(x => x.ToVirtualDesktop())
            .FirstOrDefault(x => x.Id != this.Id) ?? Create();
        this.Remove(fallback);
    }

    /// <summary>
    /// Removes this virtual desktop and switches to <paramref name="fallbackDesktop" />.
    /// </summary>
    /// <param name="fallbackDesktop">A virtual desktop to be displayed after the virtual desktop is removed.</param>
    public void Remove(VirtualDesktop fallbackDesktop)
    {
        if (fallbackDesktop == null) throw new ArgumentNullException(nameof(fallbackDesktop));

        _provider.VirtualDesktopManagerInternal.RemoveDesktop(this._source, fallbackDesktop._source);
    }

    /// <summary>
    /// Returns the adjacent virtual desktop on the left, or null if there isn't one.
    /// </summary>
    public VirtualDesktop? GetLeft()
    {
        try
        {
            return _provider.VirtualDesktopManagerInternal
                .GetAdjacentDesktop(this._source, AdjacentDesktop.LeftDirection)
                .ToVirtualDesktop();
        }
        catch (COMException ex) when (ex.Match(HResult.TYPE_E_OUTOFBOUNDS))
        {
            return null;
        }
    }

    /// <summary>
    /// Returns the adjacent virtual desktop on the right, or null if there isn't one.
    /// </summary>
    public VirtualDesktop? GetRight()
    {
        try
        {
            return _provider.VirtualDesktopManagerInternal
                .GetAdjacentDesktop(this._source, AdjacentDesktop.RightDirection)
                .ToVirtualDesktop();
        }
        catch (COMException ex) when (ex.Match(HResult.TYPE_E_OUTOFBOUNDS))
        {
            return null;
        }
    }

    public override string ToString()
        => $"VirtualDesktop {this.Id} '{this._name}'";

    #endregion

    #region static members (get or create)

    /// <summary>
    /// Gets the virtual desktop that is currently displayed.
    /// </summary>
    public static VirtualDesktop Current
    {
        get
        {
            InitializeIfNeeded();

            return _provider.VirtualDesktopManagerInternal
                .GetCurrentDesktop()
                .ToVirtualDesktop();
        }
    }

    /// <summary>
    /// Returns an array of available virtual desktops.
    /// </summary>
    public static VirtualDesktop[] GetDesktops()
    {
        InitializeIfNeeded();

        return _provider.VirtualDesktopManagerInternal
            .GetDesktops()
            .Select(x => x.ToVirtualDesktop())
            .ToArray();
    }

    /// <summary>
    /// Returns a new virtual desktop.
    /// </summary>
    public static VirtualDesktop Create()
    {
        InitializeIfNeeded();

        return _provider.VirtualDesktopManagerInternal
            .CreateDesktop()
            .ToVirtualDesktop();
    }

    /// <summary>
    /// Returns a virtual desktop matching the specified identifier.
    /// </summary>
    /// <param name="desktopId">The identifier of the virtual desktop to return.</param>
    /// <remarks>Returns null if the identifier is not associated with any available desktops.</remarks>
    public static VirtualDesktop? FromId(Guid desktopId)
    {
        InitializeIfNeeded();

        try
        {
            return _provider.VirtualDesktopManagerInternal
                .FindDesktop(desktopId)
                .ToVirtualDesktop();
        }
        catch (COMException ex) when (ex.Match(HResult.TYPE_E_ELEMENTNOTFOUND))
        {
            return null;
        }
    }

    /// <summary>
    /// Returns the virtual desktop the window is located on.
    /// </summary>
    /// <param name="hWnd">The handle of the window.</param>
    /// <remarks>Returns null if the handle is not associated with any open windows.</remarks>
    public static VirtualDesktop? FromHwnd(IntPtr hWnd)
    {
        InitializeIfNeeded();

        if (hWnd == IntPtr.Zero) return null;

        try
        {
            _provider.VirtualDesktopManager.GetWindowDesktopId((HWND)hWnd, out var desktopId);
            return _provider.VirtualDesktopManagerInternal
                .FindDesktop(desktopId)
                .ToVirtualDesktop();
        }
        catch (COMException ex) when (ex.Match(HResult.REGDB_E_CLASSNOTREG, HResult.TYPE_E_ELEMENTNOTFOUND))
        {
            return null;
        }
    }

    #endregion

    #region static members (pinned apps)

    /// <summary>
    /// Determines whether the specified window is pinned.
    /// </summary>
    /// <param name="hWnd">The handle of the window.</param>
    public static bool IsPinnedWindow(IntPtr hWnd)
    {
        InitializeIfNeeded();

        return _provider.VirtualDesktopPinnedApps.IsViewPinned(hWnd);
    }

    /// <summary>
    /// Pins the specified window, showing it on all virtual desktops.
    /// </summary>
    /// <param name="hWnd">The handle of the window.</param>
    public static void PinWindow(IntPtr hWnd)
    {
        InitializeIfNeeded();

        if (_provider.VirtualDesktopPinnedApps.IsViewPinned(hWnd) == false)
        {
            _provider.VirtualDesktopPinnedApps.PinView(hWnd);
        }
    }

    /// <summary>
    /// Unpins the specified window.
    /// </summary>
    /// <param name="hWnd">The handle of the window.</param>
    public static void UnpinWindow(IntPtr hWnd)
    {
        InitializeIfNeeded();

        if (_provider.VirtualDesktopPinnedApps.IsViewPinned(hWnd))
        {
            _provider.VirtualDesktopPinnedApps.UnpinView(hWnd);
        }
    }

    /// <summary>
    /// Determines whether the specified app is pinned.
    /// </summary>
    /// <param name="appId">The identifier of the app.</param>
    public static bool IsPinnedApplication(string appId)
    {
        InitializeIfNeeded();

        return _provider.VirtualDesktopPinnedApps.IsAppIdPinned(appId);
    }

    /// <summary>
    /// Pins the specified app, showing it on all virtual desktops.
    /// </summary>
    /// <param name="appId">The identifier of the app.</param>
    public static void PinApplication(string appId)
    {
        InitializeIfNeeded();

        if (_provider.VirtualDesktopPinnedApps.IsAppIdPinned(appId) == false)
        {
            _provider.VirtualDesktopPinnedApps.PinAppID(appId);
        }
    }

    /// <summary>
    /// Unpins the specified app.
    /// </summary>
    /// <param name="appId">The identifier of the app.</param>
    public static void UnpinApplication(string appId)
    {
        InitializeIfNeeded();

        if (_provider.VirtualDesktopPinnedApps.IsAppIdPinned(appId))
        {
            _provider.VirtualDesktopPinnedApps.UnpinAppID(appId);
        }
    }

    #endregion

    #region static members (others)

    /// <summary>
    /// Moves a window to the specified virtual desktop.
    /// </summary>
    /// <param name="hWnd">The handle of the window to be moved.</param>
    /// <param name="virtualDesktop">The virtual desktop to move the window to.</param>
    public static void MoveToDesktop(IntPtr hWnd, VirtualDesktop virtualDesktop)
    {
        InitializeIfNeeded();

        var result = PInvoke.GetWindowThreadProcessId(hWnd, out var processId);
        if (result < 0) throw new Exception($"The process associated with '{hWnd}' not found.");

        if (Process.GetCurrentProcess().Id == processId)
        {
            _provider.VirtualDesktopManager.MoveWindowToDesktop((HWND)hWnd, virtualDesktop.Id);
        }
        else
        {
            _provider.VirtualDesktopManagerInternal.MoveViewToDesktop(hWnd, virtualDesktop._source);
        }
    }

    /// <summary>
    /// Determines whether this window is on the current virtual desktop.
    /// </summary>
    /// <param name="hWnd">The handle of the window.</param>
    public static bool IsCurrentVirtualDesktop(IntPtr hWnd)
    {
        InitializeIfNeeded();

        _provider.VirtualDesktopManager.IsWindowOnCurrentVirtualDesktop((HWND)hWnd, out var onCurrentDesktop);
        return onCurrentDesktop;
    }

    public static string? GetAppId(IntPtr hWnd)
    {
        InitializeIfNeeded();

        try
        {
            return _provider.ApplicationViewCollection
                .GetViewForHwnd(hWnd)
                .GetAppUserModelId();
        }
        catch (COMException ex) when (ex.Match(HResult.TYPE_E_ELEMENTNOTFOUND))
        {
            return null;
        }
    }

    #endregion
}
