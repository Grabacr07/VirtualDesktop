using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace WindowsDesktop.Interop
{
	public static class ComObjects
	{
		private static IDisposable _listener;
		private static readonly ConcurrentDictionary<Guid, IVirtualDesktop> _virtualDesktops = new ConcurrentDictionary<Guid, IVirtualDesktop>();

		internal static IVirtualDesktopManager VirtualDesktopManager { get; private set; }
		internal static VirtualDesktopManagerInternal VirtualDesktopManagerInternal { get; private set; }
		internal static IVirtualDesktopNotificationService VirtualDesktopNotificationService { get; private set; }
		internal static IVirtualDesktopPinnedApps VirtualDesktopPinnedApps { get; private set; }
		internal static IApplicationViewCollection ApplicationViewCollection { get; private set; }

		internal static void Initialize()
		{
			VirtualDesktopManager = GetVirtualDesktopManager();
			VirtualDesktopManagerInternal = VirtualDesktopManagerInternal.GetInstance();
			VirtualDesktopNotificationService = GetVirtualDesktopNotificationService();
			VirtualDesktopPinnedApps = GetVirtualDesktopPinnedApps();
			ApplicationViewCollection = GetApplicationViewCollection();

			_virtualDesktops.Clear();
			_listener = VirtualDesktop.RegisterListener();
		}

		internal static void Register(IVirtualDesktop vd)
		{
			_virtualDesktops.AddOrUpdate(vd.GetID(), vd, (guid, desktop) => vd);
		}

		internal static IVirtualDesktop GetVirtualDesktop(Guid id)
		{
			return _virtualDesktops.GetOrAdd(id, x => VirtualDesktopManagerInternal.FindDesktop(ref x));
		}

		internal static void Terminate()
		{
			_listener?.Dispose();
		}


		#region public methods

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

		#endregion
	}
}
