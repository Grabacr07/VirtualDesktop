using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace WindowsDesktop.Interop
{
	[AttributeUsage(AttributeTargets.Class, Inherited = false)]
	public class ComInterfaceWrapperAttribute : Attribute
	{
		public string InterfaceName { get; set; }

		public ComInterfaceWrapperAttribute() { }

		public ComInterfaceWrapperAttribute(string interfaceName)
		{
			this.InterfaceName = interfaceName;
		}

		public static string GetInterfaceName(Type wrapperType)
			=> $"I{wrapperType.Name}";
	}

	public static class ComInterfaceWrapperAttributeExtensions
	{
		/// <summary>
		/// Gets COM interface name if specific type has '<see cref="ComInterfaceWrapperAttribute"/>' attribute.
		/// </summary>
		public static string GetComInterfaceNameIfWrapper(this Type type)
		{
			var attr = type.GetCustomAttribute<ComInterfaceWrapperAttribute>();
			return attr != null 
				? attr.InterfaceName ?? $"I{type.Name}" 
				: null;
		}
	}
}
