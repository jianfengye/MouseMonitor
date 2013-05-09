using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MouseMonitor
{
    class MouseMsg
    {
        public Point pt;
        public IntPtr hwnd;
        public uint wHitTestCode;
        public IntPtr dwExtraInfo;
    }
}
