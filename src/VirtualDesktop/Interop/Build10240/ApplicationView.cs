using System;
using System.Collections.Generic;
using System.Linq;

namespace WindowsDesktop.Interop.Build10240;

public class ApplicationView : ComWrapperBase<IApplicationView>, IApplicationView
{
    internal ApplicationView(ComInterfaceAssembly assembly, object comObject)
        : base(assembly, comObject)
    {
    }

    public string GetAppUserModelId()
        => this.InvokeMethod<string>() ?? throw new Exception("Failed to get AppUserModelId.");
}
