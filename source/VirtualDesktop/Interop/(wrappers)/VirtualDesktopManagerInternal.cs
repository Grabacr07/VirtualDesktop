using System;

namespace WindowsDesktop.Interop
{
	[ComInterfaceWrapper]
	internal class VirtualDesktopManagerInternal : ComInterfaceWrapperBase
	{
		public VirtualDesktopManagerInternal()
			: base(service: CLSID.VirtualDesktopAPIUnknown) { }
		
		public void MoveViewToDesktop(ApplicationView pView, IVirtualDesktop desktop)
		{
			this.Invoke(Args(pView.Instance, desktop));
		}

		public IVirtualDesktop GetCurrentDesktop()
		{
			return this.Invoke<IVirtualDesktop>();
		}

		public IObjectArray GetDesktops()
		{
			return this.Invoke<IObjectArray>();
		}

		public IVirtualDesktop GetAdjacentDesktop(IVirtualDesktop pDesktopReference, AdjacentDesktop uDirection)
		{
			return this.Invoke<IVirtualDesktop>(Args(pDesktopReference, uDirection));
		}

		public void SwitchDesktop(IVirtualDesktop desktop)
		{
			this.Invoke<IVirtualDesktop>(Args(desktop));
		}

		public IVirtualDesktop CreateDesktopW()
		{
			return this.Invoke<IVirtualDesktop>();
		}

		public void RemoveDesktop(IVirtualDesktop pRemove, IVirtualDesktop pFallbackDesktop)
		{
			this.Invoke(Args(pRemove, pFallbackDesktop));
		}

		public IVirtualDesktop FindDesktop(ref Guid desktopId)
		{
			return this.Invoke<IVirtualDesktop>(Args(desktopId));
		}
	}
}
