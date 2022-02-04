using WindowsDesktop.Interop;
using WindowsDesktop.Interop.Proxy;

namespace WindowsDesktop;

public static class VirtualDesktopExtensions
{
    internal static VirtualDesktop ToVirtualDesktop(this IVirtualDesktop desktop)
        => VirtualDesktop.FromComObject(desktop);
}
