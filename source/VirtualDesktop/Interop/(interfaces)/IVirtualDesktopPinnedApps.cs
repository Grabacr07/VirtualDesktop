using System;
using System.Runtime.InteropServices;

namespace WindowsDesktop.Interop
{
	[ComImport]
	[Guid("00000000-0000-0000-0000-000000000000") /* replace at runtime */]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	public interface IVirtualDesktopPinnedApps
	{
		bool IsAppIdPinned(string appId);

		void PinAppID(string appId);

		void UnpinAppID(string appId);

		bool IsViewPinned(IApplicationView applicationView);

		void PinView(IApplicationView applicationView);

		void UnpinView(IApplicationView applicationView);
	}
}
