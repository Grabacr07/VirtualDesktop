namespace WindowsDesktop.Interop.Proxy;

[ComInterface]
public interface IApplicationViewCollection
{
    IApplicationView GetViewForHwnd(IntPtr hWnd);
}
