using System;
using System.Linq.Expressions;
using MelonLoader.Tomlyn.Model;
using MelonLoader.Tomlyn.Syntax;

namespace MelonLoader.Preferences.Types
{
    internal class BooleanArrayParser : TypeParser
    {
        private static string TypeName = "bool[]";
        private static Type ReflectedType = typeof(bool[]);
        private static MelonPreferences_Entry.TypeEnum TypeEnum = MelonPreferences_Entry.TypeEnum.BOOL_ARRAY;

        internal static void Resolve(object sender, ResolveEventArgs args)
        {
            if (((args.ReflectedType != null) && (args.ReflectedType == ReflectedType))
                || ((args.TypeEnum != MelonPreferences_Entry.TypeEnum.UNKNOWN) && (args.TypeEnum == TypeEnum)))
                args.TypeParser = new BooleanArrayParser();
        }

        internal override void Construct<T>(MelonPreferences_Entry entry, T value) =>
            entry.DefaultValue_array_bool = entry.ValueEdited_array_bool = entry.Value_array_bool = Expression.Lambda<Func<bool[]>>(Expression.Convert(Expression.Constant(value), ReflectedType)).Compile()();

        internal override KeyValueSyntax Save(MelonPreferences_Entry entry)
        {
            entry.SetValue(entry.GetEditedValue<bool[]>());
            return new KeyValueSyntax(entry.Name, new ArraySyntax(entry.GetValue<bool[]>()));
        }

        internal override void Load(MelonPreferences_Entry entry, TomlObject obj) =>
            entry.SetValue(((TomlArray)obj).ToArray<bool>());

        internal override void ConvertCurrentValueType(MelonPreferences_Entry entry) { entry.Type = TypeEnum; ResetToDefault(entry); }

        internal override void ResetToDefault(MelonPreferences_Entry entry) =>
            entry.SetValue(entry.DefaultValue_array_bool);

        internal override T GetValue<T>(MelonPreferences_Entry entry) =>
            Expression.Lambda<Func<T>>(Expression.Convert(Expression.Constant(entry.Value_array_bool), typeof(T))).Compile()();
        internal override void SetValue<T>(MelonPreferences_Entry entry, T value) =>
            entry.Value_array_bool = entry.ValueEdited_array_bool = Expression.Lambda<Func<bool[]>>(Expression.Convert(Expression.Constant(value), ReflectedType)).Compile()();

        internal override T GetEditedValue<T>(MelonPreferences_Entry entry) =>
            Expression.Lambda<Func<T>>(Expression.Convert(Expression.Constant(entry.ValueEdited_array_bool), typeof(T))).Compile()();
        internal override void SetEditedValue<T>(MelonPreferences_Entry entry, T value) =>
            entry.ValueEdited_array_bool = Expression.Lambda<Func<bool[]>>(Expression.Convert(Expression.Constant(value), ReflectedType)).Compile()();

        internal override T GetDefaultValue<T>(MelonPreferences_Entry entry) =>
            Expression.Lambda<Func<T>>(Expression.Convert(Expression.Constant(entry.DefaultValue_array_bool), typeof(T))).Compile()();
        internal override void SetDefaultValue<T>(MelonPreferences_Entry entry, T value) =>
            entry.DefaultValue_array_bool = Expression.Lambda<Func<bool[]>>(Expression.Convert(Expression.Constant(value), ReflectedType)).Compile()();

        internal override Type GetReflectedType() => ReflectedType;
        internal override MelonPreferences_Entry.TypeEnum GetTypeEnum() => TypeEnum;
        internal override string GetTypeName() => TypeName;
    }
}
