using System;
using System.Collections.Generic;
using System.Reflection;
using MelonLoader;
using NET_SDK.Reflection;

namespace NET_SDK.Harmony
{
    public static class Manager
    {
        private static List<Instance> InstanceList = new List<Instance>();

        public static Instance CreateInstance(string name)
        {
            Instance newinst = new Instance(name);
            InstanceList.Add(newinst);
            return newinst;
        }

        public static void UnpatchAll()
        {
            if (InstanceList.Count > 0)
            {
                foreach (Instance inst in InstanceList)
                    inst.UnpatchAll();
                InstanceList.Clear();
            }
        }
    }

    public class Instance
    {
        public string Name;
        private List<Patch> PatchList = new List<Patch>();

        internal Instance(string name) => Name = name;

        public Patch Patch(IL2CPP_Method targetMethod, MethodInfo newMethod) => Patch(targetMethod.Ptr, newMethod);
        public Patch Patch(IntPtr targetMethod, MethodInfo newMethod)
        {
            if ((targetMethod == null) || (newMethod == null))
                return null;
            Patch patch = new Patch(targetMethod, newMethod.MethodHandle.GetFunctionPointer());
            PatchList.Add(patch);
            return patch;
        }

        public void UnpatchAll()
        {
            if (PatchList.Count > 0)
            {
                foreach (Patch patch in PatchList)
                    patch.UninstallPatch();
                PatchList.Clear();
            }
        }
    }

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

        public IL2CPP_Object InvokeOriginal() => InvokeOriginal(IntPtr.Zero, new IntPtr[] { IntPtr.Zero });
        public IL2CPP_Object InvokeOriginal(IntPtr obj) => InvokeOriginal(obj, new IntPtr[] { IntPtr.Zero });
        public IL2CPP_Object InvokeOriginal(IL2CPP_Object obj) => InvokeOriginal(obj.Ptr, new IntPtr[] { IntPtr.Zero });
        public IL2CPP_Object InvokeOriginal(params IntPtr[] paramtbl) => InvokeOriginal(IntPtr.Zero, paramtbl);
        public IL2CPP_Object InvokeOriginal(params IL2CPP_Object[] paramtbl) => InvokeOriginal(IntPtr.Zero, IL2CPP.IL2CPPObjectArrayToIntPtrArray(paramtbl));
        public IL2CPP_Object InvokeOriginal(IntPtr obj, params IL2CPP_Object[] paramtbl) => InvokeOriginal(obj, IL2CPP.IL2CPPObjectArrayToIntPtrArray(paramtbl));
        public IL2CPP_Object InvokeOriginal(IL2CPP_Object obj, params IntPtr[] paramtbl) => InvokeOriginal(obj.Ptr, paramtbl);
        public IL2CPP_Object InvokeOriginal(IntPtr obj, params IntPtr[] paramtbl)
        {
            IL2CPP_Object returnval = null;
            uint param_count = IL2CPP.il2cpp_method_get_param_count(TargetMethod);
            if (param_count == 0)
                UninstallPatch();
            IntPtr returnvalptr = IL2CPP.InvokeMethod(TargetMethod, obj, paramtbl);
            if (returnvalptr != IntPtr.Zero)
                returnval = new IL2CPP_Object(returnvalptr, new IL2CPP_Type(IL2CPP.il2cpp_method_get_return_type(TargetMethod)));
            if (param_count == 0)
                InstallPatch();
            return returnval;
        }

        unsafe internal void InstallPatch()
        {
            uint param_count = IL2CPP.il2cpp_method_get_param_count(TargetMethod);
            if (param_count == 0)
                *(IntPtr*)TargetMethod.ToPointer() = NewMethod;
            else
                Imports.Hook(TargetMethod, NewMethod);
        }

        unsafe internal void UninstallPatch()
        {
            uint param_count = IL2CPP.il2cpp_method_get_param_count(TargetMethod);
            if (param_count == 0)
                *(IntPtr*)TargetMethod.ToPointer() = OriginalMethod;
            else
                Imports.Unhook(TargetMethod, OriginalMethod);
        }
    }
}
 