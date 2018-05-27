using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using WindowsDesktop.Internal;

namespace WindowsDesktop.Interop
{
	[ComInterfaceWrapper]
	internal class VirtualDesktopNotificationService : ComInterfaceWrapperBase
	{
		public VirtualDesktopNotificationService(ComInterfaceAssembly assembly)
			: base(assembly, service: CLSID.VirtualDesktopNotificationService) { }

		public IDisposable Register(VirtualDesktopNotification pNotification)
		{
			var dwCookie = this.Invoke<uint>(Args(pNotification));
			return Disposable.Create(() => this.Unregister(dwCookie));
		}

		private void Unregister(uint dwCookie)
		{
			try
			{
				this.Invoke(Args(dwCookie));
			}
			catch (COMException ex) when (ex.Match(HResult.RPC_S_SERVER_UNAVAILABLE))
			{
			}
		}
	}
}
