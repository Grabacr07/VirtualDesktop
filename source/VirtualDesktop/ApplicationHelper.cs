using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using WindowsDesktop.Interop;
using JetBrains.Annotations;

namespace WindowsDesktop
{
	public static class ApplicationHelper
	{
		internal static ApplicationView GetApplicationView(this IntPtr hWnd)
		{
			return ComInterface.ApplicationViewCollection.GetViewForHwnd(hWnd);
		}

		[CanBeNull]
		public static string GetAppId(IntPtr hWnd)
		{
			VirtualDesktopHelper.ThrowIfNotSupported();

			try
			{
				return hWnd.GetApplicationView().GetAppUserModelId();
			}
			catch (COMException ex) when (ex.Match(HResult.TYPE_E_ELEMENTNOTFOUND))
			{
				return null;
			}
		}
	}
}
