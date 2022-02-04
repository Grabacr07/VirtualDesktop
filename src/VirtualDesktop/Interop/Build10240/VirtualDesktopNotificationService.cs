using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using WindowsDesktop.Interop.Proxy;
using WindowsDesktop.Utils;

namespace WindowsDesktop.Interop.Build10240;

public class VirtualDesktopNotificationService : ComWrapperBase<IVirtualDesktopNotificationService>, IVirtualDesktopNotificationService
{
    private readonly ComWrapperFactory _factory;

    internal VirtualDesktopNotificationService(ComInterfaceAssembly assembly, ComWrapperFactory factory)
        : base(assembly, CLSID.VirtualDesktopNotificationService)
    {
        this._factory = factory;
    }

    public IDisposable Register(IVirtualDesktopNotification notification)
    {
        var type = this.ComInterfaceAssembly.GetType("VirtualDesktopNotification");
        var listener = Activator.CreateInstance(type) as EventListenerBase
            ?? throw new Exception($"{nameof(EventListenerBase)} inheritance type is not found in the COM interface assembly.");

        listener.Notification = notification;
        listener.Factory = this._factory;

        var dwCookie = this.InvokeMethod<uint>(Args(listener));
        return Disposable.Create(() => this.Unregister(dwCookie));
    }

    private void Unregister(uint dwCookie)
    {
        try
        {
            this.InvokeMethod(Args(dwCookie));
        }
        catch (COMException ex) when (ex.Match(HResult.RPC_S_SERVER_UNAVAILABLE))
        {
            // Nothing particular to do.
        }
    }

    public abstract class EventListenerBase
    {
        internal ComWrapperFactory Factory { get; set; } = null!;

        internal IVirtualDesktopNotification Notification { get; set; } = null!;

        protected void CreatedCore(object pDesktop)
            => this.Notification.VirtualDesktopCreated(this.Wrap(pDesktop));

        protected void DestroyBeginCore(object pDesktopDestroyed, object pDesktopFallback)
            => this.Notification.VirtualDesktopDestroyBegin(this.Wrap(pDesktopDestroyed), this.Wrap(pDesktopFallback));

        protected void DestroyFailedCore(object pDesktopDestroyed, object pDesktopFallback)
            => this.Notification.VirtualDesktopDestroyFailed(this.Wrap(pDesktopDestroyed), this.Wrap(pDesktopFallback));

        protected void DestroyedCore(object pDesktopDestroyed, object pDesktopFallback)
            => this.Notification.VirtualDesktopDestroyed(this.Wrap(pDesktopDestroyed), this.Wrap(pDesktopFallback));

        protected void ViewChangedCore(object view)
            => this.Notification.ViewVirtualDesktopChanged(this.Factory.ApplicationView(view).Interface);

        protected void CurrentChangedCore(object pDesktopOld, object pDesktopNew)
            => this.Notification.CurrentVirtualDesktopChanged(this.Wrap(pDesktopOld), this.Wrap(pDesktopNew));
        
        private IVirtualDesktop Wrap(object desktop)
            => this.Factory.VirtualDesktop(desktop).Interface;
    }
}
