using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using MelonLoader.Tomlyn;
using MelonLoader.Tomlyn.Model;
using MelonLoader.Tomlyn.Syntax;

namespace MelonLoader.Preferences.Types
{
    internal class LongParser : TypeParser
    {
        private static string TypeName = "long";
        private static Type ReflectedType = typeof(long);
        private static MelonPreferences_Entry.TypeEnum TypeEnum = MelonPreferences_Entry.TypeEnum.LONG;

        public static void Resolve(object sender, TypeManager.TypeParserArgs args)
        {
            if (((args.ReflectedType != null) && (args.ReflectedType == ReflectedType))
                || ((args.TypeEnum != MelonPreferences_Entry.TypeEnum.UNKNOWN) && (args.TypeEnum == TypeEnum)))
                args.TypeParser = new LongParser();
        }

        internal override void Construct<T>(MelonPreferences_Entry entry, T value) =>
            entry.DefaultValue_long = entry.ValueEdited_long = entry.Value_long = Expression.Lambda<Func<long>>(Expression.Convert(Expression.Constant(value), ReflectedType)).Compile()();

        internal override KeyValueSyntax Save(MelonPreferences_Entry entry)
        {
            entry.SetValue(entry.GetEditedValue<long>());
            return new KeyValueSyntax(entry.Name, new IntegerValueSyntax(entry.GetValue<long>()));
        }

        internal override void Load(MelonPreferences_Entry entry, TomlObject obj)
        {
            switch (obj.Kind)
            {
                case ObjectKind.Boolean:
                    entry.SetValue<long>(((TomlBoolean)obj).Value ? 1 : 0);
                    break;
                case ObjectKind.Integer:
                    entry.SetValue(((TomlInteger)obj).Value);
                    break;
                case ObjectKind.Float:
                    entry.SetValue((long)((TomlFloat)obj).Value);
                    break;
                default:
                    break;
            }
        }

        internal override void ConvertCurrentValueType(MelonPreferences_Entry entry)
        {
            long val_long = 0;
            switch (entry.Type)
            {
                case MelonPreferences_Entry.TypeEnum.BOOL:
                    val_long = (entry.GetValue<bool>() ? 1 : 0);
                    break;
                case MelonPreferences_Entry.TypeEnum.INT:
                    val_long = entry.GetValue<int>();
                    break;
                case MelonPreferences_Entry.TypeEnum.FLOAT:
                    val_long = (long)entry.GetValue<float>();
                    break;
                case MelonPreferences_Entry.TypeEnum.DOUBLE:
                    val_long = (long)entry.GetValue<double>();
                    break;
                default:
                    break;
            }
            entry.Type = TypeEnum;
            entry.SetValue(val_long);
        }

        internal override void ResetToDefault(MelonPreferences_Entry entry) =>
            entry.SetValue(entry.DefaultValue_long);

        internal override T GetValue<T>(MelonPreferences_Entry entry) =>
            Expression.Lambda<Func<T>>(Expression.Convert(Expression.Constant(entry.Value_long), typeof(T))).Compile()();
        internal override void SetValue<T>(MelonPreferences_Entry entry, T value) =>
            entry.Value_long = entry.ValueEdited_long = Expression.Lambda<Func<long>>(Expression.Convert(Expression.Constant(value), typeof(long))).Compile()();

        internal override T GetEditedValue<T>(MelonPreferences_Entry entry) =>
            Expression.Lambda<Func<T>>(Expression.Convert(Expression.Constant(entry.ValueEdited_long), typeof(T))).Compile()();
        internal override void SetEditedValue<T>(MelonPreferences_Entry entry, T value) =>
            entry.ValueEdited_long = Expression.Lambda<Func<long>>(Expression.Convert(Expression.Constant(value), typeof(long))).Compile()();

        internal override T GetDefaultValue<T>(MelonPreferences_Entry entry) =>
            Expression.Lambda<Func<T>>(Expression.Convert(Expression.Constant(entry.DefaultValue_long), typeof(T))).Compile()();
        internal override void SetDefaultValue<T>(MelonPreferences_Entry entry, T value) =>
            entry.DefaultValue_long = Expression.Lambda<Func<long>>(Expression.Convert(Expression.Constant(value), ReflectedType)).Compile()();

        internal override Type GetReflectedType() => ReflectedType;
        internal override MelonPreferences_Entry.TypeEnum GetTypeEnum() => TypeEnum;
        internal override string GetTypeName() => TypeName;
    }
}
