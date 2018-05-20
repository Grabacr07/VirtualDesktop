using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace WindowsDesktop.Interop
{
	[ComInterfaceWrapper]
	[UsedImplicitly(ImplicitUseTargetFlags.Members)]
	public abstract class VirtualDesktopNotification
	{
		public static VirtualDesktopNotification CreateInstance()
		{
			var type = ComActivator.GetType("VirtualDesktopNotificationListener");
			var instance = (VirtualDesktopNotification)Activator.CreateInstance(type);

			return instance;
		}

		public void VirtualDesktopCreated(IVirtualDesktop pDesktop)
		{
			VirtualDesktop.RaiseCreatedEvent(this, pDesktop);
		}

		public void VirtualDesktopDestroyBegin(IVirtualDesktop pDesktopDestroyed, IVirtualDesktop pDesktopFallback)
		{
			VirtualDesktop.RaiseDestroyBeginEvent(this, pDesktopDestroyed, pDesktopFallback);
		}

		public void VirtualDesktopDestroyFailed(IVirtualDesktop pDesktopDestroyed, IVirtualDesktop pDesktopFallback)
		{
			VirtualDesktop.RaiseDestroyFailedEvent(this, pDesktopDestroyed, pDesktopFallback);
		}

		public void VirtualDesktopDestroyed(IVirtualDesktop pDesktopDestroyed, IVirtualDesktop pDesktopFallback)
		{
			VirtualDesktop.RaiseDestroyedEvent(this, pDesktopDestroyed, pDesktopFallback);
		}

		public void ViewVirtualDesktopChanged(IntPtr pView)
		{
			VirtualDesktop.RaiseApplicationViewChangedEvent(this, pView);
		}

		public void CurrentVirtualDesktopChanged(IVirtualDesktop pDesktopOld, IVirtualDesktop pDesktopNew)
		{
			VirtualDesktop.RaiseCurrentChangedEvent(this, pDesktopOld, pDesktopNew);
		}
	}
}
