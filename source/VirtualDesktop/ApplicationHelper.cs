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
			try
			{
				IApplicationView view;
				ComObjects.ApplicationViewCollection.GetViewForHwnd(hWnd, out view);
				return view;
			}
			catch (System.Runtime.InteropServices.COMException ex) when (ex.Match(HResult.TYPE_E_ELEMENTNOTFOUND))
			{
				return null;
			}
		}

		public static string GetAppId(IntPtr hWnd)
		{
			var view = hWnd.GetApplicationView();

			if (view == null)
			{
				throw new ArgumentException(nameof(hWnd));
			}

			string appId;
			view.GetAppUserModelId(out appId);

			return appId;
		}
	}
}
