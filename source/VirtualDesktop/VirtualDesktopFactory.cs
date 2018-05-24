using System;
using WindowsDesktop.Interop;

namespace WindowsDesktop
{
	public interface IVirtualDesktopFactory
	{
		Func<Guid, object, VirtualDesktop> Factory { get; set; }

		VirtualDesktop Get(object comObject);
	}

	internal class VirtualDesktopFactory
	{
		private static IVirtualDesktopFactory _factory;

		public VirtualDesktopFactory(ComInterfaceAssembly assembly)
		{
			if (_factory == null)
			{
				var type = assembly.GetType("VirtualDesktopFactoryImpl");
				_factory = (IVirtualDesktopFactory)Activator.CreateInstance(type);
				_factory.Factory = (id, comObject) => new VirtualDesktop(assembly, id, comObject);
			}
		}

		public VirtualDesktop Get(object comObject)
			=> _factory.Get(comObject);
	}
}
