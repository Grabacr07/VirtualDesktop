using System;
using System.Linq;
// ReSharper disable InconsistentNaming

namespace WindowsDesktop.Interop
{
	public enum HResult : uint
	{
		TYPE_E_OUTOFBOUNDS = 0x80028CA1,
		TYPE_E_ELEMENTNOTFOUND = 0x8002802B,
		REGDB_E_CLASSNOTREG = 0x80040154,
	}

	public static class HResultExtensions
	{
		public static bool Match(this Exception ex, params HResult[] hResult)
		{
			return hResult.Select(x => (uint)x).Any(x => ((uint)ex.HResult) == x);
		}
	}
}
