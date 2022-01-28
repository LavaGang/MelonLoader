namespace MelonLoader.NativeUtils.PEParser
{
    public struct ImageFileHeader
    {
        public ushort machine;
        public ushort numberOfSections;
        public uint timeDateStamp;
        public uint pointerToSymbolTable;
        public uint numberOfSymbols;
        public ushort sizeOfOptionalHeader;
        public ushort characteristrics;
    }
}