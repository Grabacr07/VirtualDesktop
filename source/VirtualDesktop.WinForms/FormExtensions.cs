using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsDesktop
{
	public static class FormExtensions
	{
		public static bool IsCurrentVirtualDesktop(this Form form)
		{
			return VirtualDesktopHelper.IsCurrentVirtualDesktop(form.Handle);
		}

		public static void MoveToDesktop(this Form form, VirtualDesktop virtualDesktop)
		{
			VirtualDesktopHelper.MoveToDesktop(form.Handle, virtualDesktop);
		}

		public static VirtualDesktop GetCurrentDesktop(this Form form)
		{
			return VirtualDesktop.FromHwnd(form.Handle);
		}
	}
}
