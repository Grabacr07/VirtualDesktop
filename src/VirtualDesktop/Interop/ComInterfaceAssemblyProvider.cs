using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using WindowsDesktop.Properties;

namespace WindowsDesktop.Interop;

internal class ComInterfaceAssemblyProvider
{
    private const string _placeholderGuid = "00000000-0000-0000-0000-000000000000";
    private const string _assemblyName = "VirtualDesktop.{0}.generated.dll";

    private static readonly int _osBuild = Environment.OSVersion.Version.Build;
    private static readonly Regex _assemblyRegex = new(@"VirtualDesktop\.(?<build>\d{5}?)(\.\w*|)\.dll");
    private static readonly Regex _buildNumberRegex = new(@"\.Build(?<build>\d{5})\.");
    private static readonly Version _requireVersion = new("2.0");

    private readonly VirtualDesktopCompilerConfiguration _configuration;
    

    public ComInterfaceAssemblyProvider(VirtualDesktopCompilerConfiguration configuration)
    {
        this._configuration = configuration;
    }

    public ComInterfaceAssembly GetAssembly()
        => new(this.GetExistingAssembly() ?? this.CreateAssembly());

    private Assembly? GetExistingAssembly()
    {
        var searchTargets = new[]
            {
#if DEBUG
                "",
#else
                Environment.CurrentDirectory,
                Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
#endif
            }
            .Where(x => string.IsNullOrEmpty(x) == false)
            .Select(x => new DirectoryInfo(x!))
            .Append(this._configuration.CompiledAssemblySaveDirectory)
            .Where(x => x.Exists);

        foreach (var file in searchTargets.SelectMany(x => x.GetFiles()))
        {
            if (int.TryParse(_assemblyRegex.Match(file.Name).Groups["build"].ToString(), out var build)
                && build == _osBuild)
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

        return null;
    }

    private Assembly CreateAssembly()
    {
        var executingAssembly = Assembly.GetExecutingAssembly();
        var compileTargets = new List<string>();
        {
            var assemblyInfo = executingAssembly.GetManifestResourceNames().Single(x => x.Contains("AssemblyInfo"));
            var stream = executingAssembly.GetManifestResourceStream(assemblyInfo);
            if (stream != null)
            {
                using var reader = new StreamReader(stream, Encoding.UTF8);
                var sourceCode = reader.ReadToEnd().Replace("{VERSION}", _osBuild.ToString());
                compileTargets.Add(sourceCode);
            }
        }

        var interfaceNames = executingAssembly
            .GetTypes()
            .Select(x => x.GetComInterfaceNameIfWrapper())
            .Where(x => string.IsNullOrEmpty(x) == false)
            .Cast<string>()
            .ToArray();
        var iids = IID.GetIIDs(interfaceNames);

        // e.g.
        //   IVirtualDesktop
        //           ├── 10240, VirtualDesktop.Interop.Build10240..interfaces.IVirtualDesktop.cs
        //           └── 22000, VirtualDesktop.Interop.Build22000..interfaces.IVirtualDesktop.cs
        //   IVirtualDesktopPinnedApps
        //           └── 10240, VirtualDesktop.Interop.Build10240..interfaces.IVirtualDesktopPinnedApps.cs
        var interfaceSourceFiles = new Dictionary<string, SortedList<int, string>>();

        foreach (var name in executingAssembly.GetManifestResourceNames())
        {
            var interfaceName = Path.GetFileNameWithoutExtension(name).Split('.').LastOrDefault();
            if (interfaceName != null
                && interfaceNames.Contains(interfaceName)
                && int.TryParse(_buildNumberRegex.Match(name).Groups["build"].ToString(), out var build))
            {
                if (interfaceSourceFiles.TryGetValue(interfaceName, out var sourceFiles) == false)
                {
                    sourceFiles = new SortedList<int, string>();
                    interfaceSourceFiles.Add(interfaceName, sourceFiles);
                }

                sourceFiles.Add(build, name);
            }
        }

        foreach (var (interfaceName, sourceFiles) in interfaceSourceFiles)
        {
            var resourceName = sourceFiles.Aggregate("", (current, kvp) =>
            {
                var (build, resourceName) = kvp;
                return build <= _osBuild ? resourceName : current;
            });

            var stream = executingAssembly.GetManifestResourceStream(resourceName);
            if (stream == null) continue;

            using var reader = new StreamReader(stream, Encoding.UTF8);
            var sourceCode = reader.ReadToEnd().Replace(_placeholderGuid, iids[interfaceName].ToString());
            compileTargets.Add(sourceCode);
        }

        return Compile(compileTargets.ToArray(), this._configuration);
    }

    private static Assembly Compile(IEnumerable<string> sources, VirtualDesktopCompilerConfiguration conf)
    {
        var dir = conf.SaveCompiledAssembly
            ? conf.CompiledAssemblySaveDirectory
            : new DirectoryInfo(Path.GetTempPath());
        if (dir.Exists == false) dir.Create();

        var name = string.Format(_assemblyName, _osBuild);
        var path = Path.Combine(dir.FullName, name);
        var syntaxTrees = sources.Select(x => SyntaxFactory.ParseSyntaxTree(x));
        var references = AppDomain.CurrentDomain.GetAssemblies()
            .Concat(new[] { Assembly.GetExecutingAssembly(), })
            .Where(x => x.IsDynamic == false)
            .Select(x => MetadataReference.CreateFromFile(x.Location));
        var options = new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary);
        var compilation = CSharpCompilation.Create(name)
            .WithOptions(options)
            .WithReferences(references)
            .AddSyntaxTrees(syntaxTrees);

        var result = compilation.Emit(path);
        if (result.Success) return AssemblyLoadContext.Default.LoadFromAssemblyPath(path);

        File.Delete(path);

        var nl = Environment.NewLine;
        var message = $"Failed to compile COM interfaces assembly.{nl}{string.Join(nl, result.Diagnostics.Select(x => $"  {x.GetMessage()}"))}";
        throw new Exception(message);
    }
}
