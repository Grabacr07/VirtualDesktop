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
		/// Occurs when the current virtual desktop is changed.
		/// </summary>
		public static event EventHandler<VirtualDesktopChangedEventArgs> CurrentChanged;
		
		internal static class EventRaiser
		{
			public static void RaiseCreated(object sender, VirtualDesktop pDesktop)
			{
				Created?.Invoke(sender, pDesktop);
			}

			public static void RaiseDestroyBegin(object sender, VirtualDesktop pDesktopDestroyed, VirtualDesktop pDesktopFallback)
			{
				var args = new VirtualDesktopDestroyEventArgs(pDesktopDestroyed, pDesktopFallback);
				DestroyBegin?.Invoke(sender, args);
			}

			public static void RaiseDestroyFailed(object sender, VirtualDesktop pDesktopDestroyed, VirtualDesktop pDesktopFallback)
			{
				var args = new VirtualDesktopDestroyEventArgs(pDesktopDestroyed, pDesktopFallback);
				DestroyFailed?.Invoke(sender, args);
			}

			public static void RaiseDestroyed(object sender, VirtualDesktop pDesktopDestroyed, VirtualDesktop pDesktopFallback)
			{
				var args = new VirtualDesktopDestroyEventArgs(pDesktopDestroyed, pDesktopFallback);
				Destroyed?.Invoke(sender, args);
			}

			public static void RaiseApplicationViewChanged(object sender, IntPtr pView)
			{
				ApplicationViewChanged?.Invoke(sender, EventArgs.Empty);
			}

			public static void RaiseCurrentChanged(object sender, VirtualDesktop pDesktopOld, VirtualDesktop pDesktopNew)
			{
				var args = new VirtualDesktopChangedEventArgs(pDesktopOld, pDesktopNew);
				CurrentChanged?.Invoke(sender, args);
			}
		}
	}
}
