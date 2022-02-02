namespace WindowsDesktop.Interop;

[ComInterface]
public interface IVirtualDesktop
{
    bool IsViewVisible(IntPtr hWnd);

    Guid GetID();
    
    string GetName();
    
    string GetWallpaperPath();
}
