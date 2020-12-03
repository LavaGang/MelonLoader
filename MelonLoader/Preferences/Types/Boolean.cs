using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MelonLoader.Tomlyn;
using MelonLoader.Tomlyn.Model;
using MelonLoader.Tomlyn.Syntax;

namespace MelonLoader.Preferences.Types
{
    internal class BooleanParser : TypeParser
    {
        public static void Resolve(object sender, TypeManager.TypeParserArgs args)
        {
            if (args.TypeEnum == MelonPreferences_Entry.TypeEnum.BOOL)
                args.TypeParser = new BooleanParser();
        }

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
    }
}
