using System;
using System.Text;

namespace MelonLoader.MelonStartScreen
{
    internal static class CppUtils
    {
        public static unsafe string CharArrayPtrToString(this IntPtr ptr)
        {
            byte* text = (byte*)ptr;
            int length = 0;
            while (text[length] != 0)
                ++length;

            return Encoding.UTF8.GetString(text, length);
        }
        internal static unsafe IntPtr Sigscan(IntPtr module, int moduleSize, string signature)
        {
            string signatureSpaceless = signature.Replace(" ", "");
            int signatureLength = signatureSpaceless.Length / 2;
            byte[] signatureBytes = new byte[signatureLength];
            bool[] signatureNullBytes = new bool[signatureLength];
            for (int i = 0; i < signatureLength; ++i)
            {
                if (signatureSpaceless[i * 2] == '?')
                    signatureNullBytes[i] = true;
                else
                    signatureBytes[i] = (byte)((GetHexVal(signatureSpaceless[i * 2]) << 4) + (GetHexVal(signatureSpaceless[(i * 2) + 1])));
            }

            long index = module.ToInt64();
            long maxIndex = index + moduleSize;
            long tmpAddress = 0;
            int processed = 0;

            while (index < maxIndex)
            {
                if (signatureNullBytes[processed] || *(byte*)index == signatureBytes[processed])
                {
                    if (processed == 0)
                        tmpAddress = index;

                    ++processed;

                    if (processed == signatureLength)
                        return (IntPtr)tmpAddress;
                }
                else
                {
                    processed = 0;
                }

                ++index;
            }

            return IntPtr.Zero;
        }

        // Credits: https://stackoverflow.com/a/9995303
        private static int GetHexVal(char hex)
        {
            int val = (int)hex;
            return val - (val < 58 ? 48 : (val < 97 ? 55 : 87));
        }
    }
}
