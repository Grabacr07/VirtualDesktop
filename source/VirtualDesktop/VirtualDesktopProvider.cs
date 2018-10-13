using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WindowsDesktop.Interop;

namespace WindowsDesktop
{
	public class VirtualDesktopProvider : IDisposable
	{
		#region Default instance

		private static readonly Lazy<VirtualDesktopProvider> _default = new Lazy<VirtualDesktopProvider>(() => new VirtualDesktopProvider());

		public static VirtualDesktopProvider Default => _default.Value;

		#endregion

		public string ComInterfaceAssemblyPath { get; set; }

		public bool AutoRestart { get; set; } = true;

		internal ComObjects ComObjects { get; private set; }

		public Task InitializeAsync()
			=> Task.Run(Initialize);

		public void Initialize()
		{
			var assemblyProvider = new ComInterfaceAssemblyProvider(this.ComInterfaceAssemblyPath);
			var assembly = new ComInterfaceAssembly(assemblyProvider.GetAssembly());

			this.ComObjects = new ComObjects(assembly);

			if (this.AutoRestart)
				this.ComObjects.Listen();
		}

		public void Dispose()
		{
			this.ComObjects?.Dispose();
		}
	}

	partial class VirtualDesktop
	{
		public static VirtualDesktopProvider Provider { get; set; }

		internal static VirtualDesktopProvider ProviderInternal
			=> Provider ?? VirtualDesktopProvider.Default;
	}
}
