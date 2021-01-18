using System;
using System.Linq.Expressions;
using MelonLoader.Tomlyn.Model;
using MelonLoader.Tomlyn.Syntax;

namespace MelonLoader.Preferences.Types
{
    internal class DoubleParser : TypeParser
    {
        private static string TypeName = "double";
        private static Type ReflectedType = typeof(double);
        private static MelonPreferences_Entry.TypeEnum TypeEnum = MelonPreferences_Entry.TypeEnum.DOUBLE;

        internal static void Resolve(object sender, ResolveEventArgs args)
        {
            if (((args.ReflectedType != null) && (args.ReflectedType == ReflectedType))
                || ((args.TypeEnum != MelonPreferences_Entry.TypeEnum.UNKNOWN) && (args.TypeEnum == TypeEnum)))
                args.TypeParser = new DoubleParser();
        }

        internal override void Construct<T>(MelonPreferences_Entry entry, T value) =>
            entry.DefaultValue_double = entry.ValueEdited_double = entry.Value_double = Expression.Lambda<Func<double>>(Expression.Convert(Expression.Constant(value), ReflectedType)).Compile()();

        internal override KeyValueSyntax Save(MelonPreferences_Entry entry)
        {
            entry.SetValue(entry.GetEditedValue<double>());
            return new KeyValueSyntax(entry.Name, new FloatValueSyntax(entry.GetValue<double>()));
        }

        internal override void Load(MelonPreferences_Entry entry, TomlObject obj)
        {
            switch (obj.Kind)
            {
                case ObjectKind.Boolean:
                    entry.SetValue<double>(((TomlBoolean)obj).Value ? 1 : 0);
                    break;
                case ObjectKind.Integer:
                    entry.SetValue<double>(((TomlInteger)obj).Value);
                    break;
                case ObjectKind.Float:
                    entry.SetValue(((TomlFloat)obj).Value);
                    break;
                default:
                    break;
            }
        }

        internal override void ConvertCurrentValueType(MelonPreferences_Entry entry) { entry.Type = TypeEnum; ResetToDefault(entry); }

        internal override void ResetToDefault(MelonPreferences_Entry entry) =>
            entry.SetValue(entry.DefaultValue_double);

        internal override T GetValue<T>(MelonPreferences_Entry entry) =>
            Expression.Lambda<Func<T>>(Expression.Convert(Expression.Constant(entry.Value_double), typeof(T))).Compile()();
        internal override void SetValue<T>(MelonPreferences_Entry entry, T value) =>
            entry.Value_double = entry.ValueEdited_double = Expression.Lambda<Func<double>>(Expression.Convert(Expression.Constant(value), ReflectedType)).Compile()();

        internal override T GetEditedValue<T>(MelonPreferences_Entry entry) =>
            Expression.Lambda<Func<T>>(Expression.Convert(Expression.Constant(entry.ValueEdited_double), typeof(T))).Compile()();
        internal override void SetEditedValue<T>(MelonPreferences_Entry entry, T value) =>
            entry.ValueEdited_double = Expression.Lambda<Func<double>>(Expression.Convert(Expression.Constant(value), ReflectedType)).Compile()();

        internal override T GetDefaultValue<T>(MelonPreferences_Entry entry) =>
            Expression.Lambda<Func<T>>(Expression.Convert(Expression.Constant(entry.DefaultValue_double), typeof(T))).Compile()();
        internal override void SetDefaultValue<T>(MelonPreferences_Entry entry, T value) =>
            entry.DefaultValue_double = Expression.Lambda<Func<double>>(Expression.Convert(Expression.Constant(value), ReflectedType)).Compile()();

        internal override Type GetReflectedType() => ReflectedType;
        internal override MelonPreferences_Entry.TypeEnum GetTypeEnum() => TypeEnum;
        internal override string GetTypeName() => TypeName;
    }
}
