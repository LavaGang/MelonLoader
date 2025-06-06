#if ANDROID
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;
using MelonLoader.Java;
using MelonLoader.Bootstrap.Utils;

namespace MelonLoader.Bootstrap.Proxy.Android;

public static class AndroidProxy
{
    [DllImport("liblog", EntryPoint = "__android_log_print")]
    private static extern int LogInternal(int prio, string tag,  string text);

    public static void Log(string text) => LogInternal(4, "MelonLoader", text);
   
    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    [RequiresDynamicCode("Calls MelonLoader.Bootstrap.Proxy.Android.AndroidBootstrap.LoadBootstrap()")]
    private static unsafe byte Load(void* env, void* jobject, void* str)
    {
        LoadUnity();
        AndroidBootstrap.LoadBootstrap();
        return 1;
    }

    public static unsafe void LoadUnity()
    {
        if (!NativeLibrary.TryLoad("libunity.so", out var libUnity))
        {
            Log("Failed to load libunity.so");
            return;
        }

        if (!NativeFunc.GetExport<JNI_OnLoadFunc>(libUnity, "JNI_OnLoad", out var jniOnLoad))
        {
            Log("Can't load Export via JNI_OnLoad");
            return;
        }

        jniOnLoad((IntPtr)JNI.VM, IntPtr.Zero);
    }
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate nint JNI_OnLoadFunc(IntPtr vm, IntPtr reserved);   

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    private static unsafe byte Unload(void* vm, void* reserved)
    {
        Log("Unload called");
        return 1;
    }
    
    [UnmanagedCallersOnly(EntryPoint = "JNI_OnLoad")]
    public static unsafe JNI.Version JNI_OnLoad(IntPtr vm, void* reserved)
    {
        JNI.Initialize(vm);
        JClass nativeLoader = JNI.FindClass("com/unity3d/player/NativeLoader");
        if (!nativeLoader.Valid())
        {
            Log("Cannot find NativeLoader class");
            return JNI.Version.V1_6;
        }
     
        var methods = (JNINativeMethod*)NativeMemory.Alloc((nuint)(sizeof(JNINativeMethod) * 2));
            
        methods[0] = new JNINativeMethod { Name = Utf8StringMarshaller.ConvertToUnmanaged("load"), Signature = Utf8StringMarshaller.ConvertToUnmanaged("(Ljava/lang/String;)Z"), FnPtr = (delegate* unmanaged[Cdecl]<void*, void*, void*, byte>)&Load };
        methods[1] = new JNINativeMethod { Name = Utf8StringMarshaller.ConvertToUnmanaged("unload"), Signature = Utf8StringMarshaller.ConvertToUnmanaged("()Z"), FnPtr = (delegate* unmanaged[Cdecl]<void*, void*, byte>)&Unload };

        var registerNatives = JNI.Env->Functions->RegisterNatives(JNI.Env, nativeLoader.Handle, (IntPtr)methods, 2);
        if (registerNatives != 0)
        {
            Log("Failed to register native methods");
        }
        return JNI.Version.V1_6;
    }

    private unsafe struct JNINativeMethod
    {
        public byte* Name;
        public byte* Signature;
        public void* FnPtr;
    }
}
#endif