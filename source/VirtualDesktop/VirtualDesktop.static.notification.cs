using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using WindowsDesktop.Interop;

namespace WindowsDesktop
{
	partial class VirtualDesktop
	{
		private static uint? dwCookie;
		private static VirtualDesktopNotificationListener listener;

		/// <summary>
		/// Occurs when a virtual desktop is created.
		/// </summary>
		public static event EventHandler<VirtualDesktop> Created;
		public static event EventHandler<VirtualDesktopDestroyEventArgs> DestroyBegin;
		public static event EventHandler<VirtualDesktopDestroyEventArgs> DestroyFailed;

		/// <summary>
		/// Occurs when a virtual desktop is destroyed.
		/// </summary>
		public static event EventHandler<VirtualDesktopDestroyEventArgs> Destroyed;

		[EditorBrowsable(EditorBrowsableState.Never)]
		public static event EventHandler ApplicationViewChanged;

		/// <summary>
		/// Occurs when a current virtual desktop is changed.
		/// </summary>
		public static event EventHandler<VirtualDesktopChangedEventArgs> CurrentChanged;


		private static void RegisterListener()
		{
			var service = VirtualDesktopInteropHelper.GetVirtualDesktopNotificationService();
			listener = new VirtualDesktopNotificationListener();
			dwCookie = service.Register(listener);
		}

		private static void UnregisterListener()
		{
			if (dwCookie == null) return;

			var service = VirtualDesktopInteropHelper.GetVirtualDesktopNotificationService();
			service.Unregister(dwCookie.Value);
		}


		private class VirtualDesktopNotificationListener : IVirtualDesktopNotification
		{
			void IVirtualDesktopNotification.VirtualDesktopCreated(IVirtualDesktop pDesktop)
			{
				Created?.Invoke(this, FromComObject(pDesktop));
			}

			void IVirtualDesktopNotification.VirtualDesktopDestroyBegin(IVirtualDesktop pDesktopDestroyed, IVirtualDesktop pDesktopFallback)
			{
				var args = new VirtualDesktopDestroyEventArgs(FromComObject(pDesktopDestroyed), FromComObject(pDesktopFallback));
				DestroyBegin?.Invoke(this, args);
			}

			void IVirtualDesktopNotification.VirtualDesktopDestroyFailed(IVirtualDesktop pDesktopDestroyed, IVirtualDesktop pDesktopFallback)
			{
				var args = new VirtualDesktopDestroyEventArgs(FromComObject(pDesktopDestroyed), FromComObject(pDesktopFallback));
				DestroyFailed?.Invoke(this, args);
			}

			void IVirtualDesktopNotification.VirtualDesktopDestroyed(IVirtualDesktop pDesktopDestroyed, IVirtualDesktop pDesktopFallback)
			{
				var args = new VirtualDesktopDestroyEventArgs(FromComObject(pDesktopDestroyed), FromComObject(pDesktopFallback));
				Destroyed?.Invoke(this, args);
			}

			void IVirtualDesktopNotification.ViewVirtualDesktopChanged(IntPtr pView)
			{
				ApplicationViewChanged?.Invoke(this, EventArgs.Empty);
			}

			void IVirtualDesktopNotification.CurrentVirtualDesktopChanged(IVirtualDesktop pDesktopOld, IVirtualDesktop pDesktopNew)
			{
				var args = new VirtualDesktopChangedEventArgs(FromComObject(pDesktopOld), FromComObject(pDesktopNew));
				CurrentChanged?.Invoke(this, args);
			}
		}
	}
}
