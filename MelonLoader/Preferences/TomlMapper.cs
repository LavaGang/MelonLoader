using System;
using System.Collections.Generic;
using System.Linq;
using MelonLoader.Tomlyn.Model;

namespace MelonLoader
{
    public class TomlMapper
    {
        private readonly Dictionary<Type, KeyValuePair<Delegate, Delegate>> myMappers = new Dictionary<Type, KeyValuePair<Delegate, Delegate>>();

        public TomlMapper()
        {
            RegisterMapper(o => (o as TomlString)?.Value, s => new TomlString(s));
            RegisterMapper(o => (o as TomlBoolean)?.Value ?? default, s => new TomlBoolean(s));
            
            RegisterMapper(o => (o as TomlInteger)?.Value ?? default, s => new TomlInteger(s));
            RegisterMapper(o => (int) ((o as TomlInteger)?.Value ?? default), s => new TomlInteger(s));
            RegisterMapper(o => (byte) ((o as TomlInteger)?.Value ?? default), s => new TomlInteger(s));
            RegisterMapper(o => (short) ((o as TomlInteger)?.Value ?? default), s => new TomlInteger(s));
            
            RegisterMapper(o => (o as TomlFloat)?.Value ?? (o as TomlInteger)?.Value ?? default, s => new TomlFloat(s));
            RegisterMapper(o => (float) ((o as TomlFloat)?.Value ?? (o as TomlInteger)?.Value ?? default), s => new TomlFloat(s));
        }

        public void RegisterMapper<T>(Func<TomlObject, T> read, Func<T, TomlObject> write)
        {
            myMappers.Add(typeof(T), new KeyValuePair<Delegate, Delegate>(read, write));
            myMappers.Add(typeof(T[]), new KeyValuePair<Delegate, Delegate>((Func<TomlObject, T[]>) ReadArray<T>, (Func<T[], TomlObject>) WriteArray<T>));
        }
        
        public T[] ReadArray<T>(TomlObject value)
        {
            if (!(value is TomlArray array))
                return new T[0];

            return array.GetTomlEnumerator().Select(FromToml<T>).ToArray();
        }

        public TomlArray WriteArray<T>(T[] value)
        {
            var arr = new TomlArray(value.Length);

            for (var i = 0; i < value.Length; i++)
                arr[i] = ToToml(value[i]);

            return arr;
        }
        
        public TomlObject ToToml<T>(T value)
        {
            if (typeof(T).IsEnum)
                return ((Func<string, TomlObject>) myMappers[typeof(string)].Value)(value.ToString());

            if (!myMappers.TryGetValue(typeof(T), out var mapper))
                throw new ArgumentException($"Attempting to serialized unknown type {typeof(T)}");

            return ((Func<T, TomlObject>) mapper.Value)(value);
        }

        public T FromToml<T>(TomlObject value)
        {
            if (typeof(T).IsEnum)
            {
                string enumValue = ((Func <TomlObject, string>) myMappers[typeof(string)].Key)(value);
                if (Enum.TryParse(typeof(T), enumValue, out var parsedEnum))
                    return (T) parsedEnum;
                else
                    throw new ArgumentException($"Attempting to serialize Enum {typeof(T).Name} with invalid value {enumValue}");
            }
                    
            if (!myMappers.TryGetValue(typeof(T), out var mapper))
                throw new ArgumentException($"Attempting to serialized unknown type {typeof(T)}");

            return ((Func<TomlObject, T>) mapper.Key)(value);
        }
    }
}
