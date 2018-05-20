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
	public static class ComInterface
	{
		private const string _placeholderGuid = "00000000-0000-0000-0000-000000000000";
		private static readonly string _assemblyName = "VirtualDesktop.{0}.generated.dll";

		private static readonly Regex _assemblyRegex = new Regex(@"VirtualDesktop\.(?<build>\d{5}?)(\.\w*|)\.dll");
		private static readonly string _defaultAssemblyDirectoryPath = Path.Combine(ProductInfo.LocalAppData.FullName, "assemblies");

		public static string AssemblyDirectoryPath { get; set; } = _defaultAssemblyDirectoryPath;

		public static Assembly GetAssembly()
		{
			var assembly = GetExisitingAssembly();
			if (assembly != null) return assembly;

			return CreateAssembly();
		}

		private static Assembly GetExisitingAssembly()
		{
			var searchTargets = new[]
			{
				AssemblyDirectoryPath,
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
						System.Diagnostics.Debug.WriteLine($"Assembly found: {file.FullName}");
						return Assembly.LoadFile(file.FullName);
					}
				}
			}

			return null;
		}

		private static Assembly CreateAssembly()
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
					var sourceCode = reader.ReadToEnd().Replace(_placeholderGuid, iids[interfaceName].ToString());
					compileTargets.Add(sourceCode);
				}
			}

			return Compile(compileTargets.ToArray());
		}

		private static Assembly Compile(string[] sources)
		{
			var dir = new DirectoryInfo(AssemblyDirectoryPath);
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
