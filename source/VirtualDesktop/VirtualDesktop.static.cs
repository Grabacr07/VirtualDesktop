﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using WindowsDesktop.Interop;

namespace WindowsDesktop
{
	partial class VirtualDesktop
	{
		private static readonly bool _isSupportedInternal = true;
		private static ConcurrentDictionary<Guid, VirtualDesktop> _wrappers = new ConcurrentDictionary<Guid, VirtualDesktop>();

		/// <summary>
		/// Gets a value indicating whether the operating system is support virtual desktop.
		/// </summary>
		public static bool IsSupported =>
#if DEBUG
			_isSupportedInternal;
#else
			Environment.OSVersion.Version.Major >= 10 && _isSupportedInternal;
#endif

		[EditorBrowsable(EditorBrowsableState.Never)]
		public static Exception InitializationException { get; }

		/// <summary>
		/// Gets the virtual desktop that is currently displayed.
		/// </summary>
		public static VirtualDesktop Current
		{
			get
			{
				VirtualDesktopHelper.ThrowIfNotSupported();

				var current = ComObjects.VirtualDesktopManagerInternal.GetCurrentDesktop();
				var wrapper = _wrappers.GetOrAdd(current.GetID(), _ => new VirtualDesktop(current));

				return wrapper;
			}
		}

		static VirtualDesktop()
		{
			if (!IsSupported) return;

			try
			{
				ComObjects.Initialize();
			}
			catch (Exception ex)
			{
				InitializationException = ex;
				_isSupportedInternal = false;
			}

			var clearWrappers = new Action(() => {
				var oldWrappers = Interlocked.Exchange(ref _wrappers, new ConcurrentDictionary<Guid, VirtualDesktop>());
				foreach (var v in oldWrappers.Values) { Marshal.ReleaseComObject(v.ComObject); }
			});

			VirtualDesktop.Created += (o, e) => clearWrappers();
			VirtualDesktop.Destroyed += (o, e) => clearWrappers();

			AppDomain.CurrentDomain.ProcessExit += (sender, args) => ComObjects.Terminate();
		}

		/// <summary>
		/// Returns all the virtual desktops of currently valid.
		/// </summary>
		/// <returns></returns>
		public static VirtualDesktop[] GetDesktops()
		{
			VirtualDesktopHelper.ThrowIfNotSupported();

			return GetDesktopsInternal().ToArray();
		}

		internal static IEnumerable<VirtualDesktop> GetDesktopsInternal()
		{
			var desktops = ComObjects.VirtualDesktopManagerInternal.GetDesktops();
			var count = desktops.GetCount();

			for (var i = 0u; i < count; i++)
			{
				object ppvObject;
				desktops.GetAt(i, typeof(IVirtualDesktop).GUID, out ppvObject);

				var desktop = (IVirtualDesktop)ppvObject;
				var wrapper = _wrappers.GetOrAdd(desktop.GetID(), _ => new VirtualDesktop(desktop));

				yield return wrapper;
			}
		}

		/// <summary>
		/// Creates a virtual desktop.
		/// </summary>
		public static VirtualDesktop Create()
		{
			VirtualDesktopHelper.ThrowIfNotSupported();

			var desktop = ComObjects.VirtualDesktopManagerInternal.CreateDesktopW();
			var wrapper = _wrappers.GetOrAdd(desktop.GetID(), _ => new VirtualDesktop(desktop));

			return wrapper;
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public static VirtualDesktop FromComObject(IVirtualDesktop desktop)
		{
			VirtualDesktopHelper.ThrowIfNotSupported();

			var wrapper = _wrappers.GetOrAdd(desktop.GetID(), _ => new VirtualDesktop(desktop));
			return wrapper;
		}

		/// <summary>
		/// Returns the virtual desktop of the specified identifier.
		/// </summary>
		public static VirtualDesktop FromId(Guid desktopId)
		{
			VirtualDesktopHelper.ThrowIfNotSupported();

			IVirtualDesktop desktop;
			try
			{
				desktop = ComObjects.VirtualDesktopManagerInternal.FindDesktop(ref desktopId);
			}
			catch (COMException ex) when (ex.Match(HResult.TYPE_E_ELEMENTNOTFOUND))
			{
				return null;
			}
			var wrapper = _wrappers.GetOrAdd(desktop.GetID(), _ => new VirtualDesktop(desktop));

			return wrapper;
		}

		/// <summary>
		/// Returns the virtual desktop that the specified window is located.
		/// </summary>
		public static VirtualDesktop FromHwnd(IntPtr hwnd)
		{
			VirtualDesktopHelper.ThrowIfNotSupported();

			if (hwnd == IntPtr.Zero) return null;

			IVirtualDesktop desktop;
			try
			{
				var desktopId = ComObjects.VirtualDesktopManager.GetWindowDesktopId(hwnd);
				desktop = ComObjects.VirtualDesktopManagerInternal.FindDesktop(ref desktopId);
			}
			catch (COMException ex) when (ex.Match(HResult.REGDB_E_CLASSNOTREG, HResult.TYPE_E_ELEMENTNOTFOUND))
			{
				return null;
			}
			var wrapper = _wrappers.GetOrAdd(desktop.GetID(), _ => new VirtualDesktop(desktop));

			return wrapper;
		}
	}
}
