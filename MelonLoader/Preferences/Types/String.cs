using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MelonLoader.Tomlyn;
using MelonLoader.Tomlyn.Model;
using MelonLoader.Tomlyn.Syntax;

namespace MelonLoader.Preferences.Types
{
    internal class StringParser : TypeParser
    {
        internal static void Resolve(object sender, TypeManager.TypeParserArgs args)
        {
            if (args.TypeEnum == MelonPreferences_Entry.TypeEnum.STRING)
                args.TypeParser = new StringParser();
        }

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
    }
}
