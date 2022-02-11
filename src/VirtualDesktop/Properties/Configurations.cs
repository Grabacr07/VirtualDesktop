using System.IO;

namespace WindowsDesktop.Properties;

public record VirtualDesktopConfiguration : VirtualDesktopCompilerConfiguration
{
}

public record VirtualDesktopCompilerConfiguration
{
    /// <summary>
    /// Gets or sets a value indicating whether the compiled assembly should be saved or not.
    /// </summary>
    /// <remarks>
    /// This library uses the non-public Windows API.<br/>
    /// Since the ID of the COM Interface may differ depending on the build of Windows, it works by checking the ID in that environment at runtime and generating the assembly.<br/>
    /// <br/>
    /// Here you can set whether to save the assembly.<br/>
    /// Saving will improve the speed of the next launch.
    /// </remarks>
    /// <returns>
    /// A value indicating whether the compiled assembly should be saved or not. Default is <see langword="true" />.
    /// </returns>
    public bool SaveCompiledAssembly { get; init; } = true;

    /// <summary>
    /// Gets or sets a value indicating the directory where the assembly will be saved.
    /// </summary>
    /// <remarks>
    /// See <see cref="SaveCompiledAssembly"/> property for details.
    /// </remarks>
    /// <returns>
    /// A value indicating whether the compiled assembly should be saved or not. Default is %LocalAppData%.
    /// </returns>
    public DirectoryInfo CompiledAssemblySaveDirectory { get; init; } = new(Path.Combine(LocationInfo.LocalAppData.FullName, "assemblies"));
}
