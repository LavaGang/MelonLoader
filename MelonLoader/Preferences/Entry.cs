using System;
using System.Collections.Generic;
using MelonLoader.Tomlyn.Model;

namespace MelonLoader
{
    public class MelonPreferences_Entry
    {
        public string Identifier { get; internal set; }
        public string DisplayName { get; internal set; }
        public bool IsHidden { get; internal set; }
        public MelonPreferences_Category Category { get; internal set; }

        public string GetExceptionMessage(string submsg) => ("Attempted to " + submsg + " " + DisplayName + " when it is a " + GetReflectedType().FullName + "!");

        public virtual Type GetReflectedType() => default;

        public virtual T GetValue<T>() => default;
        public virtual void SetValue<T>(T val) { }

        public virtual T GetEditedValue<T>() => default;
        public virtual void SetEditedValue<T>(T val) { }

        public virtual T GetDefaultValue<T>() => default;
        public virtual void SetDefaultValue<T>(T val) { }
        public virtual void ResetToDefault() { }

        public virtual void Load(TomlObject obj) { }
        public virtual TomlObject Save() => default;

        private List<Delegate> OnValueChanged;
        public void AddValueChangeCallback<T>(Action<T, T> callback)
        {
            if (typeof(T) != GetReflectedType())
                throw new Exception(GetExceptionMessage("Add " + typeof(T).FullName + " Value Change Callback to"));
            if (OnValueChanged == null)
                OnValueChanged = new List<Delegate>();
            OnValueChanged.Add(callback);
        }
        public void RemoveValueChangeCallback<T>(Action<T, T> callback)
        {
            if (typeof(T) != GetReflectedType())
                throw new Exception(GetExceptionMessage("Remove " + typeof(T).FullName + " Value Change Callback from"));
            if ((OnValueChanged == null) || (OnValueChanged.Count <= 0))
                return;
            OnValueChanged.Remove(callback);
        }
        public void ClearAllValueChangeCallbacks()
        {
            if ((OnValueChanged == null) || (OnValueChanged.Count <= 0))
                return;
            OnValueChanged.Clear();
        }
        public void InvokeValueChangeCallbacks<T>(T old_value, T new_value)
        {
            if (typeof(T) != GetReflectedType())
                throw new Exception(GetExceptionMessage("Invoke " + typeof(T).FullName + " Value Change Callback when it the Value is"));
            if ((OnValueChanged == null) || (OnValueChanged.Count <= 0))
                return;
            foreach (Delegate callback in OnValueChanged)
                callback.DynamicInvoke(old_value, new_value);
        }

        public class ResolveEventArgs : EventArgs
        {
            public Type ReflectedType { get; set; }
            public MelonPreferences_Entry Entry { get; set; }
        }
    }
}