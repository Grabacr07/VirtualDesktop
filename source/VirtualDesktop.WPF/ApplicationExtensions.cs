using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace WindowsDesktop
{
	public static class ApplicationExtensions
	{
		/// <summary>
		/// Determines whether this application is pinned.
		/// </summary>
		public static bool IsPinned(this Application app)
		{
			var appId = ApplicationHelper.GetAppId(app.GetWindowHandle());
			if (appId == null) return false;

			return VirtualDesktop.IsPinnedApplication(appId);
		}

		/// <summary>
		/// Pins an application, showing it on all virtual desktops.
		/// </summary>
		public static void Pin(this Application app)
		{
			var appId = ApplicationHelper.GetAppId(app.GetWindowHandle());
			if (appId == null) return;

			VirtualDesktop.PinApplication(appId);
		}

		/// <summary>
		/// Unpins an application.
		/// </summary>
		public static void Unpin(this Application app)
		{
			var appId = ApplicationHelper.GetAppId(app.GetWindowHandle());
			if (appId == null) return;

			VirtualDesktop.UnpinApplication(appId);
		}

		/// <summary>
		/// Toggles an application between being pinned and unpinned.
		/// </summary>
		public static void TogglePin(this Application app)
		{
			var appId = ApplicationHelper.GetAppId(app.GetWindowHandle());
			if (appId == null) return;

			if (VirtualDesktop.IsPinnedApplication(appId))
			{
				VirtualDesktop.UnpinApplication(appId);
			}
			else
			{
				VirtualDesktop.PinApplication(appId);
			}
		}

		private static IntPtr GetWindowHandle(this Application app)
		{
			var window = app.Windows.OfType<Window>().FirstOrDefault();
			if (window == null) throw new InvalidOperationException();

			return window.GetHandle();
		}
	}
}
