using WindowsDesktop.Interop;

namespace WindowsDesktop;

public static class VirtualDesktopExtensions
{
    internal static VirtualDesktop ToVirtualDesktop(this IVirtualDesktop desktop)
        => VirtualDesktop.FromComObject(desktop);
}
