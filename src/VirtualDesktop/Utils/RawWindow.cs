using System;
using System.Windows.Interop;
using System.Windows.Threading;
using WindowsDesktop.Interop;

namespace WindowsDesktop.Utils;

internal abstract class RawWindow
{
    private HwndSource? _source;
    
    public IntPtr Handle
        => this._source?.Handle ?? IntPtr.Zero;
    public string? Name { get; init; }

    public virtual void Show()
    {
        this.Show(new HwndSourceParameters(this.Name));
    }

    protected void Show(HwndSourceParameters parameters)
    {
        this._source = new HwndSource(parameters);
        this._source.AddHook(this.WndProc);
    }

    public virtual void Close()
    {
        this._source?.RemoveHook(this.WndProc);
        // Source could have been created on a different thread, which means we 
        // have to Dispose of it on the UI thread or it will crash.
        this._source?.Dispatcher?.BeginInvoke(DispatcherPriority.Send, () => this._source?.Dispose());
        this._source = null;
			
        PInvoke.CloseWindow(this.Handle);
    }

    protected virtual IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
    {
        return IntPtr.Zero;
    }
}
