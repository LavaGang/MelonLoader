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
        public string TypeName { get => Preferences.TypeManager.TypeEnumToTypeName(Type); }
        private string GetExceptionMessage(string submsg) => ("Attempted to " + submsg + " " + Name + " when it is a " + TypeName + "!");
        internal MelonPreferences_Entry() { }

        public T GetValue<T>()
        {
            TypeEnum requestedType = Preferences.TypeManager.TypeToTypeEnum<T>();
            if (requestedType == TypeEnum.UNKNOWN)
                throw new Exception(GetExceptionMessage("Get " + nameof(T) + " Value from"));
            else if (requestedType != Type)
                throw new Exception(GetExceptionMessage("Get " + Preferences.TypeManager.TypeEnumToTypeName(requestedType) + " Value from"));
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
            TypeEnum requestedType = Preferences.TypeManager.TypeToTypeEnum<T>();
            if (requestedType == TypeEnum.UNKNOWN)
                throw new Exception(GetExceptionMessage("Set " + nameof(T) + " Value in"));
            else if (requestedType != Type)
                throw new Exception(GetExceptionMessage("Set " + Preferences.TypeManager.TypeEnumToTypeName(requestedType) + " Value in"));
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
            TypeEnum requestedType = Preferences.TypeManager.TypeToTypeEnum<T>();
            if (requestedType == TypeEnum.UNKNOWN)
                throw new Exception(GetExceptionMessage("Get Edited " + nameof(T) + " Value from"));
            else if (requestedType != Type)
                throw new Exception(GetExceptionMessage("Get Edited " + Preferences.TypeManager.TypeEnumToTypeName(requestedType) + " Value from"));
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
            TypeEnum requestedType = Preferences.TypeManager.TypeToTypeEnum<T>();
            if (requestedType == TypeEnum.UNKNOWN)
                throw new Exception(GetExceptionMessage("Set Edited " + nameof(T) + " Value in"));
            else if (requestedType != Type)
                throw new Exception(GetExceptionMessage("Set Edited " + Preferences.TypeManager.TypeEnumToTypeName(requestedType) + " Value in"));
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
            TypeEnum requestedType = Preferences.TypeManager.TypeToTypeEnum<T>();
            if (requestedType == TypeEnum.UNKNOWN)
                throw new Exception(GetExceptionMessage("Get Default " + nameof(T) + " Value from"));
            else if (requestedType != Type)
                throw new Exception(GetExceptionMessage("Get Default " + Preferences.TypeManager.TypeEnumToTypeName(requestedType) + " Value from"));
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
            TypeEnum callback_typeenum = Preferences.TypeManager.TypeToTypeEnum<T>();
            if (callback_typeenum == TypeEnum.UNKNOWN)
                throw new Exception(GetExceptionMessage("Add " + nameof(T) + " Value Change Callback to"));
            else if (callback_typeenum != Type)
                throw new Exception(GetExceptionMessage("Add " + Preferences.TypeManager.TypeEnumToTypeName(callback_typeenum) + " Value Change Callback to"));
            if (OnValueChanged == null)
                OnValueChanged = new List<Delegate>();
            OnValueChanged.Add(callback);
        }
        public void RemoveValueChangeCallback<T>(Action<T, T> callback)
        {
            TypeEnum callback_typeenum = Preferences.TypeManager.TypeToTypeEnum<T>();
            if (callback_typeenum == TypeEnum.UNKNOWN)
                throw new Exception(GetExceptionMessage("Remove " + nameof(T) + " Value Change Callback from"));
            else if (callback_typeenum != Type)
                throw new Exception(GetExceptionMessage("Remove " + Preferences.TypeManager.TypeEnumToTypeName(callback_typeenum) + " Value Change Callback from"));
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