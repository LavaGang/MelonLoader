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
        internal long DefaultValue_long { get; set; }
        internal double DefaultValue_double { get; set; }

        internal string Value_string { get; set; }
        internal bool Value_bool { get; set; }
        internal int Value_int { get; set; }
        internal float Value_float { get; set; }
        internal long Value_long { get; set; }
        internal double Value_double { get; set; }

        internal string ValueEdited_string { get; set; }
        internal bool ValueEdited_bool { get; set; }
        internal int ValueEdited_int { get; set; }
        internal float ValueEdited_float { get; set; }
        internal long ValueEdited_long { get; set; }
        internal double ValueEdited_double { get; set; }

        public enum TypeEnum
        {
            STRING,
            BOOL,
            INT,
            FLOAT,
            LONG,
            DOUBLE
        }
        public TypeEnum Type { get; internal set; }
        public string GetTypeName() =>
            ((Type == TypeEnum.STRING) ? "string"
                : (Type == TypeEnum.BOOL) ? "bool"
                : ((Type == TypeEnum.INT) ? "int"
                : ((Type == TypeEnum.FLOAT) ? "float"
                : ((Type == TypeEnum.LONG) ? "long"
                : ((Type == TypeEnum.DOUBLE) ? "double"
                : "UNKNOWN")))));
        private string GetExceptionMessage(string submsg) => ("Attempted to " + submsg + " " + Name + " when it is a " + GetTypeName() + "!");

        internal MelonPreferences_Entry(MelonPreferences_Category category, string name, string value, string displayname, bool hidden)
        {
            Category = category;
            Type = TypeEnum.STRING;
            Name = name;
            ValueEdited_string = Value_string = value;
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
            ValueEdited_bool = Value_bool = value;
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
            ValueEdited_int = Value_int = value;
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
            ValueEdited_float = Value_float = value;
            DefaultValue_float = Value_float;
            DisplayName = displayname;
            Hidden = hidden;
            Category.prefstbl.Add(this);
        }
        internal MelonPreferences_Entry(MelonPreferences_Category category, string name, long value, string displayname, bool hidden)
        {
            Category = category;
            Type = TypeEnum.LONG;
            Name = name;
            ValueEdited_long = Value_long = value;
            DefaultValue_long = Value_long;
            DisplayName = displayname;
            Hidden = hidden;
            Category.prefstbl.Add(this);
        }
        internal MelonPreferences_Entry(MelonPreferences_Category category, string name, double value, string displayname, bool hidden)
        {
            Category = category;
            Type = TypeEnum.DOUBLE;
            Name = name;
            ValueEdited_double = Value_double = value;
            DefaultValue_double = Value_double;
            DisplayName = displayname;
            Hidden = hidden;
            Category.prefstbl.Add(this);
        }

        public string GetString()
        {
            if (Type != TypeEnum.STRING)
                throw new Exception(GetExceptionMessage("Get string from")); 
            return Value_string;
        }
        public bool GetBool()
        {
            if (Type != TypeEnum.BOOL)
                throw new Exception(GetExceptionMessage("Get bool from"));
            return Value_bool;
        }
        public int GetInt()
        {
            if (Type != TypeEnum.INT)
                throw new Exception(GetExceptionMessage("Get int from"));
            return Value_int;
        }
        public float GetFloat()
        {
            if (Type != TypeEnum.FLOAT)
                throw new Exception(GetExceptionMessage("Get float from"));
            return Value_float;
        }
        public long GetLong()
        {
            if (Type != TypeEnum.LONG)
                throw new Exception(GetExceptionMessage("Get long from"));
            return Value_long;
        }
        public double GetDouble()
        {
            if (Type != TypeEnum.DOUBLE)
                throw new Exception(GetExceptionMessage("Get double from"));
            return Value_double;
        }

        public void SetString(string value)
        {
            if (Type != TypeEnum.STRING)
                throw new Exception(GetExceptionMessage("Set string in"));
            string oldval = Value_string;
            ValueEdited_string = Value_string = value;
            if (value != oldval)
                InvokeValueChangeCallbacks(oldval, value);
        }
        public void SetBool(bool value)
        {
            if (Type != TypeEnum.BOOL)
                throw new Exception(GetExceptionMessage("Set bool in"));
            bool oldval = Value_bool;
            ValueEdited_bool = Value_bool = value;
            if (value != oldval)
                InvokeValueChangeCallbacks(oldval, value);
        }
        public void SetInt(int value)
        {
            if (Type != TypeEnum.INT)
                throw new Exception(GetExceptionMessage("Set int in"));
            int oldval = Value_int;
            ValueEdited_int = Value_int = value;
            if (value != oldval)
                InvokeValueChangeCallbacks(oldval, value);
        }
        public void SetFloat(float value)
        {
            if (Type != TypeEnum.FLOAT)
                throw new Exception(GetExceptionMessage("Set float in"));
            float oldval = Value_float;
            ValueEdited_float = Value_float = value;
            if (value != oldval)
                InvokeValueChangeCallbacks(oldval, value);
        }
        public void SetLong(long value)
        {
            if (Type != TypeEnum.LONG)
                throw new Exception(GetExceptionMessage("Set long in"));
            long oldval = Value_long;
            ValueEdited_long = Value_long = value;
            if (value != oldval)
                InvokeValueChangeCallbacks(oldval, value);
        }
        public void SetDouble(double value)
        {
            if (Type != TypeEnum.DOUBLE)
                throw new Exception(GetExceptionMessage("Set double in"));
            double oldval = Value_double;
            ValueEdited_double = Value_double = value;
            if (value != oldval)
                InvokeValueChangeCallbacks(oldval, value);
        }

        public string GetEditedString()
        {
            if (Type != TypeEnum.STRING)
                throw new Exception(GetExceptionMessage("Get Edited string from"));
            return ValueEdited_string;
        }
        public bool GetEditedBool()
        {
            if (Type != TypeEnum.BOOL)
                throw new Exception(GetExceptionMessage("Get Edited bool from"));
            return ValueEdited_bool;
        }
        public int GetEditedInt()
        {
            if (Type != TypeEnum.INT)
                throw new Exception(GetExceptionMessage("Get Edited int from"));
            return ValueEdited_int;
        }
        public float GetEditedFloat()
        {
            if (Type != TypeEnum.FLOAT)
                throw new Exception(GetExceptionMessage("Get Edited float from"));
            return ValueEdited_float;
        }
        public long GetEditedLong()
        {
            if (Type != TypeEnum.LONG)
                throw new Exception(GetExceptionMessage("Get Edited long from"));
            return ValueEdited_long;
        }
        public double GetEditedDouble()
        {
            if (Type != TypeEnum.DOUBLE)
                throw new Exception(GetExceptionMessage("Get Edited double from"));
            return ValueEdited_double;
        }

        public void SetEditedString(string value)
        {
            if (Type != TypeEnum.STRING)
                throw new Exception(GetExceptionMessage("Set Edited string in"));
            ValueEdited_string = value;
        }
        public void SetEditedBool(bool value)
        {
            if (Type != TypeEnum.BOOL)
                throw new Exception(GetExceptionMessage("Set Edited bool in"));
            ValueEdited_bool = value;
        }
        public void SetEditedInt(int value)
        {
            if (Type != TypeEnum.INT)
                throw new Exception(GetExceptionMessage("Set Edited int in"));
            ValueEdited_int = value;
        }
        public void SetEditedFloat(float value)
        {
            if (Type != TypeEnum.FLOAT)
                throw new Exception(GetExceptionMessage("Set Edited float in"));
            ValueEdited_float = value;
        }
        public void SetEditedLong(long value)
        {
            if (Type != TypeEnum.LONG)
                throw new Exception(GetExceptionMessage("Set Edited long in"));
            ValueEdited_long = value;
        }
        public void SetEditedDouble(double value)
        {
            if (Type != TypeEnum.DOUBLE)
                throw new Exception(GetExceptionMessage("Set Edited double in"));
            ValueEdited_double = value;
        }

        public string GetDefaultString()
        {
            if (Type != TypeEnum.STRING)
                throw new Exception(GetExceptionMessage("Get Default string from"));
            return DefaultValue_string;
        }
        public bool GetDefaultBool()
        {
            if (Type != TypeEnum.BOOL)
                throw new Exception(GetExceptionMessage("Get Default bool from"));
            return DefaultValue_bool;
        }
        public int GetDefaultInt()
        {
            if (Type != TypeEnum.INT)
                throw new Exception(GetExceptionMessage("Get Default int from"));
            return DefaultValue_int;
        }
        public float GetDefaultFloat()
        {
            if (Type != TypeEnum.FLOAT)
                throw new Exception(GetExceptionMessage("Get Default float from"));
            return DefaultValue_float;
        }
        public long GetDefaultLong()
        {
            if (Type != TypeEnum.LONG)
                throw new Exception(GetExceptionMessage("Get Default long from"));
            return DefaultValue_long;
        }
        public double GetDefaultDouble()
        {
            if (Type != TypeEnum.DOUBLE)
                throw new Exception(GetExceptionMessage("Get Default double from"));
            return DefaultValue_double;
        }

        public void ResetToDefault()
        {
            if (Type == TypeEnum.STRING)
                Value_string = ValueEdited_string = DefaultValue_string;
            else if (Type == TypeEnum.BOOL)
                Value_bool = ValueEdited_bool = DefaultValue_bool;
            else if (Type == TypeEnum.INT)
                Value_int = ValueEdited_int = DefaultValue_int;
            else if (Type == TypeEnum.FLOAT)
                Value_float = ValueEdited_float = DefaultValue_float;
            else if (Type == TypeEnum.LONG)
                Value_long = ValueEdited_long = DefaultValue_long;
            else if (Type == TypeEnum.DOUBLE)
                Value_double = ValueEdited_double = DefaultValue_double;
        }

        private List<Delegate> OnValueChanged;
        public void AddValueChangeCallback<T>(Action<T, T> callback)
        {
            Type callback_type = typeof(T);
            Type string_type = typeof(string);
            Type bool_type = typeof(bool);
            Type int_type = typeof(int);
            Type float_type = typeof(float);
            Type long_type = typeof(long);
            Type double_type = typeof(double);
            if ((callback_type == string_type) && (Type != TypeEnum.STRING))
                throw new Exception(GetExceptionMessage("Add string Value Change Callback to"));
            else if ((callback_type == bool_type) && (Type != TypeEnum.BOOL))
                throw new Exception(GetExceptionMessage("Add bool Value Change Callback to"));
            else if ((callback_type == int_type) && (Type != TypeEnum.INT))
                throw new Exception(GetExceptionMessage("Add int Value Change Callback to"));
            else if ((callback_type == float_type) && (Type != TypeEnum.FLOAT))
                throw new Exception(GetExceptionMessage("Add float Value Change Callback to"));
            else if ((callback_type == long_type) && (Type != TypeEnum.LONG))
                throw new Exception(GetExceptionMessage("Add long Value Change Callback to"));
            else if ((callback_type == double_type) && (Type != TypeEnum.DOUBLE))
                throw new Exception(GetExceptionMessage("Add double Value Change Callback to"));
            else if ((callback_type != string_type) && (callback_type != bool_type) && (callback_type != int_type) && (callback_type != float_type) && (callback_type != long_type) && (callback_type != double_type))
                throw new Exception(GetExceptionMessage("Add " + nameof(T) + " Value Change Callback to"));
            if (OnValueChanged == null)
                OnValueChanged = new List<Delegate>();
            OnValueChanged.Add(callback);
        }
        public void RemoveValueChangeCallback<T>(Action<T, T> callback)
        {
            Type callback_type = typeof(T);
            Type string_type = typeof(string);
            Type bool_type = typeof(bool);
            Type int_type = typeof(int);
            Type float_type = typeof(float);
            Type long_type = typeof(long);
            Type double_type = typeof(double);
            if ((callback_type == string_type) && (Type != TypeEnum.STRING))
                throw new Exception(GetExceptionMessage("Remove string Value Change Callback from"));
            else if ((callback_type == bool_type) && (Type != TypeEnum.BOOL))
                throw new Exception(GetExceptionMessage("Remove bool Value Change Callback from"));
            else if ((callback_type == int_type) && (Type != TypeEnum.INT))
                throw new Exception(GetExceptionMessage("Remove int Value Change Callback from"));
            else if ((callback_type == float_type) && (Type != TypeEnum.FLOAT))
                throw new Exception(GetExceptionMessage("Remove float Value Change Callback from"));
            else if ((callback_type == long_type) && (Type != TypeEnum.LONG))
                throw new Exception(GetExceptionMessage("Remove long Value Change Callback from"));
            else if ((callback_type == double_type) && (Type != TypeEnum.DOUBLE))
                throw new Exception(GetExceptionMessage("Remove double Value Change Callback from"));
            else if ((callback_type != string_type) && (callback_type != bool_type) && (callback_type != int_type) && (callback_type != float_type))
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