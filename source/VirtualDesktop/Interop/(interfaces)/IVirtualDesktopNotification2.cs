using System;
using System.Runtime.InteropServices;

namespace WindowsDesktop.Interop
{
	[ComImport]
	[Guid("00000000-0000-0000-0000-000000000000") /* replace at runtime */]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	public interface IVirtualDesktopNotification2
	{
		void VirtualDesktopCreated(IVirtualDesktop pDesktop);

		void VirtualDesktopDestroyBegin(IVirtualDesktop pDesktopDestroyed, IVirtualDesktop pDesktopFallback);

		void VirtualDesktopDestroyFailed(IVirtualDesktop pDesktopDestroyed, IVirtualDesktop pDesktopFallback);

		void VirtualDesktopDestroyed(IVirtualDesktop pDesktopDestroyed, IVirtualDesktop pDesktopFallback);

		void ViewVirtualDesktopChanged(IntPtr pView);

		void CurrentVirtualDesktopChanged(IVirtualDesktop pDesktopOld, IVirtualDesktop pDesktopNew);
		
		void VirtualDesktopRenamed(IVirtualDesktop pDesktop, [MarshalAs(UnmanagedType.HString)] string name);
	}

	public class VirtualDesktopNotificationListener2 : VirtualDesktopNotification, IVirtualDesktopNotification, IVirtualDesktopNotification2
	{
		public void VirtualDesktopCreated(IVirtualDesktop pDesktop)
		{
			this.VirtualDesktopCreatedCore(pDesktop);
		}

		public void VirtualDesktopDestroyBegin(IVirtualDesktop pDesktopDestroyed, IVirtualDesktop pDesktopFallback)
		{
			this.VirtualDesktopDestroyBeginCore(pDesktopDestroyed, pDesktopFallback);
		}

		public void VirtualDesktopDestroyFailed(IVirtualDesktop pDesktopDestroyed, IVirtualDesktop pDesktopFallback)
		{
			this.VirtualDesktopDestroyFailedCore(pDesktopDestroyed, pDesktopFallback);
		}

		public void VirtualDesktopDestroyed(IVirtualDesktop pDesktopDestroyed, IVirtualDesktop pDesktopFallback)
		{
			this.VirtualDesktopDestroyedCore(pDesktopDestroyed, pDesktopFallback);
		}

		public void ViewVirtualDesktopChanged(IntPtr pView)
		{
			this.ViewVirtualDesktopChangedCore(pView);
		}

		public void CurrentVirtualDesktopChanged(IVirtualDesktop pDesktopOld, IVirtualDesktop pDesktopNew)
		{
			this.CurrentVirtualDesktopChangedCore(pDesktopOld, pDesktopNew);
		}

		public void VirtualDesktopRenamed(IVirtualDesktop pDesktop, [MarshalAs(UnmanagedType.HString)] string name)
		{
			this.VirtualDesktopRenamedCore(pDesktop, name);
		}
	}
}
