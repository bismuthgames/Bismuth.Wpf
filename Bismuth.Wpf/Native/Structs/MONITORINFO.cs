using System.Runtime.InteropServices;

namespace Bismuth.Wpf.Native
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct MONITORINFO
    {
        public int sizeInBytes;
        public RECT monitorArea;
        public RECT workArea;
        public int flags;
    }
}
