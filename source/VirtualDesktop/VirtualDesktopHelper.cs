using System;
using System.Diagnostics;
using WindowsDesktop.Interop;

namespace WindowsDesktop
{
	public static class VirtualDesktopHelper
	{
		/// <summary>
		/// Throws a <see cref="NotSupportedException" /> if virtual desktops are not supported.
		/// </summary>
		internal static void ThrowIfNotSupported()
		{
			if (!VirtualDesktop.IsSupported)
			{
				throw new NotSupportedException("You must target Windows 10 in your app.manifest and run without debugging.");
			}
		}

		/// <summary>
		/// Determines whether this window is on the current virtual desktop.
		/// </summary>
		/// <param name="hWnd">The handle of the window.</param>
		public static bool IsCurrentVirtualDesktop(IntPtr hWnd)
		{
			ThrowIfNotSupported();

			return ComInterface.VirtualDesktopManager.IsWindowOnCurrentVirtualDesktop(hWnd);
		}

		/// <summary>
		/// Moves a window to the specified virtual desktop.
		/// </summary>
		/// <param name="hWnd">The handle of the window to be moved.</param>
		/// <param name="virtualDesktop">The virtual desktop to move the window to.</param>
		public static void MoveToDesktop(IntPtr hWnd, VirtualDesktop virtualDesktop)
		{
			ThrowIfNotSupported();

			NativeMethods.GetWindowThreadProcessId(hWnd, out var processId);

			if (Process.GetCurrentProcess().Id == processId)
			{
				var guid = virtualDesktop.Id;
				ComInterface.VirtualDesktopManager.MoveWindowToDesktop(hWnd, ref guid);
			}
			else
			{
				var view = ComInterface.ApplicationViewCollection.GetViewForHwnd(hWnd);
				ComInterface.VirtualDesktopManagerInternal.MoveViewToDesktop(view, virtualDesktop);
			}
		}
	}
}
