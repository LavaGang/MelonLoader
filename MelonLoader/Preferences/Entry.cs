using System;
using MelonLoader.Tomlyn.Model;

namespace MelonLoader
{
    public abstract class MelonPreferences_Entry
    {
        public string Identifier { get; internal set; }
        public string DisplayName { get; internal set; }
        public string Description { get; internal set; }
        public bool IsHidden { get; internal set; }
        public bool DontSaveDefault { get; internal set; }
        public MelonPreferences_Category Category { get; internal set; }

        public abstract object BoxedValue { get; set; }
        public ValueValidator Validator { get; internal set; }

        public string GetExceptionMessage(string submsg) 
            => $"Attempted to {submsg} {DisplayName} when it is a {GetReflectedType().FullName}!";

        public abstract Type GetReflectedType();

        public abstract void ResetToDefault();

        public abstract string GetEditedValueAsString();
        public abstract string GetDefaultValueAsString();
        public abstract string GetValueAsString();

        public abstract void Load(TomlObject obj);
        public abstract TomlObject Save();

        public event Action OnValueChangedUntyped;

        protected void FireUntypedValueChanged() => OnValueChangedUntyped?.Invoke();
    }

    public class MelonPreferences_Entry<T> : MelonPreferences_Entry
    {
        private T myValue;
        public T Value
        {
            get => myValue;
            set
            {
                if (Validator != null)
                    value = (T)Validator.EnsureValid(value);

                if ((myValue == null && value == null) || (myValue != null && myValue.Equals(value)))
                    return;

                var old = myValue;
                myValue = value;
                EditedValue = myValue;
                OnValueChanged?.Invoke(old, value);
                FireUntypedValueChanged();
            } 
        }

        public override object BoxedValue
        {
            get => myValue;
            set => Value = (T)value;
        }

        public T EditedValue { get; set; }
        public T DefaultValue { get; set; }

        public override void ResetToDefault() => Value = DefaultValue;

        public event Action<T, T> OnValueChanged;

        public override Type GetReflectedType() => typeof(T);

        public override string GetEditedValueAsString() => EditedValue?.ToString();
        public override string GetDefaultValueAsString() => DefaultValue?.ToString();
        public override string GetValueAsString() => Value?.ToString();

        public override void Load(TomlObject obj)
        {
            Value = MelonPreferences.Mapper.FromToml<T>(obj);
        }

        public override TomlObject Save()
        {
            Value = EditedValue;
            return MelonPreferences.Mapper.ToToml(Value);
        }
    }

    public abstract class ValueValidator
    {
        public abstract bool IsValid(object value);

        public abstract object EnsureValid(object value);
    }

    public interface IValueRange
    {
        object MinValue { get; }
        object MaxValue { get; }
    }
    
    public class ValueRange<T> : ValueValidator, IValueRange where T : IComparable
    {
        public T MinValue { get; }
        public T MaxValue { get; }

        public ValueRange(T minValue, T maxValue)
        {
            if (minValue.CompareTo(maxValue) >= 0)
                throw new ArgumentException($"Min value ({minValue}) must be less than max value ({maxValue})!");

            MinValue = minValue;
            MaxValue = maxValue;
        }

        public override bool IsValid(object value)
        {
            return MaxValue.CompareTo(value) >= 0 && MinValue.CompareTo(value) <= 0;
        }

        public override object EnsureValid(object value)
        {
            if (MaxValue.CompareTo(value) < 0)
                return MaxValue;
            if (MinValue.CompareTo(value) > 0)
                return MinValue;
            return value;
        }

        object IValueRange.MinValue => MinValue;
        object IValueRange.MaxValue => MaxValue;
    }
}