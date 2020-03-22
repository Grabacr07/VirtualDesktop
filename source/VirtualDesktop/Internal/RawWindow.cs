using System;
using System.Windows.Interop;
using System.Windows.Threading;
using WindowsDesktop.Interop;

namespace WindowsDesktop.Internal
{
	internal abstract class RawWindow
	{
		public string Name { get; set; }

		public HwndSource Source { get; private set; }

		public IntPtr Handle => this.Source?.Handle ?? IntPtr.Zero;

		public virtual void Show()
		{
			this.Show(new HwndSourceParameters(this.Name));
		}

		protected void Show(HwndSourceParameters parameters)
		{
			this.Source = new HwndSource(parameters);
			this.Source.AddHook(this.WndProc);
		}

		public virtual void Close()
		{
			this.Source?.RemoveHook(this.WndProc);
			// Source could have been created on a different thread, which means we 
			// have to Dispose of it on the UI thread or it will crash.
			this.Source?.Dispatcher?.BeginInvoke(DispatcherPriority.Send, (Action)(() => this.Source?.Dispose()));
			this.Source = null;

			NativeMethods.CloseWindow(this.Handle);
		}

		protected virtual IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
		{
			return IntPtr.Zero;
		}
	}
}
