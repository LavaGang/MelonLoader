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
    internal class BooleanParser : TypeParser
    {
        private static string TypeName = "bool";
        private static Type ReflectedType = typeof(bool);
        private static MelonPreferences_Entry.TypeEnum TypeEnum = MelonPreferences_Entry.TypeEnum.BOOL;

        public static void Resolve(object sender, TypeManager.TypeParserArgs args)
        {
            if (((args.ReflectedType != null) && (args.ReflectedType == ReflectedType))
                || ((args.TypeEnum != MelonPreferences_Entry.TypeEnum.UNKNOWN) && (args.TypeEnum == TypeEnum)))
                args.TypeParser = new BooleanParser();
        }

        internal override void Construct<T>(MelonPreferences_Entry entry, T value) =>
            entry.DefaultValue_bool = entry.ValueEdited_bool = entry.Value_bool = Expression.Lambda<Func<bool>>(Expression.Convert(Expression.Constant(value), ReflectedType)).Compile()();

        internal override void SetDefaultValue<T>(MelonPreferences_Entry entry, T value) =>
            entry.DefaultValue_bool = Expression.Lambda<Func<bool>>(Expression.Convert(Expression.Constant(value), ReflectedType)).Compile()();

        internal override KeyValueSyntax Save(MelonPreferences_Entry entry)
        {
            entry.SetValue(entry.GetEditedValue<bool>());
            return new KeyValueSyntax(entry.Name, new BooleanValueSyntax(entry.GetValue<bool>()));
        }

        internal override void Load(MelonPreferences_Entry entry, TomlObject obj)
        {
            switch (obj.Kind)
            {
                case ObjectKind.Boolean:
                    entry.SetValue(((TomlBoolean)obj).Value);
                    break;
                case ObjectKind.Integer:
                    entry.SetValue(((TomlInteger)obj).Value > 0);
                    break;
                case ObjectKind.Float:
                    entry.SetValue(((TomlFloat)obj).Value > 0);
                    break;
                default:
                    break;
            }
        }

        internal override void ConvertCurrentValueType(MelonPreferences_Entry entry)
        {
            bool val_bool = false;
            switch (entry.Type)
            {
                case MelonPreferences_Entry.TypeEnum.INT:
                    val_bool = (entry.GetValue<int>() != 0);
                    break;
                case MelonPreferences_Entry.TypeEnum.FLOAT:
                    val_bool = (entry.GetValue<float>() != 0);
                    break;
                case MelonPreferences_Entry.TypeEnum.DOUBLE:
                    val_bool = (entry.GetValue<double>() != 0);
                    break;
                case MelonPreferences_Entry.TypeEnum.LONG:
                    val_bool = (entry.GetValue<long>() != 0);
                    break;
                default:
                    break;
            }
            entry.Type = TypeEnum;
            entry.SetValue(val_bool);
        }

        internal override Type GetReflectedType() => ReflectedType;
        internal override MelonPreferences_Entry.TypeEnum GetTypeEnum() => TypeEnum;
        internal override string GetTypeName() => TypeName;
    }
}
