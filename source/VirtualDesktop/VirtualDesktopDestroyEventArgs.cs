using System;

namespace WindowsDesktop
{
	public class VirtualDesktopDestroyEventArgs : EventArgs
	{
		public VirtualDesktop Destroyed { get; }
		public VirtualDesktop Fallback { get; }

		public VirtualDesktopDestroyEventArgs(VirtualDesktop destroyed, VirtualDesktop fallback)
		{
			this.Destroyed = destroyed;
			this.Fallback = fallback;
		}
	}
}
