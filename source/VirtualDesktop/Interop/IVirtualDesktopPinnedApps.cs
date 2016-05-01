using System;
using System.Runtime.InteropServices;

namespace WindowsDesktop.Interop
{
	[ComImport]
	[Guid("4ce81583-1e4c-4632-a621-07a53543148f")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	public interface IVirtualDesktopPinnedApps
	{
		void Reserved1();

		bool IsPinnedWindow(IntPtr hwnd);

		bool IsPinnedApp(IntPtr hwnd);
	}
}
