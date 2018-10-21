using System;
using System.Collections.Generic;
using System.Linq;
using WindowsDesktop.Interop;
using JetBrains.Annotations;

namespace WindowsDesktop
{
	partial class VirtualDesktop
	{
		/// <summary>
		/// Determines whether the specified window is pinned.
		/// </summary>
		/// <param name="hWnd">The handle of the window.</param>
		public static bool IsPinnedWindow(IntPtr hWnd)
		{
			VirtualDesktopHelper.ThrowIfNotSupported();

			return ComInterface.VirtualDesktopPinnedApps.IsViewPinned(hWnd.GetApplicationView());
		}

		/// <summary>
		/// Pins the specified window, showing it on all virtual desktops.
		/// </summary>
		/// <param name="hWnd">The handle of the window.</param>
		public static void PinWindow(IntPtr hWnd)
		{
			VirtualDesktopHelper.ThrowIfNotSupported();

			var view = hWnd.GetApplicationView();

			if (!ComInterface.VirtualDesktopPinnedApps.IsViewPinned(view))
			{
				ComInterface.VirtualDesktopPinnedApps.PinView(view);
			}
		}

		/// <summary>
		/// Unpins the specified window.
		/// </summary>
		/// <param name="hWnd">The handle of the window.</param>
		public static void UnpinWindow(IntPtr hWnd)
		{
			VirtualDesktopHelper.ThrowIfNotSupported();

			var view = hWnd.GetApplicationView();

			if (ComInterface.VirtualDesktopPinnedApps.IsViewPinned(view))
			{
				ComInterface.VirtualDesktopPinnedApps.UnpinView(view);
			}
		}

		/// <summary>
		/// Determines whether the specified app is pinned.
		/// </summary>
		/// <param name="appId">The identifier of the app.</param>
		public static bool IsPinnedApplication([NotNull] string appId)
		{
			VirtualDesktopHelper.ThrowIfNotSupported();

			return ComInterface.VirtualDesktopPinnedApps.IsAppIdPinned(appId);
		}

		/// <summary>
		/// Pins the specified app, showing it on all virtual desktops.
		/// </summary>
		/// <param name="appId">The identifier of the app.</param>
		public static void PinApplication([NotNull] string appId)
		{
			VirtualDesktopHelper.ThrowIfNotSupported();

			if (!ComInterface.VirtualDesktopPinnedApps.IsAppIdPinned(appId))
			{
				ComInterface.VirtualDesktopPinnedApps.PinAppID(appId);
			}
		}

		/// <summary>
		/// Unpins the specified app.
		/// </summary>
		/// <param name="appId">The identifier of the app.</param>
		public static void UnpinApplication([NotNull] string appId)
		{
			VirtualDesktopHelper.ThrowIfNotSupported();

			if (ComInterface.VirtualDesktopPinnedApps.IsAppIdPinned(appId))
			{
				ComInterface.VirtualDesktopPinnedApps.UnpinAppID(appId);
			}
		}
	}
}
