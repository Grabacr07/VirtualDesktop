using System;
using System.Runtime.InteropServices;

namespace WindowsDesktop.Interop
{
	[ComImport]
	[Guid("af8da486-95bb-4460-b3b7-6e7a6b2962b5")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	public interface IVirtualDesktopManagerInternal
	{
		int GetCount();

		void MoveViewToDesktop(object pView, IVirtualDesktop desktop);

		bool CanViewMoveDesktops(object pView);

		IVirtualDesktop GetCurrentDesktop();

		IObjectArray GetDesktops();

		IVirtualDesktop GetAdjacentDesktop(IVirtualDesktop pDesktopReference, AdjacentDesktop uDirection);

		void SwitchDesktop(IVirtualDesktop desktop);

		IVirtualDesktop CreateDesktopW();

		void RemoveDesktop(IVirtualDesktop pRemove, IVirtualDesktop pFallbackDesktop);

		IVirtualDesktop FindDesktop(ref Guid desktopId);
	}

	public enum AdjacentDesktop
	{
		LeftDirection = 3,
		RightDirection = 4
	}
	
}
