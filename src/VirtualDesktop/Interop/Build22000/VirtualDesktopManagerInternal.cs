using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Windows.Win32;
using Windows.Win32.UI.Shell.Common;
using WindowsDesktop.Interop.Proxy;

namespace WindowsDesktop.Interop.Build22000;

public class VirtualDesktopManagerInternal : ComWrapperBase<IVirtualDesktopManagerInternal>, IVirtualDesktopManagerInternal
{
    private readonly ComWrapperFactory _factory;

    internal VirtualDesktopManagerInternal(ComInterfaceAssembly assembly, ComWrapperFactory factory)
        : base(assembly, CLSID.VirtualDesktopManagerInternal)
    {
        this._factory = factory;
    }

    public IEnumerable<IVirtualDesktop> GetDesktops()
    {
        var array = this.InvokeMethod<IObjectArray>(Args(IntPtr.Zero));
        if (array == null) yield break;

        array.GetCount(out var count);
        var vdType = this.ComInterfaceAssembly.GetType(nameof(IVirtualDesktop));

        for (var i = 0u; i < count; i++)
        {
            array.GetAt(i, vdType.GUID, out var ppvObject);
            yield return new VirtualDesktop(this.ComInterfaceAssembly, ppvObject);
        }
    }

    public IVirtualDesktop GetCurrentDesktop()
        => this.InvokeMethodAndWrap();

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
        => this.InvokeMethod(Args(((VirtualDesktop)desktop).ComObject, name.MarshalToHString()));

    public void SetDesktopWallpaper(IVirtualDesktop desktop, string path)
        => this.InvokeMethod(Args(((VirtualDesktop)desktop).ComObject, path.MarshalToHString()));

    private VirtualDesktop InvokeMethodAndWrap(object?[]? parameters = null, [CallerMemberName] string methodName = "")
        => new(this.ComInterfaceAssembly, this.InvokeMethod<object>(parameters, methodName) ?? throw new Exception("Failed to get IVirtualDesktop instance."));
}
