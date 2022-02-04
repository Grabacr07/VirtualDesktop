using WindowsDesktop.Interop;

namespace WindowsDesktop.Utils;

internal class ExplorerRestartListenerWindow : TransparentWindow
{
    private uint _explorerRestartedMessage;
    private readonly Action _action;

    public ExplorerRestartListenerWindow(Action action)
    {
        this.Name = nameof(ExplorerRestartListenerWindow);
        this._action = action;
    }

    public override void Show()
    {
        base.Show();
        this._explorerRestartedMessage = PInvoke.RegisterWindowMessage("TaskbarCreated");
    }

    protected override IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
    {
        if (msg == this._explorerRestartedMessage)
        {
            this._action();
            return IntPtr.Zero;
        }

        return base.WndProc(hwnd, msg, wParam, lParam, ref handled);
    }
}
