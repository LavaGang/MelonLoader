using System;
using System.Linq.Expressions;
using MelonLoader.Tomlyn.Model;
using MelonLoader.Tomlyn.Syntax;

namespace MelonLoader.Preferences.Types
{
    internal class ByteParser : TypeParser
    {
        private static string TypeName = "byte";
        private static Type ReflectedType = typeof(byte);
        private static MelonPreferences_Entry.TypeEnum TypeEnum = MelonPreferences_Entry.TypeEnum.BYTE;
        internal static void Resolve(object sender, ResolveEventArgs args)
        {
            if (((args.ReflectedType != null) && (args.ReflectedType == ReflectedType))
                || ((args.TypeEnum != MelonPreferences_Entry.TypeEnum.UNKNOWN) && (args.TypeEnum == TypeEnum)))
                args.TypeParser = new ByteParser();
        }

        internal override void Construct<T>(MelonPreferences_Entry entry, T value) =>
            entry.DefaultValue_byte = entry.ValueEdited_byte = entry.Value_byte = Expression.Lambda<Func<byte>>(Expression.Convert(Expression.Constant(value), ReflectedType)).Compile()();

        internal override KeyValueSyntax Save(MelonPreferences_Entry entry)
        {
            entry.SetValue(entry.GetEditedValue<byte>());
            return new KeyValueSyntax(entry.Name, new IntegerValueSyntax(entry.GetValue<byte>()));
        }

        internal override void Load(MelonPreferences_Entry entry, TomlObject obj)
        {
            switch (obj.Kind)
            {
                case ObjectKind.Boolean:
                    entry.SetValue((byte)(((TomlBoolean)obj).Value ? 1 : 0));
                    break;
                case ObjectKind.Integer:
                    entry.SetValue((byte)((TomlInteger)obj).Value);
                    break;
                case ObjectKind.Float:
                    entry.SetValue((byte)((TomlFloat)obj).Value);
                    break;
                default:
                    break;
            }
        }

        internal override void ConvertCurrentValueType(MelonPreferences_Entry entry)
        {
            byte val = GetDefaultValue<byte>(entry);
            if (entry.Type == MelonPreferences_Entry.TypeEnum.LONG)
                val = (byte)entry.GetValue<long>();
            entry.Type = TypeEnum;
            entry.SetValue(val);
        }

        internal override void ResetToDefault(MelonPreferences_Entry entry) =>
            entry.SetValue(entry.DefaultValue_byte);

        internal override T GetValue<T>(MelonPreferences_Entry entry) =>
            Expression.Lambda<Func<T>>(Expression.Convert(Expression.Constant(entry.Value_byte), typeof(T))).Compile()();
        internal override void SetValue<T>(MelonPreferences_Entry entry, T value) =>
            entry.Value_byte = entry.ValueEdited_byte = Expression.Lambda<Func<byte>>(Expression.Convert(Expression.Constant(value), ReflectedType)).Compile()();

        internal override T GetEditedValue<T>(MelonPreferences_Entry entry) =>
            Expression.Lambda<Func<T>>(Expression.Convert(Expression.Constant(entry.ValueEdited_byte), typeof(T))).Compile()();
        internal override void SetEditedValue<T>(MelonPreferences_Entry entry, T value) =>
            entry.ValueEdited_byte = Expression.Lambda<Func<byte>>(Expression.Convert(Expression.Constant(value), ReflectedType)).Compile()();

        internal override T GetDefaultValue<T>(MelonPreferences_Entry entry) =>
            Expression.Lambda<Func<T>>(Expression.Convert(Expression.Constant(entry.DefaultValue_byte), typeof(T))).Compile()();
        internal override void SetDefaultValue<T>(MelonPreferences_Entry entry, T value) =>
            entry.DefaultValue_byte = Expression.Lambda<Func<byte>>(Expression.Convert(Expression.Constant(value), ReflectedType)).Compile()();

        internal override Type GetReflectedType() => ReflectedType;
        internal override MelonPreferences_Entry.TypeEnum GetTypeEnum() => TypeEnum;
        internal override string GetTypeName() => TypeName;
    }
}
