#if ANDROID
namespace MelonLoader.Java;

public static partial class JNI
{
    public enum ReleaseMode
    {
        Default = 0,
        Commit = 1,
        Abort = 2
    }
}
#endif
