using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Text.RegularExpressions;
using WindowsDesktop.Properties;
using Microsoft.Win32;

namespace WindowsDesktop.Interop;

internal static class IID
{
    private static readonly Regex _osBuildRegex = new(@"v_(?<build>\d{5}?)");

    // ReSharper disable once InconsistentNaming
    public static Dictionary<string, Guid> GetIIDs(string[] interfaceNames)
    {
        var result = new Dictionary<string, Guid>();

        foreach (var prop in Settings.Default.Properties.OfType<SettingsProperty>())
        {
            if (int.TryParse(_osBuildRegex.Match(prop.Name).Groups["build"].ToString(), out var build)
                && build == Environment.OSVersion.Version.Build)
            {
                foreach (var str in (StringCollection)Settings.Default[prop.Name])
                {
                    if (str == null) continue;

                    var pair = str.Split(',');
                    if (pair.Length != 2) continue;
                    if (interfaceNames.Contains(pair[0]) == false || result.ContainsKey(pair[0])) continue;
                    if (Guid.TryParse(pair[1], out var guid) == false) continue;

                    result.Add(pair[0], guid);
                }

                break;
            }
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
