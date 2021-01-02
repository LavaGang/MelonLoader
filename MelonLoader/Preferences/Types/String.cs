using System;
using System.Linq.Expressions;
using MelonLoader.Tomlyn.Model;
using MelonLoader.Tomlyn.Syntax;

namespace MelonLoader.Preferences.Types
{
    internal class StringParser : TypeParser
    {
        private static string TypeName = "string";
        private static Type ReflectedType = typeof(string);
        private static MelonPreferences_Entry.TypeEnum TypeEnum = MelonPreferences_Entry.TypeEnum.STRING;

        internal static void Resolve(object sender, ResolveEventArgs args)
        {
            if (((args.ReflectedType != null) && (args.ReflectedType == ReflectedType))
                || ((args.TypeEnum != MelonPreferences_Entry.TypeEnum.UNKNOWN) && (args.TypeEnum == TypeEnum)))
                args.TypeParser = new StringParser();
        }

        internal override void Construct<T>(MelonPreferences_Entry entry, T value) =>
            entry.DefaultValue_string = entry.ValueEdited_string = entry.Value_string = Expression.Lambda<Func<string>>(Expression.Convert(Expression.Constant(value), ReflectedType)).Compile()();

        internal override KeyValueSyntax Save(MelonPreferences_Entry entry)
        {
            entry.SetValue(entry.GetEditedValue<string>());
            return new KeyValueSyntax(entry.Name, new StringValueSyntax(entry.GetValue<string>()));
        }

        internal override void Load(MelonPreferences_Entry entry, TomlObject obj)
        {
            if (obj.Kind == ObjectKind.String)
                entry.SetValue(((TomlString)obj).Value);
        }

        internal override void ConvertCurrentValueType(MelonPreferences_Entry entry) { entry.Type = TypeEnum; ResetToDefault(entry); }

        internal override void ResetToDefault(MelonPreferences_Entry entry) =>
            entry.SetValue(entry.DefaultValue_string);

        internal override T GetValue<T>(MelonPreferences_Entry entry) =>
            Expression.Lambda<Func<T>>(Expression.Convert(Expression.Constant(entry.Value_string), typeof(T))).Compile()();
        internal override void SetValue<T>(MelonPreferences_Entry entry, T value) =>
            entry.Value_string = entry.ValueEdited_string = Expression.Lambda<Func<string>>(Expression.Convert(Expression.Constant(value), ReflectedType)).Compile()();

        internal override T GetEditedValue<T>(MelonPreferences_Entry entry) =>
            Expression.Lambda<Func<T>>(Expression.Convert(Expression.Constant(entry.ValueEdited_string), typeof(T))).Compile()();
        internal override void SetEditedValue<T>(MelonPreferences_Entry entry, T value) =>
            entry.ValueEdited_string = Expression.Lambda<Func<string>>(Expression.Convert(Expression.Constant(value), ReflectedType)).Compile()();

        internal override T GetDefaultValue<T>(MelonPreferences_Entry entry) =>
            Expression.Lambda<Func<T>>(Expression.Convert(Expression.Constant(entry.DefaultValue_string), typeof(T))).Compile()();
        internal override void SetDefaultValue<T>(MelonPreferences_Entry entry, T value) =>
            entry.DefaultValue_string = Expression.Lambda<Func<string>>(Expression.Convert(Expression.Constant(value), ReflectedType)).Compile()();

        internal override Type GetReflectedType() => ReflectedType;
        internal override MelonPreferences_Entry.TypeEnum GetTypeEnum() => TypeEnum;
        internal override string GetTypeName() => TypeName;
    }
}
