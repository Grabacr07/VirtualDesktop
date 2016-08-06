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

			return ComObjects.VirtualDesktopManager.IsWindowOnCurrentVirtualDesktop(handle);
		}

		public static void MoveToDesktop(IntPtr hWnd, VirtualDesktop virtualDesktop)
		{
			ThrowIfNotSupported();

			int processId;
			NativeMethods.GetWindowThreadProcessId(hWnd, out processId);

			if (Process.GetCurrentProcess().Id == processId)
			{
				var guid = virtualDesktop.Id;
				ComObjects.VirtualDesktopManager.MoveWindowToDesktop(hWnd, ref guid);
			}
			else
			{
				IApplicationView view;
				ComObjects.ApplicationViewCollection.GetViewForHwnd(hWnd, out view);
				ComObjects.VirtualDesktopManagerInternal.MoveViewToDesktop(view, virtualDesktop.ComObject);
			}
		}

		public static bool IsPinnedWindow(IntPtr hWnd)
		{
			ThrowIfNotSupported();

			return ComObjects.VirtualDesktopPinnedApps.IsViewPinned(hWnd.GetApplicationView());
		}

		public static void PinWindow(IntPtr hWnd)
		{
			ThrowIfNotSupported();

			var view = hWnd.GetApplicationView();

			if (!ComObjects.VirtualDesktopPinnedApps.IsViewPinned(view))
			{
				ComObjects.VirtualDesktopPinnedApps.PinView(view);
			}
		}

		public static void UnpinWindow(IntPtr hWnd)
		{
			ThrowIfNotSupported();

			var view = hWnd.GetApplicationView();

			if (ComObjects.VirtualDesktopPinnedApps.IsViewPinned(view))
			{
				ComObjects.VirtualDesktopPinnedApps.UnpinView(view);
			}
		}

		public static void TogglePinWindow(IntPtr hWnd)
		{
			ThrowIfNotSupported();

			var view = hWnd.GetApplicationView();

			if (ComObjects.VirtualDesktopPinnedApps.IsViewPinned(view))
			{
				ComObjects.VirtualDesktopPinnedApps.UnpinView(view);
			}
			else
			{
				ComObjects.VirtualDesktopPinnedApps.PinView(view);
			}
		}

		public static bool IsPinnedApplication(IntPtr hWnd)
		{
			ThrowIfNotSupported();

			return ComObjects.VirtualDesktopPinnedApps.IsAppIdPinned(hWnd.GetAppId());
		}

		public static void PinApplication(IntPtr hWnd)
		{
			ThrowIfNotSupported();

			var appId = hWnd.GetAppId();

			if (!ComObjects.VirtualDesktopPinnedApps.IsAppIdPinned(appId))
			{
				ComObjects.VirtualDesktopPinnedApps.PinAppID(appId);
			}
		}

		public static void UnpinApplication(IntPtr hWnd)
		{
			ThrowIfNotSupported();

			var appId = hWnd.GetAppId();

			if (ComObjects.VirtualDesktopPinnedApps.IsAppIdPinned(appId))
			{
				ComObjects.VirtualDesktopPinnedApps.UnpinAppID(appId);
			}
		}

		public static void TogglePinApplication(IntPtr hWnd)
		{
			ThrowIfNotSupported();

			var appId = hWnd.GetAppId();

			if (ComObjects.VirtualDesktopPinnedApps.IsAppIdPinned(appId))
			{
				ComObjects.VirtualDesktopPinnedApps.UnpinAppID(appId);
			}
			else
			{
				ComObjects.VirtualDesktopPinnedApps.PinAppID(appId);
			}
		}


		private static IApplicationView GetApplicationView(this IntPtr hWnd)
		{
			IApplicationView view;
			ComObjects.ApplicationViewCollection.GetViewForHwnd(hWnd, out view);

			return view;
		}

		private static string GetAppId(this IntPtr hWnd)
		{
			string appId;
			hWnd.GetApplicationView().GetAppUserModelId(out appId);

			return appId;
		}
	}
}
