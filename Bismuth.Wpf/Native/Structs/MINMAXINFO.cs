using System.Runtime.InteropServices;

namespace Bismuth.Wpf.Native
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct MINMAXINFO
    {
        public POINT reserved;
        public POINT maxSize;
        public POINT maxPosition;
        public POINT minTrackSize;
        public POINT maxTrackSize;
    }
}
