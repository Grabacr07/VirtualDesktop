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
			VirtualDesktop.EventRaiser.RaiseCreated(this, pDesktop);
		}

		public void VirtualDesktopDestroyBegin(IVirtualDesktop pDesktopDestroyed, IVirtualDesktop pDesktopFallback)
		{
			VirtualDesktop.EventRaiser.RaiseDestroyBegin(this, pDesktopDestroyed, pDesktopFallback);
		}

		public void VirtualDesktopDestroyFailed(IVirtualDesktop pDesktopDestroyed, IVirtualDesktop pDesktopFallback)
		{
			VirtualDesktop.EventRaiser.RaiseDestroyFailed(this, pDesktopDestroyed, pDesktopFallback);
		}

		public void VirtualDesktopDestroyed(IVirtualDesktop pDesktopDestroyed, IVirtualDesktop pDesktopFallback)
		{
			VirtualDesktop.EventRaiser.RaiseDestroyed(this, pDesktopDestroyed, pDesktopFallback);
		}

		public void ViewVirtualDesktopChanged(IntPtr pView)
		{
			VirtualDesktop.EventRaiser.RaiseApplicationViewChanged(this, pView);
		}

		public void CurrentVirtualDesktopChanged(IVirtualDesktop pDesktopOld, IVirtualDesktop pDesktopNew)
		{
			VirtualDesktop.EventRaiser.RaiseCurrentChanged(this, pDesktopOld, pDesktopNew);
		}
	}
}
