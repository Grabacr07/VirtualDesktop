namespace WindowsDesktop.Interop.Proxy;

public enum AdjacentDesktop
{
    LeftDirection = 3,

    RightDirection = 4,
}

[ComInterface]
public interface IVirtualDesktopManagerInternal
{
    IEnumerable<IVirtualDesktop> GetDesktops();

    IVirtualDesktop GetCurrentDesktop();

    IVirtualDesktop GetAdjacentDesktop(IVirtualDesktop pDesktopReference, AdjacentDesktop uDirection);

    IVirtualDesktop FindDesktop(Guid desktopId);

    IVirtualDesktop CreateDesktop();

    void RemoveDesktop(IVirtualDesktop pRemove, IVirtualDesktop pFallbackDesktop);

    void SwitchDesktop(IVirtualDesktop desktop);

    void MoveViewToDesktop(IntPtr hWnd, IVirtualDesktop desktop);

    void SetDesktopName(IVirtualDesktop desktop, string name);

    void SetDesktopWallpaper(IVirtualDesktop desktop, string path);

    void UpdateWallpaperPathForAllDesktops(string path);
}
