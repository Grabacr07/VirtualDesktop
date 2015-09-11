using System;

namespace WindowsDesktop
{
	public class VirtualDesktopChangedEventArgs : EventArgs
	{
		public VirtualDesktop OldDesktop { get; }
		public VirtualDesktop NewDesktop { get; }

		public VirtualDesktopChangedEventArgs(VirtualDesktop oldDesktop, VirtualDesktop newDesktop)
		{
			this.OldDesktop = oldDesktop;
			this.NewDesktop = newDesktop;
		}
	}
}
