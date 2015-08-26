using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using WindowsDesktop.Interop;

namespace WindowsDesktop
{
	partial class VirtualDesktop
	{
		private static readonly bool isSupportedInternal = true;
		private static readonly ConcurrentDictionary<Guid, VirtualDesktop> wrappers = new ConcurrentDictionary<Guid, VirtualDesktop>();

		internal static IVirtualDesktopManager ComManager { get; }
		internal static IVirtualDesktopManagerInternal ComInternal { get; }

		public static bool IsSupported => (Environment.OSVersion.Version.Major >= 10) && isSupportedInternal;

		[EditorBrowsable(EditorBrowsableState.Never)]
		public static Exception InitializationException { get; }

		static VirtualDesktop()
		{
			if (!IsSupported) return;

			try
			{
				ComManager = VirtualDesktopInteropHelper.GetVirtualDesktopManager();
				ComInternal = VirtualDesktopInteropHelper.GetVirtualDesktopManagerInternal();
			}
			catch (Exception ex)
			{
				InitializationException = ex;
				isSupportedInternal = false;
			}
		}


		public static VirtualDesktop[] GetDesktops()
		{
			VirtualDesktopHelper.ThrowIfNotSupported();

			return GetDesktopsInternal().ToArray();
		}

		internal static IEnumerable<VirtualDesktop> GetDesktopsInternal()
		{
			var desktops = ComInternal.GetDesktops();
			var count = desktops.GetCount();

			for (var i = 0u; i < count; i++)
			{
				object ppvObject;
				desktops.GetAt(i, typeof(IVirtualDesktop).GUID, out ppvObject);

				var desktop = (IVirtualDesktop)ppvObject;
				var wrapper = wrappers.GetOrAdd(desktop.GetID(), _ => new VirtualDesktop(desktop));

				yield return wrapper;
			}
		}

		public static VirtualDesktop Create()
		{
			VirtualDesktopHelper.ThrowIfNotSupported();

			var desktop = ComInternal.CreateDesktopW();
			var wrapper = wrappers.GetOrAdd(desktop.GetID(), _ => new VirtualDesktop(desktop));

			return wrapper;
		}

		public static VirtualDesktop FromId(Guid desktopId)
		{
			VirtualDesktopHelper.ThrowIfNotSupported();

			IVirtualDesktop desktop;
			try
			{
				desktop = ComInternal.FindDesktop(desktopId);
			}
			catch (COMException ex) when (ex.Match(HResult.TYPE_E_ELEMENTNOTFOUND))
			{
				return null;
			}
			var wrapper = wrappers.GetOrAdd(desktop.GetID(), _ => new VirtualDesktop(desktop));

			return wrapper;
		}

		public static VirtualDesktop FromHwnd(IntPtr hwnd)
		{
			VirtualDesktopHelper.ThrowIfNotSupported();

			if (hwnd == IntPtr.Zero) return null;

			IVirtualDesktop desktop;
			try
			{
				var desktopId = ComManager.GetWindowDesktopId(hwnd);
				desktop = ComInternal.FindDesktop(desktopId);
			}
			catch (COMException ex) when (ex.Match(HResult.REGDB_E_CLASSNOTREG, HResult.TYPE_E_ELEMENTNOTFOUND))
			{
				return null;
			}
			var wrapper = wrappers.GetOrAdd(desktop.GetID(), _ => new VirtualDesktop(desktop));

			return wrapper;
		}
	}
}
