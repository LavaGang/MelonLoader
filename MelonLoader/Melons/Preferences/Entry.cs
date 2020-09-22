using System;
using System.Collections.Generic;

namespace MelonLoader
{
    public class MelonPreferences_Entry
    {
        public MelonPreferences_Category Category { get; internal set; }
        public string Name { get; internal set; }
        public string DisplayName { get; internal set; }
        public bool Hidden { get; internal set; }

        internal string DefaultValue_string { get; set; }
        internal bool DefaultValue_bool { get; set; }
        internal int DefaultValue_int { get; set; }
        internal float DefaultValue_float { get; set; }

        internal string Value_string { get; set; }
        internal bool Value_bool { get; set; }
        internal int Value_int { get; set; }
        internal float Value_float { get; set; }

        public enum TypeEnum
        {
            STRING,
            BOOL,
            INT,
            FLOAT
        }
        public TypeEnum Type { get; internal set; }
        public string GetTypeName() =>
            ((Type == TypeEnum.STRING) ? "string"
                : (Type == TypeEnum.BOOL) ? "bool"
                : ((Type == TypeEnum.INT) ? "int"
                : ((Type == TypeEnum.FLOAT) ? "float"
                : "UNKNOWN")));
        private string GetExceptionMessage(string submsg) => ("Attempted to " + submsg + " " + Name + " when it is a " + GetTypeName() + "!");

        internal MelonPreferences_Entry(MelonPreferences_Category category, string name, string value, string displayname, bool hidden)
        {
            Category = category;
            Type = TypeEnum.STRING;
            Name = name;
            Value_string = value;
            DefaultValue_string = Value_string;
            DisplayName = displayname;
            Hidden = hidden;
            Category.prefstbl.Add(this);
        }
        internal MelonPreferences_Entry(MelonPreferences_Category category, string name, bool value, string displayname, bool hidden)
        {
            Category = category;
            Type = TypeEnum.BOOL;
            Name = name;
            Value_bool = value;
            DefaultValue_bool = Value_bool;
            DisplayName = displayname;
            Hidden = hidden;
            Category.prefstbl.Add(this);
        }
        internal MelonPreferences_Entry(MelonPreferences_Category category, string name, int value, string displayname, bool hidden)
        {
            Category = category;
            Type = TypeEnum.INT;
            Name = name;
            Value_int = value;
            DefaultValue_int = Value_int;
            DisplayName = displayname;
            Hidden = hidden;
            Category.prefstbl.Add(this);
        }
        internal MelonPreferences_Entry(MelonPreferences_Category category, string name, float value, string displayname, bool hidden)
        {
            Category = category;
            Type = TypeEnum.FLOAT;
            Name = name;
            Value_float = value;
            DefaultValue_float = Value_float;
            DisplayName = displayname;
            Hidden = hidden;
            Category.prefstbl.Add(this);
        }

        public string GetString()
        {
            if (Type != TypeEnum.STRING)
                throw new Exception("Attempted to Get string from " + Name + " when it is a " + GetTypeName() + "!");
            return Value_string;
        }
        public bool GetBool()
        {
            if (Type != TypeEnum.BOOL)
                throw new Exception("Attempted to Get bool from " + Name + " when it is a " + GetTypeName() + "!");
            return Value_bool;
        }
        public int GetInt()
        {
            if (Type != TypeEnum.INT)
                throw new Exception("Attempted to Get int from " + Name + " when it is a " + GetTypeName() + "!");
            return Value_int;
        }
        public float GetFloat()
        {
            if (Type != TypeEnum.FLOAT)
                throw new Exception("Attempted to Get float from " + Name + " when it is a " + GetTypeName() + "!");
            return Value_float;
        }

        public void SetString(string value)
        {
            if (Type != TypeEnum.STRING)
                throw new Exception("Attempted to Set string in " + Name + " when it is a " + GetTypeName() + "!");
            string oldval = Value_string;
            Value_string = value;
            InvokeValueChangeCallbacks(oldval, value);
        }
        public void SetBool(bool value)
        {
            if (Type != TypeEnum.BOOL)
                throw new Exception("Attempted to Set bool in " + Name + " when it is a " + GetTypeName() + "!");
            bool oldval = Value_bool;
            Value_bool = value;
            InvokeValueChangeCallbacks(oldval, value);
        }
        public void SetInt(int value)
        {
            if (Type != TypeEnum.INT)
                throw new Exception("Attempted to Set int in " + Name + " when it is a " + GetTypeName() + "!");
            int oldval = Value_int;
            Value_int = value;
            InvokeValueChangeCallbacks(oldval, value);
        }
        public void SetFloat(float value)
        {
            if (Type != TypeEnum.FLOAT)
                throw new Exception("Attempted to Set float in " + Name + " when it is a " + GetTypeName() + "!");
            float oldval = Value_float;
            Value_float = value;
            InvokeValueChangeCallbacks(oldval, value);
        }
        
        public string GetDefaultString()
        {
            if (Type != TypeEnum.STRING)
                throw new Exception("Attempted to Get Default string from " + Name + " when it is a " + GetTypeName() + "!");
            return DefaultValue_string;
        }
        public bool GetDefaultBool()
        {
            if (Type != TypeEnum.BOOL)
                throw new Exception("Attempted to Get Default bool from " + Name + " when it is a " + GetTypeName() + "!");
            return DefaultValue_bool;
        }
        public int GetDefaultInt()
        {
            if (Type != TypeEnum.INT)
                throw new Exception("Attempted to Get Default int from " + Name + " when it is a " + GetTypeName() + "!");
            return DefaultValue_int;
        }
        public float GetDefaultFloat()
        {
            if (Type != TypeEnum.FLOAT)
                throw new Exception("Attempted to Get Default float from " + Name + " when it is a " + GetTypeName() + "!");
            return DefaultValue_float;
        }

        public void ResetToDefault()
        {
            if (Type == TypeEnum.STRING)
                Value_string = DefaultValue_string;
            else if (Type == TypeEnum.BOOL)
                Value_bool = DefaultValue_bool;
            else if (Type == TypeEnum.INT)
                Value_int = DefaultValue_int;
            else if (Type == TypeEnum.FLOAT)
                Value_float = DefaultValue_float;
        }

        private List<Delegate> OnValueChanged;
        public void AddValueChangeCallback<T>(Action<T, T> callback)
        {
            Type callback_type = typeof(T);
            if ((callback_type == typeof(string)) && (Type != TypeEnum.STRING))
                throw new Exception("Attempted to Add string Value Change Callback to " + Name + " when it is a " + GetTypeName() + "!");
            else if ((callback_type == typeof(bool)) && (Type != TypeEnum.BOOL))
                throw new Exception("Attempted to Add bool Value Change Callback to " + Name + " when it is a " + GetTypeName() + "!");
            else if ((callback_type == typeof(int)) && (Type != TypeEnum.INT))
                throw new Exception("Attempted to Add int Value Change Callback to " + Name + " when it is a " + GetTypeName() + "!");
            else if ((callback_type == typeof(float)) && (Type != TypeEnum.FLOAT))
                throw new Exception("Attempted to Add float Value Change Callback to " + Name + " when it is a " + GetTypeName() + "!");
            if (OnValueChanged == null)
                OnValueChanged = new List<Delegate>();
            OnValueChanged.Add(callback);
        }

        public void RemoveValueChangeCallback<T>(Action<T, T> callback)
        {
            Type callback_type = typeof(T);
            if ((callback_type == typeof(string)) && (Type != TypeEnum.STRING))
                throw new Exception("Attempted to Remove string Value Change Callback from " + Name + " when it is a " + GetTypeName() + "!");
            else if ((callback_type == typeof(bool)) && (Type != TypeEnum.BOOL))
                throw new Exception("Attempted to Remove bool Value Change Callback from " + Name + " when it is a " + GetTypeName() + "!");
            else if ((callback_type == typeof(int)) && (Type != TypeEnum.INT))
                throw new Exception("Attempted to Remove int Value Change Callback from " + Name + " when it is a " + GetTypeName() + "!");
            else if ((callback_type == typeof(float)) && (Type != TypeEnum.FLOAT))
                throw new Exception("Attempted to Remove float Value Change Callback from " + Name + " when it is a " + GetTypeName() + "!");
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