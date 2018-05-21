using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace WindowsDesktop.Interop
{
	internal abstract class ComInterfaceWrapperBase
	{
		private readonly Dictionary<string, MethodInfo> _methods = new Dictionary<string, MethodInfo>();

		public Type InterfaceType { get; }

		public object Instance { get; }

		protected ComInterfaceWrapperBase(string comInterfaceName = null, Guid? service = null)
		{
			var (type, instance) = VirtualDesktop.ProviderInternal.CreateInstance(comInterfaceName ?? this.GetType().GetComInterfaceNameIfWrapper(), service);

			this.InterfaceType = type;
			this.Instance = instance;
		}

		protected ComInterfaceWrapperBase(object instance, string comInterfaceName = null)
		{
			this.InterfaceType = VirtualDesktop.ProviderInternal.GetType(comInterfaceName ?? this.GetType().GetComInterfaceNameIfWrapper());
			this.Instance = instance;
		}

		protected static object[] Args(params object[] args)
			=> args;

		protected void Invoke(object[] parameters = null, [CallerMemberName] string methodName = "")
			=> this.Invoke<object>(parameters, methodName);

		protected T Invoke<T>(object[] parameters = null, [CallerMemberName] string methodName = "")
		{
			if (!this._methods.TryGetValue(methodName, out var methodInfo))
			{
				this._methods[methodName] = methodInfo = this.InterfaceType.GetMethod(methodName);

				if (methodInfo == null)
				{
					throw new NotSupportedException($"{methodName} is not supported.");
				}
			}

			return (T)methodInfo.Invoke(this.Instance, parameters);
		}
	}
}
