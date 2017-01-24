using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using WindowsDesktop.Interop;
using WindowsInput;
using WindowsInput.Native;
using Timer = System.Timers.Timer;

#pragma warning disable 4014 // disable await warning for when adding to animation queue

namespace WindowsDesktop
{
    /// <summary>
    /// Encapsulates a virtual desktop on Windows 10.
    /// </summary>
    [DebuggerDisplay("{Id}")]
    public partial class VirtualDesktop
    {
        private static VirtualDesktop _finalSwitchToDesktop;
        private static readonly LatestTaskRunner _animator = new LatestTaskRunner();

        /// <summary>
        /// Gets the unique identifier for the virtual desktop.
        /// </summary>
        public Guid Id { get; }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public IVirtualDesktop ComObject => ComObjects.GetVirtualDesktop(this.Id);

        private InputSimulator Input { get; }

        private VirtualDesktop(IVirtualDesktop comObject)
        {
            ComObjects.Register(comObject);
            this.Id = comObject.GetID();
            this.Input = new InputSimulator();
        }

        public void MoveHere(IntPtr hWnd)
        {
            int processId;
            NativeMethods.GetWindowThreadProcessId(hWnd, out processId);

            if (Process.GetCurrentProcess().Id == processId)
            {
                var guid = this.Id;
                ComObjects.VirtualDesktopManager.MoveWindowToDesktop(hWnd, ref guid);
            }
            else
            {
                IApplicationView view;
                ComObjects.ApplicationViewCollection.GetViewForHwnd(hWnd, out view);
                ComObjects.VirtualDesktopManagerInternal.MoveViewToDesktop(view, this.ComObject);
            }
        }

        /// <summary>
        /// Display the virtual desktop.
        /// </summary>
        public void Switch(IShortcutKeyDetector keyDetector, bool smoothSwitch, IShortcutKey switchLeftShortcutKey, IShortcutKey switchRightShortcutKey)
        {
            if (smoothSwitch)
            {
                this.SmoothSwitch(IntPtr.Zero, keyDetector, switchLeftShortcutKey, switchRightShortcutKey, null);
            }
            else
            {
                ComObjects.VirtualDesktopManagerInternal.SwitchDesktop(this.ComObject);
            }
        }

        /// <summary>
        /// Display the virtual desktop and reactivate the hWnd.
        /// </summary>
        public void Switch(IntPtr hWnd, IShortcutKeyDetector keyDetector, bool smoothSwitch, IShortcutKey switchLeftShortcutKey, IShortcutKey switchRightShortcutKey, IShortcutKey keyPressed)
        {
            if (smoothSwitch)
            {
                this.SmoothSwitch(hWnd, keyDetector, switchLeftShortcutKey, switchRightShortcutKey, keyPressed);
            }
            else
            {
                ComObjects.VirtualDesktopManagerInternal.SwitchDesktop(this.ComObject);
            }
        }

        public void SmoothSwitch(IntPtr hWnd, IShortcutKeyDetector keyDetector, IShortcutKey switchLeftShortcutKey, IShortcutKey switchRightShortcutKey, IShortcutKey keyPressed)
        {
            var current = Current;
            var desktops = GetDesktops();

            _finalSwitchToDesktop = this;

            Task task = null;
            if (LastKnownVirtualDesktop.WasUserDefinitelyHere(current))
            {
                // if we need to wrap from last to first
                if (current.Id == desktops.Last().Id && this.Id == desktops.First().Id)
                {
                    task = this.SmoothSwitchFromRightToLeft(hWnd, keyDetector, switchLeftShortcutKey, keyPressed);
                }
                // if we need to wrap from first to last
                else if (current.Id == desktops.First().Id && this.Id == desktops.Last().Id)
                {
                    task = this.SmoothSwitchFromLeftToRight(hWnd, keyDetector, switchRightShortcutKey, keyPressed);
                }
                else if (keyPressed?.Equals(switchLeftShortcutKey) == true)
                {
                    // do nothing
                }
                else if (keyPressed?.Equals(switchRightShortcutKey) == true)
                {
                    // do nothing
                }
                else if (this.IsThisOnLeftOf(current))
                {
                    task = this.SmoothSwitchFromRightToLeft(hWnd, keyDetector, switchLeftShortcutKey, keyPressed);
                }
                else if (this.IsThisOnRightOf(current))
                {
                    task = this.SmoothSwitchFromLeftToRight(hWnd, keyDetector, switchRightShortcutKey, keyPressed);
                }
            }

            if (task != null)
            {
                _animator.Set(() => task, Task.Factory.StartNew(() => keyDetector.WaitForNoKeysPressed()));
            }
        }


        private int IndexOf(VirtualDesktop that)
        {
            var desktops = GetDesktops();
            for (var i = 0; i < desktops.Length; ++i)
            {
                var desktop = desktops[i];
                if (desktop.Id == that.Id)
                {
                    return i;
                }
            }

            return -1;
        }

        private bool IsThisOnLeftOf(VirtualDesktop that)
        {
            var thisIndex = this.IndexOf(this);
            var thatIndex = this.IndexOf(that);
            return thisIndex >= 0 && thatIndex >= 0 && thisIndex < thatIndex;
        }

        private bool IsThisOnRightOf(VirtualDesktop that)
        {
            var thisIndex = this.IndexOf(this);
            var thatIndex = this.IndexOf(that);
            return thisIndex >= 0 && thatIndex >= 0 && thisIndex > thatIndex;
        }

        private Task SmoothSwitchFromLeftToRight(IntPtr hWnd, IShortcutKeyDetector keyDetector, IShortcutKey switchRightShortcutKey, IShortcutKey keyPressed)
        {
            return new Task(() =>
            {
                if (keyDetector.WaitForNoKeysPressed())
                {
                    // avoid a bunch of unnecessary switching if the user moved multiple desktops at a time
                    if (this.Id != _finalSwitchToDesktop?.Id) return;
                    _finalSwitchToDesktop = null;

                    var currentIndex = this.IndexOf(Current);
                    var moveToIndex = this.IndexOf(this);

                    // suspend our input until the keyDetector has seen the number of inputs
                    if (keyPressed?.Equals(switchRightShortcutKey) == true)
                    {
                        var keyCountToIgnore = moveToIndex - currentIndex;
                        keyDetector.SuspendUntil(switchRightShortcutKey, keyCountToIgnore);
                    }

                    for (var i = currentIndex; i < moveToIndex; ++i)
                    {
                        this.Input.Keyboard.ModifiedKeyStroke(
                            switchRightShortcutKey.Modifiers.Select(x => (VirtualKeyCode)x),
                            (VirtualKeyCode)switchRightShortcutKey.Key);
                        Thread.Sleep(200);
                    }

                    this.RestoreForegroundWindow(hWnd);
                }
            });
        }

        private Task SmoothSwitchFromRightToLeft(IntPtr hWnd, IShortcutKeyDetector keyDetector, IShortcutKey switchLeftShortcutKey, IShortcutKey keyPressed)
        {
            return new Task(() =>
            {
                if (keyDetector.WaitForNoKeysPressed())
                {
                    // avoid a bunch of unnecessary switching if the user moved multiple desktops at a time
                    if (this.Id != _finalSwitchToDesktop?.Id) return;
                    _finalSwitchToDesktop = null;

                    var currentIndex = this.IndexOf(Current);
                    var moveToIndex = this.IndexOf(this);

                    // suspend our input until the keyDetector has seen the number of inputs
                    if (keyPressed?.Equals(switchLeftShortcutKey) == true)
                    {
                        var keyCountToIgnore = currentIndex - moveToIndex;
                        keyDetector.SuspendUntil(switchLeftShortcutKey, keyCountToIgnore);
                    }

                    for (var i = currentIndex; i > moveToIndex; --i)
                    {
                        this.Input.Keyboard.ModifiedKeyStroke(
                            switchLeftShortcutKey.Modifiers.Select(x => (VirtualKeyCode)x),
                            (VirtualKeyCode)switchLeftShortcutKey.Key);
                        Thread.Sleep(200);
                    }

                    this.RestoreForegroundWindow(hWnd);
                }
            });
        }

        private void RestoreForegroundWindow(IntPtr hWnd)
        {
            if (hWnd != IntPtr.Zero) NativeMethods.SetForegroundWindow(hWnd);
        }

        /// <summary>
        /// Remove the virtual desktop.
        /// </summary>
        public void Remove()
        {
            this.Remove(GetDesktopsInternal().FirstOrDefault(x => x.Id != this.Id) ?? Create());
        }

        /// <summary>
        /// Remove the virtual desktop, specifying a virtual desktop that display after destroyed.
        /// </summary>
        public void Remove(VirtualDesktop fallbackDesktop)
        {
            if (fallbackDesktop == null) throw new ArgumentNullException(nameof(fallbackDesktop));

            ComObjects.VirtualDesktopManagerInternal.RemoveDesktop(this.ComObject, fallbackDesktop.ComObject);
        }

        /// <summary>
        /// Returns a virtual desktop on the left.
        /// </summary>
        public VirtualDesktop GetLeft()
        {
            IVirtualDesktop desktop;
            try
            {
                desktop = ComObjects.VirtualDesktopManagerInternal.GetAdjacentDesktop(this.ComObject, AdjacentDesktop.LeftDirection);
            }
            catch (COMException ex) when (ex.Match(HResult.TYPE_E_OUTOFBOUNDS))
            {
                return null;
            }
            var wrapper = _wrappers.GetOrAdd(desktop.GetID(), _ => new VirtualDesktop(desktop));

            return wrapper;
        }

        /// <summary>
        /// Returns a virtual desktop on the right.
        /// </summary>
        public VirtualDesktop GetRight()
        {
            IVirtualDesktop desktop;
            try
            {
                desktop = ComObjects.VirtualDesktopManagerInternal.GetAdjacentDesktop(this.ComObject, AdjacentDesktop.RightDirection);
            }
            catch (COMException ex) when (ex.Match(HResult.TYPE_E_OUTOFBOUNDS))
            {
                return null;
            }
            var wrapper = _wrappers.GetOrAdd(desktop.GetID(), _ => new VirtualDesktop(desktop));

            return wrapper;
        }
    }
}
