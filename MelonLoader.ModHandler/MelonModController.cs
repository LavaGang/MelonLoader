using System;
using System.Reflection;

namespace MelonLoader
{
    internal class MelonModController
    {
        internal Assembly modAssembly;
        internal MelonMod modInstance;
        private readonly MethodInfo onApplicationStartMethod;
        private readonly MethodInfo onLevelWasLoadedMethod;
        private readonly MethodInfo onLevelWasInitializedMethod;
        private readonly MethodInfo onUpdateMethod;
        private readonly MethodInfo onFixedUpdateMethod;
        private readonly MethodInfo onLateUpdateMethod;
        private readonly MethodInfo onGUIMethod;
        private readonly MethodInfo onApplicationQuitMethod;
        private readonly MethodInfo onModSettingsAppliedMethod;

        private readonly Action onApplicationStart;
        private readonly Action<int> onLevelWasLoaded;
        private readonly Action<int> onLevelWasInitialized;
        private readonly Action onUpdate;
        private readonly Action onFixedUpdate;
        private readonly Action onLateUpdate;
        private readonly Action onGUI;
        private readonly Action onApplicationQuit;
        private readonly Action onModSettingsApplied;
        internal MelonModController(MelonMod mod, Type t, Assembly asm)
        {
            modAssembly = asm;
            modInstance = mod;
            MethodInfo[] methods = t.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            foreach (MethodInfo method in methods)
            {
                if (method.Name.Equals("OnApplicationStart") && (method.GetParameters().Length == 0))
                {
                    onApplicationStartMethod = method;
                    onApplicationStart = (Action)Delegate.CreateDelegate(typeof(Action), modInstance, onApplicationStartMethod);
                }
                else if (method.Name.Equals("OnLevelWasLoaded") && method.GetParameters().Length == 1 && method.GetParameters()[0].ParameterType == typeof(int))
                {
                    onLevelWasLoadedMethod = method;
                    onLevelWasLoaded = (Action<int>)Delegate.CreateDelegate(typeof(Action<int>), modInstance, onLevelWasLoadedMethod);
                }
                else if (method.Name.Equals("OnLevelWasInitialized") && method.GetParameters().Length == 1 && method.GetParameters()[0].ParameterType == typeof(int))
                {
                    onLevelWasInitializedMethod = method;
                    onLevelWasInitialized = (Action<int>)Delegate.CreateDelegate(typeof(Action<int>), modInstance, onLevelWasInitializedMethod);
                }
                else if (method.Name.Equals("OnUpdate") && (method.GetParameters().Length == 0))
                {
                    onUpdateMethod = method;
                    onUpdate = (Action)Delegate.CreateDelegate(typeof(Action), modInstance, onUpdateMethod);
                }
                else if (method.Name.Equals("OnFixedUpdate") && method.GetParameters().Length == 0)
                {
                    onFixedUpdateMethod = method;
                    onFixedUpdate = (Action)Delegate.CreateDelegate(typeof(Action), modInstance, onFixedUpdateMethod);
                }
                else if (method.Name.Equals("OnLateUpdate") && method.GetParameters().Length == 0)
                {
                    onLateUpdateMethod = method;
                    onLateUpdate = (Action)Delegate.CreateDelegate(typeof(Action), modInstance, onLateUpdateMethod);
                }
                else if (method.Name.Equals("OnGUI") && method.GetParameters().Length == 0)
                {
                    onGUIMethod = method;
                    onGUI = (Action)Delegate.CreateDelegate(typeof(Action), modInstance, onGUIMethod);
                }
                else if (method.Name.Equals("OnApplicationQuit") && (method.GetParameters().Length == 0))
                {
                    onApplicationQuitMethod = method;
                    onApplicationQuit = (Action)Delegate.CreateDelegate(typeof(Action), modInstance, onApplicationQuitMethod);
                }
                else if (method.Name.Equals("OnModSettingsApplied") && (method.GetParameters().Length == 0))
                {
                    onModSettingsAppliedMethod = method;
                    onModSettingsApplied = (Action)Delegate.CreateDelegate(typeof(Action), modInstance, onModSettingsAppliedMethod);
                }
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
    }
}