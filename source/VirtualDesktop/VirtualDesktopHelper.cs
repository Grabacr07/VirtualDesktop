using System;

namespace WindowsDesktop
{
	public static class VirtualDesktopHelper
	{
		internal static void ThrowIfNotSupported()
		{
			if (!VirtualDesktop.IsSupported) throw new NotSupportedException();
		}


		public static bool IsCurrentVirtualDesktop(IntPtr handle)
		{
			ThrowIfNotSupported();

			return VirtualDesktop.ComManager.IsWindowOnCurrentVirtualDesktop(handle);
		}

		public static void MoveToDesktop(IntPtr handle, VirtualDesktop virtualDesktop)
		{
			ThrowIfNotSupported();

			VirtualDesktop.ComManager.MoveWindowToDesktop(handle, virtualDesktop.Id);
		}

		public static VirtualDesktop GetCurrentDesktop(IntPtr handle)
		{
			ThrowIfNotSupported();

			var id = VirtualDesktop.ComManager.GetWindowDesktopId(handle);
			return VirtualDesktop.FromId(id);
		}
	}
}