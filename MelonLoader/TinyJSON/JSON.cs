using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;

namespace MelonLoader.TinyJSON;

/// <summary>
/// Mark members that should be included.
/// Public fields are included by default.
/// </summary>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public sealed class Include : Attribute { }

/// <summary>
/// Mark members that should be excluded.
/// Private fields and all properties are excluded by default.
/// </summary>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public class Exclude : Attribute { }

/// <summary>
/// Mark methods to be called after an object is decoded.
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public class AfterDecode : Attribute { }

/// <summary>
/// Mark methods to be called before an object is encoded.
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public class BeforeEncode : Attribute { }

/// <summary>
/// Mark members to force type hinting even when EncodeOptions.NoTypeHints is set.
/// </summary>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public class TypeHint : Attribute { }

/// <summary>
/// Provide field and property aliases when an object is decoded.
/// If a field or property is not found while decoding, this list will be searched for a matching alias.
/// </summary>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = true)]
public class DecodeAlias : Attribute
{
    public string[] Names { get; private set; }

    public DecodeAlias(params string[] names)
    {
        Names = names;
    }

    public bool Contains(string name)
    {
        return Array.IndexOf(Names, name) > -1;
    }
}

[Obsolete("Use the Exclude attribute instead.")]
// ReSharper disable once UnusedMember.Global
public sealed class Skip : Exclude { }

[Obsolete("Use the AfterDecode attribute instead.")]
// ReSharper disable once UnusedMember.Global
public sealed class Load : AfterDecode { }

public sealed class DecodeException : Exception
{
    public DecodeException(string message)
        : base(message) { }

    public DecodeException(string message, Exception innerException)
        : base(message, innerException) { }
}

// ReSharper disable once InconsistentNaming
public static class JSON
{
    private static readonly Type includeAttrType = typeof(Include);
    private static readonly Type excludeAttrType = typeof(Exclude);
    private static readonly Type decodeAliasAttrType = typeof(DecodeAlias);

    public static Variant Load(string json)
    {
        return string.IsNullOrEmpty(json) ? throw new ArgumentNullException(nameof(json)) : Decoder.Decode(json);
    }

    public static string Dump(object data)
    {
        return Dump(data, EncodeOptions.None);
    }

    public static string Dump(object data, EncodeOptions options)
    {
        // Invoke methods tagged with [BeforeEncode] attribute.
        if (data != null)
        {
            var type = data.GetType();
            if (!(type.IsEnum || type.IsPrimitive || type.IsArray))
            {
                foreach (var method in type.GetMethods(instanceBindingFlags))
                {
                    if (method.GetCustomAttributes(false).AnyOfType(typeof(BeforeEncode)))
                    {
                        if (method.GetParameters().Length == 0)
                        {
                            method.Invoke(data, null);
                        }
                    }
                }
            }
        }

        return Encoder.Encode(data, options);
    }

    public static void MakeInto<T>(Variant data, out T item)
    {
        item = DecodeType<T>(data);
    }

    public static void Populate<T>(Variant data, T item) where T : class
    {
        if (item == null)
        {
            throw new ArgumentNullException(nameof(item));
        }

        DecodeFields(data, ref item);
    }

    private static readonly Dictionary<string, Type> typeCache = [];

    private static Type FindType(string fullName)
    {
        if (fullName == null)
        {
            return null;
        }

        Type type;
        if (typeCache.TryGetValue(fullName, out type))
        {
            return type;
        }

        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            type = assembly.GetType(fullName);
            if (type != null)
            {
                typeCache.Add(fullName, type);
                return type;
            }
        }

        return null;
    }

    private static T DecodeType<T>(Variant data)
    {
        if (data == null)
        {
            return default;
        }

        var type = typeof(T);

        var nulledType = Nullable.GetUnderlyingType(type);
        if (nulledType != null)
        {
            var makeFunc = decodeTypeMethod.MakeGenericMethod(nulledType);
            var v = makeFunc.Invoke(null, new object[] { data });
            return (T)v;
        }

        if (type.IsEnum)
        {
            return (T)Enum.Parse(type, data.ToString(CultureInfo.InvariantCulture));
        }

        if (type.IsPrimitive || type == typeof(string) || type == typeof(decimal))
        {
            return (T)Convert.ChangeType(data, type);
        }

        if (type == typeof(Guid))
        {
            return (T)(object)new Guid(data.ToString(CultureInfo.InvariantCulture));
        }

        if (type.IsArray)
        {
            if (type.GetArrayRank() == 1)
            {
                var makeFunc = decodeArrayMethod.MakeGenericMethod(type.GetElementType());
                return (T)makeFunc.Invoke(null, new object[] { data });
            }

            if (data is not ProxyArray arrayData)
            {
                throw new DecodeException("Variant is expected to be a ProxyArray here, but it is not.");
            }

            var arrayRank = type.GetArrayRank();
            var rankLengths = new int[arrayRank];
            if (arrayData.CanBeMultiRankArray(rankLengths))
            {
                var elementType = type.GetElementType();
                if (elementType == null)
                {
                    throw new DecodeException("Array element type is expected to be not null, but it is.");
                }

                var array = Array.CreateInstance(elementType, rankLengths);
                var makeFunc = decodeMultiRankArrayMethod.MakeGenericMethod(elementType);
                try
                {
                    makeFunc.Invoke(null, new object[] { arrayData, array, 1, rankLengths });
                }
                catch (Exception e)
                {
                    throw new DecodeException("Error decoding multidimensional array. Did you try to decode into an array of incompatible rank or element type?", e);
                }

                return (T)Convert.ChangeType(array, typeof(T));
            }

            throw new DecodeException("Error decoding multidimensional array; JSON data doesn't seem fit this structure.");
        }

        if (typeof(IList).IsAssignableFrom(type))
        {
            var makeFunc = decodeListMethod.MakeGenericMethod(type.GetGenericArguments());
            return (T)makeFunc.Invoke(null, new object[] { data });
        }

        if (typeof(IDictionary).IsAssignableFrom(type))
        {
            var makeFunc = decodeDictionaryMethod.MakeGenericMethod(type.GetGenericArguments());
            return (T)makeFunc.Invoke(null, new object[] { data });
        }

        // At this point we should be dealing with a class or struct.
        T instance;
        if (data is not ProxyObject proxyObject)
        {
            throw new InvalidCastException("ProxyObject expected when decoding into '" + type.FullName + "'.");
        }

        // If there's a type hint, use it to create the instance.
        var typeHint = proxyObject.TypeHint;
        if (typeHint != null && typeHint != type.FullName)
        {
            var makeType = FindType(typeHint);
            if (makeType == null)
            {
                throw new TypeLoadException("Could not load type '" + typeHint + "'.");
            }

            if (type.IsAssignableFrom(makeType))
            {
                instance = (T)Activator.CreateInstance(makeType);
                type = makeType;
            }
            else
            {
                throw new InvalidCastException("Cannot assign type '" + typeHint + "' to type '" + type.FullName + "'.");
            }
        }
        else
        {
            // We don't have a type hint, so just instantiate the type we have.
            instance = Activator.CreateInstance<T>();
        }

        foreach (var pair in proxyObject)
        {
            var field = type.GetField(pair.Key, instanceBindingFlags);

            // If the field doesn't exist, search through any [DecodeAlias] attributes.
            if (field == null)
            {
                var fields = type.GetFields(instanceBindingFlags);
                foreach (var fieldInfo in fields)
                {
                    foreach (var attribute in fieldInfo.GetCustomAttributes(true))
                    {
                        if (decodeAliasAttrType.IsInstanceOfType(attribute))
                        {
                            if (((DecodeAlias)attribute).Contains(pair.Key))
                            {
                                field = fieldInfo;
                                break;
                            }
                        }
                    }
                }
            }

            if (field != null)
            {
                var shouldDecode = field.IsPublic;
                foreach (var attribute in field.GetCustomAttributes(true))
                {
                    if (excludeAttrType.IsInstanceOfType(attribute))
                    {
                        shouldDecode = false;
                    }

                    if (includeAttrType.IsInstanceOfType(attribute))
                    {
                        shouldDecode = true;
                    }
                }

                if (shouldDecode)
                {
                    var makeFunc = decodeTypeMethod.MakeGenericMethod(field.FieldType);
                    if (type.IsValueType)
                    {
                        // Type is a struct.
                        var instanceRef = (object)instance;
                        field.SetValue(instanceRef, makeFunc.Invoke(null, new object[] { pair.Value }));
                        instance = (T)instanceRef;
                    }
                    else
                    {
                        // Type is a class.
                        field.SetValue(instance, makeFunc.Invoke(null, new object[] { pair.Value }));
                    }
                }
            }

            var property = type.GetProperty(pair.Key, instanceBindingFlags);

            // If the property doesn't exist, search through any [DecodeAlias] attributes.
            if (property == null)
            {
                var properties = type.GetProperties(instanceBindingFlags);
                foreach (var propertyInfo in properties)
                {
                    foreach (var attribute in propertyInfo.GetCustomAttributes(false))
                    {
                        if (decodeAliasAttrType.IsInstanceOfType(attribute))
                        {
                            if (((DecodeAlias)attribute).Contains(pair.Key))
                            {
                                property = propertyInfo;
                                break;
                            }
                        }
                    }
                }
            }

            if (property != null)
            {
                if (property.CanWrite && property.GetCustomAttributes(false).AnyOfType(includeAttrType))
                {
                    var makeFunc = decodeTypeMethod.MakeGenericMethod(new Type[] { property.PropertyType });
                    if (type.IsValueType)
                    {
                        // Type is a struct.
                        var instanceRef = (object)instance;
                        property.SetValue(instanceRef, makeFunc.Invoke(null, new object[] { pair.Value }), null);
                        instance = (T)instanceRef;
                    }
                    else
                    {
                        // Type is a class.
                        property.SetValue(instance, makeFunc.Invoke(null, new object[] { pair.Value }), null);
                    }
                }
            }
        }

        // Invoke methods tagged with [AfterDecode] attribute.
        foreach (var method in type.GetMethods(instanceBindingFlags))
        {
            if (method.GetCustomAttributes(false).AnyOfType(typeof(AfterDecode)))
            {
                method.Invoke(instance, method.GetParameters().Length == 0 ? null : new object[] { data });
            }
        }

        return instance;
    }

    private static void DecodeFields<T>(Variant data, ref T instance)
    {
        var type = typeof(T);
        if (data is not ProxyObject proxyObject)
        {
            throw new InvalidCastException("ProxyObject expected when decoding into '" + type.FullName + "'.");
        }

        foreach (var pair in proxyObject)
        {
            var field = type.GetField(pair.Key, instanceBindingFlags);

            // If the field doesn't exist, search through any [DecodeAlias] attributes.
            if (field == null)
            {
                var fields = type.GetFields(instanceBindingFlags);
                foreach (var fieldInfo in fields)
                {
                    foreach (var attribute in fieldInfo.GetCustomAttributes(true))
                    {
                        if (decodeAliasAttrType.IsInstanceOfType(attribute))
                        {
                            if (((DecodeAlias)attribute).Contains(pair.Key))
                            {
                                field = fieldInfo;
                                break;
                            }
                        }
                    }
                }
            }

            if (field != null)
            {
                var shouldDecode = field.IsPublic;
                foreach (var attribute in field.GetCustomAttributes(true))
                {
                    if (excludeAttrType.IsInstanceOfType(attribute))
                    {
                        shouldDecode = false;
                    }

                    if (includeAttrType.IsInstanceOfType(attribute))
                    {
                        shouldDecode = true;
                    }
                }

                if (shouldDecode)
                {
                    var makeFunc = decodeTypeMethod.MakeGenericMethod(field.FieldType);
                    if (type.IsValueType)
                    {
                        // Type is a struct.
                        var instanceRef = (object)instance;
                        field.SetValue(instanceRef, makeFunc.Invoke(null, new object[] { pair.Value }));
                        instance = (T)instanceRef;
                    }
                    else
                    {
                        // Type is a class.
                        field.SetValue(instance, makeFunc.Invoke(null, new object[] { pair.Value }));
                    }
                }
            }

            var property = type.GetProperty(pair.Key, instanceBindingFlags);

            // If the property doesn't exist, search through any [DecodeAlias] attributes.
            if (property == null)
            {
                var properties = type.GetProperties(instanceBindingFlags);
                foreach (var propertyInfo in properties)
                {
                    foreach (var attribute in propertyInfo.GetCustomAttributes(false))
                    {
                        if (decodeAliasAttrType.IsInstanceOfType(attribute))
                        {
                            if (((DecodeAlias)attribute).Contains(pair.Key))
                            {
                                property = propertyInfo;
                                break;
                            }
                        }
                    }
                }
            }

            if (property != null)
            {
                if (property.CanWrite && property.GetCustomAttributes(false).AnyOfType(includeAttrType))
                {
                    var makeFunc = decodeTypeMethod.MakeGenericMethod(new Type[] { property.PropertyType });
                    if (type.IsValueType)
                    {
                        // Type is a struct.
                        var instanceRef = (object)instance;
                        property.SetValue(instanceRef, makeFunc.Invoke(null, new object[] { pair.Value }), null);
                        instance = (T)instanceRef;
                    }
                    else
                    {
                        // Type is a class.
                        property.SetValue(instance, makeFunc.Invoke(null, new object[] { pair.Value }), null);
                    }
                }
            }
        }

        // Invoke methods tagged with [AfterDecode] attribute.
        foreach (var method in type.GetMethods(instanceBindingFlags))
        {
            if (method.GetCustomAttributes(false).AnyOfType(typeof(AfterDecode)))
            {
                method.Invoke(instance, method.GetParameters().Length == 0 ? null : new object[] { data });
            }
        }
    }

    // ReSharper disable once UnusedMethodReturnValue.Local
    private static List<T> DecodeList<T>(Variant data)
    {
        var list = new List<T>();

        if (data is not ProxyArray proxyArray)
        {
            throw new DecodeException("Variant is expected to be a ProxyArray here, but it is not.");
        }

        foreach (var item in proxyArray)
        {
            list.Add(DecodeType<T>(item));
        }

        return list;
    }

    // ReSharper disable once UnusedMethodReturnValue.Local
    private static Dictionary<TKey, TValue> DecodeDictionary<TKey, TValue>(Variant data)
    {
        var dict = new Dictionary<TKey, TValue>();
        var type = typeof(TKey);

        if (data is not ProxyObject proxyObject)
        {
            throw new DecodeException("Variant is expected to be a ProxyObject here, but it is not.");
        }

        foreach (var pair in proxyObject)
        {
            var k = (TKey)(type.IsEnum ? Enum.Parse(type, pair.Key) : Convert.ChangeType(pair.Key, type));
            var v = DecodeType<TValue>(pair.Value);
            dict.Add(k, v);
        }

        return dict;
    }

    // ReSharper disable once UnusedMethodReturnValue.Local
    private static T[] DecodeArray<T>(Variant data)
    {
        if (data is not ProxyArray arrayData)
        {
            throw new DecodeException("Variant is expected to be a ProxyArray here, but it is not.");
        }

        var arraySize = arrayData.Count;
        var array = new T[arraySize];

        var i = 0;
        foreach (var item in arrayData)
        {
            array[i++] = DecodeType<T>(item);
        }

        return array;
    }

    // ReSharper disable once UnusedMember.Local
    private static void DecodeMultiRankArray<T>(ProxyArray arrayData, Array array, int arrayRank, int[] indices)
    {
        var count = arrayData.Count;
        for (var i = 0; i < count; i++)
        {
            indices[arrayRank - 1] = i;
            if (arrayRank < array.Rank)
            {
                DecodeMultiRankArray<T>(arrayData[i] as ProxyArray, array, arrayRank + 1, indices);
            }
            else
            {
                array.SetValue(DecodeType<T>(arrayData[i]), indices);
            }
        }
    }

    private const BindingFlags instanceBindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
    private const BindingFlags staticBindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;
    private static readonly MethodInfo decodeTypeMethod = typeof(JSON).GetMethod("DecodeType", staticBindingFlags);
    private static readonly MethodInfo decodeListMethod = typeof(JSON).GetMethod("DecodeList", staticBindingFlags);
    private static readonly MethodInfo decodeDictionaryMethod = typeof(JSON).GetMethod("DecodeDictionary", staticBindingFlags);
    private static readonly MethodInfo decodeArrayMethod = typeof(JSON).GetMethod("DecodeArray", staticBindingFlags);
    private static readonly MethodInfo decodeMultiRankArrayMethod = typeof(JSON).GetMethod("DecodeMultiRankArray", staticBindingFlags);

    // ReSharper disable once InconsistentNaming
    public static void SupportTypeForAOT<T>()
    {
        DecodeType<T>(null);
        DecodeList<T>(null);
        DecodeArray<T>(null);
        DecodeDictionary<short, T>(null);
        DecodeDictionary<ushort, T>(null);
        DecodeDictionary<int, T>(null);
        DecodeDictionary<uint, T>(null);
        DecodeDictionary<long, T>(null);
        DecodeDictionary<ulong, T>(null);
        DecodeDictionary<float, T>(null);
        DecodeDictionary<double, T>(null);
        DecodeDictionary<decimal, T>(null);
        DecodeDictionary<bool, T>(null);
        DecodeDictionary<string, T>(null);
    }

    // ReSharper disable once InconsistentNaming
    // ReSharper disable once UnusedMember.Local
    private static void SupportValueTypesForAOT()
    {
        SupportTypeForAOT<short>();
        SupportTypeForAOT<ushort>();
        SupportTypeForAOT<int>();
        SupportTypeForAOT<uint>();
        SupportTypeForAOT<long>();
        SupportTypeForAOT<ulong>();
        SupportTypeForAOT<float>();
        SupportTypeForAOT<double>();
        SupportTypeForAOT<decimal>();
        SupportTypeForAOT<bool>();
        SupportTypeForAOT<string>();
    }
}
