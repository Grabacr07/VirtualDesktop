
namespace WindowsDesktop.Interop
{
	internal static class ComInterface
	{
		public static IVirtualDesktopManager VirtualDesktopManager
			=> VirtualDesktop.ProviderInternal.ComObjects.VirtualDesktopManager;

		public static VirtualDesktopManagerInternal VirtualDesktopManagerInternal
			=> VirtualDesktop.ProviderInternal.ComObjects.VirtualDesktopManagerInternal;

		public static VirtualDesktopNotificationService VirtualDesktopNotificationService
			=> VirtualDesktop.ProviderInternal.ComObjects.VirtualDesktopNotificationService;

		public static VirtualDesktopPinnedApps VirtualDesktopPinnedApps
			=> VirtualDesktop.ProviderInternal.ComObjects.VirtualDesktopPinnedApps;

		public static ApplicationViewCollection ApplicationViewCollection
			=> VirtualDesktop.ProviderInternal.ComObjects.ApplicationViewCollection;
	}
}
