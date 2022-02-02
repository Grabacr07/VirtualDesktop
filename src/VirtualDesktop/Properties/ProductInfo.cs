using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace WindowsDesktop.Properties;

public static class ProductInfo
{
    private static readonly Assembly _assembly = Assembly.GetExecutingAssembly();
    private static readonly Lazy<string> _titleLazy = CreateLazy<AssemblyTitleAttribute>(x => x.Title);
    private static readonly Lazy<string> _descriptionLazy = CreateLazy<AssemblyDescriptionAttribute>(x => x.Description);
    private static readonly Lazy<string> _companyLazy = CreateLazy<AssemblyCompanyAttribute>(x => x.Company);
    private static readonly Lazy<string> _productLazy = CreateLazy<AssemblyProductAttribute>(x => x.Product);
    private static readonly Lazy<string> _copyrightLazy = CreateLazy<AssemblyCopyrightAttribute>(x => x.Copyright);
    private static readonly Lazy<string> _trademarkLazy = CreateLazy<AssemblyTrademarkAttribute>(x => x.Trademark);
    private static readonly Lazy<string> _versionLazy = new(() => $"{Version.ToString(3)}");
    private static readonly Lazy<DirectoryInfo> _localAppDataLazy = new(() => new DirectoryInfo(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), Company, Product)));

    public static string Title
        => _titleLazy.Value;

    public static string Description
        => _descriptionLazy.Value;

    public static string Company
        => _companyLazy.Value;

    public static string Product
        => _productLazy.Value;

    public static string Copyright
        => _copyrightLazy.Value;

    public static string Trademark
        => _trademarkLazy.Value;

    public static Version Version
        => _assembly.GetName().Version ?? new Version();

    public static string VersionString
        => _versionLazy.Value;

    internal static DirectoryInfo LocalAppData
        => _localAppDataLazy.Value;

    private static Lazy<string> CreateLazy<T>(Func<T, string> propSelector)
        where T : Attribute
    {
        var attribute = _assembly.GetCustomAttribute<T>();
        return new Lazy<string>(() => attribute != null ? propSelector(attribute) : "");
    }
}
