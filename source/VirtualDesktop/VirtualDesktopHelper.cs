using System;
using System.Diagnostics;
using System.Linq;
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

        public static VirtualDesktopActor MoveToDesktop(IntPtr hWnd, VirtualDesktop virtualDesktop, AdjacentDesktop direction, bool loop)
        {
            ThrowIfNotSupported();

            return virtualDesktop.Move(hWnd, direction, loop);
        }
    }
}
