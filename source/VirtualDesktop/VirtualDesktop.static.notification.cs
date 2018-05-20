using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using WindowsDesktop.Interop;

namespace WindowsDesktop
{
	partial class VirtualDesktop
	{
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


		internal static IDisposable RegisterListener()
		{
			var service = ComObjects.VirtualDesktopNotificationService;

			return service.Register(VirtualDesktopNotification.CreateInstance());
		}

		internal static void RaiseCreatedEvent(object sender, IVirtualDesktop pDesktop)
		{
			Created?.Invoke(sender, FromComObject(pDesktop));
		}

		internal static void RaiseDestroyBeginEvent(object sender, IVirtualDesktop pDesktopDestroyed, IVirtualDesktop pDesktopFallback)
		{
			var args = new VirtualDesktopDestroyEventArgs(FromComObject(pDesktopDestroyed), FromComObject(pDesktopFallback));
			DestroyBegin?.Invoke(sender, args);
		}

		internal static void RaiseDestroyFailedEvent(object sender, IVirtualDesktop pDesktopDestroyed, IVirtualDesktop pDesktopFallback)
		{
			var args = new VirtualDesktopDestroyEventArgs(FromComObject(pDesktopDestroyed), FromComObject(pDesktopFallback));
			DestroyFailed?.Invoke(sender, args);
		}

		internal static void RaiseDestroyedEvent(object sender, IVirtualDesktop pDesktopDestroyed, IVirtualDesktop pDesktopFallback)
		{
			var args = new VirtualDesktopDestroyEventArgs(FromComObject(pDesktopDestroyed), FromComObject(pDesktopFallback));
			Destroyed?.Invoke(sender, args);
		}

		internal static void RaiseApplicationViewChangedEvent(object sender, IntPtr pView)
		{
			ApplicationViewChanged?.Invoke(sender, EventArgs.Empty);
		}

		internal static void RaiseCurrentChangedEvent(object sender, IVirtualDesktop pDesktopOld, IVirtualDesktop pDesktopNew)
		{
			var args = new VirtualDesktopChangedEventArgs(FromComObject(pDesktopOld), FromComObject(pDesktopNew));
			CurrentChanged?.Invoke(sender, args);
		}
	}
}
