using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace WindowsDesktop.Interop
{
	public static class ComActivator
	{
		private static Task _initializationTask;
		private static Assembly _compiledAssembly;
		
		public static Task Initialize()
		{
			return _initializationTask ?? (_initializationTask = Task.Run(() => Core()));

			void Core()
			{
				_compiledAssembly = ComInterface.GetAssembly();
			}
		}

		internal static Type GetType(string typeName)
		{
			Initialize().Wait();

			return _compiledAssembly
				.GetTypes()
				.Single(x => x.Name.Split('.').Last() == typeName);
		}

		internal static object CreateInstance(Type type, Guid? guidService)
		{
			Initialize().Wait();

			var shellType = Type.GetTypeFromCLSID(CLSID.ImmersiveShell);
			var shell = (IServiceProvider)Activator.CreateInstance(shellType);

			shell.QueryService(guidService ?? type.GUID, type.GUID, out var ppvObject);

			return ppvObject;
		}

		internal static (Type type, object instance) CreateInstance(string comInterfaceName, Guid? guidService)
		{
			var type = GetType(comInterfaceName);
			var instance = CreateInstance(type, guidService);

			return (type, instance);
		}
	}
}
