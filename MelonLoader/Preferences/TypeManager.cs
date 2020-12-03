using System;
using System.Collections.Generic;
using MelonLoader.Tomlyn;
using MelonLoader.Tomlyn.Model;
using MelonLoader.Tomlyn.Syntax;

namespace MelonLoader.Preferences
{
    internal static class TypeManager
    {
        private static event EventHandler<TypeParserArgs> ResolveEvents;
        private static Dictionary<MelonPreferences_Entry.TypeEnum, TypeParser> TypeParserDict = new Dictionary<MelonPreferences_Entry.TypeEnum, TypeParser>();

        static TypeManager()
        {
            ResolveEvents += Types.StringParser.Resolve;
            ResolveEvents += Types.BooleanParser.Resolve;
            ResolveEvents += Types.IntegerParser.Resolve;
            ResolveEvents += Types.FloatParser.Resolve;
            ResolveEvents += Types.LongParser.Resolve;
            ResolveEvents += Types.DoubleParser.Resolve;
        }

        internal static KeyValueSyntax Save(MelonPreferences_Entry entry)
        {
            TypeParser parser = null;
            lock (TypeParserDict)
            {
                if (TypeParserDict.TryGetValue(entry.Type, out TypeParser nparser))
                    parser = nparser;
                else
                {
                    var args = new TypeParserArgs { TypeEnum = entry.Type };
                    ResolveEvents?.Invoke(null, args);
                    if (args.TypeParser == null)
                        throw new NullReferenceException("No Parser for Type " + entry.Type.ToString());
                    parser = TypeParserDict[entry.Type] = args.TypeParser;
                }
            }
            if (parser != null)
                return parser.Save(entry);
            return null;
        }

        internal static void Load(MelonPreferences_Entry entry, TomlObject obj)
        {
            TypeParser parser = null;
            lock (TypeParserDict)
            {
                if (TypeParserDict.TryGetValue(entry.Type, out TypeParser nparser))
                    parser = nparser;
                else
                {
                    var args = new TypeParserArgs { TypeEnum = entry.Type };
                    ResolveEvents?.Invoke(null, args);
                    if (args.TypeParser == null)
                        throw new NullReferenceException("No Parser for Type " + entry.Type.ToString());
                    parser = TypeParserDict[entry.Type] = args.TypeParser;
                }
            }
            if (parser == null)
                return;
            parser.Load(entry, obj);
        }

        internal class TypeParserArgs : EventArgs { internal MelonPreferences_Entry.TypeEnum TypeEnum { get; set; } internal TypeParser TypeParser { get; set; } }
    }

    internal abstract class TypeParser
    {
        internal abstract KeyValueSyntax Save(MelonPreferences_Entry entry);
        internal abstract void Load(MelonPreferences_Entry entry, TomlObject obj);
    }
}
