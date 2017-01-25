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
    public enum SwitchType
    {
        Quick = 1,
        Smooth = 2
    }

    public class SmoothSwitchData
    {
        public IntPtr Hwnd { get; set; }
        public SwitchType SwitchType { get; set; }
        public IShortcutKeyDetector KeyDetector { get; set; }
        public IShortcutKey SwitchLeftShortcutKey { get; set; }
        public IShortcutKey SwitchRightShortcutKey { get; set; }
        public IShortcutKey KeyPressed { get; set; }

        public SmoothSwitchData()
        {
        }

        public SmoothSwitchData(
            SwitchType switchType,
            IShortcutKeyDetector keyDetector,
            IShortcutKey switchLeftShortcutKey, IShortcutKey switchRightShortcutKey,
            IShortcutKey keyPressed)
            : this(IntPtr.Zero, switchType, keyDetector, switchLeftShortcutKey, switchRightShortcutKey, keyPressed)
        {
        }

        public SmoothSwitchData(IntPtr hWnd,
            SwitchType switchType,
            IShortcutKeyDetector keyDetector,
            IShortcutKey switchLeftShortcutKey, IShortcutKey switchRightShortcutKey,
            IShortcutKey keyPressed)
        {
            this.Hwnd = hWnd;
            this.SwitchType = switchType;
            this.KeyDetector = keyDetector;
            this.SwitchLeftShortcutKey = switchLeftShortcutKey;
            this.SwitchRightShortcutKey = switchRightShortcutKey;
            this.KeyPressed = keyPressed;
        }
    }

    /// <summary>
    /// Encapsulates a virtual desktop on Windows 10.
    /// </summary>
    [DebuggerDisplay("{Id}")]
    public partial class VirtualDesktop
    {
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

        public VirtualDesktopActor Move(IntPtr hWnd, AdjacentDesktop direction, bool loop)
        {
            return _actor.Move(this, hWnd, direction, loop);
        }

        public VirtualDesktopActor Switch(AdjacentDesktop direction, bool loop)
        {
            return _actor.Switch(this, direction, loop);
        }

        public VirtualDesktopActor Switch(SmoothSwitchData switchData, AdjacentDesktop direction, bool loop)
        {
            return _actor.Switch(this, switchData, direction, loop);
        }

        public void Execute(IShortcutKeyDetector keyDetector)
        {
            _actor.Execute(keyDetector);
        }

        /// <summary>
        /// Display the virtual desktop.
        /// </summary>
        internal void InternalQuickSwitch()
        {
            Console.WriteLine("InternalQuickSwitch");
            ComObjects.VirtualDesktopManagerInternal.SwitchDesktop(this.ComObject);
        }


        /// <summary>
        /// Display the virtual desktop.
        /// </summary>
        /// <summary>
        /// Display the virtual desktop and reactivate the hWnd.
        /// </summary>
        internal void InternalSwitchSelector(SmoothSwitchData switchData)
        {
            Console.WriteLine("InternalSwitchSelector");
            if (switchData.SwitchType == SwitchType.Smooth)
            {
                this.InternalSmoothSwitch(switchData);
            }
            else
            {
                this.InternalQuickSwitch();
            }
        }

        internal void InternalSmoothSwitch(SmoothSwitchData switchData)
        {
            var current = Current;
            var desktops = GetDesktops();

            Console.WriteLine($"InternalSmoothSwitch: Current is {current}");

            _finalSwitchToDesktop = this;

            Task task = null;

            // if we need to wrap from last to something on the left
            if (current.Id == desktops.Last().Id && this.IsThisOnLeftOf(current))
            {
                Console.WriteLine("Wrapping last to something left");
                task = this.SmoothSwitchFromRightToLeft(switchData.Hwnd, switchData.KeyDetector, switchData.SwitchLeftShortcutKey, switchData.KeyPressed);
            }
            // if we need to wrap from first to something on the right
            else if (current.Id == desktops.First().Id && this.IsThisOnRightOf(current))
            {
                Console.WriteLine("Wrapping first to something right");
                task = this.SmoothSwitchFromLeftToRight(switchData.Hwnd, switchData.KeyDetector, switchData.SwitchRightShortcutKey, switchData.KeyPressed);
            }
            //else if (switchData.KeyPressed?.Equals(switchData.SwitchLeftShortcutKey) == true)
            //{
            //    // do nothing
            //    Console.WriteLine("Ignoring left switch shortcut");
            //}
            //else if (switchData.KeyPressed?.Equals(switchData.SwitchRightShortcutKey) == true)
            //{
            //    // do nothing
            //    Console.WriteLine("Ignoring right switch shortcut");
            //}
            else if (this.IsThisOnLeftOf(current))
            {
                Console.WriteLine("Switching left");
                task = this.SmoothSwitchFromRightToLeft(switchData.Hwnd, switchData.KeyDetector, switchData.SwitchLeftShortcutKey, switchData.KeyPressed);
            }
            else if (this.IsThisOnRightOf(current))
            {
                Console.WriteLine("Switching right");
                task = this.SmoothSwitchFromLeftToRight(switchData.Hwnd, switchData.KeyDetector, switchData.SwitchRightShortcutKey, switchData.KeyPressed);
            }

            if (task != null)
            {
                _animator.Set(() => task, Task.Factory.StartNew(() => switchData.KeyDetector.WaitForNoKeysPressed()));
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
                        Console.WriteLine($"Generating: {switchRightShortcutKey}");
                        this.Input.Keyboard.ModifiedKeyStroke(
                            switchRightShortcutKey.Modifiers.Select(x => (VirtualKeyCode)x),
                            (VirtualKeyCode)switchRightShortcutKey.Key);
                        Thread.Sleep(300);
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
                        Console.WriteLine($"Generating: {switchLeftShortcutKey}");
                        this.Input.Keyboard.ModifiedKeyStroke(
                            switchLeftShortcutKey.Modifiers.Select(x => (VirtualKeyCode)x),
                            (VirtualKeyCode)switchLeftShortcutKey.Key);
                        Thread.Sleep(300);
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
        public VirtualDesktop GetLeft(bool loop)
        {
            IVirtualDesktop desktop;
            try
            {
                desktop = ComObjects.VirtualDesktopManagerInternal.GetAdjacentDesktop(this.ComObject, AdjacentDesktop.LeftDirection);
            }
            catch (COMException ex) when (ex.Match(HResult.TYPE_E_OUTOFBOUNDS))
            {
                if (loop)
                {
                    var desktops = GetDesktops();
                    Console.WriteLine($"GetLeft: this is {this}");
                    return desktops.Length >= 2 && this.Id == desktops.First().Id ? desktops.Last() : this.GetLeft(false);
                }
                else
                {
                    return null;
                }
            }
            var wrapper = _wrappers.GetOrAdd(desktop.GetID(), _ => new VirtualDesktop(desktop));

            return wrapper;
        }

        /// <summary>
        /// Returns a virtual desktop on the right.
        /// </summary>
        public VirtualDesktop GetRight(bool loop)
        {
            IVirtualDesktop desktop;
            try
            {
                desktop = ComObjects.VirtualDesktopManagerInternal.GetAdjacentDesktop(this.ComObject, AdjacentDesktop.RightDirection);
            }
            catch (COMException ex) when (ex.Match(HResult.TYPE_E_OUTOFBOUNDS))
            {
                if (loop)
                {
                    var desktops = GetDesktops();
                    Console.WriteLine($"GetRight: this is {this}");
                    return desktops.Length >= 2 && this.Id == desktops.Last().Id ? desktops.First() : this.GetRight(false);
                }
                else
                {
                    return null;
                }
            }
            var wrapper = _wrappers.GetOrAdd(desktop.GetID(), _ => new VirtualDesktop(desktop));

            return wrapper;
        }

        public override string ToString()
        {
            return $"{this.IndexOf(this)}:{this.Id}";
        }
    }
}
