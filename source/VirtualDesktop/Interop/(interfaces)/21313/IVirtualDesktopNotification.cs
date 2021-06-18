using System;
using System.Runtime.InteropServices;

namespace WindowsDesktop.Interop
{
	[ComImport]
	[Guid("00000000-0000-0000-0000-000000000000") /* replace at runtime */]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	public interface IVirtualDesktopNotification
	{
		void VirtualDesktopCreated(IVirtualDesktop pDesktop);

		void VirtualDesktopDestroyBegin(IVirtualDesktop pDesktopDestroyed, IVirtualDesktop pDesktopFallback);

		void VirtualDesktopDestroyFailed(IVirtualDesktop pDesktopDestroyed, IVirtualDesktop pDesktopFallback);

		void VirtualDesktopDestroyed(IVirtualDesktop pDesktopDestroyed, IVirtualDesktop pDesktopFallback);

		void Unknown1(int nNumber);

		void VirtualDesktopMoved(IVirtualDesktop pDesktop, int nFromIndex, int nToIndex);

		void VirtualDesktopRenamed(IVirtualDesktop pDesktop, [MarshalAs(UnmanagedType.HString)] string chName);

		void ViewVirtualDesktopChanged(IApplicationView pView);

		void CurrentVirtualDesktopChanged(IVirtualDesktop pDesktopOld, IVirtualDesktop pDesktopNew);

		void VirtualDesktopWallpaperChanged(IVirtualDesktop pDesktop, [MarshalAs(UnmanagedType.HString)] string chPath);
	}

	public class VirtualDesktopNotificationListener : VirtualDesktopNotification, IVirtualDesktopNotification
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

		public void Unknown1(int nNumber)
		{
		}

		public void VirtualDesktopMoved(IVirtualDesktop pDesktop, int nFromIndex, int nToIndex)
		{
			this.VirtualDesktopMovedCore(pDesktop, nFromIndex, nToIndex);
		}

		public void VirtualDesktopRenamed(IVirtualDesktop pDesktop, [MarshalAs(UnmanagedType.HString)] string chName)
		{
			this.VirtualDesktopRenamedCore(pDesktop, chName);
		}

		public void ViewVirtualDesktopChanged(IApplicationView pView)
		{
			this.ViewVirtualDesktopChangedCore(pView);
		}

		public void CurrentVirtualDesktopChanged(IVirtualDesktop pDesktopOld, IVirtualDesktop pDesktopNew)
		{
			this.CurrentVirtualDesktopChangedCore(pDesktopOld, pDesktopNew);
		}

		public void VirtualDesktopWallpaperChanged(IVirtualDesktop pDesktop, [MarshalAs(UnmanagedType.HString)] string chName)
		{
			this.VirtualDesktopWallpaperChangedCore(pDesktop, chName);
		}
	}
}
