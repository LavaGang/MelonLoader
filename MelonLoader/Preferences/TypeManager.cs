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
        private static Dictionary<MelonPreferences_Entry.TypeEnum, TypeParser> TypeParserDict_TypeEnum = new Dictionary<MelonPreferences_Entry.TypeEnum, TypeParser>();
        private static Dictionary<Type, TypeParser> TypeParserDict_ReflectedType = new Dictionary<Type, TypeParser>();

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
            lock (TypeParserDict_TypeEnum)
            {
                if (TypeParserDict_TypeEnum.TryGetValue(entry.Type, out TypeParser nparser))
                    parser = nparser;
                else
                {
                    var args = new TypeParserArgs { TypeEnum = entry.Type };
                    ResolveEvents?.Invoke(null, args);
                    if (args.TypeParser == null)
                        throw new NullReferenceException("No Parser for Type " + entry.Type.ToString());
                    parser = TypeParserDict_TypeEnum[entry.Type] = args.TypeParser;
                }
            }
            if (parser != null)
                return parser.Save(entry);
            return null;
        }

        internal static void Load(MelonPreferences_Entry entry, TomlObject obj)
        {
            TypeParser parser = null;
            lock (TypeParserDict_TypeEnum)
            {
                if (TypeParserDict_TypeEnum.TryGetValue(entry.Type, out TypeParser nparser))
                    parser = nparser;
                else
                {
                    var args = new TypeParserArgs { TypeEnum = entry.Type };
                    ResolveEvents?.Invoke(null, args);
                    if (args.TypeParser == null)
                        throw new NullReferenceException("No Parser for Type " + entry.Type.ToString());
                    parser = TypeParserDict_TypeEnum[entry.Type] = args.TypeParser;
                }
            }
            if (parser == null)
                return;
            parser.Load(entry, obj);
        }

        internal static MelonPreferences_Entry.TypeEnum TypeToTypeEnum<T>()
        {
            Type requestedType = typeof(T);
            TypeParser parser = null;
            lock (TypeParserDict_ReflectedType)
            {
                if (TypeParserDict_ReflectedType.TryGetValue(requestedType, out TypeParser nparser))
                    parser = nparser;
                else
                {
                    var args = new TypeParserArgs { ReflectedType = requestedType };
                    ResolveEvents?.Invoke(null, args);
                    if (args.TypeParser == null)
                        throw new NullReferenceException("No Parser for Type " + nameof(T));
                    parser = TypeParserDict_ReflectedType[requestedType] = args.TypeParser;
                }
            }
            if (parser == null)
                return MelonPreferences_Entry.TypeEnum.UNKNOWN;
            return parser.GetTypeEnum();
        }

        internal static Type TypeEnumToType(MelonPreferences_Entry.TypeEnum type)
        {
            TypeParser parser = null;
            lock (TypeParserDict_TypeEnum)
            {
                if (TypeParserDict_TypeEnum.TryGetValue(type, out TypeParser nparser))
                    parser = nparser;
                else
                {
                    var args = new TypeParserArgs { TypeEnum = type };
                    ResolveEvents?.Invoke(null, args);
                    if (args.TypeParser == null)
                        throw new NullReferenceException("No Parser for Type " + type.ToString());
                    parser = TypeParserDict_TypeEnum[type] = args.TypeParser;
                }
            }
            if (parser == null)
                return null;
            return parser.GetReflectedType();
        }

        internal static string TypeEnumToTypeName(MelonPreferences_Entry.TypeEnum type)
        {
            TypeParser parser = null;
            lock (TypeParserDict_TypeEnum)
            {
                if (TypeParserDict_TypeEnum.TryGetValue(type, out TypeParser nparser))
                    parser = nparser;
                else
                {
                    var args = new TypeParserArgs { TypeEnum = type };
                    ResolveEvents?.Invoke(null, args);
                    if (args.TypeParser == null)
                        throw new NullReferenceException("No Parser for Type " + type.ToString());
                    parser = TypeParserDict_TypeEnum[type] = args.TypeParser;
                }
            }
            if (parser == null)
                return null;
            return parser.GetTypeName();
        }

        internal static MelonPreferences_Entry ConstructEntry<T>(MelonPreferences_Category category, string name, T value, string displayname = null, bool hidden = false)
        {
            Type requestedType = typeof(T);
            TypeParser parser = null;
            lock (TypeParserDict_ReflectedType)
            {
                if (TypeParserDict_ReflectedType.TryGetValue(requestedType, out TypeParser nparser))
                    parser = nparser;
                else
                {
                    var args = new TypeParserArgs { ReflectedType = requestedType };
                    ResolveEvents?.Invoke(null, args);
                    if (args.TypeParser == null)
                        throw new NullReferenceException("No Parser for Type " + nameof(T));
                    parser = TypeParserDict_ReflectedType[requestedType] = args.TypeParser;
                }
            }
            if (parser == null)
                return null;
            MelonPreferences_Entry entry = new MelonPreferences_Entry();
            entry.Category = category;
            entry.Name = name;
            entry.DisplayName = displayname;
            entry.Hidden = hidden;
            entry.Type = parser.GetTypeEnum();
            parser.Construct(entry, value);
            category.prefstbl.Add(entry);
            return entry;
        }

        internal static void ConvertCurrentValueType<T>(MelonPreferences_Entry entry, T defaultvalue)
        {
            Type requestedType = typeof(T);
            TypeParser parser = null;
            lock (TypeParserDict_ReflectedType)
            {
                if (TypeParserDict_ReflectedType.TryGetValue(requestedType, out TypeParser nparser))
                    parser = nparser;
                else
                {
                    var args = new TypeParserArgs { ReflectedType = requestedType };
                    ResolveEvents?.Invoke(null, args);
                    if (args.TypeParser == null)
                        throw new NullReferenceException("No Parser for Type " + nameof(T));
                    parser = TypeParserDict_ReflectedType[requestedType] = args.TypeParser;
                }
            }
            if ((parser == null) || (entry.Type == parser.GetTypeEnum()))
                return;
            parser.SetDefaultValue(entry, defaultvalue);
            parser.ConvertCurrentValueType(entry);
        }

        internal class TypeParserArgs : EventArgs
        {
            internal Type ReflectedType { get; set; }
            internal MelonPreferences_Entry.TypeEnum TypeEnum { get; set; }
            internal TypeParser TypeParser { get; set; }
        }
    }

    internal abstract class TypeParser
    {
        internal abstract Type GetReflectedType();
        internal abstract MelonPreferences_Entry.TypeEnum GetTypeEnum();
        internal abstract string GetTypeName();
        internal abstract void Construct<T>(MelonPreferences_Entry entry, T value);
        internal abstract void SetDefaultValue<T>(MelonPreferences_Entry entry, T value);
        internal abstract KeyValueSyntax Save(MelonPreferences_Entry entry);
        internal abstract void Load(MelonPreferences_Entry entry, TomlObject obj);
        internal abstract void ConvertCurrentValueType(MelonPreferences_Entry entry);
    }
}
