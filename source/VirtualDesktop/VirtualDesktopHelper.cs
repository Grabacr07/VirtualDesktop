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
	}
}
