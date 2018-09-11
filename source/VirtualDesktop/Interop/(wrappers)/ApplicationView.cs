using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using JetBrains.Annotations;

namespace WindowsDesktop.Interop
{
	[ComInterfaceWrapper]
	internal class ApplicationView : ComInterfaceWrapperBase
	{
		public ApplicationView(ComInterfaceAssembly assembly, object comObject, string comInterfaceName = null)
			: base(assembly, comObject, comInterfaceName) { }

		public string GetAppUserModelId()
		{
			var param = Args((string)null);
			this.Invoke(param);

			return (string)param[0];
		}
	}

	[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
	[StructLayout(LayoutKind.Sequential)]
	public struct Size
	{
		public int X;
		public int Y;
	}

	[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
	[StructLayout(LayoutKind.Sequential)]
	public struct Rect
	{
		public int Left;
		public int Top;
		public int Right;
		public int Bottom;
	}

	[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
	public enum ApplicationViewCloakType
	{
		// ReSharper disable InconsistentNaming
		AVCT_NONE = 0,
		AVCT_DEFAULT = 1,
		AVCT_VIRTUAL_DESKTOP = 2
		// ReSharper restore InconsistentNaming
	}

	[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
	public enum ApplicationViewCompatibilityPolicy
	{
		// ReSharper disable InconsistentNaming
		AVCP_NONE = 0,
		AVCP_SMALL_SCREEN = 1,
		AVCP_TABLET_SMALL_SCREEN = 2,
		AVCP_VERY_SMALL_SCREEN = 3,
		AVCP_HIGH_SCALE_FACTOR = 4
		// ReSharper restore InconsistentNaming
	}
}
