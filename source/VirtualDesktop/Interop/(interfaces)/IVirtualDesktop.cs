using System;
using System.Collections.Concurrent;
using System.Runtime.InteropServices;

namespace WindowsDesktop.Interop
{
	[ComImport]
	[Guid("00000000-0000-0000-0000-000000000000") /* replace at runtime */]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	public interface IVirtualDesktop
	{
		bool IsViewVisible(object pView);

		Guid GetID();
	}

	public class VirtualDesktopCacheImpl : IVirtualDesktopCache
	{
		private readonly ConcurrentDictionary<Guid, VirtualDesktop> _wrappers = new ConcurrentDictionary<Guid, VirtualDesktop>();

		public Func<Guid, object, VirtualDesktop> Factory { get; set; }

		public VirtualDesktop GetOrCreate(object comObject)
		{
			if (comObject is IVirtualDesktop)
			{
				return this._wrappers.GetOrAdd(((IVirtualDesktop)comObject).GetID(), id => this.Factory(id, comObject));
			}

			throw new ArgumentException();
		}

		public void Clear()
		{
			this._wrappers.Clear();
		}
	}
}
