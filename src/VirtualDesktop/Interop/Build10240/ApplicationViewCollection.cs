using System;

namespace WindowsDesktop.Interop.Build10240
{
    public class ApplicationViewCollection : ComWrapperBase<IApplicationViewCollection>, IApplicationViewCollection
    {
        internal ApplicationViewCollection(ComInterfaceAssembly assembly)
            : base(assembly)
        {
        }

        public ApplicationView GetViewForHwnd(IntPtr hWnd)
        {
            var view = this.InvokeMethod<object>(Args(hWnd))
                ?? new ArgumentException("ApplicationView is not found.", nameof(hWnd));

            return new ApplicationView(this.ComInterfaceAssembly, view);
        }

        IApplicationView IApplicationViewCollection.GetViewForHwnd(IntPtr hWnd)
            => this.GetViewForHwnd(hWnd);
    }
}
