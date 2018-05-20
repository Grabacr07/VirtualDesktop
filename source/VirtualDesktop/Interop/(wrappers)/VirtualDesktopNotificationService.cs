using System;
using System.Collections.Generic;
using System.Linq;
using WindowsDesktop.Internal;

namespace WindowsDesktop.Interop
{
	[ComInterfaceWrapper]
	internal class VirtualDesktopNotificationService : ComInterfaceWrapperBase
	{
		public VirtualDesktopNotificationService()
			: base(service: CLSID.VirtualDesktopNotificationService) { }

		public IDisposable Register(VirtualDesktopNotification pNotification)
		{
			var dwCookie = this.Invoke<uint>(Args(pNotification));
			return Disposable.Create(() => this.Unregister(dwCookie));
		}

		private void Unregister(uint dwCookie)
		{
			this.Invoke(Args(dwCookie));
		}
	}
}
