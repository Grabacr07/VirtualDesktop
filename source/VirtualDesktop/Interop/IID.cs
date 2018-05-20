using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Text.RegularExpressions;
using WindowsDesktop.Properties;
using Microsoft.Win32;

namespace WindowsDesktop.Interop
{
	public static class IID
	{
		private static readonly Regex _osBuildRegex = new Regex(@"v_(?<build>\d{5}?)");

		// ReSharper disable once InconsistentNaming
		public static Dictionary<string, Guid> GetIIDs(string[] targets)
		{
			var known = new Dictionary<string, Guid>();

			foreach (var prop in Settings.Default.Properties.OfType<SettingsProperty>())
			{
				if (int.TryParse(_osBuildRegex.Match(prop.Name).Groups["build"]?.ToString(), out var build)
					&& build == ProductInfo.OSBuild)
				{
					foreach (var str in (StringCollection)Settings.Default[prop.Name])
					{
						var pair = str.Split(',');
						if (pair.Length != 2) continue;

						var @interface = pair[0];
						if (targets.All(x => @interface != x) || known.ContainsKey(@interface)) continue;

						if (!Guid.TryParse(pair[1], out var guid)) continue;
						
						known.Add(@interface, guid);
					}

					break;
				}
			}

			var except = targets.Except(known.Keys).ToArray();
			if (except.Length > 0)
			{
				var fromRegistry = GetIIDsFromRegistry(except);
				foreach (var kvp in fromRegistry) known.Add(kvp.Key, kvp.Value);
			}

			return known;
		}

		// ReSharper disable once InconsistentNaming
		private static Dictionary<string, Guid> GetIIDsFromRegistry(string[] targets)
		{
			using (var interfaceKey = Registry.ClassesRoot.OpenSubKey("Interface"))
			{
				if (interfaceKey == null)
				{
					throw new Exception(@"Registry key '\HKEY_CLASSES_ROOT\Interface' is missing.");
				}

				var result = new Dictionary<string, Guid>();
				var names = interfaceKey.GetSubKeyNames();

				foreach (var name in names)
				{
					using (var key = interfaceKey.OpenSubKey(name))
					{
						if (key?.GetValue("") is string value)
						{
							var match = targets.FirstOrDefault(x => x == value);
							if (match != null && Guid.TryParse(key.Name.Split('\\').Last(), out var guid))
							{
								result[match] = guid;
							}
						}
					}
				}

				return result;
			}
		}
	}
}
