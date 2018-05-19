using System;
using System.Collections.Generic;
using System.Linq;
using WindowsDesktop.Interop;

namespace WindowsDesktop
{
	public static class ApplicationHelper
	{
		internal static ApplicationView GetApplicationView(this IntPtr hWnd)
		{
			return ComObjects.ApplicationViewCollection.GetViewForHwnd(hWnd);
		}

		public static string GetAppId(IntPtr hWnd)
		{
			return hWnd.GetApplicationView().GetAppUserModelId();
		}
	}
}
