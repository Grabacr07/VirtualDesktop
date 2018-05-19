using System;
using System.Collections.Generic;
using System.Linq;

namespace WindowsDesktop.Interop
{
	[ComInterfaceWrapper]
	internal class VirtualDesktopPinnedApps : ComInterfaceWrapperBase
	{
		public VirtualDesktopPinnedApps()
			: base(service: CLSID.VirtualDesktopPinnedApps) { }

		public bool IsViewPinned(ApplicationView applicationView)
		{
			return this.Invoke<bool>(Args(applicationView.Instance));
		}

		public void PinView(ApplicationView applicationView)
		{
			this.Invoke(Args(applicationView.Instance));
		}

		public void UnpinView(ApplicationView applicationView)
		{
			this.Invoke(Args(applicationView.Instance));
		}

		public bool IsAppIdPinned(string appId)
		{
			return this.Invoke<bool>(Args(appId));
		}

		public void PinAppID(string appId)
		{
			this.Invoke(Args(appId));
		}

		public void UnpinAppID(string appId)
		{
			this.Invoke(Args(appId));
		}
	}
}
