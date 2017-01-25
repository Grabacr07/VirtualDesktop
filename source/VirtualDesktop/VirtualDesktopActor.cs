using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WindowsDesktop.Interop;

namespace WindowsDesktop
{
    public class VirtualDesktopActor
    {
        private readonly List<VirtualDesktopMoveAction> _queuedMoves = new List<VirtualDesktopMoveAction>();
        private VirtualDesktopSwitchAction _switchAction;
        private readonly LatestTaskRunner _actor = new LatestTaskRunner();

        public VirtualDesktop DesktopToSwitchTo => this._switchAction?.ToDesktop;

        public VirtualDesktopActor Move(VirtualDesktop toDesktop, IntPtr hWnd, AdjacentDesktop direction, bool loop)
        {
            if (this._queuedMoves.Any())
            {
                // allow buffering of keyboard inputs, but since the desktop doesn't change until
                // Execute actually does something, we need to keep track of where we are
                toDesktop = direction == AdjacentDesktop.LeftDirection ? this._queuedMoves.Last().ToDesktop.GetLeft(loop) :
                    direction == AdjacentDesktop.RightDirection ? this._queuedMoves.Last().ToDesktop.GetRight(loop) :
                        toDesktop;
            }

            if (toDesktop != null)
            {
                Console.WriteLine($"Move to {toDesktop}");
                var move = new VirtualDesktopMoveAction(toDesktop, hWnd);
                this._queuedMoves.Add(move);
            }
            else
            {
                Console.WriteLine($"Move to desktop was null. Canelling move action.");
                this._queuedMoves.Clear();
            }

            return this;
        }

        public VirtualDesktopActor Switch(VirtualDesktop toDesktop, AdjacentDesktop direction, bool loop)
        {
            if (this._switchAction != null)
            {
                // allow buffering of keyboard inputs, but since the desktop doesn't change until
                // Execute actually does something, we need to keep track of where we are
                toDesktop = direction == AdjacentDesktop.LeftDirection ? this._switchAction.ToDesktop.GetLeft(loop) :
                    direction == AdjacentDesktop.RightDirection ? this._switchAction.ToDesktop.GetRight(loop) :
                        toDesktop;
            }

            if (toDesktop != null)
            {
                Console.WriteLine($"QuickSwitch to {toDesktop}");
                var switcher = new VirtualDesktopSwitchAction(toDesktop);
                this._switchAction = switcher;
            }
            else
            {
                Console.WriteLine("QuickSwitch desktop was null. Cancelling switch action.");
                var switchAction = this._switchAction;
                if (switchAction != null) switchAction.Cancel = true;
            }

            return this;
        }

        public VirtualDesktopActor Switch(VirtualDesktop toDesktop, SmoothSwitchData switchData, AdjacentDesktop direction, bool loop)
        {
            var originalDesktop = toDesktop;
            if (this._switchAction != null)
            {
                // allow buffering of keyboard inputs, but since the desktop doesn't change until
                // Execute actually does something, we need to keep track of where we are
                toDesktop = direction == AdjacentDesktop.LeftDirection ? this._switchAction?.ToDesktop.GetLeft(loop) :
                    direction == AdjacentDesktop.RightDirection ? this._switchAction?.ToDesktop.GetRight(loop) :
                        toDesktop;
            }

            if (toDesktop != null)
            {
                Console.WriteLine($"SmoothSwitch to {toDesktop}");
                var switcher = new VirtualDesktopSwitchAction(toDesktop, switchData);
                this._switchAction = switcher;
            }
            else
            {
                Console.WriteLine("SmoothSwitch desktop was null. Cancelling switch action.");
                var switchAction = this._switchAction;
                if (switchAction != null) switchAction.Cancel = true;
            }

            return this;
        }

        public void Execute(IShortcutKeyDetector keyDetector)
        {
            if (this._queuedMoves.Any() || (this._switchAction != null && false == this._switchAction?.Cancel))
            {
                Console.WriteLine($"VirtualDesktopActor._actor.Set");
                this._actor.Set(() =>
                        new Task(() =>
                        {
                            this._queuedMoves.ForEach(m => m.Execute());
                            this._queuedMoves.Clear();

                            if (false == this._switchAction?.Cancel)
                            {
                                this._switchAction?.Execute();
                                this._switchAction = null;
                            }
                        }),
                    Task.Factory.StartNew(() => keyDetector?.WaitForNoKeysPressed()));
            }
            else
            {
                Console.WriteLine("Cancelling switch action");
            }
        }
    }

    public class VirtualDesktopMoveAction
    {
        public readonly VirtualDesktop ToDesktop;
        public static readonly string Type = "MoveAction";

        private readonly IntPtr _hWnd;

        public VirtualDesktopMoveAction(VirtualDesktop toDesktop, IntPtr hWnd)
        {
            this.ToDesktop = toDesktop;
            this._hWnd = hWnd;
        }

        public void Execute()
        {
            int processId;
            NativeMethods.GetWindowThreadProcessId(this._hWnd, out processId);

            if (Process.GetCurrentProcess().Id == processId)
            {
                var guid = this.ToDesktop.Id;
                ComObjects.VirtualDesktopManager.MoveWindowToDesktop(this._hWnd, ref guid);
            }
            else
            {
                IApplicationView view;
                ComObjects.ApplicationViewCollection.GetViewForHwnd(this._hWnd, out view);
                ComObjects.VirtualDesktopManagerInternal.MoveViewToDesktop(view, this.ToDesktop.ComObject);
            }
        }
    }

    public class VirtualDesktopSwitchAction
    {
        public static readonly string Type = "SwitchAction";

        public readonly VirtualDesktop ToDesktop;
        public bool Cancel { get; set; }

        private readonly SmoothSwitchData _switchData;

        public VirtualDesktopSwitchAction(VirtualDesktop toDesktop)
            : this(toDesktop, null)
        {
        }

        public VirtualDesktopSwitchAction(VirtualDesktop toDesktop, SmoothSwitchData switchData)
        {
            this.ToDesktop = toDesktop;
            this._switchData = switchData;
        }

        public void Execute()
        {
            if (this._switchData == null)
            {
                this.ToDesktop.InternalQuickSwitch();
            }
            else
            {
                this.ToDesktop.InternalSwitchSelector(this._switchData);
            }
        }
    }
}
