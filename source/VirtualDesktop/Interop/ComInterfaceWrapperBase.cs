using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace WindowsDesktop.Interop
{
	internal abstract class ComInterfaceWrapperBase
	{
		private readonly object _instance;
		private readonly Type _interfaceType;
		private readonly Dictionary<string, MethodInfo> _methods = new Dictionary<string, MethodInfo>();

		protected ComInterfaceWrapperBase(string comInterfaceName = null)
		{
			var instance = ComActivator.CreateInstance(comInterfaceName ?? this.GetType().GetComInterfaceNameIfWrapper());

			this._instance = instance.obj;
			this._interfaceType = instance.type;
		}

		protected static object[] Args(params object[] args)
			=> args;

		protected void Invoke(object[] parameters = null, [CallerMemberName] string methodName = "")
			=> this.Invoke<object>(parameters, methodName);

		protected T Invoke<T>(object[] parameters = null, [CallerMemberName] string methodName = "")
		{
			if (!this._methods.TryGetValue(methodName, out var methodInfo))
			{
				this._methods[methodName] = methodInfo = this._interfaceType.GetMethod(methodName);

				if (methodInfo == null)
				{
					throw new NotSupportedException($"{methodName} is not supported.");
				}
			}

			return (T)methodInfo.Invoke(this._instance, parameters);
		}
	}
}
