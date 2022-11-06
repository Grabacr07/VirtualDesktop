using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Text.RegularExpressions;
using WindowsDesktop.Properties;
using Microsoft.Win32;

namespace WindowsDesktop.Interop;

internal record OsBuildSettings(
    int osBuild,
    SettingsProperty prop);

internal static class IID
{
    private static readonly Regex _osBuildRegex = new(@"v_(?<build>\d{5}?)");
    
    // ReSharper disable once InconsistentNaming
    public static Dictionary<string, Guid> GetIIDs(string[] interfaceNames)
    {
        var result = new Dictionary<string, Guid>();
        
        // Order configuration props by build version
        var orderedProps = Settings.Default.Properties.OfType<SettingsProperty>()
            .Select(prop =>
            {
                if (int.TryParse(_osBuildRegex.Match(prop.Name).Groups["build"].ToString(), out var build))
                {
                    return new OsBuildSettings(build, prop);
                }

                return null;
            })
            .Where(s => s != null)
            .OrderByDescending(s => s.osBuild)
            .ToArray();

        // Find first prop with build version <= current OS version
        var selectedSettings = orderedProps.FirstOrDefault(p =>
            p.osBuild <= Environment.OSVersion.Version.Build
        );
        
        if (selectedSettings == null)
        {
            var supportedBuilds = orderedProps.Select(v => v.osBuild).ToArray();
            throw new ConfigurationException(
                "Invalid application configuration. Unable to determine interop interfaces for " +
                $"current OS Build: {Environment.OSVersion.Version.Build}. All configured OS Builds " +
                $"have build version greater than current OS: {supportedBuilds}");
        }

        foreach (var str in (StringCollection)Settings.Default[selectedSettings.prop.Name])
        {
            if (str == null) continue;

            var pair = str.Split(',');
            if (pair.Length != 2) continue;
            if (interfaceNames.Contains(pair[0]) == false || result.ContainsKey(pair[0])) continue;
            if (Guid.TryParse(pair[1], out var guid) == false) continue;

            result.Add(pair[0], guid);
        }

        var except = interfaceNames.Except(result.Keys).ToArray();
        if (except.Length > 0)
        {
            foreach (var (key, value) in GetIIDsFromRegistry(except)) result.Add(key, value);
        }

        return result;
    }

    // ReSharper disable once InconsistentNaming
    private static Dictionary<string, Guid> GetIIDsFromRegistry(string[] targets)
    {
        using var interfaceKey = Registry.ClassesRoot.OpenSubKey("Interface")
            ?? throw new Exception(@"Registry key '\HKEY_CLASSES_ROOT\Interface' is missing.");

        var result = new Dictionary<string, Guid>();

        foreach (var name in interfaceKey.GetSubKeyNames())
        {
            using var key = interfaceKey.OpenSubKey(name);

            if (key?.GetValue("") is string value)
            {
                var match = targets.FirstOrDefault(x => x == value);
                if (match != null && Guid.TryParse(key.Name.Split('\\').Last(), out var guid))
                {
                    result[match] = guid;
                }
            }
        }

        return result;
    }
}
