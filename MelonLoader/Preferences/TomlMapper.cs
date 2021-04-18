using System;
using System.Collections.Generic;
using Tomlet.Models;


namespace MelonLoader
{
    public class TomlMapper
    {
        public T[] ReadArray<T>(TomlValue value) => Tomlet.Tomlet.To<T[]>(value);

        public TomlArray WriteArray<T>(T[] value) => (TomlArray) Tomlet.Tomlet.ValueFrom(value);

        public List<T> ReadList<T>(TomlValue value) => Tomlet.Tomlet.To<List<T>>(value);
        public TomlArray WriteList<T>(List<T> value) => (TomlArray) Tomlet.Tomlet.ValueFrom(value);

        public TomlValue ToToml<T>(T value) => Tomlet.Tomlet.ValueFrom(value);

        public T FromToml<T>(TomlValue value) => Tomlet.Tomlet.To<T>(value);

        private static bool TryParseEnum<T>(string enumValue, out object parsedEnum)
        {
            parsedEnum = default(T);
            if (string.IsNullOrEmpty(enumValue))
                return false;
            try
            {
                parsedEnum = Enum.Parse(typeof(T), enumValue, false);
                return true;
            }
            catch {}
            return false;
        }
    }
}
