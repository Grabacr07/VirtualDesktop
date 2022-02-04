using System;
using System.Runtime.InteropServices;

namespace WindowsDesktop.Interop.Build10240
{
    // ## Fixes for breaking changes in .NET 5
    // * InterfaceIsIInspectable -> InterfaceIsIUnknown
    // * Add three dummy entries to the start of the interface; Proc3() - Proc5()
    // 
    // see also: https://docs.microsoft.com/en-us/dotnet/core/compatibility/interop/5.0/casting-rcw-to-inspectable-interface-throws-exception

    [ComImport]
    [Guid("00000000-0000-0000-0000-000000000000") /* replace at runtime */]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IApplicationView
    {
        void Proc3();

        void Proc4();

        void Proc5();

        void SetFocus();

        void SwitchTo();

        void TryInvokeBack(IntPtr callback);

        IntPtr GetThumbnailWindow();

        IntPtr GetMonitor();

        int GetVisibility();

        void SetCloak(ApplicationViewCloakType cloakType, int unknown);

        IntPtr GetPosition(in Guid guid, out IntPtr position);

        void SetPosition(in IntPtr position);

        void InsertAfterWindow(IntPtr hwnd);

        Rect GetExtendedFramePosition();

        [return: MarshalAs(UnmanagedType.LPWStr)]
        string GetAppUserModelId();

        void SetAppUserModelId([MarshalAs(UnmanagedType.LPWStr)] string id);

        bool IsEqualByAppUserModelId(string id);

        uint GetViewState();

        void SetViewState(uint state);

        int GetNeediness();

        ulong GetLastActivationTimestamp();

        void SetLastActivationTimestamp(ulong timestamp);

        Guid GetVirtualDesktopId();

        void SetVirtualDesktopId(in Guid guid);

        int GetShowInSwitchers();

        void SetShowInSwitchers(int flag);

        int GetScaleFactor();

        bool CanReceiveInput();

        ApplicationViewCompatibilityPolicy GetCompatibilityPolicyType();

        void SetCompatibilityPolicyType(ApplicationViewCompatibilityPolicy flags);

        IntPtr GetPositionPriority();

        void SetPositionPriority(IntPtr priority);

        void GetSizeConstraints(IntPtr monitor, out Size size1, out Size size2);

        void GetSizeConstraintsForDpi(uint uint1, out Size size1, out Size size2);

        void SetSizeConstraintsForDpi(ref uint uint1, in Size size1, in Size size2);

        int QuerySizeConstraintsFromApp();

        void OnMinSizePreferencesUpdated(IntPtr hwnd);

        void ApplyOperation(IntPtr operation);

        bool IsTray();

        bool IsInHighZOrderBand();

        bool IsSplashScreenPresented();

        void Flash();

        IApplicationView GetRootSwitchableOwner();

        IObjectArray EnumerateOwnershipTree();

        [return: MarshalAs(UnmanagedType.LPWStr)]
        string GetEnterpriseId();

        bool IsMirrored();
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Size
    {
        public int X;
        public int Y;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Rect
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;
    }

    public enum ApplicationViewCloakType
    {
        AVCT_NONE = 0,
        AVCT_DEFAULT = 1,
        AVCT_VIRTUAL_DESKTOP = 2
    }

    public enum ApplicationViewCompatibilityPolicy
    {
        AVCP_NONE = 0,
        AVCP_SMALL_SCREEN = 1,
        AVCP_TABLET_SMALL_SCREEN = 2,
        AVCP_VERY_SMALL_SCREEN = 3,
        AVCP_HIGH_SCALE_FACTOR = 4
    }
}
