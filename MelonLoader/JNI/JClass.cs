#if ANDROID
using System.Collections.Generic;

namespace MelonLoader.Java;

public class JClass : JObject
{
    private Dictionary<(string, string), JFieldID> FieldCache { get; set; } = new();

    private Dictionary<(string, string), JMethodID> MethodCache { get; set; } = new();

    public JClass() : base() { }

    public JFieldID GetFieldID(string name, string sig)
    {
        (string, string) key = new(name, sig);

        if (this.FieldCache.TryGetValue(key, out JFieldID found))
        {
            return found;
        }
        else
        {
            JFieldID id = JNI.GetFieldID(this, name, sig);
            this.FieldCache.Add(key, id);
            return id;
        }
    }

    public JFieldID GetStaticFieldID(string name, string sig)
    {
        (string, string) key = new(name, sig);

        if (this.FieldCache.TryGetValue(key, out JFieldID found))
        {
            return found;
        }
        else
        {
            JFieldID id = JNI.GetStaticFieldID(this, name, sig);
            this.FieldCache.Add(key, id);
            return id;
        }
    }

    public JMethodID GetMethodID(string name, string sig)
    {
        (string, string) key = new(name, sig);

        if (this.MethodCache.TryGetValue(key, out JMethodID found))
        {
            return found;
        }
        else
        {
            JMethodID id = JNI.GetMethodID(this, name, sig);
            this.MethodCache.Add(key, id);
            return id;
        }
    }

    public JMethodID GetStaticMethodID(string name, string sig)
    {
        (string, string) key = new(name, sig);

        if (this.MethodCache.TryGetValue(key, out JMethodID found))
        {
            return found;
        }
        else
        {
            JMethodID id = JNI.GetStaticMethodID(this, name, sig);
            this.MethodCache.Add(key, id);
            return id;
        }
    }

    public T GetStaticObjectField<T>(string name, string sig) where T : JObject, new()
    {
        return JNI.GetStaticObjectField<T>(this, this.GetStaticFieldID(name, sig));
    }

    public T GetStaticField<T>(string name)
    {
        return JNI.GetStaticField<T>(this, this.GetStaticFieldID(name, JNI.GetTypeSignature<T>()));
    }

    public void SetStaticField<T>(string name, T value)
    {
        JNI.SetStaticField<T>(this, this.GetStaticFieldID(name, JNI.GetTypeSignature<T>()), value);
    }

    public T GetObjectField<T>(JObject obj, string name, string sig) where T : JObject, new()
    {
        return JNI.GetObjectField<T>(obj, this.GetFieldID(name, sig));
    }

    public T GetField<T>(JObject obj, string name)
    {
        return JNI.GetField<T>(obj, this.GetFieldID(name, JNI.GetTypeSignature<T>()));
    }

    public void SetObjectField(JObject obj, string name, string sig, JObject value)
    {
        JNI.SetObjectField(obj, this.GetFieldID(name, sig), value);
    }

    public void SetField<T>(JObject obj, string name, T value)
    {
        JNI.SetField<T>(obj, this.GetFieldID(name, JNI.GetTypeSignature<T>()), value);
    }

    public T CallStaticObjectMethod<T>(string name, string sig, params JValue[] args) where T : JObject, new()
    {
        return JNI.CallStaticObjectMethod<T>(this, this.GetStaticMethodID(name, sig), args);
    }

    public T CallStaticMethod<T>(string name, string sig, params JValue[] args)
    {
        return JNI.CallStaticMethod<T>(this, this.GetStaticMethodID(name, sig), args);
    }

    public void CallStaticVoidMethod(string name, string sig, params JValue[] args)
    {
        JNI.CallStaticVoidMethod(this, this.GetMethodID(name, sig), args);
    }

    public T CallObjectMethod<T>(JObject obj, string name, string sig, params JValue[] args) where T : JObject, new()
    {
        return JNI.CallObjectMethod<T>(obj, this.GetMethodID(name, sig), args);
    }

    public T CallMethod<T>(JObject obj, string name, string sig, params JValue[] args)
    {
        return JNI.CallMethod<T>(obj, this.GetMethodID(name, sig), args);
    }

    public void CallVoidMethod(JObject obj, string name, string sig, params JValue[] args)
    {
        JNI.CallVoidMethod(obj, this.GetMethodID(name, sig), args);
    }

    public T NewObject<T>(string name, string sig, params JValue[] args) where T : JObject, new()
    {
        return JNI.NewObject<T>(this, this.GetMethodID(name, sig), args);
    }
}
#endif
