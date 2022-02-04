namespace WindowsDesktop.Interop.Proxy;

[ComInterface]
public interface IApplicationView
{
    IntPtr GetThumbnailWindow();

    string GetAppUserModelId();

    Guid GetVirtualDesktopId();
}
