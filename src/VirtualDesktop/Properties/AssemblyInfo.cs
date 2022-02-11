using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

[assembly: ComVisible(false)]
[assembly: Guid("ab848ecd-76aa-41c0-b63d-86a8591b25aa")]

namespace WindowsDesktop.Properties;

internal static class AssemblyInfo
{
    private static readonly Assembly _assembly = Assembly.GetExecutingAssembly();
    private static string? _title;
    private static string? _description;
    private static string? _company;
    private static string? _product;
    private static string? _copyright;
    private static string? _trademark;
    private static string? _versionString;

    public static string Title
        => _title ??= Prop<AssemblyTitleAttribute>(x => x.Title);

    public static string Description
        => _description ??= Prop<AssemblyDescriptionAttribute>(x => x.Description);

    public static string Company
        => _company ??= Prop<AssemblyCompanyAttribute>(x => x.Company);

    public static string Product
        => _product ??= Prop<AssemblyProductAttribute>(x => x.Product);

    public static string Copyright
        => _copyright ??= Prop<AssemblyCopyrightAttribute>(x => x.Copyright);

    public static string Trademark
        => _trademark ??= Prop<AssemblyTrademarkAttribute>(x => x.Trademark);

    public static Version Version
        => _assembly.GetName().Version ?? new Version();

    public static string VersionString
        => _versionString ??= Version.ToString(3);

    private static string Prop<T>(Func<T, string> propSelector)
        where T : Attribute
    {
        var attribute = _assembly.GetCustomAttribute<T>();
        return attribute != null ? propSelector(attribute) : "";
    }
}

internal static class LocationInfo
{
    private static DirectoryInfo? _localAppData;

    internal static DirectoryInfo LocalAppData
        => _localAppData ??= new DirectoryInfo(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), AssemblyInfo.Company, AssemblyInfo.Product));
}
