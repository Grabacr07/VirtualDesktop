using System;
using System.Runtime.InteropServices;

namespace WindowsDesktop.Interop
{
	[ComImport, Guid("00000000-0000-0000-0000-000000000000") /* replace at runtime */]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	public interface IVirtualDesktopNotification
	{
		void VirtualDesktopCreated(IVirtualDesktop pDesktop);

		void VirtualDesktopDestroyBegin(IVirtualDesktop pDesktopDestroyed, IVirtualDesktop pDesktopFallback);

		void VirtualDesktopDestroyFailed(IVirtualDesktop pDesktopDestroyed, IVirtualDesktop pDesktopFallback);

		void VirtualDesktopDestroyed(IVirtualDesktop pDesktopDestroyed, IVirtualDesktop pDesktopFallback);

		void ViewVirtualDesktopChanged(IntPtr pView);

		void CurrentVirtualDesktopChanged(IVirtualDesktop pDesktopOld, IVirtualDesktop pDesktopNew);
	}

	public class VirtualDesktopNotificationListener : VirtualDesktopNotification, IVirtualDesktopNotification
	{
	}
}
