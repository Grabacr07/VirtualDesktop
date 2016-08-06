using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace WindowsDesktop
{
	public static class ApplicationExtensions
	{
		public static bool IsPinned(this Application app)
		{
			return VirtualDesktopHelper.IsPinnedApplication(app.GetWindowHandle());
		}

		public static void Pin(this Application app)
		{
			VirtualDesktopHelper.PinApplication(app.GetWindowHandle());
		}

		public static void Unpin(this Application app)
		{
			VirtualDesktopHelper.UnpinApplication(app.GetWindowHandle());
		}

		public static void TogglePin(this Application app)
		{
			VirtualDesktopHelper.TogglePinApplication(app.GetWindowHandle());
		}

		private static IntPtr GetWindowHandle(this Application app)
		{
			var window = app.Windows.OfType<Window>().FirstOrDefault();
			if (window == null) throw new InvalidOperationException();

			return window.GetHandle();
		}
	}
}
