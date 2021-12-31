using System.Runtime.InteropServices;

namespace Windows
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    class DropFile
    {
        uint pFiles = 14;
        public Point pt;
        public bool fNC;
        bool fWide = true;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 300)] // Max path size on windows is 260
        public string file = "";
    }
}
