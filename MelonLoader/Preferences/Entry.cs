using System;
using System.Collections.Generic;

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
            UNKNOWN = -1,
            STRING = 0,
            BOOL = 1,
            INT = 2,
            FLOAT = 3,
            LONG = 4,
            DOUBLE = 5,
            BYTE = 6,
            STRING_ARRAY = 7,
            BOOL_ARRAY = 8,
            INT_ARRAY = 9,
            FLOAT_ARRAY = 10,
            LONG_ARRAY = 11,
            DOUBLE_ARRAY = 12,
            BYTE_ARRAY = 13
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

        internal byte DefaultValue_byte { get; set; }
        internal byte Value_byte { get; set; }
        internal byte ValueEdited_byte { get; set; }

        internal string[] DefaultValue_array_string { get; set; }
        internal string[] Value_array_string { get; set; }
        internal string[] ValueEdited_array_string { get; set; }

        internal bool[] DefaultValue_array_bool { get; set; }
        internal bool[] Value_array_bool { get; set; }
        internal bool[] ValueEdited_array_bool { get; set; }

        internal int[] DefaultValue_array_int { get; set; }
        internal int[] Value_array_int { get; set; }
        internal int[] ValueEdited_array_int { get; set; }

        internal float[] DefaultValue_array_float { get; set; }
        internal float[] Value_array_float { get; set; }
        internal float[] ValueEdited_array_float { get; set; }

        internal long[] DefaultValue_array_long { get; set; }
        internal long[] Value_array_long { get; set; }
        internal long[] ValueEdited_array_long { get; set; }

        internal double[] DefaultValue_array_double { get; set; }
        internal double[] Value_array_double { get; set; }
        internal double[] ValueEdited_array_double { get; set; }

        internal byte[] DefaultValue_array_byte { get; set; }
        internal byte[] Value_array_byte { get; set; }
        internal byte[] ValueEdited_array_byte { get; set; }

        public T GetValue<T>()
        {
            TypeEnum requestedType = Preferences.TypeManager.TypeToTypeEnum<T>();
            if ((requestedType == TypeEnum.UNKNOWN) || (requestedType != Type))
                throw new Exception(GetExceptionMessage("Get " + typeof(T).FullName + " Value from"));
            return Preferences.TypeManager.GetValue<T>(this);
        }
        public void SetValue<T>(T value)
        {
            TypeEnum requestedType = Preferences.TypeManager.TypeToTypeEnum<T>();
            if ((requestedType == TypeEnum.UNKNOWN) || (requestedType != Type))
                throw new Exception(GetExceptionMessage("Set " + typeof(T).FullName + " Value in"));
            T oldval = GetValue<T>();
            Preferences.TypeManager.SetValue(this, value);
            if (!oldval.Equals(value))
                InvokeValueChangeCallbacks(oldval, value);
        }

        public T GetEditedValue<T>()
        {
            TypeEnum requestedType = Preferences.TypeManager.TypeToTypeEnum<T>();
            if ((requestedType == TypeEnum.UNKNOWN) || (requestedType != Type))
                throw new Exception(GetExceptionMessage("Get Edited " + typeof(T).FullName + " Value from"));
            return Preferences.TypeManager.GetEditedValue<T>(this);
        }
        public void SetEditedValue<T>(T value)
        {
            TypeEnum requestedType = Preferences.TypeManager.TypeToTypeEnum<T>();
            if ((requestedType == TypeEnum.UNKNOWN) || (requestedType != Type))
                throw new Exception(GetExceptionMessage("Set Edited " + typeof(T).FullName + " Value in"));
            Preferences.TypeManager.SetEditedValue(this, value);
        }

        public T GetDefaultValue<T>()
        {
            TypeEnum requestedType = Preferences.TypeManager.TypeToTypeEnum<T>();
            if ((requestedType == TypeEnum.UNKNOWN) || (requestedType != Type))
                throw new Exception(GetExceptionMessage("Get Default " + typeof(T).FullName + " Value from"));
            return Preferences.TypeManager.GetDefaultValue<T>(this);
        }

        public void ResetToDefault() => Preferences.TypeManager.ResetToDefault(this);

        private List<Delegate> OnValueChanged;
        public void AddValueChangeCallback<T>(Action<T, T> callback)
        {
            TypeEnum callback_typeenum = Preferences.TypeManager.TypeToTypeEnum<T>();
            if ((callback_typeenum == TypeEnum.UNKNOWN) || (callback_typeenum != Type))
                throw new Exception(GetExceptionMessage("Add " + typeof(T).FullName + " Value Change Callback to"));
            if (OnValueChanged == null)
                OnValueChanged = new List<Delegate>();
            OnValueChanged.Add(callback);
        }
        public void RemoveValueChangeCallback<T>(Action<T, T> callback)
        {
            TypeEnum callback_typeenum = Preferences.TypeManager.TypeToTypeEnum<T>();
            if ((callback_typeenum == TypeEnum.UNKNOWN) || (callback_typeenum != Type))
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
        private void InvokeValueChangeCallbacks<T>(T old_value, T new_value)
        {
            if ((OnValueChanged == null) || (OnValueChanged.Count <= 0))
                return;
            foreach (Delegate callback in OnValueChanged)
                callback.DynamicInvoke(old_value, new_value);
        }
    }
}