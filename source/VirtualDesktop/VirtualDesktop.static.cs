using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using WindowsDesktop.Interop;

namespace WindowsDesktop
{
	partial class VirtualDesktop
	{
		private static bool? _isSupported = true;

		/// <summary>
		/// Gets a value indicating whether the operating system is support virtual desktop.
		/// </summary>
		public static bool IsSupported => GetIsSupported();

		/// <summary>
		/// Gets the virtual desktop that is currently displayed.
		/// </summary>
		public static VirtualDesktop Current
		{
			get
			{
				VirtualDesktopHelper.ThrowIfNotSupported();
				return ComInterface.VirtualDesktopManagerInternal.GetCurrentDesktop();
			}
		}

		internal static bool GetIsSupported()
		{
			return _isSupported ?? (_isSupported = Core()).Value;
			
			bool Core()
			{
#if DEBUG
				if (Environment.OSVersion.Version.Major < 10) return false;
#endif
				try
				{
					ProviderInternal.Initialize().Wait();
				}
				catch (Exception ex)
				{
					System.Diagnostics.Debug.WriteLine("VirtualDesktop initialization error:");
					System.Diagnostics.Debug.WriteLine(ex);

					return false;
				}

				return true;
			}
		}

		/// <summary>
		/// Returns all the virtual desktops of currently valid.
		/// </summary>
		/// <returns></returns>
		public static VirtualDesktop[] GetDesktops()
		{
			VirtualDesktopHelper.ThrowIfNotSupported();

			return ComInterface.VirtualDesktopManagerInternal.GetDesktops().ToArray();
		}

		/// <summary>
		/// Creates a virtual desktop.
		/// </summary>
		public static VirtualDesktop Create()
		{
			VirtualDesktopHelper.ThrowIfNotSupported();

			return ComInterface.VirtualDesktopManagerInternal.CreateDesktopW();
		}

		/// <summary>
		/// Returns the virtual desktop of the specified identifier.
		/// </summary>
		public static VirtualDesktop FromId(Guid desktopId)
		{
			VirtualDesktopHelper.ThrowIfNotSupported();

			try
			{
				return ComInterface.VirtualDesktopManagerInternal.FindDesktop(ref desktopId);
			}
			catch (COMException ex) when (ex.Match(HResult.TYPE_E_ELEMENTNOTFOUND))
			{
				return null;
			}
		}

		/// <summary>
		/// Returns the virtual desktop that the specified window is located.
		/// </summary>
		public static VirtualDesktop FromHwnd(IntPtr hwnd)
		{
			VirtualDesktopHelper.ThrowIfNotSupported();

			if (hwnd == IntPtr.Zero) return null;

			try
			{
				var desktopId = ComInterface.VirtualDesktopManager.GetWindowDesktopId(hwnd);
				return ComInterface.VirtualDesktopManagerInternal.FindDesktop(ref desktopId);
			}
			catch (COMException ex) when (ex.Match(HResult.REGDB_E_CLASSNOTREG, HResult.TYPE_E_ELEMENTNOTFOUND))
			{
				return null;
			}
		}
	}
}
