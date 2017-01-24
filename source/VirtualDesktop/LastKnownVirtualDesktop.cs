using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace WindowsDesktop
{
    internal static class LastKnownVirtualDesktop
    {
        private static Timer _refreshTimer;

        private static Guid _lastKnownId;
        private static bool _beenHereAWhile;
        private static int _seenCount;

        public static void Start()
        {
            _refreshTimer = new Timer
            {
                AutoReset = true,
                Interval = 500
            };
            _refreshTimer.Elapsed += _refreshTimer_Elapsed;
            _refreshTimer.Start();
        }

        public static bool WasUserDefinitelyHere(VirtualDesktop desktop)
        {
            return _lastKnownId == desktop.Id && _beenHereAWhile;
        }

        private static void _refreshTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            var desktop = VirtualDesktop.Current;
            if (desktop != null)
            {
                if (desktop.Id == _lastKnownId)
                {
                    _seenCount++;
                }
                _lastKnownId = desktop.Id;
            }
            else
            {
                _seenCount = 1;
            }

            _beenHereAWhile = _seenCount >= 2;
        }
    }

}
