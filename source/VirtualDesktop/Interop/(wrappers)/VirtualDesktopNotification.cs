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
		private VirtualDesktopFactory _factory;

		internal static VirtualDesktopNotification CreateInstance(ComInterfaceAssembly assembly)
		{
			var type = assembly.GetType("VirtualDesktopNotificationListener");
			var instance = (VirtualDesktopNotification)Activator.CreateInstance(type);
			instance._factory = new VirtualDesktopFactory(assembly);

			return instance;
		}

		protected VirtualDesktop GetDesktop(object comObject)
			=> this._factory.Get(comObject);

		protected void VirtualDesktopCreatedCore(object pDesktop)
		{
			VirtualDesktop.EventRaiser.RaiseCreated(this, this._factory.Get(pDesktop));
		}

		protected void VirtualDesktopDestroyBeginCore(object pDesktopDestroyed, object pDesktopFallback)
		{
			VirtualDesktop.EventRaiser.RaiseDestroyBegin(this, this._factory.Get(pDesktopDestroyed), this._factory.Get(pDesktopFallback));
		}

		protected void VirtualDesktopDestroyFailedCore(object pDesktopDestroyed, object pDesktopFallback)
		{
			VirtualDesktop.EventRaiser.RaiseDestroyFailed(this, this._factory.Get(pDesktopDestroyed), this._factory.Get(pDesktopFallback));
		}

		protected void VirtualDesktopDestroyedCore(object pDesktopDestroyed, object pDesktopFallback)
		{
			VirtualDesktop.EventRaiser.RaiseDestroyed(this, this._factory.Get(pDesktopDestroyed), this._factory.Get(pDesktopFallback));
		}

		protected void ViewVirtualDesktopChangedCore(IntPtr pView)
		{
			VirtualDesktop.EventRaiser.RaiseApplicationViewChanged(this, pView);
		}

		protected void CurrentVirtualDesktopChangedCore(object pDesktopOld, object pDesktopNew)
		{
			VirtualDesktop.EventRaiser.RaiseCurrentChanged(this, this._factory.Get(pDesktopOld), this._factory.Get(pDesktopNew));
		}
	}
}
