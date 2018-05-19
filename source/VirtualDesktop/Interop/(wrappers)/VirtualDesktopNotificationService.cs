using System;
using System.Collections.Generic;
using System.Linq;

namespace WindowsDesktop.Interop
{
	[ComInterfaceWrapper]
	internal class VirtualDesktopNotificationService : ComInterfaceWrapperBase
	{
		public VirtualDesktopNotificationService()
			: base(service: CLSID.VirtualDesktopNotificationService) { }

		public uint Register(IVirtualDesktopNotification pNotification)
		{
			return this.Invoke<uint>(Args(pNotification));
		}

		public void Unregister(uint dwCookie)
		{
			this.Invoke(Args(dwCookie));
		}
	}
}
