using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using WindowsDesktop.Properties;
using Microsoft.CSharp;

namespace WindowsDesktop.Interop
{
	internal class ComInterfaceAssemblyProvider
	{
		private const string _placeholderGuid = "00000000-0000-0000-0000-000000000000";
		private const string _assemblyName = "VirtualDesktop.{0}.generated.dll";

		private static readonly Regex _assemblyRegex = new Regex(@"VirtualDesktop\.(?<build>\d{5}?)(\.\w*|)\.dll");
		private static readonly string _defaultAssemblyDirectoryPath = Path.Combine(ProductInfo.LocalAppData.FullName, "assemblies");
		private static readonly Version _requireVersion = new Version("1.0");

		private readonly string _assemblyDirectoryPath;
		
		public ComInterfaceAssemblyProvider(string assemblyDirectoryPath)
		{
			this._assemblyDirectoryPath = assemblyDirectoryPath ?? _defaultAssemblyDirectoryPath;
		}

		public Assembly GetAssembly()
		{
			var assembly = this.GetExisitingAssembly();
			if (assembly != null) return assembly;

			return this.CreateAssembly();
		}

		private Assembly GetExisitingAssembly()
		{
			var searchTargets = new[]
			{
				this._assemblyDirectoryPath,
				Environment.CurrentDirectory,
				Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
				_defaultAssemblyDirectoryPath,
			};

			foreach (var searchPath in searchTargets)
			{
				var dir = new DirectoryInfo(searchPath);
				if (!dir.Exists) continue;

				foreach (var file in dir.GetFiles())
				{
					if (int.TryParse(_assemblyRegex.Match(file.Name).Groups["build"]?.ToString(), out var build)
						&& build == ProductInfo.OSBuild)
					{
						var name = AssemblyName.GetAssemblyName(file.FullName);
						if (name.Version >= _requireVersion)
						{
							System.Diagnostics.Debug.WriteLine($"Assembly found: {file.FullName}");
#if !DEBUG
							return Assembly.LoadFile(file.FullName);
#endif
						}
					}
				}
			}

			return null;
		}

		private Assembly CreateAssembly()
		{
			var executingAssembly = Assembly.GetExecutingAssembly();
			var interfaceNames = executingAssembly
				.GetTypes()
				.Select(x => x.GetComInterfaceNameIfWrapper())
				.Where(x => x != null)
				.ToArray();
			var iids = IID.GetIIDs(interfaceNames);
			var compileTargets = new List<string>();
			{
				var assemblyInfo = executingAssembly.GetManifestResourceNames().Single(x => x.Contains("AssemblyInfo"));
				var stream = executingAssembly.GetManifestResourceStream(assemblyInfo);
				if (stream != null)
				{
					using (var reader = new StreamReader(stream, Encoding.UTF8))
					{
						var sourceCode = reader.ReadToEnd().Replace("{VERSION}", ProductInfo.OSBuild.ToString());
						compileTargets.Add(sourceCode);
					}
				}
			}

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
					var sourceCode = reader.ReadToEnd().Replace(_placeholderGuid, iids[interfaceName].ToString());
					compileTargets.Add(sourceCode);
				}
			}

			return this.Compile(compileTargets.ToArray());
		}

		private Assembly Compile(string[] sources)
		{
			var dir = new DirectoryInfo(this._assemblyDirectoryPath);
			if (!dir.Exists) dir.Create();

			using (var provider = new CSharpCodeProvider())
			{
				var path = Path.Combine(dir.FullName, string.Format(_assemblyName, ProductInfo.OSBuild));
				var cp = new CompilerParameters
				{
					OutputAssembly = path,
					GenerateExecutable = false,
					GenerateInMemory = false,
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

				System.Diagnostics.Debug.WriteLine($"Assembly compiled: {path}");
				return result.CompiledAssembly;
			}
		}
	}
}
