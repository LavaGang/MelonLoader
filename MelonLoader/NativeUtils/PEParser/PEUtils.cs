using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace MelonLoader.NativeUtils.PEParser
{
    public static class PEUtils
    {
        public static unsafe ImageNtHeaders* AnalyseModuleWin(IntPtr moduleBaseAddress)
        {
            //MelonLoader.MelonLogger.Msg("[moduleBaseAddress+0x0]: " + (*(byte*)(moduleBaseAddress + 0x0)));
            if (*(byte*)((long)moduleBaseAddress + 0x0) != 0x4D || *(byte*)((long)moduleBaseAddress + 0x1) != 0x5A)
                throw new ArgumentException("The passed module isn't a valid PE file");

            int OFFSET_TO_PE_HEADER_OFFSET = 0x3c;
            uint offsetToPESig = *(uint*)((long)moduleBaseAddress + OFFSET_TO_PE_HEADER_OFFSET);
            IntPtr pPESig = new IntPtr((long)moduleBaseAddress + offsetToPESig);


            if (*(byte*)((long)pPESig + 0x0) != 0x50 || *(byte*)((long)pPESig + 0x1) != 0x45 || *(byte*)((long)pPESig + 0x2) != 0x0 || *(byte*)((long)pPESig + 0x3) != 0x0)
                throw new ArgumentException("The passed module isn't a valid PE file");

            return (ImageNtHeaders*)pPESig;
        }

        public static IntPtr GetExportedFunctionPointerForModule(string moduleName, string importName)
        {
            IntPtr moduleAddress = IntPtr.Zero;

            foreach (ProcessModule module in Process.GetCurrentProcess().Modules)
            {
                if (module.ModuleName == moduleName)
                {
                    moduleAddress = module.BaseAddress;
                    break;
                }
            }

            if (moduleAddress == IntPtr.Zero)
            {
                MelonLogger.Error($"Failed to find module \"{moduleName}\"");
                return IntPtr.Zero;
            }

            return GetExportedFunctionPointerForModule((long)moduleAddress, importName);
        }

        public static unsafe IntPtr GetExportedFunctionPointerForModule(long moduleBaseAddress, string importName)
        {
            ImageNtHeaders* imageNtHeaders = AnalyseModuleWin((IntPtr)moduleBaseAddress);
            ImageSectionHeader* pSech = ImageFirstSection(imageNtHeaders);
            ImageDataDirectory* imageDirectoryEntryExport = MelonUtils.IsGame32Bit() ? &imageNtHeaders->optionalHeader32.exportTable : &imageNtHeaders->optionalHeader64.exportTable;

            ImageExportDirectory* pExportDirectory = (ImageExportDirectory*)((long)moduleBaseAddress + imageDirectoryEntryExport->virtualAddress);
            //MelonLoader.MelonLogger.Msg("pExportDirectory at " + string.Format("{0:X}", (ulong)pExportDirectory - (ulong)moduleBaseAddress));
            for (uint i = 0; i < imageDirectoryEntryExport->size / sizeof(ImageExportDirectory); ++i)
            {
                ImageExportDirectory* pExportDirectoryI = pExportDirectory + i;
                //MelonLoader.MelonLogger.Msg("pExportDirectoryI->name: " + string.Format("{0:X}", pExportDirectoryI->name));
                if (pExportDirectoryI->name != 0)
                {
                    string imagename = Marshal.PtrToStringAnsi((IntPtr)((long)moduleBaseAddress + pExportDirectoryI->name));
                    //string imagename = CppUtils.CharArrayPtrToString((IntPtr)pExportDirectoryI->name);
                    //MelonLoader.MelonLogger.Msg("imagename: " + imagename);
                    /*
                    if (imagename != "UnityPlayer.dll")
                        continue;
                    */


                    long baseNameOrdinalOffset = moduleBaseAddress + (int)pExportDirectoryI->addressOfNameOrdinals;
                    long baseFunctionOffset = moduleBaseAddress + (int)pExportDirectoryI->addressOfFunctions;
                    long baseNameOffset = moduleBaseAddress + (int)pExportDirectoryI->addressOfNames;

                    for (int j = 0; j < pExportDirectoryI->numberOfNames; ++j)
                    {
                        ushort ordinal = *(ushort*)((long)baseNameOrdinalOffset + j * 2);
                        long functionnameAddress = moduleBaseAddress + *(int*)(baseNameOffset + j * 4);
                        long functionaddress = moduleBaseAddress + *(int*)(baseFunctionOffset + ordinal * 4);
                        string importname = Marshal.PtrToStringAnsi((IntPtr)functionnameAddress);
                        //MelonLoader.MelonLogger.Msg($"{imagename}::{importname} @ 0x{((ulong)functionaddress - (ulong)moduleBaseAddress):X} (0x{functionaddress:X} - 0x{moduleBaseAddress:X})");
                        if (importname == importName)
                            return (IntPtr)functionaddress;
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
