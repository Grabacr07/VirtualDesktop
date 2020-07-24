using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;

namespace WindowsDesktop.Interop
{
	[ComInterfaceWrapper(2)]
	internal class VirtualDesktopManagerInternal : ComInterfaceWrapperBase
	{
		public VirtualDesktopManagerInternal(ComInterfaceAssembly assembly)
			: base(assembly, latestVersion: 2, service: CLSID.VirtualDesktopAPIUnknown)
		{
		}

		public void MoveViewToDesktop(ApplicationView pView, VirtualDesktop desktop)
		{
			this.Invoke(Args(pView.ComObject, desktop.ComObject));
		}

		public VirtualDesktop GetCurrentDesktop()
		{
			return this.GetDesktop();
		}

		public IEnumerable<VirtualDesktop> GetDesktops()
		{
			var array = this.Invoke<IObjectArray>();
			var count = array.GetCount();
			var vdType = this.ComInterfaceAssembly.GetType("IVirtualDesktop");

			for (var i = 0u; i < count; i++)
			{
				array.GetAt(i, vdType.GUID, out var ppvObject);
				yield return VirtualDesktopCache.GetOrCreate(ppvObject);
			}
		}

		public VirtualDesktop GetAdjacentDesktop(VirtualDesktop pDesktopReference, AdjacentDesktop uDirection)
		{
			return this.GetDesktop(Args(pDesktopReference.ComObject, uDirection));
		}

		public void SwitchDesktop(VirtualDesktop desktop)
		{
			this.Invoke(Args(desktop.ComObject));
		}

		public VirtualDesktop CreateDesktopW()
		{
			return this.GetDesktop();
		}

		public void RemoveDesktop(VirtualDesktop pRemove, VirtualDesktop pFallbackDesktop)
		{
			this.Invoke(Args(pRemove.ComObject, pFallbackDesktop.ComObject));
		}

		public VirtualDesktop FindDesktop(ref Guid desktopId)
		{
			return this.GetDesktop(Args(desktopId));
		}

		private VirtualDesktop GetDesktop(object[] parameters = null, [CallerMemberName] string methodName = "")
			=> VirtualDesktopCache.GetOrCreate(this.Invoke<object>(parameters, methodName));

		public void SetName(VirtualDesktop desktop, string name)
		{
			if (this.ComVersion < 2) throw new PlatformNotSupportedException("This Windows 10 version is not supported.");

			this.Invoke(Args(desktop.ComObject, name));
		}
	}
}
