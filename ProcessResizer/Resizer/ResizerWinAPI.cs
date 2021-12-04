using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ProcessResizer.Resizer
{
    internal static class ResizerWinAPI
    {
        [DllImport("user32.dll")]
        public static extern bool GetWindowRect(IntPtr hwnd, out ResizerRect rectangle);

        [DllImport("user32.dll")]
        public static extern bool SetWindowPos(IntPtr hwnd, int level, int x, int y, int cx, int cy, uint uflag);
    }
}
