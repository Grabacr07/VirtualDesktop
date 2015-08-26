using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WindowsDesktop;

namespace VirtualDesktopShowcase
{
	partial class MainWindow
	{
		public MainWindow()
		{
			this.InitializeComponent();
		}

		protected override async void OnContentRendered(EventArgs e)
		{
			base.OnContentRendered(e);

			await Task.Delay(5000);

			this.GetCurrentDesktop().GetRight()?.SwitchAndMove(this);

			await Task.Delay(1000);

			this.GetCurrentDesktop().GetRight()?.SwitchAndMove(this);

			await Task.Delay(1000);

			var desktop = VirtualDesktop.FromHwnd((IntPtr)1283123);
		}
	}
}
