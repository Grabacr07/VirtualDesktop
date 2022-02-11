using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using WindowsDesktop.Interop;
using WindowsDesktop.Interop.Build10240;
using WindowsDesktop.Interop.Build22000;
using WindowsDesktop.Interop.Proxy;
using WindowsDesktop.Properties;
using WindowsDesktop.Utils;

namespace WindowsDesktop;

partial class VirtualDesktop
{
    private static readonly VirtualDesktopProvider _provider;
    private static readonly ConcurrentDictionary<Guid, VirtualDesktop> _knownDesktops = new();
    private static readonly ExplorerRestartListenerWindow _explorerRestartListener = new(() => HandleExplorerRestarted());
    private static VirtualDesktopConfiguration _configuration = new();
    private static ComInterfaceAssembly? _assembly;
    private static IDisposable? _notificationListener;
    private readonly IVirtualDesktop _source;
    private string _name;
    private string _wallpaperPath;

    /// <summary>
    /// Gets a value indicating virtual desktops are supported by the host.
    /// </summary>
    public static bool IsSupported
        => _provider.IsSupported;

    static VirtualDesktop()
    {
        _provider = Environment.OSVersion.Version.Build switch
        {
            >= 22000 => new VirtualDesktopProvider22000(),
            >= 10240 => new VirtualDesktopProvider10240(),
            _ => new VirtualDesktopProvider.NotSupported()
        };

        Debug.WriteLine($"*** {AssemblyInfo.Title} Library ***");
        Debug.WriteLine($"Version:  {AssemblyInfo.VersionString}");
        Debug.WriteLine($"OS Build: {Environment.OSVersion.Version.Build}");
        Debug.WriteLine($"Provider: {_provider.GetType().Name}");
    }

    private VirtualDesktop(IVirtualDesktop source)
    {
        this._source = source;
        this._name = source.GetName();
        this._wallpaperPath = source.GetWallpaperPath();
        this.Id = source.GetID();
    }

    /// <summary>
    /// Initialize using the default settings. This method should always be called first.
    /// </summary>
    public static void Configure()
    {
        InitializeIfNeeded();
    }

    /// <summary>
    /// Sets the behavior for compiling the assembly. This method should always be called first.
    /// </summary>
    public static void Configure(VirtualDesktopConfiguration configuration)
    {
        _configuration = configuration;
        InitializeIfNeeded();
    }

    internal static VirtualDesktop FromComObject(IVirtualDesktop desktop)
        => _knownDesktops.GetOrAdd(desktop.GetID(), _ => new VirtualDesktop(desktop));

    internal static void InitializeIfNeeded()
    {
        if (IsSupported == false) throw new NotSupportedException("You must target Windows 10 or later in your 'app.manifest' and run without debugging.");
        if (_provider.IsInitialized) return;
        
        _explorerRestartListener.Show();
        InitializeCore();
    }

    private static void HandleExplorerRestarted()
    {
        _knownDesktops.Clear();
        _provider.IsInitialized = false;
        InitializeCore();
    }

    private static void InitializeCore()
    {
        _provider.Initialize(_assembly ??= new ComInterfaceAssemblyBuilder(_configuration).GetAssembly());

        _notificationListener?.Dispose();
        _notificationListener = _provider.VirtualDesktopNotificationService.Register(new EventProxy());
    }

    private static T? SafeInvoke<T>(Func<T> action, params HResult[] hResult)
    {
        try
        {
            return action();
        }
        catch (COMException ex) when (ex.Match(hResult is { Length: 0 } ? new[] { HResult.TYPE_E_ELEMENTNOTFOUND, } : hResult))
        {
            return default;
        }
    }

    private static bool SafeInvoke(Action action, params HResult[] hResult)
    {
        try
        {
            action();
            return true;
        }
        catch (COMException ex) when (ex.Match(hResult is { Length: 0 } ? new[] { HResult.TYPE_E_ELEMENTNOTFOUND, } : hResult))
        {
            return false;
        }
    }
}
