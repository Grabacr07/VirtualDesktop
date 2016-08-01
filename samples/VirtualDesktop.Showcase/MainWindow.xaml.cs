using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using WindowsDesktop;

namespace VirtualDesktopShowcase
{
	partial class MainWindow
	{
		public MainWindow()
		{
			this.InitializeComponent();

			foreach (var id in VirtualDesktop.GetDesktops().Select(x => x.Id))
			{
				System.Diagnostics.Debug.WriteLine(id);
			}
		}

		private void CreateNew(object sender, RoutedEventArgs e)
		{
			VirtualDesktop.Create().Switch();
		}

		private void CreateNewAndMove(object sender, RoutedEventArgs e)
		{
			var desktop = VirtualDesktop.Create();

			this.MoveToDesktop(desktop);
			desktop.Switch();
		}

		private void SwitchLeft(object sender, RoutedEventArgs e)
		{
			this.GetCurrentDesktop().GetLeft()?.Switch();
		}

		private void SwitchLeftAndMove(object sender, RoutedEventArgs e)
		{
			var left = this.GetCurrentDesktop().GetLeft();
			if (left == null) return;

			this.MoveToDesktop(left);
			left.Switch();
		}

		private void SwitchRight(object sender, RoutedEventArgs e)
		{
			this.GetCurrentDesktop().GetRight()?.Switch();
		}

		private void SwitchRightAndMove(object sender, RoutedEventArgs e)
		{
			var right = this.GetCurrentDesktop().GetRight();
			if (right == null) return;

			this.MoveToDesktop(right);
			right.Switch();
		}

		private void Pin(object sender, RoutedEventArgs e)
		{
			this.TogglePin();
		}


		[DllImport("user32.dll")]
		private static extern IntPtr GetForegroundWindow();
	}
}
