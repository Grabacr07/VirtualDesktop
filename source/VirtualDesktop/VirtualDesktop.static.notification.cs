using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using WindowsDesktop.Interop;

namespace WindowsDesktop
{
	partial class VirtualDesktop
	{
		/// <summary>
		/// Occurs when a virtual desktop is created.
		/// </summary>
		public static event EventHandler<VirtualDesktop> Created;

		public static event EventHandler<VirtualDesktopDestroyEventArgs> DestroyBegin;

		public static event EventHandler<VirtualDesktopDestroyEventArgs> DestroyFailed;

		/// <summary>
		/// Occurs when a virtual desktop is destroyed.
		/// </summary>
		public static event EventHandler<VirtualDesktopDestroyEventArgs> Destroyed;

		[EditorBrowsable(EditorBrowsableState.Never)]
		public static event EventHandler ApplicationViewChanged;

		/// <summary>
		/// Occurs when the current virtual desktop is changed.
		/// </summary>
		public static event EventHandler<VirtualDesktopChangedEventArgs> CurrentChanged;

		/// <summary>
		/// Occurs when a virtual desktop is moved.
		/// </summary>
		public static event EventHandler<VirtualDesktopMovedEventArgs> Moved;

		/// <summary>
		/// Occurs when a virtual desktop is renamed.
		/// </summary>
		public static event EventHandler<VirtualDesktopRenamedEventArgs> Renamed;

		/// <summary>
		/// Occurs when the wallpaper in the virtual desktop is changed.
		/// </summary>
		public static event EventHandler<VirtualDesktopWallpaperChangedEventArgs> WallpaperChanged;

		internal static class EventRaiser
		{
			public static void RaiseCreated(object sender, VirtualDesktop pDesktop)
			{
				Created?.Invoke(sender, pDesktop);
			}

			public static void RaiseDestroyBegin(object sender, VirtualDesktop pDesktopDestroyed, VirtualDesktop pDesktopFallback)
			{
				var args = new VirtualDesktopDestroyEventArgs(pDesktopDestroyed, pDesktopFallback);
				DestroyBegin?.Invoke(sender, args);
			}

			public static void RaiseDestroyFailed(object sender, VirtualDesktop pDesktopDestroyed, VirtualDesktop pDesktopFallback)
			{
				var args = new VirtualDesktopDestroyEventArgs(pDesktopDestroyed, pDesktopFallback);
				DestroyFailed?.Invoke(sender, args);
			}

			public static void RaiseDestroyed(object sender, VirtualDesktop pDesktopDestroyed, VirtualDesktop pDesktopFallback)
			{
				var args = new VirtualDesktopDestroyEventArgs(pDesktopDestroyed, pDesktopFallback);
				Destroyed?.Invoke(sender, args);
			}

			public static void RaiseApplicationViewChanged(object sender, object pView)
			{
				ApplicationViewChanged?.Invoke(sender, EventArgs.Empty);
			}

			public static void RaiseCurrentChanged(object sender, VirtualDesktop pDesktopOld, VirtualDesktop pDesktopNew)
			{
				var args = new VirtualDesktopChangedEventArgs(pDesktopOld, pDesktopNew);
				CurrentChanged?.Invoke(sender, args);
			}

			public static void RaiseMoved(object sender, VirtualDesktop pDesktopMoved, int oldIndex, int newIndex)
			{
				var args = new VirtualDesktopMovedEventArgs(pDesktopMoved, oldIndex, newIndex);
				Moved?.Invoke(sender, args);
			}

			public static void RaiseRenamed(object sender, VirtualDesktop pDesktop, string name)
			{
				var oldName = pDesktop.Name;
				pDesktop.SetNameToCache(name);

				var args = new VirtualDesktopRenamedEventArgs(pDesktop, oldName, name);
				Renamed?.Invoke(sender, args);
			}

			public static void RaiseWallpaperChanged(object sender, VirtualDesktop pDesktop, string path)
			{
				var oldPath = pDesktop.WallpaperPath;
				pDesktop.SetWallpaperPathToCache(path);

				var args = new VirtualDesktopWallpaperChangedEventArgs(pDesktop, oldPath, path);
				WallpaperChanged?.Invoke(sender, args);
			}
		}
	}
}
