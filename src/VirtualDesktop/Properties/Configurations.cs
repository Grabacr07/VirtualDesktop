using System.IO;

namespace WindowsDesktop.Properties;

public record VirtualDesktopConfiguration : VirtualDesktopCompilerConfiguration
{
}

public record VirtualDesktopCompilerConfiguration
{
    public bool SaveCompiledAssembly { get; init; } = true;

    public DirectoryInfo CompiledAssemblySaveDirectory { get; init; } = new(Path.Combine(ProductInfo.LocalAppData.FullName, "assemblies"));
}
