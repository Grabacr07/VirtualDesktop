using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace WindowsDesktop.Interop
{
	public abstract class ComInterfaceWrapperBase
	{
		private readonly Dictionary<string, MethodInfo> _methods = new Dictionary<string, MethodInfo>();
		
		private protected ComInterfaceAssembly ComInterfaceAssembly { get; }

		public Type ComInterfaceType { get; }

		public object ComObject { get; }

		public uint ComVersion { get; }

		private protected ComInterfaceWrapperBase(ComInterfaceAssembly assembly, string comInterfaceName = null, uint latestVersion = 1, Guid? service = null)
		{
			var comInterfaceName2 = comInterfaceName ?? this.GetType().GetComInterfaceNameIfWrapper();
			for (var version = latestVersion; version >= 1; version--)
			{
				var type = assembly.GetType(version != 1 ? $"{comInterfaceName2}{version}" : comInterfaceName2);
				if (type != null)
				{
					var instance = assembly.CreateInstance(type, service);
					this.ComInterfaceAssembly = assembly;
					this.ComInterfaceType = type;
					this.ComObject = instance;
					this.ComVersion = version;
					return;
				}
			}

			throw new InvalidOperationException($"{comInterfaceName2} or later version is not found.");
		}

		private protected ComInterfaceWrapperBase(ComInterfaceAssembly assembly, object comObject, string comInterfaceName = null, uint latestVersion = 1)
		{
			var comInterfaceName2 = comInterfaceName ?? this.GetType().GetComInterfaceNameIfWrapper();
			for (var version = latestVersion; version >= 1; version--)
			{
				var type = assembly.GetType(version != 1 ? $"{comInterfaceName2}{version}" : comInterfaceName2);
				if (type != null)
				{
					this.ComInterfaceAssembly = assembly;
					this.ComInterfaceType = type;
					this.ComObject = comObject;
					this.ComVersion = version;
					return;
				}
			}

			throw new InvalidOperationException($"{comInterfaceName2} or later version is not found.");
		}

		protected static object[] Args(params object[] args)
			=> args;

		protected void Invoke(object[] parameters = null, [CallerMemberName] string methodName = "")
			=> this.Invoke<object>(parameters, methodName);

		protected T Invoke<T>(object[] parameters = null, [CallerMemberName] string methodName = "")
		{
			if (!this._methods.TryGetValue(methodName, out var methodInfo))
			{
				this._methods[methodName] = methodInfo = this.ComInterfaceType.GetMethod(methodName);

				if (methodInfo == null)
				{
					throw new NotSupportedException($"{methodName} is not supported.");
				}
			}

			try
			{
				return (T)methodInfo.Invoke(this.ComObject, parameters);
			}
			catch (TargetInvocationException ex) when (ex.InnerException != null)
			{
				throw ex.InnerException;
			}
		}
	}
}
