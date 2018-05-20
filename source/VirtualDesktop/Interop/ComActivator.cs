using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CSharp;

namespace WindowsDesktop.Interop
{
	public static class ComActivator
	{
		internal const string PlaceholderGuid = "00000000-0000-0000-0000-000000000000";

		private static Task _initializationTask;
		private static Assembly _compiledAssembly;
		
		public static Task Initialize()
		{
			return _initializationTask ?? (_initializationTask = Task.Run(() => Core()));

			void Core()
			{
				var executingAssembly = Assembly.GetExecutingAssembly();
				var interfaceNames = executingAssembly
					.GetTypes()
					.Select(x => x.GetComInterfaceNameIfWrapper())
					.Where(x => x != null)
					.ToArray();
				var iids = IID.GetIIDs(interfaceNames);
				var compileTargets = new List<string>();

				foreach (var name in executingAssembly.GetManifestResourceNames())
				{
					var typeName = Path.GetFileNameWithoutExtension(name)?.Split('.').LastOrDefault();
					if (typeName == null) continue;

					var interfaceName = interfaceNames.FirstOrDefault(x => typeName == x);
					if (interfaceName == null) continue;

					var stream = executingAssembly.GetManifestResourceStream(name);
					if (stream == null) continue;

					using (var reader = new StreamReader(stream, Encoding.UTF8))
					{
						var sourceCode = reader.ReadToEnd().Replace(PlaceholderGuid, iids[interfaceName].ToString());
						compileTargets.Add(sourceCode);
					}
				}

				_compiledAssembly = Compile(compileTargets.ToArray());
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

		private static Assembly Compile(string[] sources)
		{
			var provider = new CSharpCodeProvider();
			var cp = new CompilerParameters
			{
				OutputAssembly = "VirtualDesktop.generated.dll",
				GenerateExecutable = false,
				GenerateInMemory = true,
			};
			cp.ReferencedAssemblies.Add("System.dll");
			cp.ReferencedAssemblies.Add(Assembly.GetExecutingAssembly().Location);

			var result = provider.CompileAssemblyFromSource(cp, sources);
			if (result.Errors.Count > 0)
			{
				var nl = Environment.NewLine;
				var message = $"Failed to compile COM interfaces assembly.{nl}{string.Join(nl, result.Errors.OfType<CompilerError>().Select(x => $"  {x}"))}";

				throw new Exception(message);
			}

			return result.CompiledAssembly;
		}
	}
}
