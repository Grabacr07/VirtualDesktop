using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsDesktop
{
    public interface IShortcutKeyDetector
    {
        void Start();
        void Stop();
        void SuspendUntil(IShortcutKey key, int keyCountToIgnore);
        bool WaitForNoKeysPressed();
    }
}
