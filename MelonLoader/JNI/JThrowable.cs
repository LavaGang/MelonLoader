#if ANDROID
namespace MelonLoader.Java;

public class JThrowable : JObject
{
    public JThrowable() { }

    public string GetMessage() => JNI.FindClass("java/lang/Throwable").CallObjectMethod<JString>(this, "getMessage", "()Ljava/lang/String;").GetString();

    public override string ToString() => JNI.FindClass("java/lang/Throwable").CallObjectMethod<JString>(this, "toString", "()Ljava/lang/String;").GetString();
}
#endif
