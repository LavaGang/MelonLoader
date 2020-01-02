using System;
using System.Reflection;

namespace MelonLoader
{
    internal class MelonModController
    {
        private MethodInfo onApplicationStartMethod;
        private MethodInfo onUpdateMethod;
        private MethodInfo onApplicationQuitMethod;
        private MethodInfo onModSettingsApplied;
        internal MelonMod mod;

        internal MelonModController(MelonMod mod, Type t)
        {
            this.mod = mod;
            MethodInfo[] methods = t.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            foreach (MethodInfo method in methods)
            {
                if (method.Name.Equals("OnApplicationStart") && (method.GetParameters().Length == 0))
                    onApplicationStartMethod = method;
                else if (method.Name.Equals("OnUpdate") && (method.GetParameters().Length == 0))
                    onUpdateMethod = method;
                else if (method.Name.Equals("OnApplicationQuit") && (method.GetParameters().Length == 0))
                    onApplicationQuitMethod = method;
                else if (method.Name.Equals("OnModSettingsApplied") && (method.GetParameters().Length == 0))
                    onModSettingsApplied = method;
            }
        }

        internal virtual void OnApplicationStart() { if (mod != null) onApplicationStartMethod?.Invoke(mod, new object[0]); }
        internal virtual void OnUpdate() { if (mod != null) onUpdateMethod?.Invoke(mod, new object[0]); }
        internal virtual void OnApplicationQuit() { if (mod != null) onApplicationQuitMethod?.Invoke(mod, new object[0]); }
        internal virtual void OnModSettingsApplied() { if (mod != null) onModSettingsApplied?.Invoke(mod, new object[0]); }
    }
}