using System;
using WindowsDesktop.Interop;

namespace WindowsDesktop
{
	public interface IVirtualDesktopCache
	{
		Func<Guid, object, VirtualDesktop> Factory { get; set; }

		VirtualDesktop GetOrCreate(object comObject);

		void Clear();
	}

	internal static class VirtualDesktopCache
	{
		private static IVirtualDesktopCache _cache;

		public static void Initialize(ComInterfaceAssembly assembly)
		{
			if (_cache == null)
			{
				var type = assembly.GetType("VirtualDesktopCacheImpl");
				_cache = (IVirtualDesktopCache)Activator.CreateInstance(type);
				_cache.Factory = (id, comObject) => new VirtualDesktop(assembly, id, comObject);
			}
			else
			{
				_cache.Clear();
			}
		}

		public static VirtualDesktop GetOrCreate(object comObject)
			=> _cache.GetOrCreate(comObject);
	}
}
