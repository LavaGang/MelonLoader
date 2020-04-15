using System;
using System.Reflection;

namespace MelonLoader
{
    internal class MelonModController
    {
        internal Assembly modAssembly;
        internal MelonMod modInstance;
        private readonly Action onApplicationStart;
        private readonly Action<int> onLevelWasLoaded;
        private readonly Action<int> onLevelWasInitialized;
        private readonly Action onUpdate;
        private readonly Action onFixedUpdate;
        private readonly Action onLateUpdate;
        private readonly Action onGUI;
        private readonly Action onApplicationQuit;
        private readonly Action onModSettingsApplied;
        private readonly Action vrchat_OnUiManagerInit;

        internal MelonModController(MelonMod mod, Type t, Assembly asm)
        {
            modAssembly = asm;
            modInstance = mod;
            MethodInfo[] methods = t.GetMethods(BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            foreach (MethodInfo method in methods)
            {
                if (method.Name.Equals("OnApplicationStart") && (method.GetParameters().Length == 0))
                    onApplicationStart = (Action)Delegate.CreateDelegate(typeof(Action), modInstance, method);
                else if (method.Name.Equals("OnLevelWasLoaded") && method.GetParameters().Length == 1 && method.GetParameters()[0].ParameterType == typeof(int))
                    onLevelWasLoaded = (Action<int>)Delegate.CreateDelegate(typeof(Action<int>), modInstance, method);
                else if (method.Name.Equals("OnLevelWasInitialized") && method.GetParameters().Length == 1 && method.GetParameters()[0].ParameterType == typeof(int))
                    onLevelWasInitialized = (Action<int>)Delegate.CreateDelegate(typeof(Action<int>), modInstance, method);
                else if (method.Name.Equals("OnUpdate") && (method.GetParameters().Length == 0))
                    onUpdate = (Action)Delegate.CreateDelegate(typeof(Action), modInstance, method);
                else if (method.Name.Equals("OnFixedUpdate") && method.GetParameters().Length == 0)
                    onFixedUpdate = (Action)Delegate.CreateDelegate(typeof(Action), modInstance, method);
                else if (method.Name.Equals("OnLateUpdate") && method.GetParameters().Length == 0)
                    onLateUpdate = (Action)Delegate.CreateDelegate(typeof(Action), modInstance, method);
                else if (method.Name.Equals("OnGUI") && method.GetParameters().Length == 0)
                    onGUI = (Action)Delegate.CreateDelegate(typeof(Action), modInstance, method);
                else if (method.Name.Equals("OnApplicationQuit") && (method.GetParameters().Length == 0))
                    onApplicationQuit = (Action)Delegate.CreateDelegate(typeof(Action), modInstance, method);
                else if (method.Name.Equals("OnModSettingsApplied") && (method.GetParameters().Length == 0))
                    onModSettingsApplied = (Action)Delegate.CreateDelegate(typeof(Action), modInstance, method);
                else if (method.Name.Equals("VRChat_OnUiManagerInit") && (method.GetParameters().Length == 0))
                    vrchat_OnUiManagerInit = (Action)Delegate.CreateDelegate(typeof(Action), modInstance, method);
            }
        }

        internal virtual void OnApplicationStart() { if (modInstance != null) onApplicationStart?.Invoke(); }
        internal virtual void OnLevelWasLoaded(int level) { if (modInstance != null) onLevelWasLoaded?.Invoke(level); }
        internal virtual void OnLevelWasInitialized(int level) { if (modInstance != null) onLevelWasInitialized?.Invoke(level); }
        internal virtual void OnUpdate() { if (modInstance != null) onUpdate?.Invoke(); }
        internal virtual void OnFixedUpdate() { if (modInstance != null) onFixedUpdate?.Invoke(); }
        internal virtual void OnLateUpdate() { if (modInstance != null) onLateUpdate?.Invoke(); }
        internal virtual void OnGUI() { if (modInstance != null) onGUI?.Invoke(); }
        internal virtual void OnApplicationQuit() { if (modInstance != null) onApplicationQuit?.Invoke(); }
        internal virtual void OnModSettingsApplied() { if (modInstance != null) onModSettingsApplied?.Invoke(); }
        internal virtual void VRChat_OnUiManagerInit() { if (modInstance != null) vrchat_OnUiManagerInit?.Invoke(); }
    }
}