using System;
using System.Text;
using MelonUnityEngine;
using MelonLoader;

namespace mgGif
{
    internal class Decoder : IDisposable
    {
        public string Version;
        public ushort Width;
        public ushort Height;
        public Color32 BackgroundColour;

        //------------------------------------------------------------------------------
        // GIF format enums

        private enum ImageFlag
        {
            Interlaced = 0x40,
            ColourTable = 0x80,
            TableSizeMask = 0x07,
            BitDepthMask = 0x70,
        }

        private enum Block
        {
            Image = 0x2C,
            Extension = 0x21,
            End = 0x3B
        }

        private enum Extension
        {
            GraphicControl = 0xF9,
            Comments = 0xFE,
            PlainText = 0x01,
            ApplicationData = 0xFF
        }

        private enum Disposal
        {
            None = 0x00,
            DoNotDispose = 0x04,
            RestoreBackground = 0x08,
            ReturnToPrevious = 0x0C
        }

        private enum ControlFlags
        {
            HasTransparency = 0x01,
            DisposalMask = 0x0C
        }

        //------------------------------------------------------------------------------

        private const uint NoCode = 0xFFFF;
        private const ushort NoTransparency = 0xFFFF;

        // input stream to decode
        private byte[] Input;

        private int D;

        // colour table
        private Color32[] GlobalColourTable;

        private Color32[] LocalColourTable;
        private Color32[] ActiveColourTable;
        private ushort TransparentIndex;

        // current image
        private Image Image = new Image();

        private ushort ImageLeft;
        private ushort ImageTop;
        private ushort ImageWidth;
        private ushort ImageHeight;

        private Color32[] Output;
        private Color32[] PreviousImage;

        private readonly int[] Pow2 = { 1, 2, 4, 8, 16, 32, 64, 128, 256, 512, 1024, 2048, 4096 };

        //------------------------------------------------------------------------------
        // ctor

        public Decoder(byte[] data) : this()
            => Load(data);

        public Decoder Load(byte[] data)
        {
            Input = data;
            D = 0;

            GlobalColourTable = new Color32[256];
            LocalColourTable = new Color32[256];
            TransparentIndex = NoTransparency;
            Output = null;
            PreviousImage = null;

            Image.Delay = 0;

            return this;
        }

        //------------------------------------------------------------------------------
        // reading data utility functions

        private byte ReadByte() => Input[D++];

        private ushort ReadUInt16() => (ushort)(Input[D++] | Input[D++] << 8);

        //------------------------------------------------------------------------------

        private void ReadHeader()
        {
            if (Input == null || Input.Length <= 12)
                throw new Exception("Invalid data");

            // signature

            Version = Encoding.ASCII.GetString(Input, 0, 6);
            D = 6;

            if (Version != "GIF87a" && Version != "GIF89a")
            {
                throw new Exception("Unsupported GIF version");
            }

            // read header

            Width = ReadUInt16();
            Height = ReadUInt16();

            Image.Width = Width;
            Image.Height = Height;

            var flags = (ImageFlag)ReadByte();
            var bgIndex = ReadByte(); // background colour

            ReadByte(); // aspect ratio

            if (flags.HasFlag(ImageFlag.ColourTable))
                ReadColourTable(GlobalColourTable, flags);

            BackgroundColour = GlobalColourTable[bgIndex];
        }

        //------------------------------------------------------------------------------

        public Image NextImage()
        {
            // if at start of data, read header

            if (D == 0)
                ReadHeader();

            // read blocks until we find an image block

            while (true)
            {
                var block = (Block)ReadByte();

                switch (block)
                {
                    case Block.Image:
                        {
                            // return the image if we got one

                            var img = ReadImageBlock();

                            if (img != null)
                                return img;
                        }
                        break;

                    case Block.Extension:
                        {
                            var ext = (Extension)ReadByte();

                            if (ext == Extension.GraphicControl)
                                ReadControlBlock();
                            else
                                SkipBlocks();
                        }
                        break;

                    case Block.End:
                        {
                            // end block - stop!
                            return null;
                        }

                    default:
                        {
                            throw new Exception("Unexpected block type");
                        }
                }
            }
        }

        //------------------------------------------------------------------------------

        private Color32[] ReadColourTable(Color32[] colourTable, ImageFlag flags)
        {
            var tableSize = Pow2[(int)(flags & ImageFlag.TableSizeMask) + 1];

            for (var i = 0; i < tableSize; i++)
            {
                colourTable[i] = new Color32(
                    Input[D++],
                    Input[D++],
                    Input[D++],
                    0xFF
                );
            }

            return colourTable;
        }

        //------------------------------------------------------------------------------

        private void SkipBlocks()
        {
            var blockSize = Input[D++];

            while (blockSize != 0x00)
            {
                D += blockSize;
                blockSize = Input[D++];
            }
        }

        //------------------------------------------------------------------------------

        private void ReadControlBlock()
        {
            // read block

            ReadByte();                             // block size (0x04)
            var flags = (ControlFlags)ReadByte();  // flags
            Image.Delay = ReadUInt16() * 10;        // delay (1/100th -> milliseconds)
            var transparentColour = ReadByte();     // transparent colour
            ReadByte();                             // terminator (0x00)

            // has transparent colour?

            if (flags.HasFlag(ControlFlags.HasTransparency))
                TransparentIndex = transparentColour;
            else
                TransparentIndex = NoTransparency;

            // dispose of current image

            switch ((Disposal)(flags & ControlFlags.DisposalMask))
            {
                default:
                case Disposal.None:
                case Disposal.DoNotDispose:
                    // remember current image in case we need to "return to previous"
                    PreviousImage = Output;
                    break;

                case Disposal.RestoreBackground:
                    // empty image - don't track
                    Output = new Color32[Width * Height];
                    break;

                case Disposal.ReturnToPrevious:

                    // return to previous image

                    Output = new Color32[Width * Height];

                    if (PreviousImage != null)
                        Array.Copy(PreviousImage, Output, Output.Length);

                    break;
            }
        }

        //------------------------------------------------------------------------------

        private Image ReadImageBlock()
        {
            // read image block header

            ImageLeft = ReadUInt16();
            ImageTop = ReadUInt16();
            ImageWidth = ReadUInt16();
            ImageHeight = ReadUInt16();
            var flags = (ImageFlag)ReadByte();

            // bad image if we don't have any dimensions

            if (ImageWidth == 0 || ImageHeight == 0)
                return null;

            // read colour table

            if (flags.HasFlag(ImageFlag.ColourTable))
                ActiveColourTable = ReadColourTable(LocalColourTable, flags);
            else
                ActiveColourTable = GlobalColourTable;

            if (Output == null)
            {
                Output = new Color32[Width * Height];
                PreviousImage = Output;
            }

            // read image data

            DecompressLZW();

            // deinterlace

            if (flags.HasFlag(ImageFlag.Interlaced))
                Deinterlace();

            // return image

            Image.RawImage = Output;
            return Image;
        }

        //------------------------------------------------------------------------------
        // decode interlaced images

        private void Deinterlace()
        {
            var numRows = Output.Length / Width;
            var writePos = Output.Length - Width; // NB: work backwards due to Y-coord flip
            var input = Output;

            Output = new Color32[Output.Length];

            for (var row = 0; row < numRows; row++)
            {
                int copyRow;

                // every 8th row starting at 0
                if (row % 8 == 0)
                {
                    copyRow = row / 8;
                }
                // every 8th row starting at 4
                else if ((row + 4) % 8 == 0)
                {
                    var o = numRows / 8;
                    copyRow = o + (row - 4) / 8;
                }
                // every 4th row starting at 2
                else if ((row + 2) % 4 == 0)
                {
                    var o = numRows / 4;
                    copyRow = o + (row - 2) / 4;
                }
                // every 2nd row starting at 1
                else // if( ( r + 1 ) % 2 == 0 )
                {
                    var o = numRows / 2;
                    copyRow = o + (row - 1) / 2;
                }

                Array.Copy(input, (numRows - copyRow - 1) * Width, Output, writePos, Width);

                writePos -= Width;
            }
        }

        //------------------------------------------------------------------------------

        // dispose isn't needed for the safe implementation but keep here for interface parity

        public Decoder() { }

        public void Dispose() { Dispose(true); }

        protected virtual void Dispose(bool disposing) { }

        private int[] Indices = new int[4096];
        private ushort[] Codes = new ushort[128 * 1024];
        private uint[] CurBlock = new uint[64];

        private void DecompressLZW()
        {
            // output write position

            int row = (Height - ImageTop - 1) * Width; // reverse rows for unity texture coords
            int col = ImageLeft;
            int rightEdge = ImageLeft + ImageWidth;

            // setup codes

            int minimumCodeSize = Input[D++];

            if (minimumCodeSize > 11)
                minimumCodeSize = 11;

            var codeSize = minimumCodeSize + 1;
            var nextSize = Pow2[codeSize];
            var maximumCodeSize = Pow2[minimumCodeSize];
            var clearCode = maximumCodeSize;
            var endCode = maximumCodeSize + 1;

            // initialise buffers

            var codesEnd = 0;
            var numCodes = maximumCodeSize + 2;

            for (ushort i = 0; i < numCodes; i++)
            {
                Indices[i] = codesEnd;
                Codes[codesEnd++] = 1; // length
                Codes[codesEnd++] = i; // code
            }

            // LZW decode loop

            uint previousCode = NoCode; // last code processed
            uint mask = (uint)(nextSize - 1); // mask out code bits
            uint shiftRegister = 0; // shift register holds the bytes coming in from the input stream, we shift down by the number of bits

            int bitsAvailable = 0; // number of bits available to read in the shift register
            int bytesAvailable = 0; // number of bytes left in current block

            int blockPos = 0;

            while (true)
            {
                // get next code

                uint curCode = shiftRegister & mask;

                if (bitsAvailable >= codeSize)
                {
                    bitsAvailable -= codeSize;
                    shiftRegister >>= codeSize;
                }
                else
                {
                    // reload shift register

                    // if start of new block

                    if (bytesAvailable <= 0)
                    {
                        // read blocksize
                        bytesAvailable = Input[D++];

                        // exit if end of stream
                        if (bytesAvailable == 0)
                            return;

                        // read block
                        CurBlock[(bytesAvailable - 1) / 4] = 0; // zero last entry
                        Buffer.BlockCopy(Input, D, CurBlock, 0, bytesAvailable);
                        blockPos = 0;
                        D += bytesAvailable;
                    }

                    // load shift register

                    shiftRegister = CurBlock[blockPos++];
                    int newBits = bytesAvailable >= 4 ? 32 : bytesAvailable * 8;
                    bytesAvailable -= 4;

                    // read remaining bits

                    if (bitsAvailable > 0)
                    {
                        var bitsRemaining = codeSize - bitsAvailable;
                        curCode |= (shiftRegister << bitsAvailable) & mask;
                        shiftRegister >>= bitsRemaining;
                        bitsAvailable = newBits - bitsRemaining;
                    }
                    else
                    {
                        curCode = shiftRegister & mask;
                        shiftRegister >>= codeSize;
                        bitsAvailable = newBits - codeSize;
                    }
                }

                // process code

                if (curCode == clearCode)
                {
                    // reset codes
                    codeSize = minimumCodeSize + 1;
                    nextSize = Pow2[codeSize];
                    numCodes = maximumCodeSize + 2;

                    // reset buffer write pos
                    codesEnd = numCodes * 2;

                    // clear previous code
                    previousCode = NoCode;
                    mask = (uint)(nextSize - 1);

                    continue;
                }
                else if (curCode == endCode)
                {
                    // stop
                    break;
                }

                bool plusOne = false;
                int codePos = 0;

                if (curCode < numCodes)
                {
                    // write existing code
                    codePos = Indices[curCode];
                }
                else if (previousCode != NoCode)
                {
                    // write previous code
                    codePos = Indices[previousCode];
                    plusOne = true;
                }
                else
                    continue;
                
                // output colours

                var codeLength = Codes[codePos++];
                var newCode = Codes[codePos];

                for (int i = 0; i < codeLength; i++)
                {
                    var code = Codes[codePos++];

                    if (code != TransparentIndex && col < Width)
                    {
                        Output[row + col] = ActiveColourTable[code];
                    }

                    if (++col == rightEdge)
                    {
                        col = ImageLeft;
                        row -= Width;

                        if (row < 0)
                        {
                            SkipBlocks();
                            return;
                        }
                    }
                }

                if (plusOne)
                {
                    if (newCode != TransparentIndex && col < Width)
                        Output[row + col] = ActiveColourTable[newCode];

                    if (++col == rightEdge)
                    {
                        col = ImageLeft;
                        row -= Width;

                        if (row < 0)
                            break;
                    }
                }

                // create new code

                if (previousCode != NoCode && numCodes != Indices.Length)
                {
                    // get previous code from buffer

                    codePos = Indices[previousCode];
                    codeLength = Codes[codePos++];

                    // resize buffer if required (should be rare)

                    if (codesEnd + codeLength + 1 >= Codes.Length)
                        Array.Resize(ref Codes, Codes.Length * 2);

                    // add new code

                    Indices[numCodes++] = codesEnd;
                    Codes[codesEnd++] = (ushort)(codeLength + 1);

                    // copy previous code sequence

                    var stop = codesEnd + codeLength;

                    while (codesEnd < stop)
                        Codes[codesEnd++] = Codes[codePos++];

                    // append new code

                    Codes[codesEnd++] = newCode;
                }

                // increase code size?

                if (numCodes >= nextSize && codeSize < 12)
                {
                    nextSize = Pow2[++codeSize];
                    mask = (uint)(nextSize - 1);
                }

                // remember last code processed
                previousCode = curCode;
            }

            // skip any remaining blocks
            SkipBlocks();
        }

        public static string Ident()
        {
            var v = "1.1";
            var e = BitConverter.IsLittleEndian ? "L" : "B";
            var b = "M";
            var s = "S";
            var n = "2.0";

            return $"{v} {e}{s}{b} {n}";
        }
    }
}