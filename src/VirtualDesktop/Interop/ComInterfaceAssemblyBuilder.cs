using System;
using System.Collections.Generic;
using System.Diagnostics;
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

internal class ComInterfaceAssemblyBuilder
{
    private const string _assemblyName = "VirtualDesktop.{0}.generated.dll";
    private const string _placeholderOsBuild = "{OS_BUILD}";
    private const string _placeholderAssemblyVersion = "{ASSEMBLY_VERSION}";
    private const string _placeholderInterfaceId = "00000000-0000-0000-0000-000000000000";

    private static readonly Version _requireVersion = new("2.1.0");
    private static readonly Regex _assemblyRegex = new(@"VirtualDesktop\.(?<build>\d{5}?)(\.\w*|)\.dll");
    private static readonly Regex _buildNumberRegex = new(@"\.Build(?<build>\d{5})\.");
    private static readonly int _osBuild = Environment.OSVersion.Version.Build;
    private static ComInterfaceAssembly? _assembly;

    private readonly VirtualDesktopCompilerConfiguration _configuration;

    public ComInterfaceAssemblyBuilder(VirtualDesktopCompilerConfiguration configuration)
    {
        this._configuration = configuration;
    }

    public ComInterfaceAssembly GetAssembly()
        => _assembly ??= new ComInterfaceAssembly(this.LoadExistingAssembly() ?? this.CreateAssembly());

    private Assembly? LoadExistingAssembly()
    {
        if (this._configuration.CompiledAssemblySaveDirectory.Exists)
        {
            foreach (var file in this._configuration.CompiledAssemblySaveDirectory.GetFiles())
            {
                if (int.TryParse(_assemblyRegex.Match(file.Name).Groups["build"].ToString(), out var build)
                    && build == _osBuild)
                {
                    try
                    {
                        var name = AssemblyName.GetAssemblyName(file.FullName);
                        if (name.Version >= _requireVersion)
                        {
                            Debug.WriteLine($"Assembly found: {file.FullName}");
#if !DEBUG
                            return Assembly.LoadFile(file.FullName);
#endif
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine("Failed to load assembly: ");
                        Debug.WriteLine(ex);

                        File.Delete(file.FullName);
                    }
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
            var assemblyInfo = executingAssembly.GetManifestResourceNames().Single(x => x.Contains("AssemblyInfo.cs"));
            var stream = executingAssembly.GetManifestResourceStream(assemblyInfo);
            if (stream != null)
            {
                using var reader = new StreamReader(stream, Encoding.UTF8);
                var sourceCode = reader
                    .ReadToEnd()
                    .Replace(_placeholderOsBuild, _osBuild.ToString())
                    .Replace(_placeholderAssemblyVersion, _requireVersion.ToString(3));
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
            var sourceCode = reader.ReadToEnd().Replace(_placeholderInterfaceId, iids[interfaceName].ToString());
            compileTargets.Add(sourceCode);
        }

        return this.Compile(compileTargets.ToArray());
    }

    private Assembly Compile(IEnumerable<string> sources)
    {
        try
        {
            var name = string.Format(_assemblyName, _osBuild);
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

            string? errorMessage;

            if (this._configuration.SaveCompiledAssembly)
            {
                var dir = this._configuration.CompiledAssemblySaveDirectory;
                if (dir.Exists == false) dir.Create();

                var path = Path.Combine(dir.FullName, name);
                var result = compilation.Emit(path);
                if (result.Success) return AssemblyLoadContext.Default.LoadFromAssemblyPath(path);

                File.Delete(path);
                errorMessage = string.Join(Environment.NewLine, result.Diagnostics.Select(x => $"  {x.GetMessage()}"));
            }
            else
            {
                using var stream = new MemoryStream();
                var result = compilation.Emit(stream);
                if (result.Success)
                {
                    stream.Seek(0, SeekOrigin.Begin);
                    return AssemblyLoadContext.Default.LoadFromStream(stream);
                }

                errorMessage = string.Join(Environment.NewLine, result.Diagnostics.Select(x => $"  {x.GetMessage()}"));
            }

            throw new Exception("Failed to compile COM interfaces assembly." + Environment.NewLine + errorMessage);
        }
        finally
        {
            GC.Collect();
        }
    }
}
