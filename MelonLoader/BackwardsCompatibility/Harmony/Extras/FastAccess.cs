using MonoMod.Utils;
using System;
using System.Reflection;
using System.Reflection.Emit;

namespace Harmony;

public delegate object GetterHandler(object source);
public delegate void SetterHandler(object source, object value);
public delegate object InstantiationHandler();

public class FastAccess
{
    [Obsolete("Use AccessTools.MethodDelegate<Func<T, S>>(PropertyInfo.GetGetMethod(true)). This will be removed in a future version.", true)]
    public static InstantiationHandler CreateInstantiationHandler(Type type)
    {
        var constructorInfo = type.GetConstructor(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null, [], null) 
            ?? throw new ApplicationException(string.Format("The type {0} must declare an empty constructor (the constructor may be private, internal, protected, protected internal, or public).", type));
        var dynamicMethod = new DynamicMethodDefinition($"InstantiateObject_{type.Name}", type, null);
        var generator = dynamicMethod.GetILGenerator();
        generator.Emit(OpCodes.Newobj, constructorInfo);
        generator.Emit(OpCodes.Ret);
        return (InstantiationHandler)dynamicMethod.Generate().CreateDelegate(typeof(InstantiationHandler));
    }

    [Obsolete("Use AccessTools.MethodDelegate<Func<T, S>>(PropertyInfo.GetGetMethod(true)). This will be removed in a future version.", true)]
    public static GetterHandler CreateGetterHandler(PropertyInfo propertyInfo)
    {
        var getMethodInfo = propertyInfo.GetGetMethod(true);
        var dynamicGet = CreateGetDynamicMethod(propertyInfo.DeclaringType);
        var getGenerator = dynamicGet.GetILGenerator();
        getGenerator.Emit(OpCodes.Ldarg_0);
        getGenerator.Emit(OpCodes.Call, getMethodInfo);
        getGenerator.Emit(OpCodes.Ret);
        return (GetterHandler)dynamicGet.Generate().CreateDelegate(typeof(GetterHandler));
    }

    [Obsolete("Use AccessTools.FieldRefAccess<T, S>(fieldInfo). This will be removed in a future version.", true)]
    public static GetterHandler CreateGetterHandler(FieldInfo fieldInfo)
    {
        var dynamicGet = CreateGetDynamicMethod(fieldInfo.DeclaringType);
        var getGenerator = dynamicGet.GetILGenerator();
        getGenerator.Emit(OpCodes.Ldarg_0);
        getGenerator.Emit(OpCodes.Ldfld, fieldInfo);
        getGenerator.Emit(OpCodes.Ret);
        return (GetterHandler)dynamicGet.Generate().CreateDelegate(typeof(GetterHandler));
    }

    [Obsolete("Use AccessTools.FieldRefAccess<T, S>(name) for fields and " +
        "AccessTools.MethodDelegate<Func<T, S>>(AccessTools.PropertyGetter(typeof(T), name)) for properties. This will be removed in a future version.", true)]
    public static GetterHandler CreateFieldGetter(Type type, params string[] names)
    {
        foreach (var name in names)
        {
            var field = type.GetField(name, AccessTools.all);
            if (field is not null)
                return CreateGetterHandler(field);
            var property = type.GetProperty(name, AccessTools.all);
            if (property is not null)
                return CreateGetterHandler(property);
        }

        return null;
    }

    [Obsolete("Use AccessTools.MethodDelegate<Action<T, S>>(PropertyInfo.GetSetMethod(true)). This will be removed in a future version.", true)]
    public static SetterHandler CreateSetterHandler(PropertyInfo propertyInfo)
    {
        var setMethodInfo = propertyInfo.GetSetMethod(true);
        var dynamicSet = CreateSetDynamicMethod(propertyInfo.DeclaringType);
        var setGenerator = dynamicSet.GetILGenerator();
        setGenerator.Emit(OpCodes.Ldarg_0);
        setGenerator.Emit(OpCodes.Ldarg_1);
        setGenerator.Emit(OpCodes.Call, setMethodInfo);
        setGenerator.Emit(OpCodes.Ret);
        return (SetterHandler)dynamicSet.Generate().CreateDelegate(typeof(SetterHandler));
    }

    [Obsolete("Use AccessTools.FieldRefAccess<T, S>(fieldInfo). This will be removed in a future version.", true)]
    public static SetterHandler CreateSetterHandler(FieldInfo fieldInfo)
    {
        var dynamicSet = CreateSetDynamicMethod(fieldInfo.DeclaringType);
        var setGenerator = dynamicSet.GetILGenerator();
        setGenerator.Emit(OpCodes.Ldarg_0);
        setGenerator.Emit(OpCodes.Ldarg_1);
        setGenerator.Emit(OpCodes.Stfld, fieldInfo);
        setGenerator.Emit(OpCodes.Ret);
        return (SetterHandler)dynamicSet.Generate().CreateDelegate(typeof(SetterHandler));
    }

    private static DynamicMethodDefinition CreateGetDynamicMethod(Type type)
            => new($"DynamicGet_{type.Name}", typeof(object), [typeof(object)]);

    private static DynamicMethodDefinition CreateSetDynamicMethod(Type type)
            => new($"DynamicSet_{type.Name}", typeof(void), [typeof(object), typeof(object)]);
}