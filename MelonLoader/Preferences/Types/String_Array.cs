using System;
using System.Linq.Expressions;
using MelonLoader.Tomlyn.Model;
using MelonLoader.Tomlyn.Syntax;

namespace MelonLoader.Preferences.Types
{
    internal class StringArrayParser : TypeParser
    {
        private static string TypeName = "string[]";
        private static Type ReflectedType = typeof(string[]);
        private static MelonPreferences_Entry.TypeEnum TypeEnum = MelonPreferences_Entry.TypeEnum.STRING_ARRAY;

        internal static void Resolve(object sender, ResolveEventArgs args)
        {
            if (((args.ReflectedType != null) && (args.ReflectedType == ReflectedType))
                || ((args.TypeEnum != MelonPreferences_Entry.TypeEnum.UNKNOWN) && (args.TypeEnum == TypeEnum)))
                args.TypeParser = new StringArrayParser();
        }

        internal override void Construct<T>(MelonPreferences_Entry entry, T value) =>
            entry.DefaultValue_array_string = entry.ValueEdited_array_string = entry.Value_array_string = Expression.Lambda<Func<string[]>>(Expression.Convert(Expression.Constant(value), ReflectedType)).Compile()();

        internal override KeyValueSyntax Save(MelonPreferences_Entry entry)
        {
            entry.SetValue(entry.GetEditedValue<string[]>());
            return new KeyValueSyntax(entry.Name, new ArraySyntax(entry.GetValue<string[]>()));
        }

        internal override void Load(MelonPreferences_Entry entry, TomlObject obj) =>
            entry.SetValue(((TomlArray)obj).ToArray<string>());

        internal override void ConvertCurrentValueType(MelonPreferences_Entry entry)
        {
            string[] val_stringarr = null;
            switch (entry.Type)
            {
                case MelonPreferences_Entry.TypeEnum.BOOL_ARRAY:
                    //val_string = entry.GetValue<bool>().ToString();
                    break;
                case MelonPreferences_Entry.TypeEnum.INT_ARRAY:
                    //val_string = entry.GetValue<int>().ToString();
                    break;
                case MelonPreferences_Entry.TypeEnum.FLOAT_ARRAY:
                    //val_string = entry.GetValue<float>().ToString();
                    break;
                case MelonPreferences_Entry.TypeEnum.DOUBLE_ARRAY:
                    //val_string = entry.GetValue<double>().ToString();
                    break;
                case MelonPreferences_Entry.TypeEnum.LONG_ARRAY:
                    //val_string = entry.GetValue<long>().ToString();
                    break;
                default:
                    break;
            }
            entry.Type = TypeEnum;
            entry.SetValue(val_stringarr);
        }

        internal override void ResetToDefault(MelonPreferences_Entry entry) =>
            entry.SetValue(entry.DefaultValue_array_string);

        internal override T GetValue<T>(MelonPreferences_Entry entry) =>
            Expression.Lambda<Func<T>>(Expression.Convert(Expression.Constant(entry.Value_array_string), typeof(T))).Compile()();
        internal override void SetValue<T>(MelonPreferences_Entry entry, T value) =>
            entry.Value_array_string = entry.ValueEdited_array_string = Expression.Lambda<Func<string[]>>(Expression.Convert(Expression.Constant(value), ReflectedType)).Compile()();

        internal override T GetEditedValue<T>(MelonPreferences_Entry entry) =>
            Expression.Lambda<Func<T>>(Expression.Convert(Expression.Constant(entry.ValueEdited_array_string), typeof(T))).Compile()();
        internal override void SetEditedValue<T>(MelonPreferences_Entry entry, T value) =>
            entry.ValueEdited_array_string = Expression.Lambda<Func<string[]>>(Expression.Convert(Expression.Constant(value), ReflectedType)).Compile()();

        internal override T GetDefaultValue<T>(MelonPreferences_Entry entry) =>
            Expression.Lambda<Func<T>>(Expression.Convert(Expression.Constant(entry.DefaultValue_array_string), typeof(T))).Compile()();
        internal override void SetDefaultValue<T>(MelonPreferences_Entry entry, T value) =>
            entry.DefaultValue_array_string = Expression.Lambda<Func<string[]>>(Expression.Convert(Expression.Constant(value), ReflectedType)).Compile()();

        internal override Type GetReflectedType() => ReflectedType;
        internal override MelonPreferences_Entry.TypeEnum GetTypeEnum() => TypeEnum;
        internal override string GetTypeName() => TypeName;
    }
}
