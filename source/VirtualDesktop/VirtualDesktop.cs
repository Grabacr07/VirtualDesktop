using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using WindowsDesktop.Interop;
using JetBrains.Annotations;

namespace WindowsDesktop
{
	/// <summary>
	/// Encapsulates a virtual desktop on Windows 10.
	/// </summary>
	[ComInterfaceWrapper]
	[DebuggerDisplay("{Id}")]
	[UsedImplicitly(ImplicitUseTargetFlags.Members)]
	public partial class VirtualDesktop : ComInterfaceWrapperBase
	{
		/// <summary>
		/// Gets the unique identifier for the virtual desktop.
		/// </summary>
		public Guid Id { get; }

		[UsedImplicitly]
		internal VirtualDesktop(ComInterfaceAssembly assembly, Guid id, object comObject)
			: base(assembly, comObject)
		{
			this.Id = id;
		}

		/// <summary>
		/// Display the virtual desktop.
		/// </summary>
		public void Switch()
		{
			ComInterface.VirtualDesktopManagerInternal.SwitchDesktop(this);
		}

		/// <summary>
		/// Remove the virtual desktop.
		/// </summary>
		public void Remove()
		{
			var fallback = ComInterface.VirtualDesktopManagerInternal.GetDesktops().FirstOrDefault(x => x.Id != this.Id) ?? Create();
			this.Remove(fallback);
		}

		/// <summary>
		/// Remove the virtual desktop, specifying a virtual desktop that display after destroyed.
		/// </summary>
		public void Remove(VirtualDesktop fallbackDesktop)
		{
			if (fallbackDesktop == null) throw new ArgumentNullException(nameof(fallbackDesktop));

			ComInterface.VirtualDesktopManagerInternal.RemoveDesktop(this, fallbackDesktop);
		}

		/// <summary>
		/// Returns a virtual desktop on the left.
		/// </summary>
		public VirtualDesktop GetLeft()
		{
			try
			{
				return ComInterface.VirtualDesktopManagerInternal.GetAdjacentDesktop(this, AdjacentDesktop.LeftDirection);
			}
			catch (COMException ex) when (ex.Match(HResult.TYPE_E_OUTOFBOUNDS))
			{
				return null;
			}
		}

		/// <summary>
		/// Returns a virtual desktop on the right.
		/// </summary>
		public VirtualDesktop GetRight()
		{
			try
			{
				return ComInterface.VirtualDesktopManagerInternal.GetAdjacentDesktop(this, AdjacentDesktop.RightDirection);
			}
			catch (COMException ex) when (ex.Match(HResult.TYPE_E_OUTOFBOUNDS))
			{
				return null;
			}
		}
	}
}
