using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsDesktop
{
	public static class FormExtensions
	{
		/// <summary>
		/// Determines whether this form is on the current virtual desktop.
		/// </summary>
		public static bool IsCurrentVirtualDesktop(this Form form)
		{
			return VirtualDesktopHelper.IsCurrentVirtualDesktop(form.Handle);
		}

		/// <summary>
		/// Moves a form to the specified virtual desktop.
		/// </summary>
		public static void MoveToDesktop(this Form form, VirtualDesktop virtualDesktop)
		{
			VirtualDesktopHelper.MoveToDesktop(form.Handle, virtualDesktop);
		}

		/// <summary>
		/// Returns the virtual desktop this form is located on.
		/// </summary>
		public static VirtualDesktop GetCurrentDesktop(this Form form)
		{
			return VirtualDesktop.FromHwnd(form.Handle);
		}
	}
}
