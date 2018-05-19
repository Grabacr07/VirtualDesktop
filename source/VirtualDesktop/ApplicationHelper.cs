using System;
using System.Collections.Generic;
using System.Linq;
using WindowsDesktop.Interop;

namespace WindowsDesktop
{
	public static class ApplicationHelper
	{
		internal static IApplicationView GetApplicationView(this IntPtr hWnd)
		{
			ComObjects.ApplicationViewCollection.GetViewForHwnd(hWnd, out var view);

			return view;
		}

		public static string GetAppId(IntPtr hWnd)
		{
			hWnd.GetApplicationView().GetAppUserModelId(out var appId);

			return appId;
		}
	}
}
