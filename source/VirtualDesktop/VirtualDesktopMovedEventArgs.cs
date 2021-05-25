using System;

namespace WindowsDesktop
{
	/// <summary>
	/// Provides data for the <see cref="VirtualDesktop.WallpaperChanged" /> event.
	/// </summary>
	public class VirtualDesktopMovedEventArgs : EventArgs
	{
		/// <summary>
		/// Gets the virtual desktop that was moved.
		/// </summary>
		public VirtualDesktop Source { get; }

		/// <summary>
		/// Gets the old index of the virtual desktop.
		/// </summary>
		public int OldIndex { get; }

		/// <summary>
		/// Gets the new index of the virtual desktop.
		/// </summary>
		public int NewIndex { get; }

		public VirtualDesktopMovedEventArgs(VirtualDesktop source, int oldIndex, int newIndex)
		{
			this.Source = source;
			this.OldIndex = oldIndex;
			this.NewIndex = newIndex;
		}
	}
}
