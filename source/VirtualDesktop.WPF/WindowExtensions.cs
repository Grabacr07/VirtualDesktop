using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;

namespace WindowsDesktop
{
	public static class WindowExtensions
	{
		/// <summary>
		/// Determines whether the window is located over the virtual desktop that current displayed.
		/// </summary>
		public static bool IsCurrentVirtualDesktop(this Window window)
		{
			return VirtualDesktopHelper.IsCurrentVirtualDesktop(window.GetHandle());
		}

		/// <summary>
		/// Returns a virtual desktop that window is located.
		/// </summary>
		public static VirtualDesktop GetCurrentDesktop(this Window window)
		{
			return VirtualDesktop.FromHwnd(window.GetHandle());
		}

		/// <summary>
		/// Move this window to specified virtual desktop.
		/// </summary>
		public static void MoveToDesktop(this Window window, VirtualDesktop virtualDesktop)
		{
			VirtualDesktopHelper.MoveToDesktop(window.GetHandle(), virtualDesktop);
		}

		public static void SwitchAndMove(this VirtualDesktop virtualDesktop, Window window)
		{
			window.MoveToDesktop(virtualDesktop);
			virtualDesktop.Switch();
		}

		private static IntPtr GetHandle(this Visual window)
		{
			var hwndSource = (HwndSource)PresentationSource.FromVisual(window);
			if (hwndSource == null) throw new InvalidOperationException();

			return hwndSource.Handle;
		}
	}
}
