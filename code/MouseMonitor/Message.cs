using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MouseMonitor
{
    internal static class Message
    {
        // 鼠标左键按下
        public const int WM_LBUTTONDOWN = 0x201;

        // 鼠标右键按下
        public const int WM_RBUTTONDOWN = 0x204;

        // 鼠标中键按下
        public const int WM_MBUTTONDOWN = 0x207;
    }
}
