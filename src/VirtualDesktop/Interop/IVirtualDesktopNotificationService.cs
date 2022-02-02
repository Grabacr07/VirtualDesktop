namespace WindowsDesktop.Interop;

[ComInterface]
public interface IVirtualDesktopNotificationService
{
    IDisposable Register(IVirtualDesktopNotification proxy);
}
