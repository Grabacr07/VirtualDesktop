namespace WindowsDesktop.Interop;

[ComInterface]
public interface IApplicationViewCollection
{
    IApplicationView GetViewForHwnd(IntPtr hWnd);
}
