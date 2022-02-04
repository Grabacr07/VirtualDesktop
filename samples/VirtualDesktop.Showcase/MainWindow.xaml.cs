using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using WindowsDesktop;
using WindowsDesktop.Properties;

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

		private static void InitializeComObjects()
        {
            VirtualDesktop.Configure(new VirtualDesktopConfiguration());

            VirtualDesktop.CurrentChanged += (_, args) => Debug.WriteLine($"Switched: {args.OldDesktop.Name} -> {args.NewDesktop.Name}");
            VirtualDesktop.Renamed += (_, args) => Debug.WriteLine($"Renamed: {args.Desktop}");
            VirtualDesktop.WallpaperChanged += (_, args) => Debug.WriteLine($"Wallpaper changed: {args.Desktop}, {args.Path}");

            foreach (var desktop in VirtualDesktop.GetDesktops())
            {
                Debug.WriteLine($"Detected: {desktop}, {desktop.WallpaperPath}");
            }
        }

		private void CreateNew(object sender, RoutedEventArgs e)
            => VirtualDesktop.Create().Switch();

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
				VirtualDesktop.MoveToDesktop(GetForegroundWindow(), desktop);
			}

			desktop.Switch();
		}

		private void SwitchLeft(object sender, RoutedEventArgs e)
		{
			this.GetCurrentDesktop()?.GetLeft()?.Switch();
		}

		private async void SwitchLeftAndMove(object sender, RoutedEventArgs e)
		{
			var left = this.GetCurrentDesktop()?.GetLeft();
			if (left == null) return;

			if (this.ThisWindowMenu.IsChecked ?? false)
			{
				this.MoveToDesktop(left);
			}
			else
			{
				await Task.Delay(_delay);
				VirtualDesktop.MoveToDesktop(GetForegroundWindow(), left);
			}

			left.Switch();
        }

		private void SwitchRight(object sender, RoutedEventArgs e)
		{
			this.GetCurrentDesktop()?.GetRight()?.Switch();
		}

		private async void SwitchRightAndMove(object sender, RoutedEventArgs e)
		{
			var right = this.GetCurrentDesktop()?.GetRight();
			if (right == null) return;

			if (this.ThisWindowMenu.IsChecked ?? false)
			{
				this.MoveToDesktop(right);
			}
			else
			{
				await Task.Delay(_delay);
				VirtualDesktop.MoveToDesktop(GetForegroundWindow(), right);
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
				var appId = VirtualDesktop.GetAppId(GetForegroundWindow());
				if (appId != null) (VirtualDesktop.IsPinnedApplication(appId) ? VirtualDesktop.UnpinApplication : (Action<string>)VirtualDesktop.PinApplication)(appId);
			}
		}

		private async void Remove(object sender, RoutedEventArgs e)
		{
			if (this.ThisWindowMenu.IsChecked ?? false)
			{
				this.GetCurrentDesktop()?.Remove();
			}
			else
			{
				await Task.Delay(_delay);
				this.GetCurrentDesktop()?.Remove();
			}
		}


		[DllImport("user32.dll")]
		private static extern IntPtr GetForegroundWindow();
	}
}
