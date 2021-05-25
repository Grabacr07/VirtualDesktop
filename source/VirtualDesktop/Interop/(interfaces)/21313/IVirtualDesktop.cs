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
		[return: MarshalAs(UnmanagedType.Bool)]
		bool IsViewVisible(IApplicationView pView);

		Guid GetID();

		IntPtr Unknown1();

		[return: MarshalAs(UnmanagedType.HString)]
		string GetName();

		[return: MarshalAs(UnmanagedType.HString)]
		string GetWallpaperPath();
	}

	// see also:
	//  https://github.com/MScholtes/VirtualDesktop/blob/f7c0018069f5500bce3b170a53fb71edee44ebec/VirtualDesktop.cs#L156-L173

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
