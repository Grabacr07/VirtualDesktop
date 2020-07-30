using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace WindowsDesktop
{
	partial class VirtualDesktop : INotifyPropertyChanging, INotifyPropertyChanged
	{
		/// <summary>
		/// Occurs when a virtual desktop property changing.
		/// </summary>
		public event PropertyChangingEventHandler PropertyChanging;

		private void RaisePropertyChanging([CallerMemberName] string propertyName = "")
			=> PropertyChanging?.Invoke(this, new PropertyChangingEventArgs(propertyName));

		/// <summary>
		/// Occurs when a virtual desktop property changed.
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged;

		private void RaisePropertyChanged([CallerMemberName] string propertyName = "")
			=> PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
	}
}
