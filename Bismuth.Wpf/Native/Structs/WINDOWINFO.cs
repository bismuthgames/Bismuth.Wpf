using System.Runtime.InteropServices;

namespace Bismuth.Wpf.Native
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct WINDOWINFO
    {
        public int sizeInBytes;
        public RECT window;
        public RECT client;
        public int style;
        public int exStyle;
        public int windowStatus;
        public int cxWindowBorders;
        public int cyWindowBorders;
        public ushort atomWindowType;
        public ushort creatorVersion;
    }
}
