using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using MelonLoader.Tomlyn;
using MelonLoader.Tomlyn.Model;
using MelonLoader.Tomlyn.Syntax;

namespace MelonLoader
{
    public class MelonPreferences_Entry
    {
        private string GetExceptionMessage(string submsg) => ("Attempted to " + submsg + " " + Name + " when it is a " + TypeName + "!");
        public MelonPreferences_Category Category { get; internal set; }
        public string Name { get; internal set; }
        public string DisplayName { get; internal set; }
        public bool Hidden { get; internal set; }
        public enum TypeEnum
        {
            STRING,
            BOOL,
            INT,
            FLOAT,
            LONG,
            DOUBLE,
            UNKNOWN
        }
        public TypeEnum Type { get; internal set; }
        public string TypeName { get => Preferences.TypeManager.TypeEnumToTypeName(Type); }

        internal string DefaultValue_string { get; set; }
        internal string Value_string { get; set; }
        internal string ValueEdited_string { get; set; }

        internal bool DefaultValue_bool { get; set; }
        internal bool Value_bool { get; set; }
        internal bool ValueEdited_bool { get; set; }

        internal int DefaultValue_int { get; set; }
        internal int Value_int { get; set; }
        internal int ValueEdited_int { get; set; }

        internal float DefaultValue_float { get; set; }
        internal float Value_float { get; set; }
        internal float ValueEdited_float { get; set; }

        internal long DefaultValue_long { get; set; }
        internal long Value_long { get; set; }
        internal long ValueEdited_long { get; set; }

        internal double DefaultValue_double { get; set; }
        internal double Value_double { get; set; }
        internal double ValueEdited_double { get; set; }

        public T GetValue<T>()
        {
            TypeEnum requestedType = Preferences.TypeManager.TypeToTypeEnum<T>();
            if ((requestedType == TypeEnum.UNKNOWN) || (requestedType != Type))
                throw new Exception(GetExceptionMessage("Get " + nameof(T) + " Value from"));
            return Preferences.TypeManager.GetValue<T>(this);
        }
        public void SetValue<T>(T value)
        {
            TypeEnum requestedType = Preferences.TypeManager.TypeToTypeEnum<T>();
            if ((requestedType == TypeEnum.UNKNOWN) || (requestedType != Type))
                throw new Exception(GetExceptionMessage("Set " + nameof(T) + " Value in"));
            T oldval = GetValue<T>();
            Preferences.TypeManager.SetValue(this, value);
            if (!oldval.Equals(value))
                InvokeValueChangeCallbacks(oldval, value);
        }

        public T GetEditedValue<T>()
        {
            TypeEnum requestedType = Preferences.TypeManager.TypeToTypeEnum<T>();
            if ((requestedType == TypeEnum.UNKNOWN) || (requestedType != Type))
                throw new Exception(GetExceptionMessage("Get Edited " + nameof(T) + " Value from"));
            return Preferences.TypeManager.GetEditedValue<T>(this);
        }
        public void SetEditedValue<T>(T value)
        {
            TypeEnum requestedType = Preferences.TypeManager.TypeToTypeEnum<T>();
            if ((requestedType == TypeEnum.UNKNOWN) || (requestedType != Type))
                throw new Exception(GetExceptionMessage("Set Edited " + nameof(T) + " Value in"));
            Preferences.TypeManager.SetEditedValue(this, value);
        }

        public T GetDefaultValue<T>()
        {
            TypeEnum requestedType = Preferences.TypeManager.TypeToTypeEnum<T>();
            if ((requestedType == TypeEnum.UNKNOWN) || (requestedType != Type))
                throw new Exception(GetExceptionMessage("Get Default " + nameof(T) + " Value from"));
            return Preferences.TypeManager.GetDefaultValue<T>(this);
        }

        public void ResetToDefault() => Preferences.TypeManager.ResetToDefault(this);

        private List<Delegate> OnValueChanged;
        public void AddValueChangeCallback<T>(Action<T, T> callback)
        {
            TypeEnum callback_typeenum = Preferences.TypeManager.TypeToTypeEnum<T>();
            if ((callback_typeenum == TypeEnum.UNKNOWN) || (callback_typeenum != Type))
                throw new Exception(GetExceptionMessage("Add " + nameof(T) + " Value Change Callback to"));
            if (OnValueChanged == null)
                OnValueChanged = new List<Delegate>();
            OnValueChanged.Add(callback);
        }
        public void RemoveValueChangeCallback<T>(Action<T, T> callback)
        {
            TypeEnum callback_typeenum = Preferences.TypeManager.TypeToTypeEnum<T>();
            if ((callback_typeenum == TypeEnum.UNKNOWN) || (callback_typeenum != Type))
                throw new Exception(GetExceptionMessage("Remove " + nameof(T) + " Value Change Callback from"));
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
        private void InvokeValueChangeCallbacks<T>(T old_value, T new_value)
        {
            if ((OnValueChanged == null) || (OnValueChanged.Count <= 0))
                return;
            foreach (Delegate callback in OnValueChanged)
                callback.DynamicInvoke(old_value, new_value);
        }
    }
}