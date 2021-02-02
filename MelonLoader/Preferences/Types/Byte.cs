using System;
using System.Linq.Expressions;
using MelonLoader.Tomlyn.Model;

namespace MelonLoader.Preferences.Types
{
    internal class Byte : MelonPreferences_Entry
    {
        private static Type ReflectedType = typeof(byte);
        private byte Value;
        private byte EditedValue;
        private byte DefaultValue;

        internal static void Resolve(object sender, ResolveEventArgs args)
        {
            if ((args.Entry != null)
                || (args.ReflectedType == null)
                || (args.ReflectedType != ReflectedType))
                return;
            args.Entry = new Byte();
        }

        public override Type GetReflectedType() => ReflectedType;

        private void SetAndInvoke(byte newval)
        {
            byte oldval = Value;
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
            SetAndInvoke(Expression.Lambda<Func<byte>>(Expression.Convert(Expression.Constant(val), ReflectedType)).Compile()());
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
            EditedValue = Expression.Lambda<Func<byte>>(Expression.Convert(Expression.Constant(val), ReflectedType)).Compile()();
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
            DefaultValue = Expression.Lambda<Func<byte>>(Expression.Convert(Expression.Constant(val), ReflectedType)).Compile()();
        }
        public override void ResetToDefault() => SetAndInvoke(DefaultValue);

        public override void Load(TomlObject obj)
        {
            switch (obj.Kind)
            {
                case ObjectKind.Boolean:
                    SetAndInvoke((byte)(((TomlBoolean)obj).Value ? 1 : 0));
                    goto default;
                case ObjectKind.Integer:
                    SetAndInvoke((byte)((TomlInteger)obj).Value);
                    goto default;
                case ObjectKind.Float:
                    SetAndInvoke((byte)((TomlFloat)obj).Value);
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
