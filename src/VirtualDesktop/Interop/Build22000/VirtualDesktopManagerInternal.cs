using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using WindowsDesktop.Interop.Proxy;

namespace WindowsDesktop.Interop.Build22000;

internal class VirtualDesktopManagerInternal : ComWrapperBase<IVirtualDesktopManagerInternal>, IVirtualDesktopManagerInternal
{
    private readonly ComWrapperFactory _factory;

    public VirtualDesktopManagerInternal(ComInterfaceAssembly assembly, ComWrapperFactory factory)
        : base(assembly, CLSID.VirtualDesktopManagerInternal)
    {
        this._factory = factory;
    }

    public IEnumerable<IVirtualDesktop> GetDesktops()
    {
        var array = this.InvokeMethod<IObjectArray>(Args(IntPtr.Zero));
        if (array == null) yield break;

        var count = array.GetCount();
        var vdType = this.ComInterfaceAssembly.GetType(nameof(IVirtualDesktop));

        for (var i = 0u; i < count; i++)
        {
            var ppvObject = array.GetAt(i, vdType.GUID);
            yield return new VirtualDesktop(this.ComInterfaceAssembly, ppvObject);
        }
    }

    public IVirtualDesktop GetCurrentDesktop()
        => this.InvokeMethodAndWrap(Args(IntPtr.Zero));

    public IVirtualDesktop GetAdjacentDesktop(IVirtualDesktop pDesktopReference, AdjacentDesktop uDirection)
        => this.InvokeMethodAndWrap(Args(((VirtualDesktop)pDesktopReference).ComObject, uDirection));

    public IVirtualDesktop FindDesktop(Guid desktopId)
        => this.InvokeMethodAndWrap(Args(desktopId));

    public IVirtualDesktop CreateDesktop()
        => this.InvokeMethodAndWrap(Args(IntPtr.Zero));

    public void SwitchDesktop(IVirtualDesktop desktop)
        => this.InvokeMethod(Args(IntPtr.Zero, ((VirtualDesktop)desktop).ComObject));

    public void RemoveDesktop(IVirtualDesktop pRemove, IVirtualDesktop pFallbackDesktop)
        => this.InvokeMethod(Args(((VirtualDesktop)pRemove).ComObject, ((VirtualDesktop)pFallbackDesktop).ComObject));

    public void MoveViewToDesktop(IntPtr hWnd, IVirtualDesktop desktop)
        => this.InvokeMethod(Args(this._factory.ApplicationViewFromHwnd(hWnd).ComObject, ((VirtualDesktop)desktop).ComObject));

    public void SetDesktopName(IVirtualDesktop desktop, string name)
        => this.InvokeMethod(Args(((VirtualDesktop)desktop).ComObject, new HString(name)));

    public void SetDesktopWallpaper(IVirtualDesktop desktop, string path)
        => this.InvokeMethod(Args(((VirtualDesktop)desktop).ComObject, new HString(path)));

    public void UpdateWallpaperPathForAllDesktops(string path)
        => this.InvokeMethod(Args(new HString(path)));

    private VirtualDesktop InvokeMethodAndWrap(object?[]? parameters = null, [CallerMemberName] string methodName = "")
        => new(this.ComInterfaceAssembly, this.InvokeMethod<object>(parameters, methodName) ?? throw new Exception("Failed to get IVirtualDesktop instance."));
}
