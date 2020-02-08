using System;
using System.Reflection;

namespace MelonLoader
{
    internal class MelonModController
    {
        internal Assembly modAssembly;
        internal MelonMod modInstance;
        private MethodInfo onApplicationStartMethod;
        private MethodInfo onLevelWasLoadedMethod;
        private MethodInfo onLevelWasInitializedMethod;
        private MethodInfo onUpdateMethod;
        private MethodInfo onFixedUpdateMethod;
        private MethodInfo onLateUpdateMethod;
        private MethodInfo onGUIMethod;
        private MethodInfo onApplicationQuitMethod;
        private MethodInfo onModSettingsApplied;

        internal MelonModController(MelonMod mod, Type t, Assembly asm)
        {
            modAssembly = asm;
            modInstance = mod;
            MethodInfo[] methods = t.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            foreach (MethodInfo method in methods)
            {
                if (method.Name.Equals("OnApplicationStart") && (method.GetParameters().Length == 0))
                    onApplicationStartMethod = method;
                else if (method.Name.Equals("OnLevelWasLoaded") && method.GetParameters().Length == 1 && method.GetParameters()[0].ParameterType == typeof(int))
                    onLevelWasLoadedMethod = method;
                else if (method.Name.Equals("OnLevelWasInitialized") && method.GetParameters().Length == 1 && method.GetParameters()[0].ParameterType == typeof(int))
                    onLevelWasInitializedMethod = method;
                else if (method.Name.Equals("OnUpdate") && (method.GetParameters().Length == 0))
                    onUpdateMethod = method;
                else if (method.Name.Equals("OnFixedUpdate") && method.GetParameters().Length == 0)
                    onFixedUpdateMethod = method;
                else if (method.Name.Equals("OnLateUpdate") && method.GetParameters().Length == 0)
                    onLateUpdateMethod = method;
                else if (method.Name.Equals("OnGUI") && method.GetParameters().Length == 0)
                    onGUIMethod = method;
                else if (method.Name.Equals("OnApplicationQuit") && (method.GetParameters().Length == 0))
                    onApplicationQuitMethod = method;
                else if (method.Name.Equals("OnModSettingsApplied") && (method.GetParameters().Length == 0))
                    onModSettingsApplied = method;
            }
        }

        internal virtual void OnApplicationStart() { if (modInstance != null) onApplicationStartMethod?.Invoke(modInstance, new object[0]); }
        internal virtual void OnLevelWasLoaded(int level) { if (modInstance != null) onLevelWasLoadedMethod?.Invoke(modInstance, new object[] { level }); }
        internal virtual void OnLevelWasInitialized(int level) { if (modInstance != null) onLevelWasInitializedMethod?.Invoke(modInstance, new object[] { level }); }
        internal virtual void OnUpdate() { if (modInstance != null) onUpdateMethod?.Invoke(modInstance, new object[0]); }
        internal virtual void OnFixedUpdate() { if (modInstance != null) onFixedUpdateMethod?.Invoke(modInstance, new object[0]); }
        internal virtual void OnLateUpdate() { if (modInstance != null) onLateUpdateMethod?.Invoke(modInstance, new object[0]); }
        internal virtual void OnGUI() { if (modInstance != null) onGUIMethod?.Invoke(modInstance, new object[0]); }
        internal virtual void OnApplicationQuit() { if (modInstance != null) onApplicationQuitMethod?.Invoke(modInstance, new object[0]); }
        internal virtual void OnModSettingsApplied() { if (modInstance != null) onModSettingsApplied?.Invoke(modInstance, new object[0]); }
    }
}