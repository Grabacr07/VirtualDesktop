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

		private protected ComInterfaceWrapperBase(ComInterfaceAssembly assembly, string comInterfaceName = null, Guid? service = null)
		{
			var (type, instance) = assembly.CreateInstance(comInterfaceName ?? this.GetType().GetComInterfaceNameIfWrapper(), service);

			this.ComInterfaceAssembly = assembly;
			this.ComInterfaceType = type;
			this.ComObject = instance;
		}

		private protected ComInterfaceWrapperBase(ComInterfaceAssembly assembly, object comObject, string comInterfaceName = null)
		{
			this.ComInterfaceAssembly = assembly;
			this.ComInterfaceType = assembly.GetType(comInterfaceName ?? this.GetType().GetComInterfaceNameIfWrapper());
			this.ComObject = comObject;
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
