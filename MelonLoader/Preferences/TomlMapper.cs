using System.Collections.Generic;
using Tomlet;
using Tomlet.Models;

namespace MelonLoader
{
    public class TomlMapper
    {
        public T[] ReadArray<T>(TomlValue value) => TomletMain.To<T[]>(value);
        public TomlArray WriteArray<T>(T[] value) => (TomlArray)TomletMain.ValueFrom(value);

        public List<T> ReadList<T>(TomlValue value) => TomletMain.To<List<T>>(value);
        public TomlArray WriteList<T>(List<T> value) => (TomlArray)TomletMain.ValueFrom(value);

        public TomlValue ToToml<T>(T value) => TomletMain.ValueFrom(value);
        public T FromToml<T>(TomlValue value) => TomletMain.To<T>(value);
    }
}
