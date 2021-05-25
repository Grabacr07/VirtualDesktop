using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace WindowsDesktop.Interop
{
	[ComInterfaceWrapper(2)]
	[UsedImplicitly(ImplicitUseTargetFlags.Members)]
	public abstract class VirtualDesktopNotification
	{
		internal static VirtualDesktopNotification CreateInstance(ComInterfaceAssembly assembly)
		{
			var type2 = assembly.GetType("VirtualDesktopNotificationListener2");
			if (type2 != null)
			{
				var instance = (VirtualDesktopNotification)Activator.CreateInstance(type2);
				return instance;
			}
			else
			{
				var type = assembly.GetType("VirtualDesktopNotificationListener");
				var instance = (VirtualDesktopNotification)Activator.CreateInstance(type);
				return instance;
			}
		}

		protected VirtualDesktop GetDesktop(object comObject)
			=> VirtualDesktopCache.GetOrCreate(comObject);

		protected void VirtualDesktopCreatedCore(object pDesktop)
		{
			VirtualDesktop.EventRaiser.RaiseCreated(this, VirtualDesktopCache.GetOrCreate(pDesktop));
		}

		protected void VirtualDesktopDestroyBeginCore(object pDesktopDestroyed, object pDesktopFallback)
		{
			VirtualDesktop.EventRaiser.RaiseDestroyBegin(this, VirtualDesktopCache.GetOrCreate(pDesktopDestroyed), VirtualDesktopCache.GetOrCreate(pDesktopFallback));
		}

		protected void VirtualDesktopDestroyFailedCore(object pDesktopDestroyed, object pDesktopFallback)
		{
			VirtualDesktop.EventRaiser.RaiseDestroyFailed(this, VirtualDesktopCache.GetOrCreate(pDesktopDestroyed), VirtualDesktopCache.GetOrCreate(pDesktopFallback));
		}

		protected void VirtualDesktopDestroyedCore(object pDesktopDestroyed, object pDesktopFallback)
		{
			VirtualDesktop.EventRaiser.RaiseDestroyed(this, VirtualDesktopCache.GetOrCreate(pDesktopDestroyed), VirtualDesktopCache.GetOrCreate(pDesktopFallback));
		}

		protected void ViewVirtualDesktopChangedCore(IntPtr pView)
		{
			VirtualDesktop.EventRaiser.RaiseApplicationViewChanged(this, pView);
		}

		protected void CurrentVirtualDesktopChangedCore(object pDesktopOld, object pDesktopNew)
		{
			VirtualDesktop.EventRaiser.RaiseCurrentChanged(this, VirtualDesktopCache.GetOrCreate(pDesktopOld), VirtualDesktopCache.GetOrCreate(pDesktopNew));
		}

		protected void VirtualDesktopRenamedCore(object pDesktop, string name)
		{
			VirtualDesktop.EventRaiser.RaiseRenamed(this, VirtualDesktopCache.GetOrCreate(pDesktop), name);
		}
	}
}
