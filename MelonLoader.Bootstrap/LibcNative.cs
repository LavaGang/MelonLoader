#if LINUX
using System.Runtime.InteropServices;

namespace MelonLoader.Bootstrap;

public partial class LibcNative
{
    [LibraryImport("libc", EntryPoint = "__libc_start_main")]
    public static unsafe partial int LibCStartMain(
        delegate* unmanaged[Cdecl]<int, char**, char**, int> main,
        int argc,
        char** argv,
        nint init,
        nint fini,
        nint rtLdFini,
        nint stackEnd);

    [LibraryImport("libc", EntryPoint = "setenv", StringMarshalling = StringMarshalling.Utf8)]
    public static unsafe partial int Setenv(string name, string value,[MarshalAs(UnmanagedType.Bool)] bool overwrite);
}
#endif