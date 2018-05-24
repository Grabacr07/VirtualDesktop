using System;

namespace WindowsDesktop.Interop
{
	[ComInterfaceWrapper]
	internal class ApplicationViewCollection : ComInterfaceWrapperBase
	{
		public ApplicationViewCollection(ComInterfaceAssembly assembly)
			: base(assembly) { }

		public ApplicationView GetViewForHwnd(IntPtr hWnd)
		{
			var param = Args(hWnd, null);
			this.Invoke(param);

			return new ApplicationView(this.ComInterfaceAssembly, param[1]);
		}
	}
}
