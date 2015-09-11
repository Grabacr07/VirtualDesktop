using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WindowsDesktop.Interop;

namespace WindowsDesktop
{
	public class VirtualDesktopNotifier : IVirtualDesktopNotification, IDisposable
	{
		private readonly uint dwCookie;

		public event EventHandler<VirtualDesktop> Created;
		public event EventHandler<VirtualDesktopDestroyEventArgs> DestroyBegin;
		public event EventHandler<VirtualDesktopDestroyEventArgs> DestroyFailed;
		public event EventHandler<VirtualDesktopDestroyEventArgs> Destroyed;

		public event EventHandler ApplicationViewChanged;
		public event EventHandler<VirtualDesktopChangedEventArgs> CurrentChanged;

		public VirtualDesktopNotifier()
		{
			var service = VirtualDesktopInteropHelper.GetVirtualDesktopNotificationService();
			this.dwCookie = service.Register(this);
		}

		void IVirtualDesktopNotification.VirtualDesktopCreated(IVirtualDesktop pDesktop)
		{
			this.Created?.Invoke(this, VirtualDesktop.FromComObject(pDesktop));
		}

		void IVirtualDesktopNotification.VirtualDesktopDestroyBegin(IVirtualDesktop pDesktopDestroyed, IVirtualDesktop pDesktopFallback)
		{
			var args = new VirtualDesktopDestroyEventArgs(VirtualDesktop.FromComObject(pDesktopDestroyed), VirtualDesktop.FromComObject(pDesktopFallback));
			this.DestroyBegin?.Invoke(this, args);
		}

		void IVirtualDesktopNotification.VirtualDesktopDestroyFailed(IVirtualDesktop pDesktopDestroyed, IVirtualDesktop pDesktopFallback)
		{
			var args = new VirtualDesktopDestroyEventArgs(VirtualDesktop.FromComObject(pDesktopDestroyed), VirtualDesktop.FromComObject(pDesktopFallback));
			this.DestroyFailed?.Invoke(this, args);
		}

		void IVirtualDesktopNotification.VirtualDesktopDestroyed(IVirtualDesktop pDesktopDestroyed, IVirtualDesktop pDesktopFallback)
		{
			var args = new VirtualDesktopDestroyEventArgs(VirtualDesktop.FromComObject(pDesktopDestroyed), VirtualDesktop.FromComObject(pDesktopFallback));
			this.Destroyed?.Invoke(this, args);
		}

		void IVirtualDesktopNotification.ViewVirtualDesktopChanged(object pView)
		{
			this.ApplicationViewChanged?.Invoke(this, EventArgs.Empty);
		}

		void IVirtualDesktopNotification.CurrentVirtualDesktopChanged(IVirtualDesktop pDesktopOld, IVirtualDesktop pDesktopNew)
		{
			var args = new VirtualDesktopChangedEventArgs(VirtualDesktop.FromComObject(pDesktopOld), VirtualDesktop.FromComObject(pDesktopNew));
			this.CurrentChanged?.Invoke(this, args);
		}

		public void Dispose()
		{
			var service = VirtualDesktopInteropHelper.GetVirtualDesktopNotificationService();
			service.Unregister(this.dwCookie);
		}
	}
}
