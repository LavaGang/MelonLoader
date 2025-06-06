#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type
#pragma warning disable CS8605 // Unboxing a possibly null value.
#pragma warning disable CS8632 // The annotation for nullable reference types...
#pragma warning disable CS8602 // Dereference of a possibly null reference

#if ANDROID
namespace MelonLoader.Java;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

/// <summary>
/// Represents the Java Native Interface
/// </summary>
public unsafe static partial class JNI
{
    internal static JavaVM* VM;

    [ThreadStatic]
    private static JNIEnv* _env;
    internal static JNIEnv* Env
    {
        get
        {
            if (_env == null)
            {
                if (VM == null)
                    throw new InvalidOperationException("JNI not initialized. Call JNI.Initialize() first.");

                Initialize(IntPtr.Zero);
            }

            return _env;
        }
    }

    internal static Dictionary<string, JClass> ClassCache { get; set; } = new();

    public static void Initialize(IntPtr vmPtr)
    {
        if (VM == null && vmPtr != IntPtr.Zero)
            VM = (JavaVM*)vmPtr;
        else if (VM == null)
            throw new InvalidOperationException("JavaVM not initialized. Call JNI.Initialize() with a VM pointer first.");
        
        unsafe
        {
            JavaVMAttachArgs args = new()
            {
                version = (int)Version.V1_6,
                name = IntPtr.Zero,
                group = IntPtr.Zero
            };

            IntPtr argsPtr = 
#if NET35
                Marshal.AllocHGlobal(Marshal.SizeOf(typeof(JavaVMAttachArgs)));
#else
                Marshal.AllocHGlobal(Marshal.SizeOf<JavaVMAttachArgs>());
#endif

            try
            {
                Marshal.StructureToPtr(args, argsPtr, false);

                // TODO: threads need detached at some point, not sure how this should be handled for things like the finalizer thread
                Result res = VM->Functions->AttachCurrentThread(VM, out JNIEnv* localEnv, argsPtr);
                _env = localEnv;

                if (res != Result.Ok)
                    throw new InvalidOperationException($"Failed to attach current thread to JVM. Result: {res}");
            }
            finally
            {
                Marshal.FreeHGlobal(argsPtr);
            }
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    struct JavaVMAttachArgs 
    {
        public int version;
        public IntPtr name;
        public IntPtr group;
    }

    public static int GetVersion()
    {
        unsafe
        {
            return Env->Functions->GetVersion(Env);
        }
    }

    public static JClass DefineClass(string name, JObject loader, sbyte[] bytes)
    {
        unsafe
        {
            IntPtr bytesPtr = Marshal.UnsafeAddrOfPinnedArrayElement(bytes, 0);
            IntPtr nameAnsi = Marshal.StringToHGlobalAnsi(name);

            IntPtr res = Env->Functions->DefineClass(Env, nameAnsi, loader.Handle, bytesPtr, bytes.Length);

            Marshal.FreeHGlobal(nameAnsi);

            using JClass local = new() { Handle = res, ReferenceType = JNI.ReferenceType.Local };
            return NewGlobalRef<JClass>(local);
        }
    }

    public static JClass FindClass(string name)
    {
        unsafe
        {
            if (ClassCache.TryGetValue(name, out JClass? found))
            {
                return found;
            }
            else
            {
                IntPtr nameAnsi = Marshal.StringToHGlobalAnsi(name);
                IntPtr res = Env->Functions->FindClass(Env, nameAnsi);

                Marshal.FreeHGlobal(nameAnsi);

                using JClass local = new() { Handle = res, ReferenceType = JNI.ReferenceType.Local };
                JClass global = NewGlobalRef<JClass>(local);
                ClassCache.Add(name, global);
                return global;
            }
        }
    }

    public static JMethodID FromReflectedMethod(JObject method)
    {
        unsafe
        {
            return Env->Functions->FromReflectedMethod(Env, method.Handle);
        }
    }

    public static JFieldID FromReflectedField(JObject field)
    {
        unsafe
        {
            return Env->Functions->FromReflectedField(Env, field.Handle);
        }
    }

    public static JObject ToReflectedMethod(JClass cls, JMethodID methodID, bool isStatic)
    {
        unsafe
        {
            IntPtr res = Env->Functions->ToReflectedMethod(Env, cls.Handle, methodID, Convert.ToByte(isStatic));

            using JObject local = new() { Handle = res, ReferenceType = ReferenceType.Local };
            return NewGlobalRef<JObject>(local);
        }
    }

    public static JClass GetSuperClass(JClass sub)
    {
        unsafe
        {
            IntPtr res = Env->Functions->GetSuperClass(Env, sub.Handle);

            using JClass local = new() { Handle = res, ReferenceType = JNI.ReferenceType.Local };
            return NewGlobalRef<JClass>(local);
        }
    }

    public static bool IsAssignableFrom(JClass sub, JClass sup)
    {
        unsafe
        {
            return Convert.ToBoolean(Env->Functions->IsAssignableFrom(Env, sub.Handle, sup.Handle));
        }
    }

    public static JObject ToReflectedField(JClass cls, JFieldID fieldID, bool isStatic)
    {
        throw new NotImplementedException();
    }

    public static void Throw(JThrowable throwable)
    {
        unsafe
        {
            int res = Env->Functions->Throw(Env, throwable.Handle);
        }
    }

    public static void ThrowNew(JClass cls, string message)
    {
        unsafe
        {
            IntPtr messageAnsi = Marshal.StringToHGlobalAnsi(message);
            int res = Env->Functions->Throw(Env, messageAnsi);
            Marshal.FreeHGlobal(messageAnsi);
        }
    }

    public static JThrowable ExceptionOccurred()
    {
        unsafe
        {
            IntPtr res = Env->Functions->ExceptionOccurred(Env);

            using JThrowable local = new() { Handle = res, ReferenceType = JNI.ReferenceType.Local };
            return NewGlobalRef<JThrowable>(local);
        }
    }

    public static void ExceptionDescribe()
    {
        unsafe
        {
            Env->Functions->ExceptionDescribe(Env);
        }
    }

    public static void ExceptionClear()
    {
        unsafe
        {
            Env->Functions->ExceptionClear(Env);
        }
    }

    public static void FatalError(string message)
    {
        throw new NotImplementedException();
    }

    public static int PushLocalFrame(int capacity)
    {
        throw new NotImplementedException();
    }

    public static JObject PopLocalFrame(JObject result)
    {
        throw new NotImplementedException();
    }

    public static T NewGlobalRef<T>(JObject lobj) where T : JObject, new()
    {
        unsafe
        {
            IntPtr res = Env->Functions->NewGlobalRef(Env, lobj.Handle);
            return new T() { Handle = res, ReferenceType = JNI.ReferenceType.Global };
        }
    }

    public static void DeleteGlobalRef(JObject gref)
    {
        unsafe
        {
            if (gref == null)
                return;

            if (!gref.Valid())
                return;

            Env->Functions->DeleteGlobalRef(Env, gref.Handle);
        }
    }

    public static void CheckExceptionAndThrow()
    {
        if (ExceptionCheck())
        {
            JThrowable throwable = ExceptionOccurred();
            ExceptionClear();
            throw new JThrowableException(throwable);
        }
    }

    public static void DeleteLocalRef(JObject lref)
    {
        unsafe
        {
            if (lref == null)
                return;

            if (!lref.Valid())
                return;

            Env->Functions->DeleteLocalRef(Env, lref.Handle);
        }
    }

    public static bool IsSameObject(JObject obj1, JObject obj2)
    {
        unsafe
        {
            return Convert.ToBoolean(Env->Functions->IsSameObject(Env, obj1.Handle, obj2.Handle));
        }
    }

    public static T NewLocalRef<T>(JObject obj) where T : JObject, new()
    {
        unsafe
        {
            IntPtr res = Env->Functions->NewLocalRef(Env, obj.Handle);
            return new T() { Handle = res, ReferenceType = JNI.ReferenceType.Local };
        }
    }

    public static int EnsureLocalCapacity(int capacity)
    {
        unsafe
        {
            return Env->Functions->EnsureLocalCapacity(Env, capacity);
        }
    }

    public static T AllocObject<T>(JClass cls) where T : JObject, new()
    {
        unsafe
        {
            IntPtr res = Env->Functions->AllocObject(Env, cls.Handle);

            using JObject local = new() { Handle = res, ReferenceType = JNI.ReferenceType.Local };
            return NewGlobalRef<T>(local);
        }
    }

    public static T NewObject<T>(JClass cls, JMethodID methodID, params JValue[] args) where T : JObject, new()
    {
        unsafe
        {
            IntPtr argsPtr = Marshal.UnsafeAddrOfPinnedArrayElement(args, 0);
            IntPtr res = Env->Functions->NewObjectA(Env, cls.Handle, methodID, argsPtr);
            JObject local = new() { Handle = res, ReferenceType = JNI.ReferenceType.Local };
            return NewGlobalRef<T>(local);
        }
    }

    public static JClass GetObjectClass(JObject obj)
    {
        unsafe
        {
            IntPtr res = Env->Functions->GetObjectClass(Env, obj.Handle);

            using JClass local = new() { Handle = res, ReferenceType = JNI.ReferenceType.Local };
            return NewGlobalRef<JClass>(local);
        }
    }

    public static bool IsInstanceOf(JObject obj, JClass cls)
    {
        unsafe
        {
            return Convert.ToBoolean(Env->Functions->IsInstanceOf(Env, obj.Handle, cls.Handle));
        }
    }

    public static JMethodID GetMethodID(JClass cls, string name, string sig)
    {
        unsafe
        {
            IntPtr nameAnsi = Marshal.StringToHGlobalAnsi(name);
            IntPtr sigAnsi = Marshal.StringToHGlobalAnsi(sig);

            JMethodID id = Env->Functions->GetMethodID(Env, cls.Handle, nameAnsi, sigAnsi);

            Marshal.FreeHGlobal(nameAnsi);
            Marshal.FreeHGlobal(sigAnsi);
            return id;
        }
    }

    public static T CallObjectMethod<T>(JObject obj, JMethodID methodID, params JValue[] args) where T : JObject, new()
    {
        unsafe
        {
            IntPtr argsPtr = Marshal.UnsafeAddrOfPinnedArrayElement(args, 0);

            fixed (JValue* v = args)
            {
                IntPtr res = Env->Functions->CallObjectMethodA(Env, obj.Handle, methodID, argsPtr);
                using JObject local = new() { Handle = res, ReferenceType = JNI.ReferenceType.Local };
                return NewGlobalRef<T>(local);
            }
        }
    }

    public static T CallMethod<T>(JObject obj, JMethodID methodID, params JValue[] args)
    {
        unsafe
        {
            Type t = typeof(T);
            IntPtr argsPtr = Marshal.UnsafeAddrOfPinnedArrayElement(args, 0);

            if (t == typeof(bool))
            {
                return (T)(object)Convert.ToBoolean(Env->Functions->CallBooleanMethodA(Env, obj.Handle, methodID, argsPtr));
            }
            else if (t == typeof(sbyte))
            {
                return (T)(object)Env->Functions->CallByteMethodA(Env, obj.Handle, methodID, argsPtr);
            }
            else if (t == typeof(char))
            {
                return (T)(object)Env->Functions->CallCharMethodA(Env, obj.Handle, methodID, argsPtr);
            }
            else if (t == typeof(short))
            {
                return (T)(object)Env->Functions->CallShortMethodA(Env, obj.Handle, methodID, argsPtr);
            }
            else if (t == typeof(int))
            {
                return (T)(object)Env->Functions->CallIntMethodA(Env, obj.Handle, methodID, argsPtr);
            }
            else if (t == typeof(long))
            {
                return (T)(object)Env->Functions->CallLongMethodA(Env, obj.Handle, methodID, argsPtr);
            }
            else if (t == typeof(float))
            {
                return (T)(object)Env->Functions->CallFloatMethodA(Env, obj.Handle, methodID, argsPtr);
            }
            else if (t == typeof(double))
            {
                return (T)(object)Env->Functions->CallDoubleMethodA(Env, obj.Handle, methodID, argsPtr);
            }
            else
            {
                throw new ArgumentException($"CallMethod Type {t} not supported.");
            }
        }
    }

    public static void CallVoidMethod(JObject obj, JMethodID methodID, params JValue[] args)
    {
        unsafe
        {
            IntPtr argsPtr = Marshal.UnsafeAddrOfPinnedArrayElement(args, 0);
            Env->Functions->CallVoidMethodA(Env, obj.Handle, methodID, argsPtr);
        }
    }

    public static T CallNonvirtualObjectMethod<T>(JObject obj, JClass cls, JMethodID methodID, params JValue[] args) where T : JObject, new()
    {
        unsafe
        {
            IntPtr argsPtr = Marshal.UnsafeAddrOfPinnedArrayElement(args, 0);
            IntPtr res = Env->Functions->CallNonvirtualObjectMethodA(Env, obj.Handle, cls.Handle, methodID, argsPtr);

            using JObject local = new() { Handle = res, ReferenceType = JNI.ReferenceType.Local };
            return NewGlobalRef<T>(local);
        }
    }

    public static T CallNonvirtualMethod<T>(JObject obj, JClass cls, JMethodID methodID, params JValue[] args)
    {
        unsafe
        {
            Type t = typeof(T);
            IntPtr argsPtr = Marshal.UnsafeAddrOfPinnedArrayElement(args, 0);

            if (t == typeof(bool))
            {
                return (T)(object)Convert.ToBoolean(Env->Functions->CallNonvirtualBooleanMethodA(Env, obj.Handle, cls.Handle, methodID, argsPtr));
            }
            else if (t == typeof(sbyte))
            {
                return (T)(object)Env->Functions->CallNonvirtualByteMethodA(Env, obj.Handle, cls.Handle, methodID, argsPtr);
            }
            else if (t == typeof(char))
            {
                return (T)(object)Env->Functions->CallNonvirtualCharMethodA(Env, obj.Handle, cls.Handle, methodID, argsPtr);
            }
            else if (t == typeof(short))
            {
                return (T)(object)Env->Functions->CallNonvirtualShortMethodA(Env, obj.Handle, cls.Handle, methodID, argsPtr);
            }
            else if (t == typeof(int))
            {
                return (T)(object)Env->Functions->CallNonvirtualIntMethodA(Env, obj.Handle, cls.Handle, methodID, argsPtr);
            }
            else if (t == typeof(long))
            {
                return (T)(object)Env->Functions->CallNonvirtualLongMethodA(Env, obj.Handle, cls.Handle, methodID, argsPtr);
            }
            else if (t == typeof(float))
            {
                return (T)(object)Env->Functions->CallNonvirtualFloatMethodA(Env, obj.Handle, cls.Handle, methodID, argsPtr);
            }
            else if (t == typeof(double))
            {
                return (T)(object)Env->Functions->CallNonvirtualDoubleMethodA(Env, obj.Handle, cls.Handle, methodID, argsPtr);
            }
            else
            {
                throw new ArgumentException($"CallNonvirtualMethod Type {t} not supported.");
            }
        }
    }

    public static void CallNonvirtualVoidMethod(JObject obj, JClass cls, JMethodID methodID, params JValue[] args)
    {
        unsafe
        {
            IntPtr argsPtr = Marshal.UnsafeAddrOfPinnedArrayElement(args, 0);
            Env->Functions->CallNonvirtualVoidMethodA(Env, obj.Handle, cls.Handle, methodID, argsPtr);
        }
    }

    public static JFieldID GetFieldID(JClass cls, string name, string sig)
    {
        unsafe
        {
            IntPtr nameAnsi = Marshal.StringToHGlobalAnsi(name);
            IntPtr sigAnsi = Marshal.StringToHGlobalAnsi(sig);

            JFieldID id = Env->Functions->GetFieldID(Env, cls.Handle, nameAnsi, sigAnsi);

            Marshal.FreeHGlobal(nameAnsi);
            Marshal.FreeHGlobal(sigAnsi);
            return id;
        }
    }

    public static T GetObjectField<T>(JObject obj, JFieldID fieldID) where T : JObject, new()
    {
        unsafe
        {
            IntPtr res = Env->Functions->GetObjectField(Env, obj.Handle, fieldID);
            using JObject local = new() { Handle = res, ReferenceType = JNI.ReferenceType.Local };
            return NewGlobalRef<T>(local);
        }
    }

    public static T GetField<T>(JObject obj, JFieldID fieldID)
    {
        unsafe
        {
            Type t = typeof(T);

            if (t == typeof(bool))
            {
                return (T)(object)Convert.ToBoolean(Env->Functions->GetBooleanField(Env, obj.Handle, fieldID));
            }
            else if (t == typeof(sbyte))
            {
                return (T)(object)Env->Functions->GetByteField(Env, obj.Handle, fieldID);
            }
            else if (t == typeof(char))
            {
                return (T)(object)Env->Functions->GetCharField(Env, obj.Handle, fieldID);
            }
            else if (t == typeof(short))
            {
                return (T)(object)Env->Functions->GetShortField(Env, obj.Handle, fieldID);
            }
            else if (t == typeof(int))
            {
                return (T)(object)Env->Functions->GetIntField(Env, obj.Handle, fieldID);
            }
            else if (t == typeof(long))
            {
                return (T)(object)Env->Functions->GetLongField(Env, obj.Handle, fieldID);
            }
            else if (t == typeof(float))
            {
                return (T)(object)Env->Functions->GetFloatField(Env, obj.Handle, fieldID);
            }
            else if (t == typeof(double))
            {
                return (T)(object)Env->Functions->GetDoubleField(Env, obj.Handle, fieldID);
            }
            else
            {
                throw new ArgumentException($"GetField Type {t} not supported.");
            }
        }
    }

    public static void SetObjectField(JObject obj, JFieldID fieldID, JObject val)
    {
        unsafe
        {
            Env->Functions->SetObjectField(Env, obj.Handle, fieldID, val.Handle);
        }
    }

    public static void SetField<T>(JObject obj, JFieldID fieldID, T value)
    {
        unsafe
        {
            switch (value)
            {
                case bool b:
                    Env->Functions->SetBooleanField(Env, obj.Handle, fieldID, Convert.ToByte(b));
                    break;

                case sbyte b:
                    Env->Functions->SetByteField(Env, obj.Handle, fieldID, b);
                    break;

                case char c:
                    Env->Functions->SetCharField(Env, obj.Handle, fieldID, c);
                    break;

                case short s:
                    Env->Functions->SetShortField(Env, obj.Handle, fieldID, s);
                    break;

                case int i:
                    Env->Functions->SetIntField(Env, obj.Handle, fieldID, i);
                    break;

                case float f:
                    Env->Functions->SetFloatField(Env, obj.Handle, fieldID, f);
                    break;

                case double d:
                    Env->Functions->SetDoubleField(Env, obj.Handle, fieldID, d);
                    break;

                default:
                    throw new ArgumentException($"SetField Type {value?.GetType()} not supported.");
            }
        }
    }

    public static JMethodID GetStaticMethodID(JClass cls, string name, string sig)
    {
        unsafe
        {
            IntPtr nameAnsi = Marshal.StringToHGlobalAnsi(name);
            IntPtr sigAnsi = Marshal.StringToHGlobalAnsi(sig);
            JMethodID id = Env->Functions->GetStaticMethodID(Env, cls.Handle, nameAnsi, sigAnsi);
            return id;
        }
    }

    public static T CallStaticObjectMethod<T>(JClass cls, JMethodID methodID, params JValue[] args) where T : JObject, new()
    {
        unsafe
        {
            IntPtr argsPtr = Marshal.UnsafeAddrOfPinnedArrayElement(args, 0);
            IntPtr res = Env->Functions->CallStaticObjectMethodA(Env, cls.Handle, methodID.Handle, argsPtr);
            using JObject local = new() { Handle = res, ReferenceType = JNI.ReferenceType.Local };
            return NewGlobalRef<T>(local);
        }
    }

    public static T CallStaticMethod<T>(JClass cls, JMethodID methodID, params JValue[] args)
    {
        Type t = typeof(T);
        IntPtr argsPtr = Marshal.UnsafeAddrOfPinnedArrayElement(args, 0);

        if (t == typeof(bool))
        {
            return (T)(object)Convert.ToBoolean(Env->Functions->CallStaticBooleanMethodA(Env, cls.Handle, methodID, argsPtr));
        }
        else if (t == typeof(sbyte))
        {
            return (T)(object)Env->Functions->CallStaticByteMethodA(Env, cls.Handle, methodID, argsPtr);
        }
        else if (t == typeof(char))
        {
            return (T)(object)Env->Functions->CallStaticCharMethodA(Env, cls.Handle, methodID, argsPtr);
        }
        else if (t == typeof(short))
        {
            return (T)(object)Env->Functions->CallStaticShortMethodA(Env, cls.Handle, methodID, argsPtr);
        }
        else if (t == typeof(int))
        {
            return (T)(object)Env->Functions->CallStaticIntMethodA(Env, cls.Handle, methodID, argsPtr);
        }
        else if (t == typeof(long))
        {
            return (T)(object)Env->Functions->CallStaticLongMethodA(Env, cls.Handle, methodID, argsPtr);
        }
        else if (t == typeof(float))
        {
            return (T)(object)Env->Functions->CallStaticFloatMethodA(Env, cls.Handle, methodID, argsPtr);
        }
        else if (t == typeof(double))
        {
            return (T)(object)Env->Functions->CallStaticDoubleMethodA(Env, cls.Handle, methodID, argsPtr);
        }
        else
        {
            throw new ArgumentException($"CallStaticMethod Type {t} not supported.");
        }
    }

    public static void CallStaticVoidMethod(JClass cls, JMethodID methodID, params JValue[] args)
    {
        unsafe
        {
            IntPtr argsPtr = Marshal.UnsafeAddrOfPinnedArrayElement(args, 0);
            Env->Functions->CallStaticVoidMethodA(Env, cls.Handle, methodID, argsPtr);
        }
    }

    public static JFieldID GetStaticFieldID(JClass cls, string name, string sig)
    {
        unsafe
        {
            IntPtr nameAnsi = Marshal.StringToHGlobalAnsi(name);
            IntPtr sigAnsi = Marshal.StringToHGlobalAnsi(sig);

            JFieldID id = Env->Functions->GetStaticFieldID(Env, cls.Handle, nameAnsi, sigAnsi);

            Marshal.FreeHGlobal(nameAnsi);
            Marshal.FreeHGlobal(sigAnsi);
            return id;
        }
    }

    public static T GetStaticObjectField<T>(JClass cls, JFieldID fieldID) where T : JObject, new()
    {
        unsafe
        {
            IntPtr res = Env->Functions->GetStaticObjectField(Env, cls.Handle, fieldID);

            using JObject local = new() { Handle = res, ReferenceType = JNI.ReferenceType.Local };
            return NewGlobalRef<T>(local);
        }
    }

    public static T GetStaticField<T>(JClass cls, JFieldID fieldID)
    {
        unsafe
        {
            Type t = typeof(T);

            if (t == typeof(bool))
            {
                return (T)(object)Convert.ToBoolean(Env->Functions->GetStaticBooleanField(Env, cls.Handle, fieldID));
            }
            else if (t == typeof(sbyte))
            {
                return (T)(object)Env->Functions->GetStaticByteField(Env, cls.Handle, fieldID);
            }
            else if (t == typeof(char))
            {
                return (T)(object)Env->Functions->GetStaticCharField(Env, cls.Handle, fieldID);
            }
            else if (t == typeof(short))
            {
                return (T)(object)Env->Functions->GetStaticShortField(Env, cls.Handle, fieldID);
            }
            else if (t == typeof(int))
            {
                return (T)(object)Env->Functions->GetStaticIntField(Env, cls.Handle, fieldID);
            }
            else if (t == typeof(long))
            {
                return (T)(object)Env->Functions->GetStaticLongField(Env, cls.Handle, fieldID);
            }
            else if (t == typeof(float))
            {
                return (T)(object)Env->Functions->GetStaticFloatField(Env, cls.Handle, fieldID);
            }
            else if (t == typeof(double))
            {
                return (T)(object)Env->Functions->GetStaticDoubleField(Env, cls.Handle, fieldID);
            }
            else
            {
                throw new ArgumentException($"GetStaticField Type {t} not supported.");
            }
        }
    }

    public static void SetStaticObjectField<T>(JClass cls, JFieldID fieldID, T value) where T : JObject, new()
    {
        unsafe
        {
            Env->Functions->SetStaticObjectField(Env, cls.Handle, fieldID, value.Handle);
        }
    }

    public static void SetStaticField<T>(JClass cls, JFieldID fieldID, T value)
    {
        unsafe
        {
            switch (value)
            {
                case bool b:
                    Env->Functions->SetStaticBooleanField(Env, cls.Handle, fieldID, Convert.ToByte(b));
                    break;

                case sbyte b:
                    Env->Functions->SetStaticByteField(Env, cls.Handle, fieldID, b);
                    break;

                case char c:
                    Env->Functions->SetStaticCharField(Env, cls.Handle, fieldID, c);
                    break;

                case short s:
                    Env->Functions->SetStaticShortField(Env, cls.Handle, fieldID, s);
                    break;

                case int i:
                    Env->Functions->SetStaticIntField(Env, cls.Handle, fieldID, i);
                    break;

                case float f:
                    Env->Functions->SetStaticFloatField(Env, cls.Handle, fieldID, f);
                    break;

                case double d:
                    Env->Functions->SetStaticDoubleField(Env, cls.Handle, fieldID, d);
                    break;

                default:
                    throw new ArgumentException($"SetField Type {value.GetType()} not supported.");
            }
        }
    }

    public static JString NewString(string str)
    {
        unsafe
        {
            IntPtr strUni = Marshal.StringToHGlobalUni(str);

            IntPtr res = Env->Functions->NewString(Env, strUni, str.Length);

            Marshal.FreeHGlobal(strUni);

            using JObject local = new() { Handle = res, ReferenceType = JNI.ReferenceType.Local };
            return NewGlobalRef<JString>(local);
        }
    }

    public static int GetStringLength(JString str)
    {
        unsafe
        {
            return Env->Functions->GetStringLength(Env, str.Handle);
        }
    }

    public static string GetJStringString(JString str)
    {
        unsafe
        {
            if (!str.Valid())
                return "";

            IntPtr res = Env->Functions->GetStringUTFChars(Env, str.Handle, out byte isCopy);
            string? resultString = Marshal.PtrToStringAuto(res);
            Env->Functions->ReleaseStringChars(Env, str.Handle, res);
            return resultString ?? "";
        }
    }

    private static void ReleaseStringChars(JString str, IntPtr chars)
    {
        Env->Functions->ReleaseStringChars(Env, str.Handle, chars);
    }

    public static int GetArrayLength<T>(JArray<T> jarray)
    {
        unsafe
        {
            return Env->Functions->GetArrayLength(Env, jarray.Handle);
        }
    }

    public static int GetArrayLength<T>(JObjectArray<T> jarray) where T : JObject, new()
    {
        unsafe
        {
            return Env->Functions->GetArrayLength(Env, jarray.Handle);
        }
    }

    public static T GetObjectArrayElement<T>(JObjectArray<T> array, int index) where T : JObject, new()
    {
        unsafe
        {
            IntPtr res = Env->Functions->GetObjectArrayElement(Env, array.Handle, index);

            using JObject local = new() { Handle = res, ReferenceType = JNI.ReferenceType.Local };
            return NewGlobalRef<T>(local);
        }
    }

    public static void SetObjectArrayElement<T>(JObjectArray<T> array, int index, T value) where T : JObject, new()
    {
        unsafe
        {
            Env->Functions->SetObjectArrayElement(Env, array.Handle, index, value.Handle);
        }
    }

    public static JArray<T> NewArray<T>(int length)
    {
        unsafe
        {
            Type t = typeof(T);
            IntPtr res;

            if (t == typeof(bool))
            {
                res = Env->Functions->NewBooleanArray(Env, length);
            }
            else if (t == typeof(sbyte))
            {
                res = Env->Functions->NewByteArray(Env, length);
            }
            else if (t == typeof(char))
            {
                res = Env->Functions->NewCharArray(Env, length);
            }
            else if (t == typeof(short))
            {
                res = Env->Functions->NewShortArray(Env, length);
            }
            else if (t == typeof(int))
            {
                res = Env->Functions->NewIntArray(Env, length);
            }
            else if (t == typeof(long))
            {
                res = Env->Functions->NewBooleanArray(Env, length);
            }
            else if (t == typeof(float))
            {
                res = Env->Functions->NewBooleanArray(Env, length);
            }
            else if (t == typeof(double))
            {
                res = Env->Functions->NewBooleanArray(Env, length);
            }
            else
            {
                throw new ArgumentException($"CallStaticMethod Type {t} not supported.");
            }

            using JObject local = new() { Handle = res, ReferenceType = JNI.ReferenceType.Local };
            return NewGlobalRef<JArray<T>>(local);
        }
    }

    public static T[] GetArrayElements<T>(JArray<T> array)
    {
        unsafe
        {
            Type t = typeof(T);
            int length = GetArrayLength(array);

            if (t == typeof(bool))
            {
                byte* arr = Env->Functions->GetBooleanArrayElements(Env, array.Handle, out byte isCopy);

                bool[] buf = new bool[length];

                for (int i = 0; i < length; i++)
                    buf[i] = Convert.ToBoolean(arr[i]);

                Env->Functions->ReleaseBooleanArrayElements(Env, array.Handle, arr, (int)JNI.ReleaseMode.Abort);
                return (T[])(object)buf;
            }
            else if (t == typeof(sbyte))
            {
                sbyte* arr = Env->Functions->GetByteArrayElements(Env, array.Handle, out byte isCopy);

                sbyte[] buf = new sbyte[length];

                for (int i = 0; i < length; i++)
                    buf[i] = arr[i];

                Env->Functions->ReleaseByteArrayElements(Env, array.Handle, arr, (int)JNI.ReleaseMode.Abort);
                return (T[])(object)buf;
            }
            else if (t == typeof(char))
            {
                char* arr = Env->Functions->GetCharArrayElements(Env, array.Handle, out byte isCopy);

                char[] buf = new char[length];

                for (int i = 0; i < length; i++)
                    buf[i] = arr[i];

                Env->Functions->ReleaseCharArrayElements(Env, array.Handle, arr, (int)JNI.ReleaseMode.Abort);
                return (T[])(object)buf;
            }
            else if (t == typeof(short))
            {
                short* arr = Env->Functions->GetShortArrayElements(Env, array.Handle, out byte isCopy);

                short[] buf = new short[length];

                for (int i = 0; i < length; i++)
                    buf[i] = arr[i];

                Env->Functions->ReleaseShortArrayElements(Env, array.Handle, arr, (int)JNI.ReleaseMode.Abort);
                return (T[])(object)buf;
            }
            else if (t == typeof(int))
            {
                int* arr = Env->Functions->GetIntArrayElements(Env, array.Handle, out byte isCopy);

                int[] buf = new int[length];

                for (int i = 0; i < length; i++)
                    buf[i] = arr[i];

                Env->Functions->ReleaseIntArrayElements(Env, array.Handle, arr, (int)JNI.ReleaseMode.Abort);
                return (T[])(object)buf;
            }
            else if (t == typeof(long))
            {
                long* arr = Env->Functions->GetLongArrayElements(Env, array.Handle, out byte isCopy);

                long[] buf = new long[length];

                for (int i = 0; i < length; i++)
                    buf[i] = arr[i];

                Env->Functions->ReleaseLongArrayElements(Env, array.Handle, arr, (int)JNI.ReleaseMode.Abort);
                return (T[])(object)buf;
            }
            else if (t == typeof(float))
            {
                float* arr = Env->Functions->GetFloatArrayElements(Env, array.Handle, out byte isCopy);

                float[] buf = new float[length];

                for (int i = 0; i < length; i++)
                    buf[i] = arr[i];

                Env->Functions->ReleaseFloatArrayElements(Env, array.Handle, arr, (int)JNI.ReleaseMode.Abort);
                return (T[])(object)buf;
            }
            else if (t == typeof(double))
            {
                double* arr = Env->Functions->GetDoubleArrayElements(Env, array.Handle, out byte isCopy);

                double[] buf = new double[length];

                for (int i = 0; i < length; i++)
                    buf[i] = arr[i];

                Env->Functions->ReleaseDoubleArrayElements(Env, array.Handle, arr, (int)JNI.ReleaseMode.Abort);
                return (T[])(object)buf;
            }
            else
            {
                throw new ArgumentException($"GetArrayElements Type {t} not supported.");
            }
        }
    }

    public static T[] GetArrayRegion<T>(JArray<T> array, int start, int len)
    {
        unsafe
        {
            Type t = typeof(T);

            if (t == typeof(bool))
            {
                fixed (byte* buf = new byte[len])
                {
                    Env->Functions->GetBooleanArrayRegion(Env, array.Handle, start, len, buf);

                    bool[] res = new bool[len];
                    for (int i = 0; i < len; i++)
                    {
                        res[i] = Convert.ToBoolean(buf[i]);
                    }
                    return (T[])(object)res;
                }
            }
            else if (t == typeof(sbyte))
            {
                sbyte[] buf = new sbyte[len];

                fixed (sbyte* b = buf)
                {
                    Env->Functions->GetByteArrayRegion(Env, array.Handle, start, len, b);
                    return (T[])(object)buf;
                }
            }
            else if (t == typeof(char))
            {
                char[] buf = new char[len];

                fixed (char* b = buf)
                {
                    Env->Functions->GetCharArrayRegion(Env, array.Handle, start, len, b);
                    return (T[])(object)buf;
                }
            }
            else if (t == typeof(short))
            {
                short[] buf = new short[len];

                fixed (short* b = buf)
                {
                    Env->Functions->GetShortArrayRegion(Env, array.Handle, start, len, b);
                    return (T[])(object)buf;
                }
            }
            else if (t == typeof(int))
            {
                int[] buf = new int[len];

                fixed (int* b = buf)
                {
                    Env->Functions->GetIntArrayRegion(Env, array.Handle, start, len, b);
                    return (T[])(object)buf;
                }
            }
            else if (t == typeof(long))
            {
                long[] buf = new long[len];

                fixed (long* b = buf)
                {
                    Env->Functions->GetLongArrayRegion(Env, array.Handle, start, len, b);
                    return (T[])(object)buf;
                }
            }
            else if (t == typeof(float))
            {
                float[] buf = new float[len];

                fixed (float* b = buf)
                {
                    Env->Functions->GetFloatArrayRegion(Env, array.Handle, start, len, b);
                    return (T[])(object)buf;
                }
            }
            else if (t == typeof(double))
            {
                double[] buf = new double[len];

                fixed (double* b = buf)
                {
                    Env->Functions->GetDoubleArrayRegion(Env, array.Handle, start, len, b);
                    return (T[])(object)buf;
                }
            }
            else
            {
                throw new ArgumentException($"GetArrayRegion Type {t} not supported.");
            }
        }
    }

    public static T GetArrayElement<T>(JArray<T> array, int index)
    {
        Type t = typeof(T);

        if (t == typeof(bool))
        {
            byte b;
            Env->Functions->GetBooleanArrayRegion(Env, array.Handle, index, 1, &b);
            return (T)(object)Convert.ToBoolean(b);
        }
        else if (t == typeof(sbyte))
        {
            sbyte b;
            Env->Functions->GetByteArrayRegion(Env, array.Handle, index, 1, &b);
            return (T)(object)b;
        }
        else if (t == typeof(char))
        {
            char c;
            Env->Functions->GetCharArrayRegion(Env, array.Handle, index, 1, &c);
            return (T)(object)c;
        }
        else if (t == typeof(short))
        {
            short s;
            Env->Functions->GetShortArrayRegion(Env, array.Handle, index, 1, &s);
            return (T)(object)s;
        }
        else if (t == typeof(int))
        {
            int i;
            Env->Functions->GetIntArrayRegion(Env, array.Handle, index, 1, &i);
            return (T)(object)i;
        }
        else if (t == typeof(long))
        {
            long l;
            Env->Functions->GetLongArrayRegion(Env, array.Handle, index, 1, &l);
            return (T)(object)l;
        }
        else if (t == typeof(float))
        {
            float f;
            Env->Functions->GetFloatArrayRegion(Env, array.Handle, index, 1, &f);
            return (T)(object)f;
        }
        else if (t == typeof(double))
        {
            double d;
            Env->Functions->GetDoubleArrayRegion(Env, array.Handle, index, 1, &d);
            return (T)(object)d;
        }
        else
        {
            throw new ArgumentException($"GetArrayElement Type {t} not supported.");
        }
    }

    public static void SetArrayRegion<T>(JArray<T> array, int start, int len, T[] elems)
    {
        unsafe
        {
            Type t = typeof(T);

            if (t == typeof(bool))
            {
                fixed (byte* buf = elems.Select(b => Convert.ToByte(b)).ToArray())
                {
                    Env->Functions->SetBooleanArrayRegion(Env, array.Handle, start, len, null);
                }
            }
            else if (t == typeof(sbyte))
            {
                fixed (sbyte* buf = (sbyte[])(object)elems)
                {
                    Env->Functions->SetByteArrayRegion(Env, array.Handle, start, len, buf);
                }
            }
            else if (t == typeof(char))
            {
                fixed (char* buf = (char[])(object)elems)
                {
                    Env->Functions->SetCharArrayRegion(Env, array.Handle, start, len, buf);
                }
            }
            else if (t == typeof(short))
            {
                fixed (short* buf = (short[])(object)elems)
                {
                    Env->Functions->SetShortArrayRegion(Env, array.Handle, start, len, buf);
                }
            }
            else if (t == typeof(int))
            {
                fixed (int* buf = (int[])(object)elems)
                {
                    Env->Functions->SetIntArrayRegion(Env, array.Handle, start, len, buf);
                }
            }
            else if (t == typeof(long))
            {
                fixed (long* buf = (long[])(object)elems)
                {
                    Env->Functions->SetLongArrayRegion(Env, array.Handle, start, len, buf);
                }
            }
            else if (t == typeof(float))
            {
                fixed (float* buf = (float[])(object)elems)
                {
                    Env->Functions->SetFloatArrayRegion(Env, array.Handle, start, len, buf);
                }
            }
            else if (t == typeof(double))
            {
                fixed (double* buf = (double[])(object)elems)
                {
                    Env->Functions->SetDoubleArrayRegion(Env, array.Handle, start, len, buf);
                }
            }
            else
            {
                throw new ArgumentException($"SetArrayRegion Type {t} not supported.");
            }
        }
    }

    public static void SetArrayElement<T>(JArray<T> array, int index, T value)
    {
        Type t = typeof(T);

        if (t == typeof(bool))
        {
            byte b = Convert.ToByte(value);
            Env->Functions->SetBooleanArrayRegion(Env, array.Handle, index, 1, &b);
        }
        else if (t == typeof(sbyte))
        {
            sbyte b = (sbyte)(object)value;
            Env->Functions->SetByteArrayRegion(Env, array.Handle, index, 1, &b);
        }
        else if (t == typeof(char))
        {
            char c = (char)(object)value;
            Env->Functions->SetCharArrayRegion(Env, array.Handle, index, 1, &c);
        }
        else if (t == typeof(short))
        {
            short s = (short)(object)value;
            Env->Functions->SetShortArrayRegion(Env, array.Handle, index, 1, &s);
        }
        else if (t == typeof(int))
        {
            int c = (int)(object)value;
            Env->Functions->SetIntArrayRegion(Env, array.Handle, index, 1, &c);
        }
        else if (t == typeof(long))
        {
            long l = (long)(object)value;
            Env->Functions->SetLongArrayRegion(Env, array.Handle, index, 1, &l);
        }
        else if (t == typeof(float))
        {
            float f = (float)(object)value;
            Env->Functions->SetFloatArrayRegion(Env, array.Handle, index, 1, &f);
        }
        else if (t == typeof(double))
        {
            double d = (double)(object)value;
            Env->Functions->SetDoubleArrayRegion(Env, array.Handle, index, 1, &d);
        }
        else
        {
            throw new ArgumentException($"SetArrayElement Type {t} not supported.");
        }
    }

    private static int RegisterNatives(JClass cls, IntPtr methods, int nmethods)
    {
        throw new NotImplementedException();
    }

    private static int UnregisterNatives(JClass cls)
    {
        throw new NotImplementedException();
    }

    public static int MonitorEnter(JObject obj)
    {
        unsafe
        {
            return Env->Functions->MonitorEnter(Env, obj.Handle);
        }
    }

    public static int MonitorExit(JObject obj)
    {
        unsafe
        {
            return Env->Functions->MonitorExit(Env, obj.Handle);
        }
    }

    private static JavaVM* GetJavaVM()
    {
        throw new NotImplementedException();
    }

    public static string? GetStringRegion(JString str, int start, int len)
    {
        unsafe
        {
            //Probably need to allocate space?
            IntPtr buf = IntPtr.Zero;
            Env->Functions->GetStringRegion(Env, str.Handle, start, len, buf);

            if (buf != IntPtr.Zero)
            {
                return Marshal.PtrToStringUni(buf);
            }

            return null;
        }
    }

    private static IntPtr GetPrimitiveArrayCritical<T>(JArray<T> array)
    {
        throw new NotImplementedException();
    }

    private static void ReleasePrimitiveArrayCritical<T>(JArray<T> array, IntPtr carray, int mode)
    {
        throw new NotImplementedException();
    }

    private static string GetStringCritical(JString str)
    {
        throw new NotImplementedException();
    }

    private static void ReleaseStringCritical(JString str)
    {
        throw new NotImplementedException();
    }

    public static T NewWeakGlobalRef<T>(JObject obj) where T : JObject, new()
    {
        unsafe
        {
            IntPtr res = Env->Functions->NewWeakGlobalRef(Env, obj.Handle);
            return new T() { Handle = res, ReferenceType = JNI.ReferenceType.WeakGlobal };
        }
    }

    public static void DeleteWeakGlobalRef(JObject obj)
    {
        unsafe
        {
            Env->Functions->DeleteWeakGlobalRef(Env, obj.Handle);
        }
    }

    public static bool ExceptionCheck()
    {
        unsafe
        {
            return Convert.ToBoolean(Env->Functions->ExceptionCheck(Env));
        }
    }

    private static JObject NewDirectByteBuffer(IntPtr address, int capacity)
    {
        throw new NotImplementedException();
    }

    private static IntPtr GetDirectBufferAddress(JObject buf)
    {
        throw new NotImplementedException();
    }

    private static int GetDirectBufferCapacity(JObject obj)
    {
        throw new NotImplementedException();
    }
}
#endif

#pragma warning restore CS8600
#pragma warning restore CS8605
#pragma warning restore CS8632
#pragma warning restore CS8602