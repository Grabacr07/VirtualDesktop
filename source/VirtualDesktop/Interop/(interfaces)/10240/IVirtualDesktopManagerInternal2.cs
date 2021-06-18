using System;
using System.Runtime.InteropServices;

namespace WindowsDesktop.Interop
{
	[ComImport]
	[Guid("00000000-0000-0000-0000-000000000000") /* replace at runtime */]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	internal interface IVirtualDesktopManagerInternal2
	{
		int GetCount();

		void MoveViewToDesktop(IApplicationView pView, IVirtualDesktop pDesktop);

		[return: MarshalAs(UnmanagedType.Bool)]
		bool CanViewMoveDesktops(IApplicationView pView);

		IVirtualDesktop GetCurrentDesktop();

		IObjectArray GetDesktops();

		IVirtualDesktop GetAdjacentDesktop(IVirtualDesktop pDesktopReference, AdjacentDesktop uDirection);

		void SwitchDesktop(IVirtualDesktop pDesktop);

		IVirtualDesktop CreateDesktopW();

		void RemoveDesktop(IVirtualDesktop pRemove, IVirtualDesktop pFallbackDesktop);

		IVirtualDesktop FindDesktop([In, MarshalAs(UnmanagedType.LPStruct)] Guid desktopId);

		void Unknown1(IVirtualDesktop pDesktop, out IObjectArray pUnknown1, out IObjectArray pUnknown2);

		void SetName(IVirtualDesktop pDesktop, [MarshalAs(UnmanagedType.HString)] string chName);
	}

	// see also:
	//  https://github.com/MScholtes/VirtualDesktop/blob/f7c0018069f5500bce3b170a53fb71edee44ebec/VirtualDesktop.cs#L193-L211
}
