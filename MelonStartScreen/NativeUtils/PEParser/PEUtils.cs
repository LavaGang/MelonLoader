using MelonLoader;
using System;

namespace MelonLoader.MelonStartScreen.NativeUtils.PEParser
{
    internal static class PEUtils
    {
        private static unsafe ImageNtHeaders* AnalyseModuleWin(IntPtr moduleBaseAddress)
        {
            //MelonLoader.MelonLogger.Msg("[moduleBaseAddress+0x0]: " + (*(byte*)(moduleBaseAddress + 0x0)));
            if (*(byte*)(moduleBaseAddress + 0x0) != 0x4D || *(byte*)(moduleBaseAddress + 0x1) != 0x5A)
                throw new ArgumentException("The passed module isn't a valid PE file");

            int OFFSET_TO_PE_HEADER_OFFSET = 0x3c;
            uint offsetToPESig = *(uint*)(moduleBaseAddress + OFFSET_TO_PE_HEADER_OFFSET);
            IntPtr pPESig = IntPtr.Add(moduleBaseAddress, (int)offsetToPESig);


            if (*(byte*)(pPESig + 0x0) != 0x50 || *(byte*)(pPESig + 0x1) != 0x45 || *(byte*)(pPESig + 0x2) != 0x0 || *(byte*)(pPESig + 0x3) != 0x0)
                throw new ArgumentException("The passed module isn't a valid PE file");

            return (ImageNtHeaders*)pPESig;
        }

        internal static unsafe IntPtr GetExportedFunctionPointerForModule(IntPtr moduleBaseAddress, string importName)
        {
            ImageNtHeaders* imageNtHeaders = AnalyseModuleWin(moduleBaseAddress);
            ImageSectionHeader* pSech = ImageFirstSection(imageNtHeaders);
            ImageDataDirectory* imageDirectoryEntryExport = MelonUtils.IsGame32Bit() ? &imageNtHeaders->optionalHeader32.exportTable : &imageNtHeaders->optionalHeader64.exportTable;

            ImageExportDirectory* pExportDirectory = (ImageExportDirectory*)IntPtr.Add(moduleBaseAddress, (int)imageDirectoryEntryExport->virtualAddress);
            //MelonLoader.MelonLogger.Msg("pExportDirectory at " + string.Format("{0:X}", (ulong)pExportDirectory - (ulong)moduleBaseAddress));
            for (uint i = 0; i < imageDirectoryEntryExport->size / sizeof(ImageExportDirectory); ++i)
            {
                ImageExportDirectory* pExportDirectoryI = pExportDirectory + i;
                //MelonLoader.MelonLogger.Msg("pExportDirectoryI->name: " + string.Format("{0:X}", pExportDirectoryI->name));
                if (pExportDirectoryI->name != 0)
                {
                    string imagename = CppUtils.CharArrayPtrToString(moduleBaseAddress + (int)pExportDirectoryI->name);
                    //string imagename = CppUtils.CharArrayPtrToString((IntPtr)pExportDirectoryI->name);
                    //MelonLoader.MelonLogger.Msg("imagename: " + imagename);
                    /*
                    if (imagename != "UnityPlayer.dll")
                        continue;
                    */

                    IntPtr baseNameOrdinalOffset = IntPtr.Add(moduleBaseAddress, (int)pExportDirectoryI->addressOfNameOrdinals);
                    IntPtr baseFunctionOffset = IntPtr.Add(moduleBaseAddress, (int)pExportDirectoryI->addressOfFunctions);
                    IntPtr baseNameOffset = IntPtr.Add(moduleBaseAddress, (int)pExportDirectoryI->addressOfNames);

                    for (int j = 0; j < pExportDirectoryI->numberOfNames; ++j)
                    {
                        ushort ordinal = *(ushort*)IntPtr.Add(baseNameOrdinalOffset, j * 2);
                        IntPtr functionnameAddress = IntPtr.Add(moduleBaseAddress, *(int*)IntPtr.Add(baseNameOffset, j * 4));
                        IntPtr functionaddress = IntPtr.Add(moduleBaseAddress, *(int*)IntPtr.Add(baseFunctionOffset, ordinal * 4));
                        string importname = CppUtils.CharArrayPtrToString(functionnameAddress);
                        //MelonLoader.MelonLogger.Msg(imagename + "::" + importname + " @ " + string.Format("{0:X}", (ulong)functionaddress - (ulong)moduleBaseAddress));
                        if (importname == importName)
                            return functionaddress;
                    }
                }
            }

            return IntPtr.Zero;
        }

        private static unsafe ImageSectionHeader* ImageFirstSection(ImageNtHeaders* ntheader)
        {
            return (ImageSectionHeader*)((ulong)ntheader + 24 + ntheader->fileHeader.sizeOfOptionalHeader);
        }
    }
}
