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

			var guid = virtualDesktop.Id;
			VirtualDesktop.ComManager.MoveWindowToDesktop(handle, ref guid);
		}
	}
}
