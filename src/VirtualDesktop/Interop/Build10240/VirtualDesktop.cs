using System;
using System.Collections.Generic;
using System.Linq;
using WindowsDesktop.Interop.Proxy;

namespace WindowsDesktop.Interop.Build10240;

public class VirtualDesktop : ComWrapperBase<IVirtualDesktop>, IVirtualDesktop
{
    private Guid? _id;

    internal VirtualDesktop(ComInterfaceAssembly assembly, object comObject)
        : base(assembly, comObject)
    {
    }

    public bool IsViewVisible(IntPtr hWnd)
        => this.InvokeMethod<bool>(Args(hWnd));

    public Guid GetID()
        => this._id ?? (Guid)(this._id = this.InvokeMethod<Guid>());

    public string GetName()
        => "";

    public string GetWallpaperPath()
        => "";
}
