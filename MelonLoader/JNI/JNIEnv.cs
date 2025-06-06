#if ANDROID
namespace MelonLoader.Java;

using System;
using System.Runtime.InteropServices;

/// <summary>
/// Represents a JNIEnv C struct, the FunctionTable contains
/// all the JNI functions.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
internal readonly unsafe struct JNIEnv
{
    [StructLayout(LayoutKind.Sequential)]
    internal readonly unsafe struct FunctionTable
    {
        internal readonly IntPtr Reserved0;

        internal readonly IntPtr Reserved1;

        internal readonly IntPtr Reserved2;

        internal readonly IntPtr Reserved3;

        // jint (JNICALL *GetVersion)(JNIEnv *env);
        internal readonly delegate* unmanaged[Stdcall]<JNIEnv*, int> GetVersion;

        // jclass (JNICALL *DefineClass) (JNIEnv* env, const char* name, jobject loader, const jbyte* buf, jsize len);
        internal readonly delegate* unmanaged[Stdcall]<JNIEnv*, IntPtr, IntPtr, IntPtr, int, IntPtr> DefineClass;

        // jclass (JNICALL *FindClass) (JNIEnv* env, const char* name);
        internal readonly delegate* unmanaged[Stdcall]<JNIEnv*, IntPtr, IntPtr> FindClass;

        // jmethodID (JNICALL *FromReflectedMethod) (JNIEnv* env, jobject method);
        internal readonly delegate* unmanaged[Stdcall]<JNIEnv*, IntPtr, IntPtr> FromReflectedMethod;

        // jfieldID (JNICALL *FromReflectedField) (JNIEnv* env, jobject field);
        internal readonly delegate* unmanaged[Stdcall]<JNIEnv*, IntPtr, IntPtr> FromReflectedField;

        // jobject (JNICALL *ToReflectedMethod) (JNIEnv* env, jclass cls, jmethodID methodID, jboolean isStatic);
        internal readonly delegate* unmanaged[Stdcall]<JNIEnv*, IntPtr, IntPtr, byte, IntPtr> ToReflectedMethod;

        // jclass (JNICALL *GetSuperclass) (JNIEnv* env, jclass sub);
        internal readonly delegate* unmanaged[Stdcall]<JNIEnv*, IntPtr, IntPtr> GetSuperClass;

        // jboolean (JNICALL *IsAssignableFrom) (JNIEnv* env, jclass sub, jclass sup);
        internal readonly delegate* unmanaged[Stdcall]<JNIEnv*, IntPtr, IntPtr, byte> IsAssignableFrom;

        // jobject (JNICALL *ToReflectedField) (JNIEnv* env, jclass cls, jfieldID fieldID, jboolean isStatic);
        internal readonly delegate* unmanaged[Stdcall]<JNIEnv*, IntPtr, IntPtr, byte, IntPtr> ToReflectedField;

        // jint (JNICALL *Throw) (JNIEnv* env, jthrowable obj);
        internal readonly delegate* unmanaged[Stdcall]<JNIEnv*, IntPtr, int> Throw;

        // jint (JNICALL *ThrowNew) (JNIEnv* env, jclass clazz, const char* msg);
        internal readonly delegate* unmanaged[Stdcall]<JNIEnv*, IntPtr, IntPtr, int> ThrowNew;

        // jthrowable (JNICALL *ExceptionOccurred) (JNIEnv* env);
        internal readonly delegate* unmanaged[Stdcall]<JNIEnv*, IntPtr> ExceptionOccurred;

        // void (JNICALL *ExceptionDescribe) (JNIEnv* env);
        internal readonly delegate* unmanaged[Stdcall]<JNIEnv*, void> ExceptionDescribe;

        // void (JNICALL *ExceptionClear) (JNIEnv* env);
        internal readonly delegate* unmanaged[Stdcall]<JNIEnv*, void> ExceptionClear;

        // void (JNICALL *FatalError) (JNIEnv* env, const char* msg);
        internal readonly delegate* unmanaged[Stdcall]<JNIEnv*, IntPtr, void> FatalError;

        // jint (JNICALL *PushLocalFrame) (JNIEnv* env, jint capacity);
        internal readonly delegate* unmanaged[Stdcall]<JNIEnv*, int, int> PushLocalFrame;

        // jobject (JNICALL *PopLocalFrame) (JNIEnv* env, jobject result);
        internal readonly delegate* unmanaged[Stdcall]<JNIEnv*, IntPtr, IntPtr> PopLocalFrame;

        // jobject (JNICALL *NewGlobalRef) (JNIEnv* env, jobject lobj);
        internal readonly delegate* unmanaged[Stdcall]<JNIEnv*, IntPtr, IntPtr> NewGlobalRef;

        // void (JNICALL *DeleteGlobalRef) (JNIEnv* env, jobject gref);
        internal readonly delegate* unmanaged[Stdcall]<JNIEnv*, IntPtr, void> DeleteGlobalRef;

        // void (JNICALL *DeleteLocalRef) (JNIEnv* env, jobject obj);
        internal readonly delegate* unmanaged[Stdcall]<JNIEnv*, IntPtr, void> DeleteLocalRef;

        // jboolean (JNICALL *IsSameObject) (JNIEnv* env, jobject obj1, jobject obj2);
        internal readonly delegate* unmanaged[Stdcall]<JNIEnv*, IntPtr, IntPtr, byte> IsSameObject;

        // jobject (JNICALL *NewLocalRef) (JNIEnv* env, jobject ref);
        internal readonly delegate* unmanaged[Stdcall]<JNIEnv*, IntPtr, IntPtr> NewLocalRef;

        // jint (JNICALL *EnsureLocalCapacity) (JNIEnv* env, jint capacity);
        internal readonly delegate* unmanaged[Stdcall]<JNIEnv*, int, int> EnsureLocalCapacity;

        // jobject (JNICALL *AllocObject) (JNIEnv* env, jclass clazz);
        internal readonly delegate* unmanaged[Stdcall]<JNIEnv*, IntPtr, IntPtr> AllocObject;

        // jobject (JNICALL *NewObject) (JNIEnv* env, jclass clazz, jmethodID methodID, ...);
        internal readonly IntPtr NewObject;

        // jobject (JNICALL *NewObjectV) (JNIEnv* env, jclass clazz, jmethodID methodID, va_list args);
        internal readonly IntPtr NewObjectV;

        // jobject (JNICALL *NewObjectA) (JNIEnv* env, jclass clazz, jmethodID methodID, const jvalue* args);
        internal readonly delegate* unmanaged[Stdcall]<JNIEnv*, IntPtr, IntPtr, IntPtr, IntPtr> NewObjectA;

        // jclass (JNICALL *GetObjectClass) (JNIEnv* env, jobject obj);
        internal readonly delegate* unmanaged[Stdcall]<JNIEnv*, IntPtr, IntPtr> GetObjectClass;

        // jboolean (JNICALL *IsInstanceOf) (JNIEnv* env, jobject obj, jclass clazz);
        internal readonly delegate* unmanaged[Stdcall]<JNIEnv*, IntPtr, IntPtr, byte> IsInstanceOf;

        // jmethodID (JNICALL *GetMethodID) (JNIEnv* env, jclass clazz, const char* name, const char* sig);
        internal readonly delegate* unmanaged[Stdcall]<JNIEnv*, IntPtr, IntPtr, IntPtr, IntPtr> GetMethodID;

        // jobject (JNICALL *CallObjectMethod) (JNIEnv* env, jobject obj, jmethodID methodID, ...);
        internal readonly IntPtr CallObjectMethod;

        // jobject (JNICALL *CallObjectMethodV) (JNIEnv* env, jobject obj, jmethodID methodID, va_list args);
        internal readonly IntPtr CallObjectMethodV;

        // jobject (JNICALL *CallObjectMethodA) (JNIEnv* env, jobject obj, jmethodID methodID, const jvalue* args);
        internal readonly delegate* unmanaged[Stdcall]<JNIEnv*, IntPtr, IntPtr, IntPtr, IntPtr> CallObjectMethodA;

        // jboolean (JNICALL *CallBooleanMethod) (JNIEnv* env, jobject obj, jmethodID methodID, ...);
        internal readonly IntPtr CallBooleanMethod;

        // jboolean (JNICALL *CallBooleanMethodV) (JNIEnv* env, jobject obj, jmethodID methodID, va_list args);
        internal readonly IntPtr CallBooleanMethodV;

        // jboolean (JNICALL *CallBooleanMethodA) (JNIEnv* env, jobject obj, jmethodID methodID, const jvalue* args);
        internal readonly delegate* unmanaged[Stdcall]<JNIEnv*, IntPtr, IntPtr, IntPtr, byte> CallBooleanMethodA;

        // jbyte (JNICALL *CallByteMethod) (JNIEnv* env, jobject obj, jmethodID methodID, ...);
        internal readonly IntPtr CallByteMethod;

        // jbyte (JNICALL *CallByteMethodV) (JNIEnv* env, jobject obj, jmethodID methodID, va_list args);
        internal readonly IntPtr CallByteMethodV;

        // jbyte (JNICALL *CallByteMethodA) (JNIEnv* env, jobject obj, jmethodID methodID, const jvalue* args);
        internal readonly delegate* unmanaged[Stdcall]<JNIEnv*, IntPtr, IntPtr, IntPtr, sbyte> CallByteMethodA;

        // jchar (JNICALL *CallCharMethod) (JNIEnv* env, jobject obj, jmethodID methodID, ...);
        internal readonly IntPtr CallCharMethod;

        // jchar (JNICALL *CallCharMethodV) (JNIEnv* env, jobject obj, jmethodID methodID, va_list args);
        internal readonly IntPtr CallCharMethodV;

        // jchar (JNICALL *CallCharMethodA) (JNIEnv* env, jobject obj, jmethodID methodID, const jvalue* args);
        internal readonly delegate* unmanaged[Stdcall]<JNIEnv*, IntPtr, IntPtr, IntPtr, char> CallCharMethodA;

        // jshort (JNICALL *CallShortMethod) (JNIEnv* env, jobject obj, jmethodID methodID, ...);
        internal readonly IntPtr CallShortMethod;

        // jshort (JNICALL *CallShortMethodV) (JNIEnv* env, jobject obj, jmethodID methodID, va_list args);
        internal readonly IntPtr CallShortMethodV;

        // jshort (JNICALL *CallShortMethodA) (JNIEnv* env, jobject obj, jmethodID methodID, const jvalue* args);
        internal readonly delegate* unmanaged[Stdcall]<JNIEnv*, IntPtr, IntPtr, IntPtr, short> CallShortMethodA;

        // jint (JNICALL *CallIntMethod) (JNIEnv* env, jobject obj, jmethodID methodID, ...);
        internal readonly IntPtr CallIntMethod;

        // jshort (JNICALL *CallShortMethodV) (JNIEnv* env, jobject obj, jmethodID methodID, va_list args);
        internal readonly IntPtr CallIntMethodV;

        // jint (JNICALL *CallIntMethodA) (JNIEnv* env, jobject obj, jmethodID methodID, const jvalue* args);
        internal readonly delegate* unmanaged[Stdcall]<JNIEnv*, IntPtr, IntPtr, IntPtr, int> CallIntMethodA;

        // jlong (JNICALL *CallLongMethod) (JNIEnv* env, jobject obj, jmethodID methodID, ...);
        internal readonly IntPtr CallLongMethod;

        // jlong (JNICALL *CallLongMethodV) (JNIEnv* env, jobject obj, jmethodID methodID, va_list args);
        internal readonly IntPtr CallLongMethodV;

        // jlong (JNICALL *CallLongMethodA) (JNIEnv* env, jobject obj, jmethodID methodID, const jvalue* args);
        internal readonly delegate* unmanaged[Stdcall]<JNIEnv*, IntPtr, IntPtr, IntPtr, long> CallLongMethodA;

        // jfloat (JNICALL *CallFloatMethod) (JNIEnv* env, jobject obj, jmethodID methodID, ...);
        internal readonly IntPtr CallFloatMethod;

        // jfloat (JNICALL *CallFloatMethodV) (JNIEnv* env, jobject obj, jmethodID methodID, va_list args);
        internal readonly IntPtr CallFloatMethodV;

        // jfloat (JNICALL *CallFloatMethodA) (JNIEnv* env, jobject obj, jmethodID methodID, const jvalue* args);
        internal readonly delegate* unmanaged[Stdcall]<JNIEnv*, IntPtr, IntPtr, IntPtr, float> CallFloatMethodA;

        // jdouble (JNICALL *CallDoubleMethod) (JNIEnv* env, jobject obj, jmethodID methodID, ...);
        internal readonly IntPtr CallDoubleMethod;

        // jdouble (JNICALL *CallDoubleMethodV) (JNIEnv* env, jobject obj, jmethodID methodID, va_list args);
        internal readonly IntPtr CallDoubleMethodV;

        // jdouble (JNICALL *CallDoubleMethodA) (JNIEnv* env, jobject obj, jmethodID methodID, const jvalue* args);
        internal readonly delegate* unmanaged[Stdcall]<JNIEnv*, IntPtr, IntPtr, IntPtr, double> CallDoubleMethodA;

        // void (JNICALL *CallVoidMethod) (JNIEnv* env, jobject obj, jmethodID methodID, ...);
        internal readonly IntPtr CallVoidMethod;

        // void (JNICALL *CallVoidMethodV) (JNIEnv* env, jobject obj, jmethodID methodID, va_list args);
        internal readonly IntPtr CallVoidMethodV;

        // void (JNICALL *CallVoidMethodA) (JNIEnv* env, jobject obj, jmethodID methodID, const jvalue* args);
        internal readonly delegate* unmanaged[Stdcall]<JNIEnv*, IntPtr, IntPtr, IntPtr, void> CallVoidMethodA;

        // jobject (JNICALL *CallNonvirtualObjectMethod) (JNIEnv* env, jobject obj, jclass clazz, jmethodID methodID, ...);
        internal readonly IntPtr CallNonvirtualObjectMethod;

        // jobject (JNICALL *CallNonvirtualObjectMethodV) (JNIEnv* env, jobject obj, jclass clazz, jmethodID methodID, va_list args);
        internal readonly IntPtr CallNonvirtualObjectMethodV;

        // jobject (JNICALL *CallNonvirtualObjectMethodA) (JNIEnv* env, jobject obj, jclass clazz, jmethodID methodID, const jvalue* args);
        internal readonly delegate* unmanaged[Stdcall]<JNIEnv*, IntPtr, IntPtr, IntPtr, IntPtr, IntPtr> CallNonvirtualObjectMethodA;

        // jboolean (JNICALL *CallNonvirtualBooleanMethod) (JNIEnv* env, jobject obj, jclass clazz, jmethodID methodID, ...);
        internal readonly IntPtr CallNonvirtualBooleanMethod;

        // jboolean (JNICALL *CallNonvirtualBooleanMethodV) (JNIEnv* env, jobject obj, jclass clazz, jmethodID methodID, va_list args);
        internal readonly IntPtr CallNonvirtualBooleanMethodV;

        // jboolean (JNICALL *CallNonvirtualBooleanMethodA) (JNIEnv* env, jobject obj, jclass clazz, jmethodID methodID, const jvalue* args);
        internal readonly delegate* unmanaged[Stdcall]<JNIEnv*, IntPtr, IntPtr, IntPtr, IntPtr, byte> CallNonvirtualBooleanMethodA;

        // jbyte (JNICALL *CallNonvirtualByteMethod) (JNIEnv* env, jobject obj, jclass clazz, jmethodID methodID, ...);
        internal readonly IntPtr CallNonvirtualByteMethod;

        // jbyte (JNICALL *CallNonvirtualByteMethodV) (JNIEnv* env, jobject obj, jclass clazz, jmethodID methodID, va_list args);
        internal readonly IntPtr CallNonvirtualByteMethodV;

        // jbyte (JNICALL *CallNonvirtualByteMethodA) (JNIEnv* env, jobject obj, jclass clazz, jmethodID methodID, const jvalue* args);
        internal readonly delegate* unmanaged[Stdcall]<JNIEnv*, IntPtr, IntPtr, IntPtr, IntPtr, sbyte> CallNonvirtualByteMethodA;

        // jchar (JNICALL *CallNonvirtualCharMethod) (JNIEnv* env, jobject obj, jclass clazz, jmethodID methodID, ...);
        internal readonly IntPtr CallNonvirtualCharMethod;

        // jchar (JNICALL *CallNonvirtualCharMethodV) (JNIEnv* env, jobject obj, jclass clazz, jmethodID methodID, va_list args);
        internal readonly IntPtr CallNonvirtualCharMethodV;

        // jchar (JNICALL *CallNonvirtualCharMethodA) (JNIEnv* env, jobject obj, jclass clazz, jmethodID methodID, const jvalue* args);
        internal readonly delegate* unmanaged[Stdcall]<JNIEnv*, IntPtr, IntPtr, IntPtr, IntPtr, char> CallNonvirtualCharMethodA;

        // jshort (JNICALL *CallNonvirtualShortMethod) (JNIEnv* env, jobject obj, jclass clazz, jmethodID methodID, ...);
        internal readonly IntPtr CallNonvirtualShortMethod;

        // jshort (JNICALL *CallNonvirtualShortMethodV) (JNIEnv* env, jobject obj, jclass clazz, jmethodID methodID, va_list args);
        internal readonly IntPtr CallNonvirtualShortMethodV;

        // jshort (JNICALL *CallNonvirtualShortMethodA) (JNIEnv* env, jobject obj, jclass clazz, jmethodID methodID, const jvalue* args);
        internal readonly delegate* unmanaged[Stdcall]<JNIEnv*, IntPtr, IntPtr, IntPtr, IntPtr, short> CallNonvirtualShortMethodA;

        // jint (JNICALL *CallNonvirtualIntMethod) (JNIEnv* env, jobject obj, jclass clazz, jmethodID methodID, ...);
        internal readonly IntPtr CallNonvirtualIntMethod;

        // jint (JNICALL *CallNonvirtualIntMethodV) (JNIEnv* env, jobject obj, jclass clazz, jmethodID methodID, va_list args);
        internal readonly IntPtr CallNonvirtualIntMethodV;

        // jint (JNICALL *CallNonvirtualIntMethodA) (JNIEnv* env, jobject obj, jclass clazz, jmethodID methodID, const jvalue* args);
        internal readonly delegate* unmanaged[Stdcall]<JNIEnv*, IntPtr, IntPtr, IntPtr, IntPtr, int> CallNonvirtualIntMethodA;

        // jlong (JNICALL *CallNonvirtualLongMethod) (JNIEnv* env, jobject obj, jclass clazz, jmethodID methodID, ...);
        internal readonly IntPtr CallNonvirtualLongMethod;

        // jlong (JNICALL *CallNonvirtualLongMethodV)(JNIEnv* env, jobject obj, jclass clazz, jmethodID methodID, va_list args);
        internal readonly IntPtr CallNonvirtualLongMethodV;

        // jlong (JNICALL *CallNonvirtualLongMethodA) (JNIEnv* env, jobject obj, jclass clazz, jmethodID methodID, const jvalue* args);
        internal readonly delegate* unmanaged[Stdcall]<JNIEnv*, IntPtr, IntPtr, IntPtr, IntPtr, long> CallNonvirtualLongMethodA;

        // jfloat (JNICALL *CallNonvirtualFloatMethod) (JNIEnv* env, jobject obj, jclass clazz, jmethodID methodID, ...);
        internal readonly IntPtr CallNonvirtualFloatMethod;

        // jfloat (JNICALL *CallNonvirtualFloatMethodV) (JNIEnv* env, jobject obj, jclass clazz, jmethodID methodID, va_list args);
        internal readonly IntPtr CallNonvirtualFloatMethodV;

        // jfloat (JNICALL *CallNonvirtualFloatMethodA) (JNIEnv* env, jobject obj, jclass clazz, jmethodID methodID, const jvalue* args);
        internal readonly delegate* unmanaged[Stdcall]<JNIEnv*, IntPtr, IntPtr, IntPtr, IntPtr, float> CallNonvirtualFloatMethodA;

        // jdouble (JNICALL *CallNonvirtualDoubleMethod) (JNIEnv* env, jobject obj, jclass clazz, jmethodID methodID, ...);
        internal readonly IntPtr CallNonvirtualDoubleMethod;

        // jdouble (JNICALL *CallNonvirtualDoubleMethodV) (JNIEnv* env, jobject obj, jclass clazz, jmethodID methodID, va_list args);
        internal readonly IntPtr CallNonvirtualDoubleMethodV;

        // jdouble (JNICALL *CallNonvirtualDoubleMethodA) (JNIEnv* env, jobject obj, jclass clazz, jmethodID methodID, const jvalue* args);
        internal readonly delegate* unmanaged[Stdcall]<JNIEnv*, IntPtr, IntPtr, IntPtr, IntPtr, double> CallNonvirtualDoubleMethodA;

        // void (JNICALL *CallNonvirtualVoidMethod) (JNIEnv* env, jobject obj, jclass clazz, jmethodID methodID, ...);
        internal readonly IntPtr CallNonvirtualVoidMethod;

        // void (JNICALL *CallNonvirtualVoidMethodV) (JNIEnv* env, jobject obj, jclass clazz, jmethodID methodID, va_list args);
        internal readonly IntPtr CallNonvirtualVoidMethodV;

        // void (JNICALL *CallNonvirtualVoidMethodA) (JNIEnv* env, jobject obj, jclass clazz, jmethodID methodID, const jvalue* args);
        internal readonly delegate* unmanaged[Stdcall]<JNIEnv*, IntPtr, IntPtr, IntPtr, IntPtr, void> CallNonvirtualVoidMethodA;

        // jfieldID (JNICALL *GetFieldID) (JNIEnv* env, jclass clazz, const char* name, const char* sig);
        internal readonly delegate* unmanaged[Stdcall]<JNIEnv*, IntPtr, IntPtr, IntPtr, IntPtr> GetFieldID;

        // jobject (JNICALL *GetObjectField) (JNIEnv* env, jobject obj, jfieldID fieldID);
        internal readonly delegate* unmanaged[Stdcall]<JNIEnv*, IntPtr, IntPtr, IntPtr> GetObjectField;

        // jboolean (JNICALL *GetBooleanField) (JNIEnv* env, jobject obj, jfieldID fieldID);
        internal readonly delegate* unmanaged[Stdcall]<JNIEnv*, IntPtr, IntPtr, byte> GetBooleanField;

        // jbyte (JNICALL *GetByteField) (JNIEnv* env, jobject obj, jfieldID fieldID);
        internal readonly delegate* unmanaged[Stdcall]<JNIEnv*, IntPtr, IntPtr, sbyte> GetByteField;

        // jchar (JNICALL *GetCharField) (JNIEnv* env, jobject obj, jfieldID fieldID);
        internal readonly delegate* unmanaged[Stdcall]<JNIEnv*, IntPtr, IntPtr, char> GetCharField;

        // jshort (JNICALL *GetShortField) (JNIEnv* env, jobject obj, jfieldID fieldID);
        internal readonly delegate* unmanaged[Stdcall]<JNIEnv*, IntPtr, IntPtr, short> GetShortField;

        // jint (JNICALL *GetIntField) (JNIEnv* env, jobject obj, jfieldID fieldID);
        internal readonly delegate* unmanaged[Stdcall]<JNIEnv*, IntPtr, IntPtr, int> GetIntField;

        // jlong (JNICALL *GetLongField) (JNIEnv* env, jobject obj, jfieldID fieldID);
        internal readonly delegate* unmanaged[Stdcall]<JNIEnv*, IntPtr, IntPtr, long> GetLongField;

        // jfloat (JNICALL *GetFloatField) (JNIEnv* env, jobject obj, jfieldID fieldID);
        internal readonly delegate* unmanaged[Stdcall]<JNIEnv*, IntPtr, IntPtr, float> GetFloatField;

        // jdouble (JNICALL *GetDoubleField) (JNIEnv* env, jobject obj, jfieldID fieldID);
        internal readonly delegate* unmanaged[Stdcall]<JNIEnv*, IntPtr, IntPtr, double> GetDoubleField;

        // void (JNICALL *SetObjectField) (JNIEnv* env, jobject obj, jfieldID fieldID, jobject val);
        internal readonly delegate* unmanaged[Stdcall]<JNIEnv*, IntPtr, IntPtr, IntPtr, void> SetObjectField;

        // void (JNICALL *SetBooleanField) (JNIEnv* env, jobject obj, jfieldID fieldID, jboolean val);
        internal readonly delegate* unmanaged[Stdcall]<JNIEnv*, IntPtr, IntPtr, byte, void> SetBooleanField;

        // void (JNICALL *SetByteField) (JNIEnv* env, jobject obj, jfieldID fieldID, jbyte val);
        internal readonly delegate* unmanaged[Stdcall]<JNIEnv*, IntPtr, IntPtr, sbyte, void> SetByteField;

        // void (JNICALL *SetCharField) (JNIEnv* env, jobject obj, jfieldID fieldID, jchar val);
        internal readonly delegate* unmanaged[Stdcall]<JNIEnv*, IntPtr, IntPtr, char, void> SetCharField;

        // void (JNICALL *SetShortField) (JNIEnv* env, jobject obj, jfieldID fieldID, jshort val);
        internal readonly delegate* unmanaged[Stdcall]<JNIEnv*, IntPtr, IntPtr, short, void> SetShortField;

        // void (JNICALL *SetIntField) (JNIEnv* env, jobject obj, jfieldID fieldID, jint val);
        internal readonly delegate* unmanaged[Stdcall]<JNIEnv*, IntPtr, IntPtr, int, void> SetIntField;

        // void (JNICALL *SetLongField) (JNIEnv* env, jobject obj, jfieldID fieldID, jlong val);
        internal readonly delegate* unmanaged[Stdcall]<JNIEnv*, IntPtr, IntPtr, long, void> SetLongField;

        // void (JNICALL *SetFloatField) (JNIEnv* env, jobject obj, jfieldID fieldID, jfloat val);
        internal readonly delegate* unmanaged[Stdcall]<JNIEnv*, IntPtr, IntPtr, float, void> SetFloatField;

        // void (JNICALL *SetDoubleField) (JNIEnv* env, jobject obj, jfieldID fieldID, jdouble val);
        internal readonly delegate* unmanaged[Stdcall]<JNIEnv*, IntPtr, IntPtr, double, void> SetDoubleField;

        // jmethodID (JNICALL *GetStaticMethodID) (JNIEnv* env, jclass clazz, const char* name, const char* sig);
        internal readonly delegate* unmanaged[Stdcall]<JNIEnv*, IntPtr, IntPtr, IntPtr, IntPtr> GetStaticMethodID;

        // jobject (JNICALL *CallStaticObjectMethod) (JNIEnv* env, jclass clazz, jmethodID methodID, ...);
        internal readonly IntPtr CallStaticObjectMethod;

        // jobject (JNICALL *CallStaticObjectMethodV) (JNIEnv* env, jclass clazz, jmethodID methodID, va_list args);
        internal readonly IntPtr CallStaticObjectMethodV;

        // jobject (JNICALL *CallStaticObjectMethodA) (JNIEnv* env, jclass clazz, jmethodID methodID, const jvalue* args);
        internal readonly delegate* unmanaged[Stdcall]<JNIEnv*, IntPtr, IntPtr, IntPtr, IntPtr> CallStaticObjectMethodA;

        // jboolean (JNICALL *CallStaticBooleanMethod) (JNIEnv* env, jclass clazz, jmethodID methodID, ...);
        internal readonly IntPtr CallStaticBooleanMethod;

        // jboolean (JNICALL *CallStaticBooleanMethodV) (JNIEnv* env, jclass clazz, jmethodID methodID, va_list args);
        internal readonly IntPtr CallStaticBooleanMethodV;

        // jboolean (JNICALL *CallStaticBooleanMethodA) (JNIEnv* env, jclass clazz, jmethodID methodID, const jvalue* args);
        internal readonly delegate* unmanaged[Stdcall]<JNIEnv*, IntPtr, IntPtr, IntPtr, byte> CallStaticBooleanMethodA;

        // jbyte (JNICALL *CallStaticByteMethod)(JNIEnv* env, jclass clazz, jmethodID methodID, ...);
        internal readonly IntPtr CallStaticByteMethod;

        // jbyte (JNICALL *CallStaticByteMethodV) (JNIEnv* env, jclass clazz, jmethodID methodID, va_list args);
        internal readonly IntPtr CallStaticByteMethodV;

        // jbyte (JNICALL *CallStaticByteMethodA) (JNIEnv* env, jclass clazz, jmethodID methodID, const jvalue* args);
        internal readonly delegate* unmanaged[Stdcall]<JNIEnv*, IntPtr, IntPtr, IntPtr, sbyte> CallStaticByteMethodA;

        // jchar (JNICALL *CallStaticCharMethod) (JNIEnv* env, jclass clazz, jmethodID methodID, ...);
        internal readonly IntPtr CallStaticCharMethod;

        // jchar (JNICALL *CallStaticCharMethodV) (JNIEnv* env, jclass clazz, jmethodID methodID, va_list args);
        internal readonly IntPtr CallStaticCharMethodV;

        // jchar (JNICALL *CallStaticCharMethodA) (JNIEnv* env, jclass clazz, jmethodID methodID, const jvalue* args);
        internal readonly delegate* unmanaged[Stdcall]<JNIEnv*, IntPtr, IntPtr, IntPtr, char> CallStaticCharMethodA;

        // jshort (JNICALL *CallStaticShortMethod) (JNIEnv* env, jclass clazz, jmethodID methodID, ...);
        internal readonly IntPtr CallStaticShortMethod;

        // jshort (JNICALL *CallStaticShortMethodV) (JNIEnv* env, jclass clazz, jmethodID methodID, va_list args);
        internal readonly IntPtr CallStaticShortMethodV;

        // jshort (JNICALL *CallStaticShortMethodA) (JNIEnv* env, jclass clazz, jmethodID methodID, const jvalue* args);
        internal readonly delegate* unmanaged[Stdcall]<JNIEnv*, IntPtr, IntPtr, IntPtr, short> CallStaticShortMethodA;

        // jint (JNICALL *CallStaticIntMethod) (JNIEnv* env, jclass clazz, jmethodID methodID, ...);
        internal readonly IntPtr CallStaticIntMethod;

        // jint (JNICALL *CallStaticIntMethodV) (JNIEnv* env, jclass clazz, jmethodID methodID, va_list args);
        internal readonly IntPtr CallStaticIntMethodV;

        // jint (JNICALL *CallStaticIntMethodA) (JNIEnv* env, jclass clazz, jmethodID methodID, const jvalue* args);
        internal readonly delegate* unmanaged[Stdcall]<JNIEnv*, IntPtr, IntPtr, IntPtr, int> CallStaticIntMethodA;

        // jlong (JNICALL *CallStaticLongMethod) (JNIEnv* env, jclass clazz, jmethodID methodID, ...);
        internal readonly IntPtr CallStaticLongMethod;

        // jlong (JNICALL *CallStaticLongMethodV) (JNIEnv* env, jclass clazz, jmethodID methodID, va_list args);
        internal readonly IntPtr CallStaticLongMethodV;

        // jlong (JNICALL *CallStaticLongMethodA) (JNIEnv* env, jclass clazz, jmethodID methodID, const jvalue* args);
        internal readonly delegate* unmanaged[Stdcall]<JNIEnv*, IntPtr, IntPtr, IntPtr, long> CallStaticLongMethodA;

        // jfloat (JNICALL *CallStaticFloatMethod) (JNIEnv* env, jclass clazz, jmethodID methodID, ...);
        internal readonly IntPtr CallStaticFloatMethod;

        // jfloat (JNICALL *CallStaticFloatMethodV) (JNIEnv* env, jclass clazz, jmethodID methodID, va_list args);
        internal readonly IntPtr CallStaticFloatMethodV;

        // jfloat (JNICALL *CallStaticFloatMethodA) (JNIEnv* env, jclass clazz, jmethodID methodID, const jvalue* args);
        internal readonly delegate* unmanaged[Stdcall]<JNIEnv*, IntPtr, IntPtr, IntPtr, float> CallStaticFloatMethodA;

        // jdouble (JNICALL *CallStaticDoubleMethod) (JNIEnv* env, jclass clazz, jmethodID methodID, ...);
        internal readonly IntPtr CallStaticDoubleMethod;

        // jdouble (JNICALL *CallStaticDoubleMethodV) (JNIEnv* env, jclass clazz, jmethodID methodID, va_list args);
        internal readonly IntPtr CallStaticDoubleMethodV;

        // jdouble (JNICALL *CallStaticDoubleMethodA) (JNIEnv* env, jclass clazz, jmethodID methodID, const jvalue* args);
        internal readonly delegate* unmanaged[Stdcall]<JNIEnv*, IntPtr, IntPtr, IntPtr, double> CallStaticDoubleMethodA;

        // void (JNICALL *CallStaticVoidMethod) (JNIEnv* env, jclass cls, jmethodID methodID, ...);
        internal readonly IntPtr CallStaticVoidMethod;

        // void (JNICALL *CallStaticVoidMethodV) (JNIEnv* env, jclass cls, jmethodID methodID, va_list args);
        internal readonly IntPtr CallStaticVoidMethodV;

        // void (JNICALL *CallStaticVoidMethodA) (JNIEnv* env, jclass cls, jmethodID methodID, const jvalue* args);
        internal readonly delegate* unmanaged[Stdcall]<JNIEnv*, IntPtr, IntPtr, IntPtr, void> CallStaticVoidMethodA;

        // jfieldID (JNICALL *GetStaticFieldID) (JNIEnv* env, jclass clazz, const char* name, const char* sig);
        internal readonly delegate* unmanaged[Stdcall]<JNIEnv*, IntPtr, IntPtr, IntPtr, IntPtr> GetStaticFieldID;

        // jobject (JNICALL *GetStaticObjectField) (JNIEnv* env, jclass clazz, jfieldID fieldID);
        internal readonly delegate* unmanaged[Stdcall]<JNIEnv*, IntPtr, IntPtr, IntPtr> GetStaticObjectField;

        // jboolean (JNICALL *GetStaticBooleanField) (JNIEnv* env, jclass clazz, jfieldID fieldID);
        internal readonly delegate* unmanaged[Stdcall]<JNIEnv*, IntPtr, IntPtr, byte> GetStaticBooleanField;

        // jbyte (JNICALL *GetStaticByteField) (JNIEnv* env, jclass clazz, jfieldID fieldID);
        internal readonly delegate* unmanaged[Stdcall]<JNIEnv*, IntPtr, IntPtr, sbyte> GetStaticByteField;

        // jchar (JNICALL *GetStaticCharField) (JNIEnv* env, jclass clazz, jfieldID fieldID);
        internal readonly delegate* unmanaged[Stdcall]<JNIEnv*, IntPtr, IntPtr, char> GetStaticCharField;

        // jshort (JNICALL *GetStaticShortField) (JNIEnv* env, jclass clazz, jfieldID fieldID);
        internal readonly delegate* unmanaged[Stdcall]<JNIEnv*, IntPtr, IntPtr, short> GetStaticShortField;

        // jint (JNICALL *GetStaticIntField) (JNIEnv* env, jclass clazz, jfieldID fieldID);
        internal readonly delegate* unmanaged[Stdcall]<JNIEnv*, IntPtr, IntPtr, int> GetStaticIntField;

        // jlong (JNICALL *GetStaticLongField) (JNIEnv* env, jclass clazz, jfieldID fieldID);
        internal readonly delegate* unmanaged[Stdcall]<JNIEnv*, IntPtr, IntPtr, long> GetStaticLongField;

        // jfloat (JNICALL *GetStaticFloatField) (JNIEnv* env, jclass clazz, jfieldID fieldID);
        internal readonly delegate* unmanaged[Stdcall]<JNIEnv*, IntPtr, IntPtr, float> GetStaticFloatField;

        // jdouble (JNICALL *GetStaticDoubleField) (JNIEnv* env, jclass clazz, jfieldID fieldID);
        internal readonly delegate* unmanaged[Stdcall]<JNIEnv*, IntPtr, IntPtr, double> GetStaticDoubleField;

        // void (JNICALL *SetStaticObjectField) (JNIEnv* env, jclass clazz, jfieldID fieldID, jobject value);
        internal readonly delegate* unmanaged[Stdcall]<JNIEnv*, IntPtr, IntPtr, IntPtr, void> SetStaticObjectField;

        // void (JNICALL *SetStaticBooleanField) (JNIEnv* env, jclass clazz, jfieldID fieldID, jboolean value);
        internal readonly delegate* unmanaged[Stdcall]<JNIEnv*, IntPtr, IntPtr, byte, void> SetStaticBooleanField;

        // void (JNICALL *SetStaticByteField) (JNIEnv* env, jclass clazz, jfieldID fieldID, jbyte value);
        internal readonly delegate* unmanaged[Stdcall]<JNIEnv*, IntPtr, IntPtr, sbyte, void> SetStaticByteField;

        // void (JNICALL *SetStaticCharField) (JNIEnv* env, jclass clazz, jfieldID fieldID, jchar value);
        internal readonly delegate* unmanaged[Stdcall]<JNIEnv*, IntPtr, IntPtr, char, void> SetStaticCharField;

        // void (JNICALL *SetStaticShortField) (JNIEnv* env, jclass clazz, jfieldID fieldID, jshort value);
        internal readonly delegate* unmanaged[Stdcall]<JNIEnv*, IntPtr, IntPtr, short, void> SetStaticShortField;

        // void (JNICALL *SetStaticIntField) (JNIEnv* env, jclass clazz, jfieldID fieldID, jint value);
        internal readonly delegate* unmanaged[Stdcall]<JNIEnv*, IntPtr, IntPtr, int, void> SetStaticIntField;

        // void (JNICALL *SetStaticLongField) (JNIEnv* env, jclass clazz, jfieldID fieldID, jlong value);
        internal readonly delegate* unmanaged[Stdcall]<JNIEnv*, IntPtr, IntPtr, long, void> SetStaticLongField;

        // void (JNICALL *SetStaticFloatField) (JNIEnv* env, jclass clazz, jfieldID fieldID, jfloat value);
        internal readonly delegate* unmanaged[Stdcall]<JNIEnv*, IntPtr, IntPtr, float, void> SetStaticFloatField;

        // void (JNICALL *SetStaticDoubleField) (JNIEnv* env, jclass clazz, jfieldID fieldID, jdouble value);
        internal readonly delegate* unmanaged[Stdcall]<JNIEnv*, IntPtr, IntPtr, double, void> SetStaticDoubleField;

        // jstring (JNICALL *NewString) (JNIEnv* env, const jchar* unicode, jsize len);
        internal readonly delegate* unmanaged[Stdcall]<JNIEnv*, IntPtr, int, IntPtr> NewString;

        // jsize (JNICALL *GetStringLength) (JNIEnv* env, jstring str);
        internal readonly delegate* unmanaged[Stdcall]<JNIEnv*, IntPtr, int> GetStringLength;

        // const jchar *(JNICALL *GetStringChars) (JNIEnv* env, jstring str, jboolean* isCopy);
        internal readonly delegate* unmanaged[Stdcall]<JNIEnv*, IntPtr, out byte, IntPtr> GetStringChars;

        // void (JNICALL *ReleaseStringChars) (JNIEnv* env, jstring str, const jchar* chars);
        internal readonly delegate* unmanaged[Stdcall]<JNIEnv*, IntPtr, IntPtr, void> ReleaseStringChars;

        // jstring (JNICALL *NewStringUTF) (JNIEnv* env, const char* utf);
        internal readonly delegate* unmanaged[Stdcall]<JNIEnv*, IntPtr, IntPtr> NewStringUTF;

        // jsize (JNICALL *GetStringUTFLength) (JNIEnv* env, jstring str);
        internal readonly delegate* unmanaged[Stdcall]<JNIEnv*, IntPtr, int> GetStringUTFLength;

        // const char* (JNICALL *GetStringUTFChars) (JNIEnv* env, jstring str, jboolean* isCopy);
        internal readonly delegate* unmanaged[Stdcall]<JNIEnv*, IntPtr, out byte, IntPtr> GetStringUTFChars;

        // void (JNICALL *ReleaseStringUTFChars) (JNIEnv* env, jstring str, const char* chars);
        internal readonly delegate* unmanaged[Stdcall]<JNIEnv*, IntPtr, IntPtr, void> ReleaseStringUTFChars;

        // jsize (JNICALL *GetArrayLength) (JNIEnv* env, jarray array);
        internal readonly delegate* unmanaged[Stdcall]<JNIEnv*, IntPtr, int> GetArrayLength;

        // jobjectArray (JNICALL *NewObjectArray) (JNIEnv* env, jsize len, jclass clazz, jobject init);
        internal readonly delegate* unmanaged[Stdcall]<JNIEnv*, int, IntPtr, IntPtr, IntPtr> NewObjectArray;

        // jobject (JNICALL *GetObjectArrayElement) (JNIEnv* env, jobjectArray array, jsize index);
        internal readonly delegate* unmanaged[Stdcall]<JNIEnv*, IntPtr, int, IntPtr> GetObjectArrayElement;

        // void (JNICALL *SetObjectArrayElement) (JNIEnv* env, jobjectArray array, jsize index, jobject val);
        internal readonly delegate* unmanaged[Stdcall]<JNIEnv*, IntPtr, int, IntPtr, void> SetObjectArrayElement;

        // jbooleanArray (JNICALL *NewBooleanArray) (JNIEnv* env, jsize len);
        internal readonly delegate* unmanaged[Stdcall]<JNIEnv*, int, IntPtr> NewBooleanArray;

        // jbyteArray (JNICALL *NewByteArray) (JNIEnv* env, jsize len);
        internal readonly delegate* unmanaged[Stdcall]<JNIEnv*, int, IntPtr> NewByteArray;

        // jcharArray (JNICALL *NewCharArray) (JNIEnv* env, jsize len);
        internal readonly delegate* unmanaged[Stdcall]<JNIEnv*, int, IntPtr> NewCharArray;

        // jshortArray (JNICALL *NewShortArray) (JNIEnv* env, jsize len);
        internal readonly delegate* unmanaged[Stdcall]<JNIEnv*, int, IntPtr> NewShortArray;

        // jintArray (JNICALL *NewIntArray) (JNIEnv* env, jsize len);
        internal readonly delegate* unmanaged[Stdcall]<JNIEnv*, int, IntPtr> NewIntArray;

        // jlongArray (JNICALL *NewLongArray) (JNIEnv* env, jsize len);
        internal readonly delegate* unmanaged[Stdcall]<JNIEnv*, int, IntPtr> NewLongArray;

        // jfloatArray (JNICALL *NewFloatArray) (JNIEnv* env, jsize len);
        internal readonly delegate* unmanaged[Stdcall]<JNIEnv*, int, IntPtr> NewFloatArray;

        // jdoubleArray (JNICALL *NewDoubleArray) (JNIEnv* env, jsize len);
        internal readonly delegate* unmanaged[Stdcall]<JNIEnv*, int, IntPtr> NewDoubleArray;

        // jboolean * (JNICALL *GetBooleanArrayElements) (JNIEnv* env, jbooleanArray array, jboolean* isCopy);
        internal readonly delegate* unmanaged[Stdcall]<JNIEnv*, IntPtr, out byte, byte*> GetBooleanArrayElements;

        // jbyte * (JNICALL *GetByteArrayElements) (JNIEnv* env, jbyteArray array, jboolean* isCopy);
        internal readonly delegate* unmanaged[Stdcall]<JNIEnv*, IntPtr, out byte, sbyte*> GetByteArrayElements;

        // jchar * (JNICALL *GetCharArrayElements) (JNIEnv* env, jcharArray array, jboolean* isCopy);
        internal readonly delegate* unmanaged[Stdcall]<JNIEnv*, IntPtr, out byte, char*> GetCharArrayElements;

        // jshort * (JNICALL *GetShortArrayElements) (JNIEnv* env, jshortArray array, jboolean* isCopy);
        internal readonly delegate* unmanaged[Stdcall]<JNIEnv*, IntPtr, out byte, short*> GetShortArrayElements;

        // jint * (JNICALL *GetIntArrayElements) (JNIEnv* env, jintArray array, jboolean* isCopy);
        internal readonly delegate* unmanaged[Stdcall]<JNIEnv*, IntPtr, out byte, int*> GetIntArrayElements;

        // jlong * (JNICALL *GetLongArrayElements) (JNIEnv* env, jlongArray array, jboolean* isCopy);
        internal readonly delegate* unmanaged[Stdcall]<JNIEnv*, IntPtr, out byte, long*> GetLongArrayElements;

        // jfloat * (JNICALL *GetFloatArrayElements) (JNIEnv* env, jfloatArray array, jboolean* isCopy);
        internal readonly delegate* unmanaged[Stdcall]<JNIEnv*, IntPtr, out byte, float*> GetFloatArrayElements;

        // jdouble * (JNICALL *GetDoubleArrayElements) (JNIEnv* env, jdoubleArray array, jboolean* isCopy);
        internal readonly delegate* unmanaged[Stdcall]<JNIEnv*, IntPtr, out byte, double*> GetDoubleArrayElements;

        // void (JNICALL *ReleaseBooleanArrayElements) (JNIEnv* env, jbooleanArray array, jboolean* elems, jint mode);
        internal readonly delegate* unmanaged[Stdcall]<JNIEnv*, IntPtr, byte*, int, void> ReleaseBooleanArrayElements;

        // void (JNICALL *ReleaseByteArrayElements) (JNIEnv* env, jbyteArray array, jbyte* elems, jint mode);
        internal readonly delegate* unmanaged[Stdcall]<JNIEnv*, IntPtr, sbyte*, int, void> ReleaseByteArrayElements;

        // void (JNICALL *ReleaseCharArrayElements) (JNIEnv* env, jcharArray array, jchar* elems, jint mode);
        internal readonly delegate* unmanaged[Stdcall]<JNIEnv*, IntPtr, char*, int, void> ReleaseCharArrayElements;

        // void (JNICALL *ReleaseShortArrayElements) (JNIEnv* env, jshortArray array, jshort* elems, jint mode);
        internal readonly delegate* unmanaged[Stdcall]<JNIEnv*, IntPtr, short*, int, void> ReleaseShortArrayElements;

        // void (JNICALL *ReleaseIntArrayElements) (JNIEnv* env, jintArray array, jint* elems, jint mode);
        internal readonly delegate* unmanaged[Stdcall]<JNIEnv*, IntPtr, int*, int, void> ReleaseIntArrayElements;

        // void (JNICALL *ReleaseLongArrayElements) (JNIEnv* env, jlongArray array, jlong* elems, jint mode);
        internal readonly delegate* unmanaged[Stdcall]<JNIEnv*, IntPtr, long*, int, void> ReleaseLongArrayElements;

        // void (JNICALL *ReleaseFloatArrayElements) (JNIEnv* env, jfloatArray array, jfloat* elems, jint mode);
        internal readonly delegate* unmanaged[Stdcall]<JNIEnv*, IntPtr, float*, int, void> ReleaseFloatArrayElements;

        // void (JNICALL *ReleaseDoubleArrayElements) (JNIEnv* env, jdoubleArray array, jdouble* elems, jint mode);
        internal readonly delegate* unmanaged[Stdcall]<JNIEnv*, IntPtr, double*, int, void> ReleaseDoubleArrayElements;

        // void (JNICALL *GetBooleanArrayRegion) (JNIEnv* env, jbooleanArray array, jsize start, jsize l, jboolean* buf);
        internal readonly delegate* unmanaged[Stdcall]<JNIEnv*, IntPtr, int, int, byte*, void> GetBooleanArrayRegion;

        // void (JNICALL *GetByteArrayRegion) (JNIEnv* env, jbyteArray array, jsize start, jsize len, jbyte* buf);
        internal readonly delegate* unmanaged[Stdcall]<JNIEnv*, IntPtr, int, int, sbyte*, void> GetByteArrayRegion;

        // void (JNICALL *GetCharArrayRegion) (JNIEnv* env, jcharArray array, jsize start, jsize len, jchar* buf);
        internal readonly delegate* unmanaged[Stdcall]<JNIEnv*, IntPtr, int, int, char*, void> GetCharArrayRegion;

        // void (JNICALL *GetShortArrayRegion) (JNIEnv* env, jshortArray array, jsize start, jsize len, jshort* buf);
        internal readonly delegate* unmanaged[Stdcall]<JNIEnv*, IntPtr, int, int, short*, void> GetShortArrayRegion;

        // void (JNICALL *GetIntArrayRegion) (JNIEnv* env, jintArray array, jsize start, jsize len, jint* buf);
        internal readonly delegate* unmanaged[Stdcall]<JNIEnv*, IntPtr, int, int, int*, void> GetIntArrayRegion;

        // void (JNICALL *GetLongArrayRegion) (JNIEnv* env, jlongArray array, jsize start, jsize len, jlong* buf);
        internal readonly delegate* unmanaged[Stdcall]<JNIEnv*, IntPtr, int, int, long*, void> GetLongArrayRegion;

        // void (JNICALL *GetFloatArrayRegion) (JNIEnv* env, jfloatArray array, jsize start, jsize len, jfloat* buf);
        internal readonly delegate* unmanaged[Stdcall]<JNIEnv*, IntPtr, int, int, float*, void> GetFloatArrayRegion;

        // void (JNICALL *GetDoubleArrayRegion) (JNIEnv* env, jdoubleArray array, jsize start, jsize len, jdouble* buf);
        internal readonly delegate* unmanaged[Stdcall]<JNIEnv*, IntPtr, int, int, double*, void> GetDoubleArrayRegion;

        // void (JNICALL *SetBooleanArrayRegion) (JNIEnv* env, jbooleanArray array, jsize start, jsize l, const jboolean* buf);
        internal readonly delegate* unmanaged[Stdcall]<JNIEnv*, IntPtr, int, int, byte*, void> SetBooleanArrayRegion;

        // void (JNICALL *SetByteArrayRegion) (JNIEnv* env, jbyteArray array, jsize start, jsize len, const jbyte* buf);
        internal readonly delegate* unmanaged[Stdcall]<JNIEnv*, IntPtr, int, int, sbyte*, void> SetByteArrayRegion;

        // void (JNICALL *SetCharArrayRegion) (JNIEnv* env, jcharArray array, jsize start, jsize len, const jchar* buf);
        internal readonly delegate* unmanaged[Stdcall]<JNIEnv*, IntPtr, int, int, char*, void> SetCharArrayRegion;

        // void (JNICALL *SetShortArrayRegion) (JNIEnv* env, jshortArray array, jsize start, jsize len, const jshort* buf);
        internal readonly delegate* unmanaged[Stdcall]<JNIEnv*, IntPtr, int, int, short*, void> SetShortArrayRegion;

        // void (JNICALL *SetIntArrayRegion) (JNIEnv* env, jintArray array, jsize start, jsize len, const jint* buf);
        internal readonly delegate* unmanaged[Stdcall]<JNIEnv*, IntPtr, int, int, int*, void> SetIntArrayRegion;

        // void (JNICALL *SetLongArrayRegion) (JNIEnv* env, jlongArray array, jsize start, jsize len, const jlong* buf);
        internal readonly delegate* unmanaged[Stdcall]<JNIEnv*, IntPtr, int, int, long*, void> SetLongArrayRegion;

        // void (JNICALL *SetFloatArrayRegion) (JNIEnv* env, jfloatArray array, jsize start, jsize len, const jfloat* buf);
        internal readonly delegate* unmanaged[Stdcall]<JNIEnv*, IntPtr, int, int, float*, void> SetFloatArrayRegion;

        // void (JNICALL *SetDoubleArrayRegion) (JNIEnv* env, jdoubleArray array, jsize start, jsize len, const jdouble* buf);
        internal readonly delegate* unmanaged[Stdcall]<JNIEnv*, IntPtr, int, int, double*, void> SetDoubleArrayRegion;

// jint (JNICALL *RegisterNatives) (JNIEnv* env, jclass clazz, const JNINativeMethod* methods, jint nMethods);
        // internal readonly delegate* unmanaged[Stdcall]<JNIEnv*, IntPtr, AndroidProxy.JNINativeMethod*, int, int> RegisterNatives;
        internal readonly delegate* unmanaged[Stdcall]<JNIEnv*, IntPtr, IntPtr, int, int> RegisterNatives;

        // jint (JNICALL *UnregisterNatives) (JNIEnv* env, jclass clazz);
        internal readonly delegate* unmanaged[Stdcall]<JNIEnv*, IntPtr, int> UnregisterNatives;

        // jint (JNICALL *MonitorEnter) (JNIEnv* env, jobject obj);
        internal readonly delegate* unmanaged[Stdcall]<JNIEnv*, IntPtr, int> MonitorEnter;

        // jint (JNICALL *MonitorExit) (JNIEnv* env, jobject obj);
        internal readonly delegate* unmanaged[Stdcall]<JNIEnv*, IntPtr, int> MonitorExit;

        // jint (JNICALL *GetJavaVM) (JNIEnv* env, JavaVM** vm);
        internal readonly delegate* unmanaged[Stdcall]<JNIEnv*, out IntPtr, int> GetJavaVM;

        // void (JNICALL *GetStringRegion) (JNIEnv* env, jstring str, jsize start, jsize len, jchar* buf);
        internal readonly delegate* unmanaged[Stdcall]<JNIEnv*, IntPtr, int, int, IntPtr, void> GetStringRegion;

        // void (JNICALL *GetStringUTFRegion) (JNIEnv* env, jstring str, jsize start, jsize len, char* buf);
        internal readonly delegate* unmanaged[Stdcall]<JNIEnv*, IntPtr, int, int, char*, void> GetStringUTFRegion;

        // void * (JNICALL *GetPrimitiveArrayCritical) (JNIEnv* env, jarray array, jboolean* isCopy);
        internal readonly delegate* unmanaged[Stdcall]<JNIEnv*, IntPtr, out byte, IntPtr> GetPrimitiveArrayCritical;

        // void (JNICALL *ReleasePrimitiveArrayCritical) (JNIEnv* env, jarray array, void* carray, jint mode);
        internal readonly delegate* unmanaged[Stdcall]<JNIEnv*, IntPtr, IntPtr, int, void> ReleasePrimitiveArrayCritical;

        // const jchar * (JNICALL *GetStringCritical) (JNIEnv* env, jstring string, jboolean* isCopy);
        internal readonly delegate* unmanaged[Stdcall]<JNIEnv*, IntPtr, byte*, char*> GetStringCritical;

        // void (JNICALL *ReleaseStringCritical) (JNIEnv* env, jstring string, const jchar* cstring);
        internal readonly delegate* unmanaged[Stdcall]<JNIEnv*, IntPtr, char*, void> ReleaseStringCritical;

        // jweak (JNICALL *NewWeakGlobalRef) (JNIEnv* env, jobject obj);
        internal readonly delegate* unmanaged[Stdcall]<JNIEnv*, IntPtr, IntPtr> NewWeakGlobalRef;

        // void (JNICALL *DeleteWeakGlobalRef) (JNIEnv* env, jweak ref);
        internal readonly delegate* unmanaged[Stdcall]<JNIEnv*, IntPtr, void> DeleteWeakGlobalRef;

        // jboolean (JNICALL *ExceptionCheck) (JNIEnv* env);
        internal readonly delegate* unmanaged[Stdcall]<JNIEnv*, byte> ExceptionCheck;

        // jobject (JNICALL *NewDirectByteBuffer) (JNIEnv* env, void* address, jlong capacity);
        internal readonly delegate* unmanaged[Stdcall]<JNIEnv*, IntPtr, long, IntPtr> NewDirectByteBuffer;

        // void* (JNICALL *GetDirectBufferAddress) (JNIEnv* env, jobject buf);
        internal readonly delegate* unmanaged[Stdcall]<JNIEnv*, IntPtr, IntPtr> GetDirectBufferAddress;

        // jlong (JNICALL *GetDirectBufferCapacity) (JNIEnv* env, jobject buf);
        internal readonly delegate* unmanaged[Stdcall]<JNIEnv*, IntPtr, long> GetDirectBufferCapacity;
    }

    internal readonly FunctionTable* Functions;
}
#endif
