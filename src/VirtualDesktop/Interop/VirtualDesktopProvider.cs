using System;
using System.Collections.Generic;
using System.Linq;
using WindowsDesktop.Interop.Proxy;
using WindowsDesktop.Properties;

namespace WindowsDesktop.Interop;

internal abstract class VirtualDesktopProvider
{
    public virtual bool IsSupported
        => true;

    public abstract IApplicationViewCollection ApplicationViewCollection { get; }

    public abstract IVirtualDesktopManager VirtualDesktopManager { get; }

    public abstract IVirtualDesktopManagerInternal VirtualDesktopManagerInternal { get; }

    public abstract IVirtualDesktopPinnedApps VirtualDesktopPinnedApps { get; }

    public abstract IVirtualDesktopNotificationService VirtualDesktopNotificationService { get; }

    public bool IsInitialized { get; internal set; }

    internal void Initialize(VirtualDesktopConfiguration configuration)
    {
        if (this.IsInitialized) return;

        var assemblyProvider = new ComInterfaceAssemblyProvider(configuration);
        var assembly = assemblyProvider.GetAssembly();

        this.InitializeCore(assembly, configuration);
        this.IsInitialized = true;
    }

    private protected abstract void InitializeCore(ComInterfaceAssembly assembly, VirtualDesktopConfiguration configuration);

    internal class NotSupported : VirtualDesktopProvider
    {
        public override bool IsSupported
            => false;

        public override IApplicationViewCollection ApplicationViewCollection
            => throw new NotSupportedException();

        public override IVirtualDesktopManager VirtualDesktopManager
            => throw new NotSupportedException();

        public override IVirtualDesktopManagerInternal VirtualDesktopManagerInternal
            => throw new NotSupportedException();

        public override IVirtualDesktopPinnedApps VirtualDesktopPinnedApps
            => throw new NotSupportedException();

        public override IVirtualDesktopNotificationService VirtualDesktopNotificationService
            => throw new NotSupportedException();

        private protected override void InitializeCore(ComInterfaceAssembly assembly, VirtualDesktopConfiguration configuration)
            => throw new NotSupportedException();
    }

    protected static InvalidOperationException InitializationIsRequired
        => new("Initialization is required.");
}
