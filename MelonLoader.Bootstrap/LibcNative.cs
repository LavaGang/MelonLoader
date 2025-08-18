#if LINUX || OSX || ANDROID
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace MelonLoader.Bootstrap;

internal partial class LibcNative
{
    internal const int Stdout = 1;
    internal const int Stderr = 2;

    internal const int SeekEnd = 2;

    internal const int RtldLazy = 0x1;
    internal const int RtldNoLoad = 0x10;

    const string DL_LIB =
#if !ANDROID
        "libc";
#else
        "libdl";
#endif

    const string C_LIB = "libc";

    [LibraryImport(C_LIB, EntryPoint = "__libc_start_main")]
    public static unsafe partial int LibCStartMain(
        delegate* unmanaged[Cdecl]<int, char**, char**, int> main,
        int argc,
        char** argv,
        nint init,
        nint fini,
        nint rtLdFini,
        nint stackEnd);

    [LibraryImport(DL_LIB, EntryPoint = "dlopen", StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial nint Dlopen(string handle, int flags);

    [LibraryImport(DL_LIB, EntryPoint = "dlsym", StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial nint Dlsym(nint handle, string symbol);

    [LibraryImport(C_LIB, EntryPoint = "setenv", StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial int Setenv(string name, string value,[MarshalAs(UnmanagedType.Bool)] bool overwrite);

    [LibraryImport(C_LIB, EntryPoint = "dup2")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial int Dup2(int oldFd, int newFd);

#if OSX
    [LibraryImport(C_LIB, EntryPoint = "fopen", StringMarshalling = StringMarshalling.Utf8)]
#else
    [LibraryImport(C_LIB, EntryPoint = "fopen64", StringMarshalling = StringMarshalling.Utf8)]
#endif
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial nint Fopen64(string pathName, string modes);

    [LibraryImport(C_LIB, EntryPoint = "vfprintf", StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial int Vfprintf(nint stream, string format, nint vList);

    [LibraryImport(C_LIB, EntryPoint = "vsnprintf", StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static unsafe partial int Vsnprintf(byte* s, int maxLen, string format, nint arg);

    [LibraryImport(C_LIB, EntryPoint = "fseek", StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial int Fseek(nint stream, long offset, int whence);

    [LibraryImport(C_LIB, EntryPoint = "fwrite", StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static unsafe partial int Fwrite(byte* ptr, int size, int nItems, nint stream);

    [LibraryImport(C_LIB, EntryPoint = "fileno")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial int Fileno(nint stream);

    [LibraryImport(C_LIB, EntryPoint = "fclose")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial int FClose(nint stream);
}
#endif