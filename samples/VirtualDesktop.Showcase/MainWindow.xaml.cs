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
				await VirtualDesktopProvider.Default.Initialize(TaskScheduler.FromCurrentSynchronizationContext());
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, "Failed to initialize.");
			}

			VirtualDesktop.CurrentChanged += (sender, args) => System.Diagnostics.Debug.WriteLine($"Desktop changed: {args.NewDesktop.Id}");
			VirtualDesktop.Moved += (sender, args) => System.Diagnostics.Debug.WriteLine($"Desktop moved: {args.OldIndex} -> {args.NewIndex} ({args.Source.Id})");
			VirtualDesktop.Renamed += (sender, args) => System.Diagnostics.Debug.WriteLine($"Desktop renamed: {args.OldName} -> {args.NewName} ({args.Source.Id})");
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
				if (appId != null) (VirtualDesktop.IsPinnedApplication(appId) ? VirtualDesktop.UnpinApplication : (Action<string>)VirtualDesktop.PinApplication)(appId);
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

		private async void GetName(object sender, RoutedEventArgs e)
		{
			if (this.ThisWindowMenu.IsChecked ?? false)
			{
				var name = this.GetCurrentDesktop().Name;
				MessageBox.Show(name, "Current desktop name");
			}
			else
			{
				await Task.Delay(_delay);
				var name = this.GetCurrentDesktop().Name;
				MessageBox.Show(name, "Current desktop name");
			}
		}

		private async void SetName(object sender, RoutedEventArgs e)
		{
			if (this.ThisWindowMenu.IsChecked ?? false)
			{
				try
				{
					this.GetCurrentDesktop().Name = this.NameTextBlock.Text;
				}
				catch (PlatformNotSupportedException ex)
				{
					MessageBox.Show(ex.Message, "Error");
				}
			}
			else
			{
				await Task.Delay(_delay);
				try
				{
					this.GetCurrentDesktop().Name = this.NameTextBlock.Text;
				}
				catch (PlatformNotSupportedException ex)
				{
					MessageBox.Show(ex.Message, "Error");
				}
			}
		}

		private async void GetWallpaperPath(object sender, RoutedEventArgs e)
		{
			if (this.ThisWindowMenu.IsChecked ?? false)
			{
				var name = this.GetCurrentDesktop().WallpaperPath;
				MessageBox.Show(name, "Current wallpaper path");
			}
			else
			{
				await Task.Delay(_delay);
				var name = this.GetCurrentDesktop().WallpaperPath;
				MessageBox.Show(name, "Current wallpaper path");
			}
		}

		private void MovePrevious(object sender, RoutedEventArgs e)
		{
			var desktop = this.GetCurrentDesktop();
			if (desktop == null) return;

			desktop.Move(desktop.Index - 1);
		}

		private void MoveNext(object sender, RoutedEventArgs e)
		{
			var desktop = this.GetCurrentDesktop();
			if (desktop == null) return;

			desktop.Move(desktop.Index + 1);
		}


		[DllImport("user32.dll")]
		private static extern IntPtr GetForegroundWindow();
	}
}
