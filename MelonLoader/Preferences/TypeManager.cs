using System;
using System.Collections.Generic;
using MelonLoader.Tomlyn.Model;
using MelonLoader.Tomlyn.Syntax;

namespace MelonLoader.Preferences
{
    internal static class TypeManager
    {
        private static event EventHandler<TypeParser.ResolveEventArgs> ResolveEvents;
        private static Dictionary<MelonPreferences_Entry.TypeEnum, TypeParser> TypeParserDict_TypeEnum = new Dictionary<MelonPreferences_Entry.TypeEnum, TypeParser>();
        private static Dictionary<Type, TypeParser> TypeParserDict_ReflectedType = new Dictionary<Type, TypeParser>();

        static TypeManager()
        {
            AddResolveEventCallback(Types.StringParser.Resolve);
            AddResolveEventCallback(Types.StringArrayParser.Resolve);
            AddResolveEventCallback(Types.BooleanParser.Resolve);
            AddResolveEventCallback(Types.BooleanArrayParser.Resolve);
            AddResolveEventCallback(Types.IntegerParser.Resolve);
            AddResolveEventCallback(Types.IntegerArrayParser.Resolve);
            AddResolveEventCallback(Types.FloatParser.Resolve);
            AddResolveEventCallback(Types.FloatArrayParser.Resolve);
            AddResolveEventCallback(Types.LongParser.Resolve);
            AddResolveEventCallback(Types.LongArrayParser.Resolve);
            AddResolveEventCallback(Types.DoubleParser.Resolve);
            AddResolveEventCallback(Types.DoubleArrayParser.Resolve);
            AddResolveEventCallback(Types.ByteParser.Resolve);
            AddResolveEventCallback(Types.ByteArrayParser.Resolve);
        }

        internal static void AddResolveEventCallback(EventHandler<TypeParser.ResolveEventArgs> callback) => ResolveEvents += callback;

        private static TypeParser GetParser(Type type)
        {
            TypeParser parser = null;
            lock (TypeParserDict_ReflectedType)
            {
                if (TypeParserDict_ReflectedType.TryGetValue(type, out TypeParser nparser))
                    parser = nparser;
                else
                {
                    var args = new TypeParser.ResolveEventArgs { ReflectedType = type };
                    ResolveEvents?.Invoke(null, args);
                    //if (args.TypeParser == null)
                    //    throw new NullReferenceException("No Parser for Type " + type.Name);
                    if ((args.TypeParser != null) && (args.TypeParser.GetReflectedType() == type))
                        parser = TypeParserDict_ReflectedType[type] = args.TypeParser;
                }
            }
            return parser;
        }

        private static TypeParser GetParser(MelonPreferences_Entry.TypeEnum type)
        {
            TypeParser parser = null;
            if (type == MelonPreferences_Entry.TypeEnum.UNKNOWN)
                return parser;
            lock (TypeParserDict_TypeEnum)
            {
                if (TypeParserDict_TypeEnum.TryGetValue(type, out TypeParser nparser))
                    parser = nparser;
                else
                {
                    var args = new TypeParser.ResolveEventArgs { TypeEnum = type };
                    ResolveEvents?.Invoke(null, args);
                    //if (args.TypeParser == null)
                    //    throw new NullReferenceException("No Parser for Type " + MelonPreferences.TypeEnumToTypeName(type));
                    if ((args.TypeParser != null) && (args.TypeParser.GetTypeEnum() == type))
                        parser = TypeParserDict_TypeEnum[type] = args.TypeParser;
                }
            }
            return parser;
        }

        internal static KeyValueSyntax Save(MelonPreferences_Entry entry)
        {
            TypeParser parser = GetParser(entry.Type);
            if (parser == null)
                return null;
            return parser.Save(entry);
        }

        internal static void Load(MelonPreferences_Entry entry, TomlObject obj)
        {
            TypeParser parser = GetParser(entry.Type);
            if (parser == null)
                return;
            parser.Load(entry, obj);
        }

        internal static MelonPreferences_Entry.TypeEnum TypeToTypeEnum<T>()
        {
            TypeParser parser = GetParser(typeof(T));
            if (parser == null)
                return MelonPreferences_Entry.TypeEnum.UNKNOWN;
            return parser.GetTypeEnum();
        }

        internal static Type TypeEnumToReflectedType(MelonPreferences_Entry.TypeEnum type)
        {
            TypeParser parser = GetParser(type);
            if (parser == null)
                return null;
            return parser.GetReflectedType();
        }

        internal static string TypeEnumToTypeName(MelonPreferences_Entry.TypeEnum type)
        {
            TypeParser parser = GetParser(type);
            if (parser == null)
                return null;
            return parser.GetTypeName();
        }

        internal static MelonPreferences_Entry ConstructEntry<T>(MelonPreferences_Category category, string name, T value, string displayname = null, bool hidden = false)
        {
            TypeParser parser = GetParser(typeof(T));
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
#if PORT_DISABLE
            if (MelonPreferences.SaveAfterEntryCreation)
                MelonPreferences.Save_Internal();
#endif
            return entry;
        }

        internal static void ConvertCurrentValueType<T>(MelonPreferences_Entry entry, T defaultvalue)
        {
            TypeParser parser = GetParser(typeof(T));
            if ((parser == null) || (entry.Type == parser.GetTypeEnum()))
                return;
            parser.SetDefaultValue(entry, defaultvalue);
            parser.ConvertCurrentValueType(entry);
        }

        internal static void ResetToDefault(MelonPreferences_Entry entry)
        {
            TypeParser parser = GetParser(entry.Type);
            if (parser == null)
                return;
            parser.ResetToDefault(entry);
        }

        internal static T GetValue<T>(MelonPreferences_Entry entry)
        {
            TypeParser parser = GetParser(typeof(T));
            if (parser == null)
                return default;
            return parser.GetValue<T>(entry);
        }

        internal static void SetValue<T>(MelonPreferences_Entry entry, T value)
        {
            TypeParser parser = GetParser(typeof(T));
            if (parser == null)
                return;
            parser.SetValue<T>(entry, value);
        }

        internal static T GetEditedValue<T>(MelonPreferences_Entry entry)
        {
            TypeParser parser = GetParser(typeof(T));
            if (parser == null)
                return default;
            return parser.GetEditedValue<T>(entry);
        }

        internal static void SetEditedValue<T>(MelonPreferences_Entry entry, T value)
        {
            TypeParser parser = GetParser(typeof(T));
            if (parser == null)
                return;
            parser.SetEditedValue<T>(entry, value);
        }

        internal static T GetDefaultValue<T>(MelonPreferences_Entry entry)
        {
            TypeParser parser = GetParser(typeof(T));
            if (parser == null)
                return default;
            return parser.GetDefaultValue<T>(entry);
        }
    }

    internal abstract class TypeParser
    {
        internal class ResolveEventArgs : EventArgs
        {
            internal Type ReflectedType { get; set; }
            internal MelonPreferences_Entry.TypeEnum TypeEnum { get; set; }
            internal TypeParser TypeParser { get; set; }
        }

        internal abstract Type GetReflectedType();
        internal abstract MelonPreferences_Entry.TypeEnum GetTypeEnum();
        internal abstract string GetTypeName();
        internal abstract void Construct<T>(MelonPreferences_Entry entry, T value);
        internal abstract T GetValue<T>(MelonPreferences_Entry entry);
        internal abstract void SetValue<T>(MelonPreferences_Entry entry, T value);
        internal abstract T GetEditedValue<T>(MelonPreferences_Entry entry);
        internal abstract void SetEditedValue<T>(MelonPreferences_Entry entry, T value);
        internal abstract T GetDefaultValue<T>(MelonPreferences_Entry entry);
        internal abstract void SetDefaultValue<T>(MelonPreferences_Entry entry, T value);
        internal abstract KeyValueSyntax Save(MelonPreferences_Entry entry);
        internal abstract void Load(MelonPreferences_Entry entry, TomlObject obj);
        internal abstract void ConvertCurrentValueType(MelonPreferences_Entry entry);
        internal abstract void ResetToDefault(MelonPreferences_Entry entry);
    }
}
