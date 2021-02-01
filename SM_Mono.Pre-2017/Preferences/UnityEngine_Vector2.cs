using System;
using System.Linq.Expressions;
using MelonLoader.Tomlyn.Model;

namespace MelonLoader.Preferences.Types
{
    internal class UnityEngine_Vector2 : MelonPreferences_Entry
    {
        private static Type ReflectedType = typeof(UnityEngine.Vector2);
        private UnityEngine.Vector2 Value;
        private UnityEngine.Vector2 EditedValue;
        private UnityEngine.Vector2 DefaultValue;

        internal static void Resolve(object sender, ResolveEventArgs args)
        {
            if ((args.Entry != null)
                || (args.ReflectedType == null)
                || (args.ReflectedType != ReflectedType))
                return;
            args.Entry = new UnityEngine_Vector2();
        }

        public override Type GetReflectedType() => ReflectedType;

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
            UnityEngine.Vector2 oldval = Value;
            Value = EditedValue = Expression.Lambda<Func<UnityEngine.Vector2>>(Expression.Convert(Expression.Constant(val), ReflectedType)).Compile()();
            InvokeValueChangeCallbacks(oldval, Value);
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
            EditedValue = Expression.Lambda<Func<UnityEngine.Vector2>>(Expression.Convert(Expression.Constant(val), ReflectedType)).Compile()();
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
            DefaultValue = Expression.Lambda<Func<UnityEngine.Vector2>>(Expression.Convert(Expression.Constant(val), ReflectedType)).Compile()();
        }
        public override void ResetToDefault()
        {
            UnityEngine.Vector2 oldval = Value;
            Value = EditedValue = DefaultValue;
            InvokeValueChangeCallbacks(oldval, Value);
        }

        public override void Load(TomlObject obj)
        {
            if (obj.Kind != ObjectKind.Array)
                return;
            TomlArray arr = (TomlArray)obj;
            if (arr.Count <= 0)
                return;
            TomlObject obj2 = arr.GetTomlObject(0);
            if (obj2.Kind != ObjectKind.Float)
                return;
            float[] farr = arr.ToArray<float>();
            if (farr.Length != 2)
                return;
            SetValue(new UnityEngine.Vector2(farr[0], farr[1]));
        }

        public override TomlObject Save()
        {
            UnityEngine.Vector2 oldval = Value;
            Value = EditedValue;
            InvokeValueChangeCallbacks(oldval, Value);
            TomlArray arr = new TomlArray();
            arr.Add(Value.x);
            arr.Add(Value.y);
            return arr;
        }
    }
}
