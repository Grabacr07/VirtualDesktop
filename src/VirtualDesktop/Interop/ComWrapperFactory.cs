using System;
using System.Collections.Generic;
using System.Linq;
using WindowsDesktop.Interop.Proxy;

namespace WindowsDesktop.Interop;

internal class ComWrapperFactory
{
    public Func<object, ComWrapperBase<IApplicationView>> ApplicationView { get; }

    public Func<IntPtr, ComWrapperBase<IApplicationView>> ApplicationViewFromHwnd { get; }

    public Func<object, ComWrapperBase<IVirtualDesktop>> VirtualDesktop { get; }

    public ComWrapperFactory(
        Func<object, ComWrapperBase<IApplicationView>> applicationView,
        Func<IntPtr, ComWrapperBase<IApplicationView>> applicationViewFromHwnd,
        Func<object, ComWrapperBase<IVirtualDesktop>> virtualDesktop)
    {
        this.ApplicationView = applicationView;
        this.ApplicationViewFromHwnd = applicationViewFromHwnd;
        this.VirtualDesktop = virtualDesktop;
    }
}
