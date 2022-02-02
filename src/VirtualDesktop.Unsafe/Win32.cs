using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace Windows.Win32.System.Com
{
    [ComImport]
    [Guid("6d5140c1-7436-11ce-8034-00aa006009fa")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IServiceProvider
    {
        [return: MarshalAs(UnmanagedType.IUnknown)]
        object QueryService(in Guid guidService, in Guid riid);
    }
}

namespace Windows.Win32.UI.Shell.Common
{
}
