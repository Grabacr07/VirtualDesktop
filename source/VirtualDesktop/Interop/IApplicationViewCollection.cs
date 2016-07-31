using System;
using System.Runtime.InteropServices;

namespace WindowsDesktop.Interop
{
	[ComImport]
	[Guid("2C08ADF0-A386-4B35-9250-0FE183476FCC")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	public interface IApplicationViewCollection
	{
		int GetViews(out IObjectArray array);

		int GetViewsByZOrder(out IObjectArray array);

		int GetViewsByAppUserModelId(string id, out IObjectArray array);

		int GetViewForHwnd(IntPtr hwnd, out IntPtr view);

		int GetViewForApplication(object application, out IntPtr view);

		int GetViewForAppUserModelId(string id, out IntPtr view);

		int GetViewInFocus(out IntPtr view);

		void outreshCollection();

		int RegisterForApplicationViewChanges(object listener, out int cookie);

		int RegisterForApplicationViewPositionChanges(object listener, out int cookie);

		int UnregisterForApplicationViewChanges(int cookie);
	}
}
