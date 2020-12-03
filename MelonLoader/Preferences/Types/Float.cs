using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MelonLoader.Tomlyn;
using MelonLoader.Tomlyn.Model;
using MelonLoader.Tomlyn.Syntax;

namespace MelonLoader.Preferences.Types
{
    internal class FloatParser : TypeParser
    {
        public static void Resolve(object sender, TypeManager.TypeParserArgs args)
        {
            if (args.TypeEnum == MelonPreferences_Entry.TypeEnum.FLOAT)
                args.TypeParser = new FloatParser();
        }

        internal override KeyValueSyntax Save(MelonPreferences_Entry entry)
        {
            entry.SetValue(entry.GetEditedValue<float>());
            return new KeyValueSyntax(entry.Name, new FloatValueSyntax(entry.GetValue<float>()));
        }

        internal override void Load(MelonPreferences_Entry entry, TomlObject obj)
        {
            switch (obj.Kind)
            {
                case ObjectKind.Boolean:
                    entry.SetValue(((TomlBoolean)obj).Value ? 1f : 0f);
                    break;
                case ObjectKind.Integer:
                    entry.SetValue<float>(((TomlInteger)obj).Value);
                    break;
                case ObjectKind.Float:
                    entry.SetValue((float)((TomlFloat)obj).Value);
                    break;
                default:
                    break;
            }
        }
    }
}
