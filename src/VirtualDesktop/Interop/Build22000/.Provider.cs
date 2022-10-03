using WindowsDesktop.Interop.Build10240;
using WindowsDesktop.Interop.Proxy;

namespace WindowsDesktop.Interop.Build22000;

internal class VirtualDesktopProvider22000 : VirtualDesktopProvider
{
    private IVirtualDesktopManager? _virtualDesktopManager;
    private ApplicationViewCollection? _applicationViewCollection;
    private VirtualDesktopManagerInternal? _virtualDesktopManagerInternal;
    private VirtualDesktopPinnedApps? _virtualDesktopPinnedApps;
    private VirtualDesktopNotificationService? _virtualDesktopNotificationService;

    public override IApplicationViewCollection ApplicationViewCollection
        => this._applicationViewCollection ?? throw InitializationIsRequired;

    public override IVirtualDesktopManager VirtualDesktopManager
        => this._virtualDesktopManager ?? throw InitializationIsRequired;

    public override IVirtualDesktopManagerInternal VirtualDesktopManagerInternal
        => this._virtualDesktopManagerInternal ?? throw InitializationIsRequired;

    public override IVirtualDesktopPinnedApps VirtualDesktopPinnedApps
        => this._virtualDesktopPinnedApps ?? throw InitializationIsRequired;

    public override IVirtualDesktopNotificationService VirtualDesktopNotificationService
        => this._virtualDesktopNotificationService ?? throw InitializationIsRequired;

    private protected override void InitializeCore(ComInterfaceAssembly assembly)
    {
        var type = Type.GetTypeFromCLSID(CLSID.VirtualDesktopManager)
            ?? throw new Exception($"No type found for CLSID '{CLSID.VirtualDesktopManager}'.");
        this._virtualDesktopManager = Activator.CreateInstance(type) is IVirtualDesktopManager manager
            ? manager
            : throw new Exception($"Failed to create instance of Type '{typeof(IVirtualDesktopManager)}'.");

        this._applicationViewCollection = new ApplicationViewCollection(assembly);
        var factory = new ComWrapperFactory(
            x => new ApplicationView(assembly, x),
            x => this._applicationViewCollection.GetViewForHwnd(x),
            x => new VirtualDesktop(assembly, x));
        this._virtualDesktopManagerInternal = new VirtualDesktopManagerInternal(assembly, factory);
        this._virtualDesktopPinnedApps = new VirtualDesktopPinnedApps(assembly, factory);
        this._virtualDesktopNotificationService = new VirtualDesktopNotificationService(assembly, factory);
    }

}
