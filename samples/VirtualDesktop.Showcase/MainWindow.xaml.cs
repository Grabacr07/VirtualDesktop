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
		private static readonly int _delay = 2000;

		public MainWindow()
		{
			this.InitializeComponent();
			InitializeComObjects();
		}

		private static async void InitializeComObjects()
		{
			try
			{
				await VirtualDesktopProvider.Default.Initialize();
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, "Failed to initialize.");
			}

			VirtualDesktop.CurrentChanged += (sender, args) => System.Diagnostics.Debug.WriteLine($"Desktop changed: {args.NewDesktop.Id}");
		}

		private void CreateNew(object sender, RoutedEventArgs e)
		{
			VirtualDesktop.Create().Switch();
		}

		private async void CreateNewAndMove(object sender, RoutedEventArgs e)
		{
			var desktop = VirtualDesktop.Create();

			if (this.ThisWindowMenu.IsChecked ?? false)
			{
				this.MoveToDesktop(desktop);
			}
			else
			{
				await Task.Delay(_delay);
				VirtualDesktopHelper.MoveToDesktop(GetForegroundWindow(), desktop);
			}

			desktop.Switch();
		}

		private void SwitchLeft(object sender, RoutedEventArgs e)
		{
			this.GetCurrentDesktop().GetLeft()?.Switch();
		}

		private async void SwitchLeftAndMove(object sender, RoutedEventArgs e)
		{
			var left = this.GetCurrentDesktop().GetLeft();
			if (left == null) return;

			if (this.ThisWindowMenu.IsChecked ?? false)
			{
				this.MoveToDesktop(left);
			}
			else
			{
				await Task.Delay(_delay);
				VirtualDesktopHelper.MoveToDesktop(GetForegroundWindow(), left);
			}

			left.Switch();
		}

		private void SwitchRight(object sender, RoutedEventArgs e)
		{
			this.GetCurrentDesktop().GetRight()?.Switch();
		}

		private async void SwitchRightAndMove(object sender, RoutedEventArgs e)
		{
			var right = this.GetCurrentDesktop().GetRight();
			if (right == null) return;

			if (this.ThisWindowMenu.IsChecked ?? false)
			{
				this.MoveToDesktop(right);
			}
			else
			{
				await Task.Delay(_delay);
				VirtualDesktopHelper.MoveToDesktop(GetForegroundWindow(), right);
			}

			right.Switch();
		}

		private async void Pin(object sender, RoutedEventArgs e)
		{
			if (this.ThisWindowMenu.IsChecked ?? false)
			{
				this.TogglePin();
			}
			else
			{
				await Task.Delay(_delay);
				var handle = GetForegroundWindow();
				(VirtualDesktop.IsPinnedWindow(handle) ? VirtualDesktop.UnpinWindow : (Action<IntPtr>)VirtualDesktop.PinWindow)(handle);
			}
		}

		private async void PinApp(object sender, RoutedEventArgs e)
		{
			if (this.ThisWindowMenu.IsChecked ?? false)
			{
				Application.Current.TogglePin();
			}
			else
			{
				await Task.Delay(_delay);
				var appId = ApplicationHelper.GetAppId(GetForegroundWindow());
				(VirtualDesktop.IsPinnedApplication(appId) ? VirtualDesktop.UnpinApplication : (Action<string>)VirtualDesktop.PinApplication)(appId);
			}
		}

		private async void Remove(object sender, RoutedEventArgs e)
		{
			if (this.ThisWindowMenu.IsChecked ?? false)
			{
				this.GetCurrentDesktop().Remove();
			}
			else
			{
				await Task.Delay(_delay);
				this.GetCurrentDesktop().Remove();
			}
		}


		[DllImport("user32.dll")]
		private static extern IntPtr GetForegroundWindow();
	}
}
