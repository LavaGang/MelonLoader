#if ANDROID
namespace MelonLoader.Java;

public static partial class JNI
{
    public enum ReferenceType
    {
        Local = 0,
        Global = 1,
        WeakGlobal = 2
    }
}
#endif
