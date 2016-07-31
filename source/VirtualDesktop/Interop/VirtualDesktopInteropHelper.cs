using System;

namespace WindowsDesktop.Interop
{
	public static class VirtualDesktopInteropHelper
	{
		public static IVirtualDesktopManager GetVirtualDesktopManager()
		{
			var vdmType = Type.GetTypeFromCLSID(CLSID.VirtualDesktopManager);
			var instance = Activator.CreateInstance(vdmType);

			return (IVirtualDesktopManager)instance;
		}

		public static IVirtualDesktopNotificationService GetVirtualDesktopNotificationService()
		{
			var shellType = Type.GetTypeFromCLSID(CLSID.ImmersiveShell);
			var shell = (IServiceProvider)Activator.CreateInstance(shellType);

			object ppvObject;
			shell.QueryService(CLSID.VirtualDesktopNotificationService, typeof(IVirtualDesktopNotificationService).GUID, out ppvObject);

			return (IVirtualDesktopNotificationService)ppvObject;
		}

		public static IVirtualDesktopPinnedApps GetVirtualDesktopPinnedApps()
		{
			var shellType = Type.GetTypeFromCLSID(CLSID.ImmersiveShell);
			var shell = (IServiceProvider)Activator.CreateInstance(shellType);

			object ppvObject;
			shell.QueryService(CLSID.VirtualDesktopPinnedApps, typeof(IVirtualDesktopPinnedApps).GUID, out ppvObject);

			return (IVirtualDesktopPinnedApps)ppvObject;
		}

		public static IApplicationViewCollection GetApplicationViewCollection()
		{
			var shellType = Type.GetTypeFromCLSID(CLSID.ImmersiveShell);
			var shell = (IServiceProvider)Activator.CreateInstance(shellType);

			object ppvObject;
			shell.QueryService(typeof(IApplicationViewCollection).GUID, typeof(IApplicationViewCollection).GUID, out ppvObject);

			return (IApplicationViewCollection)ppvObject;
		}
	}
}
