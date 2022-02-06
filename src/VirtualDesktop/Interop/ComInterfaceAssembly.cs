using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace WindowsDesktop.Interop
{
	internal class ComInterfaceAssembly
	{
		private readonly Dictionary<string, Type> _knownTypes = new();
		private readonly Assembly _compiledAssembly;

        public DirectoryInfo AssemblyLocation
            => new(this._compiledAssembly.Location);

		public ComInterfaceAssembly(Assembly compiledAssembly)
		{
			this._compiledAssembly = compiledAssembly;
		}

		internal Type GetType(string typeName)
		{
			if (this._knownTypes.TryGetValue(typeName, out var type) == false)
			{
				type = this._knownTypes[typeName] = this._compiledAssembly
					.GetTypes()
					.Single(x => x.Name.Split('.').Last() == typeName);
			}

			return type;
        }

        internal (Type type, object instance) CreateInstance(string comInterfaceName)
        {
            var type = this.GetType(comInterfaceName);
            var instance = CreateInstance(type, null);

            return (type, instance);
        }

        internal (Type type, object instance) CreateInstance(string comInterfaceName, Guid clsid)
		{
			var type = this.GetType(comInterfaceName);
			var instance = CreateInstance(type, clsid);

			return (type, instance);
		}

		private static object CreateInstance(Type type, Guid? guidService)
		{
			var shellType = Type.GetTypeFromCLSID(CLSID.ImmersiveShell)
				?? throw new Exception($"Type of ImmersiveShell ('{CLSID.ImmersiveShell}') is not found.");
			var shell = Activator.CreateInstance(shellType) as IServiceProvider
				?? throw new Exception("Failed to create an instance of ImmersiveShell.");

			return shell.QueryService(guidService ?? type.GUID, type.GUID);
		}
	}
}
