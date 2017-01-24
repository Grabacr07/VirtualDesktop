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

		public static void MoveToDesktop(IntPtr hWnd, VirtualDesktop virtualDesktop)
		{
			ThrowIfNotSupported();

            virtualDesktop.MoveHere(hWnd);
		}

	    //public static bool WillWrapIfSwitchedTo(VirtualDesktop virtualDesktop)
	    //{
	    //    ThrowIfNotSupported();

	    //    var current = VirtualDesktop.Current;
	    //    var desktops = VirtualDesktop.GetDesktops();

	    //    return desktops.Length >= 2
	    //        &&
	    //        (
	    //            (current.Id == desktops.First().Id && virtualDesktop.Id == desktops.Last().Id)
	    //            || (current.Id == desktops.Last().Id && virtualDesktop.Id == desktops.First().Id)
	    //        );
	    //}
	}
}
