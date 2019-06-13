using System;
using System.Runtime.InteropServices;

namespace Bismuth.Wpf.Native
{
    internal static class DwmApi
    {
        [DllImport("dwmapi.dll")]
        public static extern int DwmExtendFrameIntoClientArea(IntPtr hWnd, ref MARGINS margins);
    }
}
