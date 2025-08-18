#if ANDROID
namespace MelonLoader.Java;

using System;
using System.Runtime.InteropServices;

/// <summary>
/// Represents a JavaVM C struct.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
internal readonly unsafe struct JavaVM
{
    [StructLayout(LayoutKind.Sequential)]
    internal readonly unsafe struct FunctionTable
    {
        internal readonly IntPtr Reserved0;

        internal readonly IntPtr Reserved1;

        internal readonly IntPtr Reserved2;

        // jint (JNICALL *DestroyJavaVM)(JavaVM *vm);
        internal readonly delegate* unmanaged[Stdcall]<JavaVM*, JNI.Result> DestroyJavaVM;

        // jint (JNICALL *AttachCurrentThread)(JavaVM *vm, void **penv, void *args);
        internal readonly delegate* unmanaged[Stdcall]<JavaVM*, out JNIEnv*, IntPtr, JNI.Result> AttachCurrentThread;

        // jint (JNICALL *DetachCurrentThread)(JavaVM *vm);
        internal readonly delegate* unmanaged[Stdcall]<JavaVM*, JNI.Result> DetachCurrentThread;

        // jint (JNICALL *GetEnv)(JavaVM *vm, void **penv, jint version);
        internal readonly delegate* unmanaged[Stdcall]<JavaVM*, out IntPtr, int, JNI.Result> GetEnv;

        // jint (JNICALL *AttachCurrentThreadAsDaemon)(JavaVM *vm, void **penv, void *args);
        internal readonly delegate* unmanaged[Stdcall]<JavaVM*, out IntPtr, IntPtr, JNI.Result> AttachCurrentThreadAsDaemon;
    }

    internal readonly FunctionTable* Functions;
}
#endif
