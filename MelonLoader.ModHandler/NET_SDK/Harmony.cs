using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using MelonLoader;
using NET_SDK.Reflection;

namespace NET_SDK.Harmony
{
    public static class Manager
    {
        internal static Instance MainInstance = null;
        private static List<Instance> InstanceList = new List<Instance>();

        public static Instance CreateInstance(string name)
        {
            Instance newinst = new Instance(name);
            InstanceList.Add(newinst);
            return newinst;
        }
        internal static Instance CreateMainInstance()
        {
            if (MainInstance == null)
                MainInstance = new Instance("NET_SDK");
            return MainInstance;
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

        internal static void UnpatchMain()
        {
            if (MainInstance != null)
            {
                MainInstance.UnpatchAll();
                MainInstance = null;
            }
        }
    }

    public class Instance
    {
        public string Name;
        private List<Patch> PatchList = new List<Patch>();

        internal Instance(string name) => Name = name;

        public Patch Patch(IL2CPP_Method targetMethod, MethodInfo newMethod)
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
        internal IL2CPP_Method TargetMethod;
        internal IntPtr NewMethod;
        internal IntPtr OriginalMethod;

        unsafe internal Patch(IL2CPP_Method targetMethod, IntPtr newMethod)
        {
            TargetMethod = targetMethod;
            NewMethod = newMethod;
            OriginalMethod = *(IntPtr*)TargetMethod.Ptr.ToPointer();
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
            if (TargetMethod.GetParameterCount() == 0)
            {
                UninstallPatch();
                IL2CPP_Object returnval = TargetMethod.Invoke(obj, paramtbl);
                InstallPatch();
                return returnval;
            }
            else 
                return TargetMethod.Invoke(obj, paramtbl);
        }

        unsafe internal void InstallPatch()
        {
            if (TargetMethod.GetParameterCount() == 0)
                *(IntPtr*)TargetMethod.Ptr.ToPointer() = NewMethod;
            else
                Imports.melonloader_detour(TargetMethod.Ptr, NewMethod);
        }

        unsafe internal void UninstallPatch()
        {
            if (TargetMethod.GetParameterCount() == 0)
                *(IntPtr*)TargetMethod.Ptr.ToPointer() = OriginalMethod;
            else
                Imports.melonloader_undetour(TargetMethod.Ptr, OriginalMethod);
        }
    }
}