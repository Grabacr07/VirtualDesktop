using System;

namespace WindowsDesktop
{
	/// <summary>
	/// Provides data for the <see cref="VirtualDesktop.Renamed" /> event.
	/// </summary>
	public class VirtualDesktopRenamedEventArgs : EventArgs
	{
		/// <summary>
		/// Gets the virtual desktop that was renamed.
		/// </summary>
		public VirtualDesktop Source { get; }

		/// <summary>
		/// Gets the old name of the virtual desktop.
		/// </summary>
		public string OldName { get; }

		/// <summary>
		/// Gets the new name of the virtual desktop.
		/// </summary>
		public string NewName { get; }

		public VirtualDesktopRenamedEventArgs(VirtualDesktop source, string oldName, string newName)
		{
			this.Source = source;
			this.OldName = oldName;
			this.NewName = newName;
		}
	}
}
