using System;
using System.Linq.Expressions;
using MelonLoader.Tomlyn.Model;

namespace MelonLoader.Preferences.Types
{
    internal class Integer : MelonPreferences_Entry
    {
        private static Type ReflectedType = typeof(int);
        private int Value;
        private int EditedValue;
        private int DefaultValue;

        internal static void Resolve(object sender, ResolveEventArgs args)
        {
            if ((args.Entry != null)
                || (args.ReflectedType == null)
                || (args.ReflectedType != ReflectedType))
                return;
            args.Entry = new Integer();
        }

        public override Type GetReflectedType() => ReflectedType;

        private void SetAndInvoke(int newval)
        {
            int oldval = Value;
            Value = EditedValue = newval;
            InvokeValueChangeCallbacks(oldval, newval);
        }

        public override T GetValue<T>()
        {
            if (typeof(T) != ReflectedType)
                throw new Exception(GetExceptionMessage("Get " + typeof(T).FullName + " Value from"));
            return Expression.Lambda<Func<T>>(Expression.Convert(Expression.Constant(Value), typeof(T))).Compile()();
        }
        public override void SetValue<T>(T val)
        {
            if (typeof(T) != ReflectedType)
                throw new Exception(GetExceptionMessage("Set " + typeof(T).FullName + " Value in"));
            SetAndInvoke(Expression.Lambda<Func<int>>(Expression.Convert(Expression.Constant(val), ReflectedType)).Compile()());
        }

        public override T GetEditedValue<T>()
        {
            if (typeof(T) != ReflectedType)
                throw new Exception(GetExceptionMessage("Get Edited " + typeof(T).FullName + " Value from"));
            return Expression.Lambda<Func<T>>(Expression.Convert(Expression.Constant(EditedValue), typeof(T))).Compile()();
        }
        public override void SetEditedValue<T>(T val)
        {
            if (typeof(T) != ReflectedType)
                throw new Exception(GetExceptionMessage("Set Edited " + typeof(T).FullName + " Value in"));
            EditedValue = Expression.Lambda<Func<int>>(Expression.Convert(Expression.Constant(val), ReflectedType)).Compile()();
        }

        public override T GetDefaultValue<T>()
        {
            if (typeof(T) != ReflectedType)
                throw new Exception(GetExceptionMessage("Get Default " + typeof(T).FullName + " Value from"));
            return Expression.Lambda<Func<T>>(Expression.Convert(Expression.Constant(DefaultValue), typeof(T))).Compile()();
        }
        public override void SetDefaultValue<T>(T val)
        {
            if (typeof(T) != ReflectedType)
                throw new Exception(GetExceptionMessage("Set Default " + typeof(T).FullName + " Value in"));
            DefaultValue = Expression.Lambda<Func<int>>(Expression.Convert(Expression.Constant(val), ReflectedType)).Compile()();
        }
        public override void ResetToDefault() => SetAndInvoke(DefaultValue);

        public override void Load(TomlObject obj)
        {
            switch (obj.Kind)
            {
                case ObjectKind.Boolean:
                    SetAndInvoke(((TomlBoolean)obj).Value ? 1 : 0);
                    goto default;
                case ObjectKind.Integer:
                    SetAndInvoke((int)((TomlInteger)obj).Value);
                    goto default;
                case ObjectKind.Float:
                    SetAndInvoke((int)((TomlFloat)obj).Value);
                    goto default;
                default:
                    break;
            }
        }

        public override TomlObject Save()
        {
            SetAndInvoke(EditedValue);
            return new TomlInteger(Value);
        }
    }
}
