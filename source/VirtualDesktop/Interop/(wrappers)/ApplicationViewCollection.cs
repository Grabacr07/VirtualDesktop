using System;

namespace WindowsDesktop.Interop
{
	[ComInterfaceWrapper]
	internal class ApplicationViewCollection : ComInterfaceWrapperBase
	{
		public ApplicationView GetViewForHwnd(IntPtr hWnd)
		{
			var param = Args(hWnd, null);
			this.Invoke(param);

			return new ApplicationView(param[1]);
		}
	}
}
