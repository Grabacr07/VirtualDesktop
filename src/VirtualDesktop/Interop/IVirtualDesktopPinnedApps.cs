namespace WindowsDesktop.Interop;

[ComInterface]
public interface IVirtualDesktopPinnedApps
{
    bool IsAppIdPinned(string appId);

    void PinAppID(string appId);

    void UnpinAppID(string appId);

    bool IsViewPinned(IntPtr hWnd);

    void PinView(IntPtr hWnd);

    void UnpinView(IntPtr hWnd);
}
