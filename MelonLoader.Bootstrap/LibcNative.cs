#if LINUX || OSX
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
    
    [LibraryImport("libc", EntryPoint = "__libc_start_main")]
    public static unsafe partial int LibCStartMain(
        delegate* unmanaged[Cdecl]<int, char**, char**, int> main,
        int argc,
        char** argv,
        nint init,
        nint fini,
        nint rtLdFini,
        nint stackEnd);

    [LibraryImport("libc", EntryPoint = "dlopen", StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial nint Dlopen(string handle, int flags);

    [LibraryImport("libc", EntryPoint = "dlsym", StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial nint Dlsym(nint handle, string symbol);

    [LibraryImport("libc", EntryPoint = "setenv", StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial int Setenv(string name, string value,[MarshalAs(UnmanagedType.Bool)] bool overwrite);

    [LibraryImport("libc", EntryPoint = "dup2")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial int Dup2(int oldFd, int newFd);

#if OSX
    [LibraryImport("libc", EntryPoint = "fopen", StringMarshalling = StringMarshalling.Utf8)]
#else
    [LibraryImport("libc", EntryPoint = "fopen64", StringMarshalling = StringMarshalling.Utf8)]
#endif
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial nint Fopen64(string pathName, string modes);

    [LibraryImport("libc", EntryPoint = "vfprintf", StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial int Vfprintf(nint stream, string format, nint vList);

    [LibraryImport("libc", EntryPoint = "vsnprintf", StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static unsafe partial int Vsnprintf(byte* s, int maxLen, string format, nint arg);

    [LibraryImport("libc", EntryPoint = "fseek", StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial int Fseek(nint stream, long offset, int whence);

    [LibraryImport("libc", EntryPoint = "fwrite", StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static unsafe partial int Fwrite(byte* ptr, int size, int nItems, nint stream);

    [LibraryImport("libc", EntryPoint = "fileno")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial int Fileno(nint stream);

    [LibraryImport("libc", EntryPoint = "fclose")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial int FClose(nint stream);
}
#endif

#if ANDROID
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace MelonLoader.Bootstrap;

internal partial class LibcNative
{
    internal const int Stdout = 1;
    internal const int Stderr = 2;

    internal const int SeekEnd = 2;
    
    [LibraryImport("libdl", EntryPoint = "dlsym", StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial nint Dlsym(nint handle, string symbol);
}
#endif