#if ANDROID
namespace MelonLoader.Java;

public class JString : JObject
{
    public JString() : base() { }

    public string GetString() => JNI.GetJStringString(this);
}
#endif
