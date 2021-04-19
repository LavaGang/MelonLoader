using System;
using Tomlet;
using Tomlet.Models;

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
        public abstract object BoxedEditedValue { get; set; }

        public Preferences.ValueValidator Validator { get; internal set; }

        public string GetExceptionMessage(string submsg) 
            => $"Attempted to {submsg} {DisplayName} when it is a {GetReflectedType().FullName}!";

        public abstract Type GetReflectedType();

        public abstract void ResetToDefault();

        public abstract string GetEditedValueAsString();
        public abstract string GetDefaultValueAsString();
        public abstract string GetValueAsString();

        public abstract void Load(TomlValue obj);
        public abstract TomlValue Save();

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

        public T EditedValue { get; set; }
        public T DefaultValue { get; set; }

        public override object BoxedValue
        {
            get => myValue;
            set => Value = (T)value;
        }

        public override object BoxedEditedValue
        {
            get => EditedValue;
            set => EditedValue = (T)value;
        }

        public override void ResetToDefault() => Value = DefaultValue;

        public event Action<T, T> OnValueChanged;

        public override Type GetReflectedType() => typeof(T);

        public override string GetEditedValueAsString() => EditedValue?.ToString();
        public override string GetDefaultValueAsString() => DefaultValue?.ToString();
        public override string GetValueAsString() => Value?.ToString();

        public override void Load(TomlValue obj)
        {
            Value = TomletMain.To<T>(obj);
        }

        public override TomlValue Save()
        {
            Value = EditedValue;
            return TomletMain.ValueFrom(Value);
        }
    }
}