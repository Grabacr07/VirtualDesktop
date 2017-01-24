using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using WindowsDesktop.Interop;
using WindowsInput;
using WindowsInput.Native;

#pragma warning disable 4014 // disable await warning for when adding to animation queue

namespace WindowsDesktop
{
    /// <summary>
    /// Encapsulates a virtual desktop on Windows 10.
    /// </summary>
    [DebuggerDisplay("{Id}")]
    public partial class VirtualDesktop
    {
        private static VirtualDesktop _finalMoveDesktop;
        private static readonly SequentialTaskQueue _animationQueue = new SequentialTaskQueue();

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

            _finalMoveDesktop = this;
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
        public void Switch(IntPtr hWnd, IShortcutKeyDetector keyDetector, bool smoothSwitch, IShortcutKey switchLeftShortcutKey, IShortcutKey switchRightShortcutKey)
        {
            if (smoothSwitch)
            {
                this.SmoothSwitch(hWnd, keyDetector, switchLeftShortcutKey, switchRightShortcutKey);
            }
            else
            {
                ComObjects.VirtualDesktopManagerInternal.SwitchDesktop(this.ComObject);
            }
        }

        public void SmoothSwitch(IntPtr hWnd, IShortcutKeyDetector keyDetector, IShortcutKey switchLeftShortcutKey, IShortcutKey switchRightShortcutKey)
        {
            var current = Current;
            var desktops = GetDesktops();

            Task task = null;
            if (this.IsThisOnLeftOf(current))
            {
                task = this.SmoothSwitchFromRightToLeft(hWnd, keyDetector, switchLeftShortcutKey, this);
            }
            else if (this.IsThisOnRightOf(current))
            {
                task = this.SmoothSwitchFromLeftToRight(hWnd, keyDetector, switchRightShortcutKey, this);
            }
            else if (current.GetLeft()?.Id == this.Id)
            {
                task = this.SmoothSwitchLeftOne(hWnd, keyDetector, switchLeftShortcutKey, this);
            }
            else if (current.GetRight()?.Id == this.Id)
            {
                task = this.SmoothSwitchRightOne(hWnd, keyDetector, switchRightShortcutKey, this);
            }

            if (task != null)
            {
                lock (VirtualDesktop._animationQueue)
                {
                    VirtualDesktop._animationQueue.Enqueue(() => task);
                }
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

        private Task SmoothSwitchFromLeftToRight(IntPtr hWnd, IShortcutKeyDetector keyDetector, IShortcutKey switchRightShortcutKey, VirtualDesktop targetDesktop)
        {
            return Task.Factory.StartNew(() =>
            {
                if (keyDetector.WaitForNoKeysPressed())
                {
                    // avoid a bunch of unnecessary switching if the user moved multiple desktops at a time
                    if (targetDesktop.Id != _finalMoveDesktop?.Id) return;

                    var currentIndex = this.IndexOf(Current);
                    var thatIndex = this.IndexOf(targetDesktop);

                    for (var i = currentIndex; i < thatIndex; ++i)
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

        private Task SmoothSwitchFromRightToLeft(IntPtr hWnd, IShortcutKeyDetector keyDetector, IShortcutKey switchLeftShortcutKey, VirtualDesktop targetDesktop)
        {
            return Task.Factory.StartNew(() =>
            {
                if (keyDetector.WaitForNoKeysPressed())
                {
                    // avoid a bunch of unnecessary switching if the user moved multiple desktops at a time
                    if (targetDesktop.Id != _finalMoveDesktop?.Id) return;

                    var currentIndex = this.IndexOf(Current);
                    var thatIndex = this.IndexOf(targetDesktop);

                    for (var i = currentIndex; i > thatIndex; --i)
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

        private Task SmoothSwitchLeftOne(IntPtr hWnd, IShortcutKeyDetector keyDetector, IShortcutKey switchLeftShortcutKey, VirtualDesktop targetDesktop)
        {
            return Task.Factory.StartNew(() =>
            {
                if (keyDetector.WaitForNoKeysPressed())
                {
                    // avoid a bunch of unnecessary switching if the user moved multiple desktops at a time
                    if (targetDesktop.Id != _finalMoveDesktop?.Id) return;

                    this.Input.Keyboard.ModifiedKeyStroke(
                        switchLeftShortcutKey.Modifiers.Select(x => (VirtualKeyCode)x),
                        (VirtualKeyCode)switchLeftShortcutKey.Key);

                    this.RestoreForegroundWindow(hWnd);
                }
            });
        }

        private Task SmoothSwitchRightOne(IntPtr hWnd, IShortcutKeyDetector keyDetector, IShortcutKey switchRightShortcutKey, VirtualDesktop targetDesktop)
        {
            return Task.Factory.StartNew(() =>
            {
                if (keyDetector.WaitForNoKeysPressed())
                {
                    // avoid a bunch of unnecessary switching if the user moved multiple desktops at a time
                    if (targetDesktop.Id != _finalMoveDesktop?.Id) return;

                    this.Input.Keyboard.ModifiedKeyStroke(
                        switchRightShortcutKey.Modifiers.Select(x => (VirtualKeyCode)x),
                        (VirtualKeyCode)switchRightShortcutKey.Key);

                    this.RestoreForegroundWindow(hWnd);
                }
            });
        }

        private void RestoreForegroundWindow(IntPtr hWnd)
        {
            NativeMethods.SetForegroundWindow(hWnd);
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
