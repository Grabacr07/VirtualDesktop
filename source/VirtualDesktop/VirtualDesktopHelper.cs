using System;
using System.Diagnostics;
using WindowsDesktop.Interop;

namespace WindowsDesktop
{
	public static class VirtualDesktopHelper
	{
		internal static void ThrowIfNotSupported()
		{
			if (!VirtualDesktop.IsSupported)
			{
				throw new NotSupportedException("Need to include the app manifest in your project so as to target Windows 10. And, run without debugging.");
			}
		}


		public static bool IsCurrentVirtualDesktop(IntPtr handle)
		{
			ThrowIfNotSupported();

			return VirtualDesktop.ComManager.IsWindowOnCurrentVirtualDesktop(handle);
		}

		public static bool MoveToDesktop(IntPtr hWnd, VirtualDesktop virtualDesktop)
		{
			ThrowIfNotSupported();

			int processId;
			NativeMethods.GetWindowThreadProcessId(hWnd, out processId);

			if (Process.GetCurrentProcess().Id == processId)
			{
				var guid = virtualDesktop.Id;
				VirtualDesktop.ComManager.MoveWindowToDesktop(hWnd, ref guid);
				return true;
			}

			return false;
		}

		public static bool IsPinnedWindow(IntPtr hWnd)
		{
			ThrowIfNotSupported();

			return VirtualDesktop.PinndApps.IsViewPinned(hWnd.GetApplicationView());
		}

		public static void PinWindow(IntPtr hWnd)
		{
			ThrowIfNotSupported();

			var view = hWnd.GetApplicationView();

			if (!VirtualDesktop.PinndApps.IsViewPinned(view))
			{
				VirtualDesktop.PinndApps.PinView(view);
			}
		}

		public static void UnpinWindow(IntPtr hWnd)
		{
			ThrowIfNotSupported();

			var view = hWnd.GetApplicationView();

			if (VirtualDesktop.PinndApps.IsViewPinned(view))
			{
				VirtualDesktop.PinndApps.UnpinView(view);
			}
		}

		public static void TogglePinWindow(IntPtr hWnd)
		{
			ThrowIfNotSupported();

			var view = hWnd.GetApplicationView();

			if (VirtualDesktop.PinndApps.IsViewPinned(view))
			{
				VirtualDesktop.PinndApps.UnpinView(view);
			}
			else
			{
				VirtualDesktop.PinndApps.PinView(view);
			}
		}

		private static IntPtr GetApplicationView(this IntPtr hWnd)
		{
			IntPtr view;
			VirtualDesktop.ApplicationViewCollection.GetViewForHwnd(hWnd, out view);

			return view;
		}
	}
}
