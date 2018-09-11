using System;
using System.Runtime.InteropServices;

namespace WindowsDesktop.Interop
{
	[ComImport]
	[Guid("00000000-0000-0000-0000-000000000000") /* replace at runtime */]
	[InterfaceType(ComInterfaceType.InterfaceIsIInspectable)]
	public interface IApplicationView
	{
		int SetFocus();

		int SwitchTo();

		int TryInvokeBack(IntPtr /* IAsyncCallback* */ callback);

		int GetThumbnailWindow(out IntPtr hwnd);

		int GetMonitor(out IntPtr /* IImmersiveMonitor */ immersiveMonitor);

		int GetVisibility(out int visibility);

		int SetCloak(ApplicationViewCloakType cloakType, int unknown);

		int GetPosition(ref Guid guid /* GUID for IApplicationViewPosition */, out IntPtr /* IApplicationViewPosition** */ position);

		int SetPosition(ref IntPtr /* IApplicationViewPosition* */ position);

		int InsertAfterWindow(IntPtr hwnd);

		int GetExtendedFramePosition(out Rect rect);

		int GetAppUserModelId([MarshalAs(UnmanagedType.LPWStr)] out string id);

		int SetAppUserModelId(string id);

		int IsEqualByAppUserModelId(string id, out int result);

		int GetViewState(out uint state);

		int SetViewState(uint state);

		int GetNeediness(out int neediness);

		int GetLastActivationTimestamp(out ulong timestamp);

		int SetLastActivationTimestamp(ulong timestamp);

		int GetVirtualDesktopId(out Guid guid);

		int SetVirtualDesktopId(ref Guid guid);

		int GetShowInSwitchers(out int flag);

		int SetShowInSwitchers(int flag);

		int GetScaleFactor(out int factor);

		int CanReceiveInput(out bool canReceiveInput);

		int GetCompatibilityPolicyType(out ApplicationViewCompatibilityPolicy flags);

		int SetCompatibilityPolicyType(ApplicationViewCompatibilityPolicy flags);

		int GetPositionPriority(out IntPtr /* IShellPositionerPriority** */ priority);

		int SetPositionPriority(IntPtr /* IShellPositionerPriority* */ priority);

		int GetSizeConstraints(IntPtr /* IImmersiveMonitor* */ monitor, out Size size1, out Size size2);

		int GetSizeConstraintsForDpi(uint uint1, out Size size1, out Size size2);

		int SetSizeConstraintsForDpi(ref uint uint1, ref Size size1, ref Size size2);

		int QuerySizeConstraintsFromApp();

		int OnMinSizePreferencesUpdated(IntPtr hwnd);

		int ApplyOperation(IntPtr /* IApplicationViewOperation* */ operation);

		int IsTray(out bool isTray);

		int IsInHighZOrderBand(out bool isInHighZOrderBand);

		int IsSplashScreenPresented(out bool isSplashScreenPresented);

		int Flash();

		int GetRootSwitchableOwner(out IApplicationView rootSwitchableOwner);

		int EnumerateOwnershipTree(out IObjectArray ownershipTree);

		/*** (Windows 10 Build 10584 or later?) ***/

		int GetEnterpriseId([MarshalAs(UnmanagedType.LPWStr)] out string enterpriseId);

		int IsMirrored(out bool isMirrored);
	}
}
