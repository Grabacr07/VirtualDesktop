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
	/// Encapsulates a Windows 10 virtual desktop.
	/// </summary>
	[ComInterfaceWrapper]
	[DebuggerDisplay("{Id}")]
	[UsedImplicitly(ImplicitUseTargetFlags.Members)]
	public partial class VirtualDesktop : ComInterfaceWrapperBase
	{
		/// <summary>
		/// Gets the unique identifier for this virtual desktop.
		/// </summary>
		public Guid Id { get; }

		[UsedImplicitly]
		internal VirtualDesktop(ComInterfaceAssembly assembly, Guid id, object comObject)
			: base(assembly, comObject)
		{
			this.Id = id;
		}

		/// <summary>
		/// Switches to this virtual desktop.
		/// </summary>
		public void Switch()
		{
			ComInterface.VirtualDesktopManagerInternal.SwitchDesktop(this);
		}

		/// <summary>
		/// Removes this virtual desktop and switches to an available one.
		/// </summary>
		/// <remarks>If this is the last virtual desktop, a new one will be created to switch to.</remarks>
		public void Remove()
		{
			var fallback = ComInterface.VirtualDesktopManagerInternal.GetDesktops().FirstOrDefault(x => x.Id != this.Id) ?? Create();
			this.Remove(fallback);
		}

		/// <summary>
		/// Removes this virtual desktop and switches to <paramref name="fallbackDesktop" />.
		/// </summary>
		/// <param name="fallbackDesktop">A virtual desktop to be displayed after the virtual desktop is removed.</param>
		public void Remove(VirtualDesktop fallbackDesktop)
		{
			if (fallbackDesktop == null) throw new ArgumentNullException(nameof(fallbackDesktop));

			ComInterface.VirtualDesktopManagerInternal.RemoveDesktop(this, fallbackDesktop);
		}

		/// <summary>
		/// Returns the adjacent virtual desktop on the left, or null if there isn't one.
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
		/// Returns the adjacent virtual desktop on the right, or null if there isn't one.
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
