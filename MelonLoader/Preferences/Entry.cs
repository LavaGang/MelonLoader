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
            DOUBLE,
            UNKNOWN
        }
        public TypeEnum Type { get; internal set; }
        public string TypeName { get => MelonPreferences.TypeEnumToTypeName(Type); }
        private string GetExceptionMessage(string submsg) => ("Attempted to " + submsg + " " + Name + " when it is a " + TypeName + "!");
        internal MelonPreferences_Entry() { }
        internal void Setup<T>(MelonPreferences_Category category, string name, T value, string displayname, bool hidden)
        {
            TypeEnum requestedType = MelonPreferences.TypeToTypeEnum<T>();
            if (requestedType == TypeEnum.UNKNOWN)
                throw new Exception("Tried to Setup MelonPreference with Invalid Type: " + nameof(T));
            Category = category;
            Name = name;
            DisplayName = displayname;
            Hidden = hidden;
            Type = requestedType;
            switch (Type)
            {
                case TypeEnum.STRING:
                    DefaultValue_string = ValueEdited_string = Value_string = Expression.Lambda<Func<string>>(Expression.Convert(Expression.Constant(value), typeof(string))).Compile()();
                    break;
                case TypeEnum.BOOL:
                    DefaultValue_bool = ValueEdited_bool = Value_bool = Expression.Lambda<Func<bool>>(Expression.Convert(Expression.Constant(value), typeof(bool))).Compile()();
                    break;
                case TypeEnum.INT:
                    DefaultValue_int = ValueEdited_int = Value_int = Expression.Lambda<Func<int>>(Expression.Convert(Expression.Constant(value), typeof(int))).Compile()();
                    break;
                case TypeEnum.FLOAT:
                    DefaultValue_float = ValueEdited_float = Value_float = Expression.Lambda<Func<float>>(Expression.Convert(Expression.Constant(value), typeof(float))).Compile()();
                    break;
                case TypeEnum.LONG:
                    DefaultValue_long = ValueEdited_long = Value_long = Expression.Lambda<Func<long>>(Expression.Convert(Expression.Constant(value), typeof(long))).Compile()();
                    break;
                case TypeEnum.DOUBLE:
                    DefaultValue_double = ValueEdited_double = Value_double = Expression.Lambda<Func<double>>(Expression.Convert(Expression.Constant(value), typeof(double))).Compile()();
                    break;
                default:
                    break;
            }
            Category.prefstbl.Add(this);
        }

        public T GetValue<T>()
        {
            TypeEnum requestedType = MelonPreferences.TypeToTypeEnum<T>();
            if (requestedType == TypeEnum.UNKNOWN)
                throw new Exception(GetExceptionMessage("Get " + nameof(T) + " Value from"));
            else if (requestedType != Type)
                throw new Exception(GetExceptionMessage("Get " + MelonPreferences.TypeEnumToTypeName(requestedType) + " Value from"));
            switch (requestedType)
            {
                case TypeEnum.STRING:
                    return Expression.Lambda<Func<T>>(Expression.Convert(Expression.Constant(Value_string), typeof(T))).Compile()();
                case TypeEnum.BOOL:
                    return Expression.Lambda<Func<T>>(Expression.Convert(Expression.Constant(Value_bool), typeof(T))).Compile()();
                case TypeEnum.INT:
                    return Expression.Lambda<Func<T>>(Expression.Convert(Expression.Constant(Value_int), typeof(T))).Compile()();
                case TypeEnum.FLOAT:
                    return Expression.Lambda<Func<T>>(Expression.Convert(Expression.Constant(Value_float), typeof(T))).Compile()();
                case TypeEnum.LONG:
                    return Expression.Lambda<Func<T>>(Expression.Convert(Expression.Constant(Value_long), typeof(T))).Compile()();
                case TypeEnum.DOUBLE:
                    return Expression.Lambda<Func<T>>(Expression.Convert(Expression.Constant(Value_double), typeof(T))).Compile()();
                default:
                    return default;
            }
        }

        public void SetValue<T>(T value)
        {
            TypeEnum requestedType = MelonPreferences.TypeToTypeEnum<T>();
            if (requestedType == TypeEnum.UNKNOWN)
                throw new Exception(GetExceptionMessage("Set " + nameof(T) + " Value in"));
            else if (requestedType != Type)
                throw new Exception(GetExceptionMessage("Set " + MelonPreferences.TypeEnumToTypeName(requestedType) + " Value in"));
            switch (requestedType)
            {
                case TypeEnum.STRING:
                    string oldval_string = Value_string;
                    Value_string = ValueEdited_string = Expression.Lambda<Func<string>>(Expression.Convert(Expression.Constant(value), typeof(string))).Compile()();
                    if (!oldval_string.Equals(Value_string))
                        InvokeValueChangeCallbacks(oldval_string, Value_string);
                    break;
                case TypeEnum.BOOL:
                    bool oldval_bool = Value_bool;
                    Value_bool = ValueEdited_bool = Expression.Lambda<Func<bool>>(Expression.Convert(Expression.Constant(value), typeof(bool))).Compile()();
                    if (!oldval_bool.Equals(Value_bool))
                        InvokeValueChangeCallbacks(oldval_bool, Value_bool);
                    break;
                case TypeEnum.INT:
                    int oldval_int = Value_int;
                    Value_int = ValueEdited_int = Expression.Lambda<Func<int>>(Expression.Convert(Expression.Constant(value), typeof(int))).Compile()();
                    if (!oldval_int.Equals(Value_int))
                        InvokeValueChangeCallbacks(oldval_int, Value_int);
                    break;
                case TypeEnum.FLOAT:
                    float oldval_float = Value_float;
                    Value_float = ValueEdited_float = Expression.Lambda<Func<float>>(Expression.Convert(Expression.Constant(value), typeof(float))).Compile()();
                    if (!oldval_float.Equals(Value_float))
                        InvokeValueChangeCallbacks(oldval_float, Value_float);
                    break;
                case TypeEnum.LONG:
                    long oldval_long = Value_long;
                    Value_long = ValueEdited_long = Expression.Lambda<Func<long>>(Expression.Convert(Expression.Constant(value), typeof(long))).Compile()();
                    if (!oldval_long.Equals(Value_long))
                        InvokeValueChangeCallbacks(oldval_long, Value_long);
                    break;
                case TypeEnum.DOUBLE:
                    double oldval_double = Value_double;
                    Value_double = ValueEdited_double = Expression.Lambda<Func<double>>(Expression.Convert(Expression.Constant(value), typeof(double))).Compile()();
                    if (!oldval_double.Equals(Value_double))
                        InvokeValueChangeCallbacks(oldval_double, Value_double);
                    break;
                default:
                    break;
            }
        }

        public T GetEditedValue<T>()
        {
            TypeEnum requestedType = MelonPreferences.TypeToTypeEnum<T>();
            if (requestedType == TypeEnum.UNKNOWN)
                throw new Exception(GetExceptionMessage("Get Edited " + nameof(T) + " Value from"));
            else if (requestedType != Type)
                throw new Exception(GetExceptionMessage("Get Edited " + MelonPreferences.TypeEnumToTypeName(requestedType) + " Value from"));
            switch (requestedType)
            {
                case TypeEnum.STRING:
                    return Expression.Lambda<Func<T>>(Expression.Convert(Expression.Constant(ValueEdited_string), typeof(T))).Compile()();
                case TypeEnum.BOOL:
                    return Expression.Lambda<Func<T>>(Expression.Convert(Expression.Constant(ValueEdited_bool), typeof(T))).Compile()();
                case TypeEnum.INT:
                    return Expression.Lambda<Func<T>>(Expression.Convert(Expression.Constant(ValueEdited_int), typeof(T))).Compile()();
                case TypeEnum.FLOAT:
                    return Expression.Lambda<Func<T>>(Expression.Convert(Expression.Constant(ValueEdited_float), typeof(T))).Compile()();
                case TypeEnum.LONG:
                    return Expression.Lambda<Func<T>>(Expression.Convert(Expression.Constant(ValueEdited_long), typeof(T))).Compile()();
                case TypeEnum.DOUBLE:
                    return Expression.Lambda<Func<T>>(Expression.Convert(Expression.Constant(ValueEdited_double), typeof(T))).Compile()();
                default:
                    return default;
            }
        }

        public void SetEditedValue<T>(T value)
        {
            TypeEnum requestedType = MelonPreferences.TypeToTypeEnum<T>();
            if (requestedType == TypeEnum.UNKNOWN)
                throw new Exception(GetExceptionMessage("Set Edited " + nameof(T) + " Value in"));
            else if (requestedType != Type)
                throw new Exception(GetExceptionMessage("Set Edited " + MelonPreferences.TypeEnumToTypeName(requestedType) + " Value in"));
            switch (requestedType)
            {
                case TypeEnum.STRING:
                    ValueEdited_string = Expression.Lambda<Func<string>>(Expression.Convert(Expression.Constant(value), typeof(string))).Compile()();
                    break;
                case TypeEnum.BOOL:
                    ValueEdited_bool = Expression.Lambda<Func<bool>>(Expression.Convert(Expression.Constant(value), typeof(bool))).Compile()();
                    break;
                case TypeEnum.INT:
                    ValueEdited_int = Expression.Lambda<Func<int>>(Expression.Convert(Expression.Constant(value), typeof(int))).Compile()();
                    break;
                case TypeEnum.FLOAT:
                    ValueEdited_float = Expression.Lambda<Func<float>>(Expression.Convert(Expression.Constant(value), typeof(float))).Compile()();
                    break;
                case TypeEnum.LONG:
                    ValueEdited_long = Expression.Lambda<Func<long>>(Expression.Convert(Expression.Constant(value), typeof(long))).Compile()();
                    break;
                case TypeEnum.DOUBLE:
                    ValueEdited_double = Expression.Lambda<Func<double>>(Expression.Convert(Expression.Constant(value), typeof(double))).Compile()();
                    break;
                default:
                    break;
            }
        }

        public T GetDefaultValue<T>()
        {
            TypeEnum requestedType = MelonPreferences.TypeToTypeEnum<T>();
            if (requestedType == TypeEnum.UNKNOWN)
                throw new Exception(GetExceptionMessage("Get Default " + nameof(T) + " Value from"));
            else if (requestedType != Type)
                throw new Exception(GetExceptionMessage("Get Default " + MelonPreferences.TypeEnumToTypeName(requestedType) + " Value from"));
            switch (requestedType)
            {
                case TypeEnum.STRING:
                    return Expression.Lambda<Func<T>>(Expression.Convert(Expression.Constant(DefaultValue_string), typeof(T))).Compile()();
                case TypeEnum.BOOL:
                    return Expression.Lambda<Func<T>>(Expression.Convert(Expression.Constant(DefaultValue_bool), typeof(T))).Compile()();
                case TypeEnum.INT:
                    return Expression.Lambda<Func<T>>(Expression.Convert(Expression.Constant(DefaultValue_int), typeof(T))).Compile()();
                case TypeEnum.FLOAT:
                    return Expression.Lambda<Func<T>>(Expression.Convert(Expression.Constant(DefaultValue_float), typeof(T))).Compile()();
                case TypeEnum.LONG:
                    return Expression.Lambda<Func<T>>(Expression.Convert(Expression.Constant(DefaultValue_long), typeof(T))).Compile()();
                case TypeEnum.DOUBLE:
                    return Expression.Lambda<Func<T>>(Expression.Convert(Expression.Constant(DefaultValue_double), typeof(T))).Compile()();
                default:
                    return default;
            }
        }

        public void ResetToDefault()
        {
            switch (Type)
            {
                case TypeEnum.STRING:
                    Value_string = ValueEdited_string = DefaultValue_string;
                    break;
                case TypeEnum.BOOL:
                    Value_bool = ValueEdited_bool = DefaultValue_bool;
                    break;
                case TypeEnum.INT:
                    Value_int = ValueEdited_int = DefaultValue_int;
                    break;
                case TypeEnum.FLOAT:
                    Value_float = ValueEdited_float = DefaultValue_float;
                    break;
                case TypeEnum.LONG:
                    Value_long = ValueEdited_long = DefaultValue_long;
                    break;
                case TypeEnum.DOUBLE:
                    Value_double = ValueEdited_double = DefaultValue_double;
                    break;
                default:
                    break;
            }
        }

        private List<Delegate> OnValueChanged;
        public void AddValueChangeCallback<T>(Action<T, T> callback)
        {
            TypeEnum callback_typeenum = MelonPreferences.TypeToTypeEnum<T>();
            if (callback_typeenum == TypeEnum.UNKNOWN)
                throw new Exception(GetExceptionMessage("Add " + nameof(T) + " Value Change Callback to"));
            else if (callback_typeenum != Type)
                throw new Exception(GetExceptionMessage("Add " + MelonPreferences.TypeEnumToTypeName(callback_typeenum) + " Value Change Callback to"));
            if (OnValueChanged == null)
                OnValueChanged = new List<Delegate>();
            OnValueChanged.Add(callback);
        }
        public void RemoveValueChangeCallback<T>(Action<T, T> callback)
        {
            TypeEnum callback_typeenum = MelonPreferences.TypeToTypeEnum<T>();
            if (callback_typeenum == TypeEnum.UNKNOWN)
                throw new Exception(GetExceptionMessage("Remove " + nameof(T) + " Value Change Callback from"));
            else if (callback_typeenum != Type)
                throw new Exception(GetExceptionMessage("Remove " + MelonPreferences.TypeEnumToTypeName(callback_typeenum) + " Value Change Callback from"));
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

        internal void ConvertCurrentValueType(TypeEnum requestedType)
        {
            if (Type == requestedType)
                return;
            switch (requestedType)
            {
                case TypeEnum.STRING:
                    string val_string = null;
                    switch (Type)
                    {
                        case TypeEnum.BOOL:
                            val_string = GetValue<bool>().ToString();
                            break;
                        case TypeEnum.INT:
                            val_string = GetValue<int>().ToString();
                            break;
                        case TypeEnum.FLOAT:
                            val_string = GetValue<float>().ToString();
                            break;
                        case TypeEnum.DOUBLE:
                            val_string = GetValue<double>().ToString();
                            break;
                        case TypeEnum.LONG:
                            val_string = GetValue<long>().ToString();
                            break;
                        default:
                            break;
                    }
                    Type = requestedType;
                    SetValue(val_string);
                    break;
                case TypeEnum.BOOL:
                    bool val_bool = false;
                    switch (Type)
                    {
                        case TypeEnum.INT:
                            val_bool = (GetValue<int>() != 0);
                            break;
                        case TypeEnum.FLOAT:
                            val_bool = (GetValue<float>() != 0f);
                            break;
                        case TypeEnum.DOUBLE:
                            val_bool = (GetValue<double>() != 0);
                            break;
                        case TypeEnum.LONG:
                            val_bool = (GetValue<long>() != 0);
                            break;
                        default:
                            break;
                    }
                    Type = requestedType;
                    SetValue(val_bool);
                    break;
                case TypeEnum.INT:
                    int val_int = 0;
                    switch (Type)
                    {
                        case TypeEnum.BOOL:
                            val_int = (GetValue<bool>() ? 1 : 0);
                            break;
                        case TypeEnum.FLOAT:
                            val_int = (int)GetValue<float>();
                            break;
                        case TypeEnum.LONG:
                            val_int = (int)GetValue<long>();
                            break;
                        case TypeEnum.DOUBLE:
                            val_int = (int)GetValue<double>();
                            break;
                        default:
                            break;
                    }
                    Type = requestedType;
                    SetValue(val_int);
                    break;
                case TypeEnum.FLOAT:
                    float val_float = 0f;
                    switch (Type)
                    {
                        case TypeEnum.BOOL:
                            val_float = (GetValue<bool>() ? 1f : 0f);
                            break;
                        case TypeEnum.INT:
                            val_float = GetValue<int>();
                            break;
                        case TypeEnum.LONG:
                            val_float = GetValue<long>();
                            break;
                        case TypeEnum.DOUBLE:
                            val_float = (float)GetValue<double>();
                            break;
                        default:
                            break;
                    }
                    Type = requestedType;
                    SetValue(val_float);
                    break;
                case TypeEnum.LONG:
                    long val_long = 0;
                    switch (Type)
                    {
                        case TypeEnum.BOOL:
                            val_long = (GetValue<bool>() ? 1 : 0);
                            break;
                        case TypeEnum.INT:
                            val_long = GetValue<int>();
                            break;
                        case TypeEnum.FLOAT:
                            val_long = (long)GetValue<float>();
                            break;
                        case TypeEnum.DOUBLE:
                            val_long = (long)GetValue<double>();
                            break;
                        default:
                            break;
                    }
                    Type = requestedType;
                    SetValue(val_long);
                    break;
                case TypeEnum.DOUBLE:
                    double val_double = 0f;
                    switch (Type)
                    {
                        case TypeEnum.BOOL:
                            val_double = (GetValue<bool>() ? 1f : 0f);
                            break;
                        case TypeEnum.INT:
                            val_double = GetValue<int>();
                            break;
                        case TypeEnum.FLOAT:
                            val_double = (long)GetValue<float>();
                            break;
                        case TypeEnum.LONG:
                            val_double = GetValue<long>();
                            break;
                        default:
                            break;
                    }
                    Type = requestedType;
                    SetValue(val_double);
                    break;
                default:
                    break;
            }
        }
    }
}