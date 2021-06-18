using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using WindowsDesktop.Interop;
using WindowsDesktop.Properties;
using JetBrains.Annotations;

namespace WindowsDesktop
{
	/// <summary>
	/// Encapsulates a Windows 10 virtual desktop.
	/// </summary>
	[ComInterfaceWrapper(2)]
	[DebuggerDisplay("{Id}")]
	[UsedImplicitly(ImplicitUseTargetFlags.Members)]
	public partial class VirtualDesktop : ComInterfaceWrapperBase, IDisposable
	{
		/// <summary>
		/// Gets the unique identifier for this virtual desktop.
		/// </summary>
		public Guid Id { get; }

		/// <summary>
		/// Gets the index for the virtual desktop.
		/// </summary>
		public int Index
		{
			get
			{
				var desktops = GetDesktops();
				var index = Array.IndexOf(desktops, this);
				return index;
			}
		}

		private string _name = null;

		/// <summary>
		/// Gets the name for the virtual desktop.
		/// </summary>
		public string Name
		{
			get => this._name;
			set
			{
				if (ProductInfo.OSBuild < 20231 && this.ComVersion < 2) throw new PlatformNotSupportedException("This Windows 10 version is not supported.");

				ComInterface.VirtualDesktopManagerInternal.SetName(this, value);
			}
		}

		private string _wallpaperPath = null;

		/// <summary>
		/// Gets the name for the virtual desktop.
		/// </summary>
		public string WallpaperPath
		{
			get => this._wallpaperPath;
			set
			{
				if (ProductInfo.OSBuild < 21313) throw new PlatformNotSupportedException("This Windows 10 version is not supported.");

				ComInterface.VirtualDesktopManagerInternal.SetWallpaperPath(this, value);
			}
		}

		[UsedImplicitly]
		internal VirtualDesktop(ComInterfaceAssembly assembly, Guid id, object comObject)
			: base(assembly, comObject, latestVersion: 2)
		{
			this.Id = id;
			
			if (ProductInfo.OSBuild >= 20231 || this.ComVersion >= 2)
			{
				this._name = this.Invoke<string>(Args(), "GetName");

				if (ProductInfo.OSBuild >= 21313)
				{
					this._wallpaperPath = this.Invoke<string>(Args(), "GetWallpaperPath");
				}
			}
		}

		/// <summary>
		/// Switches to this virtual desktop.
		/// </summary>
		public void Switch()
		{
			ComInterface.VirtualDesktopManagerInternal.SwitchDesktop(this);
		}

		/// <summary>
		/// Moves this virtual desktop to a new location.
		/// </summary>
		/// <param name="index">The zero-based index specifying the new location of the virtual desktop.</param>
		public void Move(int index)
		{
			ComInterface.VirtualDesktopManagerInternal.MoveDesktop(this, index);
		}

		/// <summary>
		/// Removes this virtual desktop and switches to an available one.
		/// </summary>
		/// <remarks>If this is the last virtual desktop, a new one will be created to switch to.</remarks>
		public void Remove()
		{
			var fallback = ComInterface.VirtualDesktopManagerInternal.GetDesktops().FirstOrDefault(x => x.Id != this.Id) ?? Create();
			this.Remove(fallback);
		}

		/// <summary>
		/// Removes this virtual desktop and switches to <paramref name="fallbackDesktop" />.
		/// </summary>
		/// <param name="fallbackDesktop">A virtual desktop to be displayed after the virtual desktop is removed.</param>
		public void Remove(VirtualDesktop fallbackDesktop)
		{
			if (fallbackDesktop == null) throw new ArgumentNullException(nameof(fallbackDesktop));

			ComInterface.VirtualDesktopManagerInternal.RemoveDesktop(this, fallbackDesktop);
		}

		/// <summary>
		/// Returns the adjacent virtual desktop on the left, or null if there isn't one.
		/// </summary>
		public VirtualDesktop GetLeft()
		{
			try
			{
				return ComInterface.VirtualDesktopManagerInternal.GetAdjacentDesktop(this, AdjacentDesktop.LeftDirection);
			}
			catch (COMException ex) when (ex.Match(HResult.TYPE_E_OUTOFBOUNDS))
			{
				return null;
			}
		}

		/// <summary>
		/// Returns the adjacent virtual desktop on the right, or null if there isn't one.
		/// </summary>
		public VirtualDesktop GetRight()
		{
			try
			{
				return ComInterface.VirtualDesktopManagerInternal.GetAdjacentDesktop(this, AdjacentDesktop.RightDirection);
			}
			catch (COMException ex) when (ex.Match(HResult.TYPE_E_OUTOFBOUNDS))
			{
				return null;
			}
		}

		private void SetNameToCache(string name)
		{
			if (this._name == name) return;

			this.RaisePropertyChanging(nameof(this.Name));
			this._name = name;
			this.RaisePropertyChanged(nameof(this.Name));
		}

		private void SetWallpaperPathToCache(string path)
		{
			if (this._wallpaperPath == path) return;

			this.RaisePropertyChanging(nameof(this.WallpaperPath));
			this._wallpaperPath = path;
			this.RaisePropertyChanged(nameof(this.WallpaperPath));
		}
		
#region IDisposable
		private bool _disposed = false;

		/// <summary>
		/// Disposes of this <see cref="VirtualDesktop"/>.
		/// </summary>
		/// <param name="disposeOfManagedObjects">If <see langword="true"/>, disposes of managed objects.</param>
		protected virtual void Dispose(bool disposeOfManagedObjects)
		{
			if (!this._disposed)
			{
				if (disposeOfManagedObjects)
				{
					this.Remove();
				}

				this._disposed = true;
			}
		}

		/// <summary>
		/// Disposes of this <see cref="VirtualDesktop"/>.
		/// </summary>
		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}
#endregion
	}
}
