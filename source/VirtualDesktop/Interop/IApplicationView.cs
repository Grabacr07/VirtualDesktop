using System;
using System.Runtime.InteropServices;

namespace WindowsDesktop.Interop
{
	[ComImport]
	[Guid("855BCAAD-3177-47B5-8571-23803421F9D8")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	public interface IApplicationView
	{
		// invalid iid in Win10 build 14393.
	}
}
