using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MelonLoader.Tomlyn;
using MelonLoader.Tomlyn.Model;
using MelonLoader.Tomlyn.Syntax;

namespace MelonLoader.Preferences.Types
{
    internal class DoubleParser : TypeParser
    {
        public static void Resolve(object sender, TypeManager.TypeParserArgs args)
        {
            if (args.TypeEnum == MelonPreferences_Entry.TypeEnum.DOUBLE)
                args.TypeParser = new DoubleParser();
        }

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
    }
}
