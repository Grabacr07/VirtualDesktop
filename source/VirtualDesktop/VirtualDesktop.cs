using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using WindowsDesktop.Interop;

namespace WindowsDesktop
{
	/// <summary>
	/// Encapsulates a virtual desktop on Windows 10.
	/// </summary>
	[DebuggerDisplay("{Id}")]
	public partial class VirtualDesktop
	{
		/// <summary>
		/// Gets the unique identifier for the virtual desktop.
		/// </summary>
		public Guid Id { get; }

		[EditorBrowsable(EditorBrowsableState.Never)]
		public IVirtualDesktop ComObject { get; }

		private VirtualDesktop(IVirtualDesktop comObject)
		{
			this.ComObject = comObject;
			this.Id = comObject.GetID();
		}


		/// <summary>
		/// Display the virtual desktop.
		/// </summary>
		public void Switch()
		{
			ComInternal.SwitchDesktop(this.ComObject);
		}

		/// <summary>
		/// Remove the virtual desktop.
		/// </summary>
		public void Remove()
		{
			this.Remove(GetDesktopsInternal().FirstOrDefault(x => x.Id != this.Id) ?? Create());
		}

		/// <summary>
		/// Remove the virtual desktop, specifying a virtual desktop that display after destroyed.
		/// </summary>
		public void Remove(VirtualDesktop fallbackDesktop)
		{
			if (fallbackDesktop == null) throw new ArgumentNullException(nameof(fallbackDesktop));

			ComInternal.RemoveDesktop(this.ComObject, fallbackDesktop.ComObject);
		}

		/// <summary>
		/// Returns a virtual desktop on the left.
		/// </summary>
		public VirtualDesktop GetLeft()
		{
			IVirtualDesktop desktop;
			try
			{
				desktop = ComInternal.GetAdjacentDesktop(this.ComObject, AdjacentDesktop.LeftDirection);
			}
			catch (COMException ex) when (ex.Match(HResult.TYPE_E_OUTOFBOUNDS))
			{
				return null;
			}
			var wrapper = wrappers.GetOrAdd(desktop.GetID(), _ => new VirtualDesktop(desktop));

			return wrapper;
		}

		/// <summary>
		/// Returns a virtual desktop on the right.
		/// </summary>
		public VirtualDesktop GetRight()
		{
			IVirtualDesktop desktop;
			try
			{
				desktop = ComInternal.GetAdjacentDesktop(this.ComObject, AdjacentDesktop.RightDirection);
			}
			catch (COMException ex) when (ex.Match(HResult.TYPE_E_OUTOFBOUNDS))
			{
				return null;
			}
			var wrapper = wrappers.GetOrAdd(desktop.GetID(), _ => new VirtualDesktop(desktop));

			return wrapper;
		}
	}
}
