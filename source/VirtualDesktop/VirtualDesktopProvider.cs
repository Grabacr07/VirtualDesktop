using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using WindowsDesktop.Interop;

namespace WindowsDesktop
{
	public class VirtualDesktopProvider
	{
		#region Default instance

		private static readonly Lazy<VirtualDesktopProvider> _default = new Lazy<VirtualDesktopProvider>(() => new VirtualDesktopProvider(null));

		public static VirtualDesktopProvider Default => _default.Value;

		#endregion

		private Task _initializationTask;
		private Assembly _compiledAssembly;
		
		public string ComInterfaceAssemblyPath { get; }

		public VirtualDesktopProvider(string comInterfaceAssemblyPath)
		{
			this.ComInterfaceAssemblyPath = comInterfaceAssemblyPath;
		}

		public Task Initialize()
		{
			return this._initializationTask ?? (this._initializationTask = Task.Run(() => Core()));

			void Core()
			{
				var assemblyProvider = new ComInterfaceAssemblyProvider(this.ComInterfaceAssemblyPath);
				this._compiledAssembly = assemblyProvider.GetAssembly();
			}
		}

		internal Type GetType(string typeName)
		{
			this.Initialize().Wait();

			return this._compiledAssembly
				.GetTypes()
				.Single(x => x.Name.Split('.').Last() == typeName);
		}

		internal object CreateInstance(Type type, Guid? guidService)
		{
			this.Initialize().Wait();

			var shellType = Type.GetTypeFromCLSID(CLSID.ImmersiveShell);
			var shell = (Interop.IServiceProvider)Activator.CreateInstance(shellType);

			shell.QueryService(guidService ?? type.GUID, type.GUID, out var ppvObject);

			return ppvObject;
		}

		internal (Type type, object instance) CreateInstance(string comInterfaceName, Guid? guidService)
		{
			var type = this.GetType(comInterfaceName);
			var instance = this.CreateInstance(type, guidService);

			return (type, instance);
		}
	}

	partial class VirtualDesktop
	{
		public static VirtualDesktopProvider Provider { get; set; }

		internal static VirtualDesktopProvider ProviderInternal
			=> Provider ?? VirtualDesktopProvider.Default;
	}
}
