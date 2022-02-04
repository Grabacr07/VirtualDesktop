namespace WindowsDesktop.Interop.Proxy;

[ComInterface]
public interface IVirtualDesktopNotificationService
{
    IDisposable Register(IVirtualDesktopNotification proxy);
}
