using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using WindowsDesktop.Interop;

namespace WindowsDesktop
{
	public partial class VirtualDesktop
	{
		public Guid Id { get; }

		[EditorBrowsable(EditorBrowsableState.Never)]
		public IVirtualDesktop ComObject { get; }

		private VirtualDesktop(IVirtualDesktop comObject)
		{
			this.ComObject = comObject;
			this.Id = comObject.GetID();
		}

		public void Switch()
		{
			ComInternal.SwitchDesktop(this.ComObject);
		}

		public void Remove()
		{
			this.Remove(GetDesktopsInternal().FirstOrDefault(x => x.Id != this.Id) ?? Create());
		}

		public void Remove(VirtualDesktop fallbackDesktop)
		{
			if (fallbackDesktop == null) throw new ArgumentNullException(nameof(fallbackDesktop));

			ComInternal.RemoveDesktop(this.ComObject, fallbackDesktop.ComObject);
		}

		public VirtualDesktop GetLeft()
		{
			IVirtualDesktop desktop;
			try
			{
				desktop = ComInternal.GetAdjacentDesktop(this.ComObject, AdjacentDesktop.LeftDirection);
			}
			catch (COMException ex) when (((uint)ex.HResult) == HResult.TYPE_E_OUTOFBOUNDS)
			{
				return null;
			}
			var wrapper = wrappers.GetOrAdd(desktop.GetID(), _ => new VirtualDesktop(desktop));

			return wrapper;
		}

		public VirtualDesktop GetRight()
		{
			IVirtualDesktop desktop;
			try
			{
				desktop = ComInternal.GetAdjacentDesktop(this.ComObject, AdjacentDesktop.RightDirection);
			}
			catch (COMException ex) when (((uint)ex.HResult) == HResult.TYPE_E_OUTOFBOUNDS)
			{
				return null;
			}
			var wrapper = wrappers.GetOrAdd(desktop.GetID(), _ => new VirtualDesktop(desktop));

			return wrapper;
		}
	}
}
