using System;
using System.Collections.Generic;
using System.Linq;
using WindowsDesktop.Interop.Proxy;

namespace WindowsDesktop.Interop.Build10240;

internal class VirtualDesktopPinnedApps : ComWrapperBase<IVirtualDesktopPinnedApps>, IVirtualDesktopPinnedApps
{
    private readonly ComWrapperFactory _factory;

    public VirtualDesktopPinnedApps(ComInterfaceAssembly assembly, ComWrapperFactory factory)
        : base(assembly, CLSID.VirtualDesktopPinnedApps)
    {
        this._factory = factory;
    }

    public bool IsViewPinned(IntPtr hWnd)
        => this.InvokeMethod<bool>(this.ArgsWithApplicationView(hWnd));

    public void PinView(IntPtr hWnd)
        => this.InvokeMethod(this.ArgsWithApplicationView(hWnd));

    public void UnpinView(IntPtr hWnd)
        => this.InvokeMethod(this.ArgsWithApplicationView(hWnd));
        
    public bool IsAppIdPinned(string appId)
        => this.InvokeMethod<bool>(Args(appId));

    public void PinAppID(string appId)
        => this.InvokeMethod(Args(appId));

    public void UnpinAppID(string appId)
        => this.InvokeMethod(Args(appId));

    private object?[] ArgsWithApplicationView(IntPtr hWnd)
        => Args(this._factory.ApplicationViewFromHwnd(hWnd).ComObject);
}
