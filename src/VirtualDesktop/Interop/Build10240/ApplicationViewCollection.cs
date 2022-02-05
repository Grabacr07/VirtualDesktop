using System;
using WindowsDesktop.Interop.Proxy;

namespace WindowsDesktop.Interop.Build10240;

internal class ApplicationViewCollection : ComWrapperBase<IApplicationViewCollection>, IApplicationViewCollection
{
    public ApplicationViewCollection(ComInterfaceAssembly assembly)
        : base(assembly)
    {
    }

    public ApplicationView GetViewForHwnd(IntPtr hWnd)
    {
        var view = this.InvokeMethod<object>(Args(hWnd))
            ?? new ArgumentException("ApplicationView is not found.", nameof(hWnd));

        return new ApplicationView(this.ComInterfaceAssembly, view);
    }

    IApplicationView IApplicationViewCollection.GetViewForHwnd(IntPtr hWnd)
        => this.GetViewForHwnd(hWnd);
}
