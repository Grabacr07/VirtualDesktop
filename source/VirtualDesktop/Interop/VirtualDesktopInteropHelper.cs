using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

		public static IVirtualDesktopManagerInternal GetVirtualDesktopManagerInternal()
		{
			var shellType = Type.GetTypeFromCLSID(CLSID.ImmersiveShell);
			var shell = (IServiceProvider)Activator.CreateInstance(shellType);

			object ppvObject;
			shell.QueryService(CLSID.VirtualDesktopAPIUnknown, typeof(IVirtualDesktopManagerInternal).GUID, out ppvObject);

			return (IVirtualDesktopManagerInternal)ppvObject;
		}

		public static IVirtualDesktopNotificationService GetVirtualDesktopNotificationService()
		{
			var shellType = Type.GetTypeFromCLSID(CLSID.ImmersiveShell);
			var shell = (IServiceProvider)Activator.CreateInstance(shellType);

			object ppvObject;
			shell.QueryService(CLSID.VirtualDesktopNotificationService, typeof(IVirtualDesktopNotificationService).GUID, out ppvObject);

			return (IVirtualDesktopNotificationService)ppvObject;
		}
	}
}
