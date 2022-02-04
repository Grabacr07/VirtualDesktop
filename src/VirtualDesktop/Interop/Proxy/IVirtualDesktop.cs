namespace WindowsDesktop.Interop.Proxy;

[ComInterface]
public interface IVirtualDesktop
{
    bool IsViewVisible(IntPtr hWnd);

    Guid GetID();
    
    string GetName();
    
    string GetWallpaperPath();
}
