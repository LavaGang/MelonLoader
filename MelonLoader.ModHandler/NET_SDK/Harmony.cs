using System;
using System.Collections.Generic;
using System.Reflection;
using MelonLoader;
using NET_SDK.Reflection;

namespace NET_SDK.Harmony
{
    [ObsoleteAttribute("This method will be removed soon. Please use normal Reflection.")]
    public static class Manager
    {
        private static List<Instance> InstanceList = new List<Instance>();
        [ObsoleteAttribute("This method will be removed soon. Please use 0Harmony.")]
        public static Instance CreateInstance(string name)
        {
            Instance newinst = new Instance(name);
            InstanceList.Add(newinst);
            return newinst;
        }
        internal static void UnpatchAll()
        {
            if (InstanceList.Count > 0)
            {
                foreach (Instance inst in InstanceList)
                    inst.UnpatchAll();
                InstanceList.Clear();
            }
        }
    }

    [ObsoleteAttribute("This method will be removed soon. Please use normal Reflection.")]
    public class Instance
    {
        public string Name;
        private List<Patch> PatchList = new List<Patch>();
        internal Instance(string name) => Name = name;
        [ObsoleteAttribute("This method will be removed soon. Please use 0Harmony.")]
        public Patch Patch(IL2CPP_Method targetMethod, MethodInfo newMethod) => Patch(targetMethod.Ptr, newMethod);
        [ObsoleteAttribute("This method will be removed soon. Please use 0Harmony.")]
        public Patch Patch(IntPtr targetMethod, MethodInfo newMethod)
        {
            if ((targetMethod == IntPtr.Zero) || NETFrameworkFix.MethodInfo_op_Equality(newMethod, null))
                return null;
            Patch patch = new Patch(targetMethod, newMethod.MethodHandle.GetFunctionPointer());
            PatchList.Add(patch);
            return patch;
        }
        internal void UnpatchAll()
        {
            if (PatchList.Count > 0)
            {
                foreach (Patch patch in PatchList)
                    patch.UninstallPatch();
                PatchList.Clear();
            }
        }
    }

    [ObsoleteAttribute("This method will be removed soon. Please use normal Reflection.")]
    public class Patch
    {
        internal IntPtr TargetMethod;
        internal IntPtr NewMethod;
        internal IntPtr OriginalMethod;
        unsafe internal Patch(IntPtr targetMethod, IntPtr newMethod)
        {
            TargetMethod = targetMethod;
            NewMethod = newMethod;
            OriginalMethod = *(IntPtr*)TargetMethod.ToPointer();
            InstallPatch();
        }
        unsafe internal Patch(IL2CPP_Method targetMethod, IntPtr newMethod)
        {
            TargetMethod = targetMethod.Ptr;
            NewMethod = newMethod;
            OriginalMethod = *(IntPtr*)TargetMethod.ToPointer();
            InstallPatch();
        }
        [ObsoleteAttribute("This method will be removed soon. Please use 0Harmony.")]
        public IL2CPP_Object InvokeOriginal() => InvokeOriginal(IntPtr.Zero, new IntPtr[] { IntPtr.Zero });
        [ObsoleteAttribute("This method will be removed soon. Please use 0Harmony.")]
        public IL2CPP_Object InvokeOriginal(IntPtr obj) => InvokeOriginal(obj, new IntPtr[] { IntPtr.Zero });
        [ObsoleteAttribute("This method will be removed soon. Please use 0Harmony.")]
        public IL2CPP_Object InvokeOriginal(IL2CPP_Object obj) => InvokeOriginal(obj.Ptr, new IntPtr[] { IntPtr.Zero });
        [ObsoleteAttribute("This method will be removed soon. Please use 0Harmony.")]
        public IL2CPP_Object InvokeOriginal(params IntPtr[] paramtbl) => InvokeOriginal(IntPtr.Zero, paramtbl);
        [ObsoleteAttribute("This method will be removed soon. Please use 0Harmony.")]
        public IL2CPP_Object InvokeOriginal(params IL2CPP_Object[] paramtbl) => InvokeOriginal(IntPtr.Zero, IL2CPP.IL2CPPObjectArrayToIntPtrArray(paramtbl));
        [ObsoleteAttribute("This method will be removed soon. Please use 0Harmony.")]
        public IL2CPP_Object InvokeOriginal(IntPtr obj, params IL2CPP_Object[] paramtbl) => InvokeOriginal(obj, IL2CPP.IL2CPPObjectArrayToIntPtrArray(paramtbl));
        [ObsoleteAttribute("This method will be removed soon. Please use 0Harmony.")]
        public IL2CPP_Object InvokeOriginal(IL2CPP_Object obj, params IntPtr[] paramtbl) => InvokeOriginal(obj.Ptr, paramtbl);
        [ObsoleteAttribute("This method will be removed soon. Please use 0Harmony.")]
        public IL2CPP_Object InvokeOriginal(IntPtr obj, params IntPtr[] paramtbl)
        {
            IL2CPP_Object returnval = null;
            uint param_count = Il2Cpp.il2cpp_method_get_param_count(TargetMethod);
            if (param_count == 0)
                UninstallPatch();
            IntPtr returnvalptr = Il2Cpp.InvokeMethod(TargetMethod, obj, paramtbl);
            if (returnvalptr != IntPtr.Zero)
                returnval = new IL2CPP_Object(returnvalptr, new IL2CPP_Type(Il2Cpp.il2cpp_method_get_return_type(TargetMethod)));
            if (param_count == 0)
                InstallPatch();
            return returnval;
        }
        unsafe internal void InstallPatch()
        {
            uint param_count = Il2Cpp.il2cpp_method_get_param_count(TargetMethod);
            if (param_count == 0)
                *(IntPtr*)TargetMethod.ToPointer() = NewMethod;
            else
                Imports.Hook(TargetMethod, NewMethod);
        }
        unsafe internal void UninstallPatch()
        {
            uint param_count = Il2Cpp.il2cpp_method_get_param_count(TargetMethod);
            if (param_count == 0)
                *(IntPtr*)TargetMethod.ToPointer() = OriginalMethod;
            else
                Imports.Unhook(TargetMethod, OriginalMethod);
        }
    }
}
 