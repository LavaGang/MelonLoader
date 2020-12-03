using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MelonLoader.Tomlyn;
using MelonLoader.Tomlyn.Model;
using MelonLoader.Tomlyn.Syntax;

namespace MelonLoader.Preferences.Types
{
    internal class IntegerParser : TypeParser
    {
        public static void Resolve(object sender, TypeManager.TypeParserArgs args)
        {
            if (args.TypeEnum == MelonPreferences_Entry.TypeEnum.INT)
                args.TypeParser = new IntegerParser();
        }

        internal override KeyValueSyntax Save(MelonPreferences_Entry entry)
        {
            entry.SetValue(entry.GetEditedValue<int>());
            return new KeyValueSyntax(entry.Name, new IntegerValueSyntax(entry.GetValue<int>()));
        }

        internal override void Load(MelonPreferences_Entry entry, TomlObject obj)
        {
            switch (obj.Kind)
            {
                case ObjectKind.Boolean:
                    entry.SetValue(((TomlBoolean)obj).Value ? 1 : 0);
                    break;
                case ObjectKind.Integer:
                    entry.SetValue((int)((TomlInteger)obj).Value);
                    break;
                case ObjectKind.Float:
                    entry.SetValue((int)((TomlFloat)obj).Value);
                    break;
                default:
                    break;
            }
        }
    }
}
