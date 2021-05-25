using System;

namespace WindowsDesktop
{
	/// <summary>
	/// Provides data for the <see cref="VirtualDesktop.WallpaperChanged" /> event.
	/// </summary>
	public class VirtualDesktopWallpaperChangedEventArgs : EventArgs
	{
		/// <summary>
		/// Gets the virtual desktop whose wallpaper path was changed.
		/// </summary>
		public VirtualDesktop Source { get; }

		/// <summary>
		/// Gets the old wallpaper path of the virtual desktop.
		/// </summary>
		public string OldPath { get; }

		/// <summary>
		/// Gets the new wallpaper path of the virtual desktop.
		/// </summary>
		public string NewPath { get; }

		public VirtualDesktopWallpaperChangedEventArgs(VirtualDesktop source, string oldPath, string newPath)
		{
			this.Source = source;
			this.OldPath = oldPath;
			this.NewPath = newPath;
		}
	}
}
