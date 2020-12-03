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
    internal class DoubleParser : TypeParser
    {
        private static string TypeName = "double";
        private static Type ReflectedType = typeof(double);
        private static MelonPreferences_Entry.TypeEnum TypeEnum = MelonPreferences_Entry.TypeEnum.DOUBLE;

        public static void Resolve(object sender, TypeManager.TypeParserArgs args)
        {
            if (((args.ReflectedType != null) && (args.ReflectedType == ReflectedType))
                || ((args.TypeEnum != MelonPreferences_Entry.TypeEnum.UNKNOWN) && (args.TypeEnum == TypeEnum)))
                args.TypeParser = new DoubleParser();
        }

        internal override void Construct<T>(MelonPreferences_Entry entry, T value) =>
           entry.DefaultValue_double = entry.ValueEdited_double = entry.Value_double = Expression.Lambda<Func<double>>(Expression.Convert(Expression.Constant(value), ReflectedType)).Compile()();

        internal override void SetDefaultValue<T>(MelonPreferences_Entry entry, T value) =>
            entry.DefaultValue_double = Expression.Lambda<Func<double>>(Expression.Convert(Expression.Constant(value), ReflectedType)).Compile()();

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

        internal override void ConvertCurrentValueType(MelonPreferences_Entry entry)
        {
            double val_double = 0f;
            switch (entry.Type)
            {
                case MelonPreferences_Entry.TypeEnum.BOOL:
                    val_double = (entry.GetValue<bool>() ? 1 : 0);
                    break;
                case MelonPreferences_Entry.TypeEnum.INT:
                    val_double = entry.GetValue<int>();
                    break;
                case MelonPreferences_Entry.TypeEnum.FLOAT:
                    val_double = entry.GetValue<float>();
                    break;
                case MelonPreferences_Entry.TypeEnum.LONG:
                    val_double = entry.GetValue<long>();
                    break;
                default:
                    break;
            }
            entry.Type = TypeEnum;
            entry.SetValue(val_double);
        }

        internal override Type GetReflectedType() => ReflectedType;
        internal override MelonPreferences_Entry.TypeEnum GetTypeEnum() => TypeEnum;
        internal override string GetTypeName() => TypeName;
    }
}
