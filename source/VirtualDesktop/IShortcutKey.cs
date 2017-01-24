using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#if WINDOWS_UWP
using Windows.System;
#else
using VirtualKey = System.Windows.Forms.Keys;
#endif

namespace WindowsDesktop
{
    public interface IShortcutKey
    {
        VirtualKey Key { get; }

        VirtualKey[] Modifiers { get; }
    }
}
