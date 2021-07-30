using System;
using System.Reflection;
using System.Reflection.Emit;
using MonoMod.Utils;

namespace Harmony
{
	public delegate object GetterHandler(object source);
	public delegate void SetterHandler(object source, object value);
	public delegate object InstantiationHandler();

	public class FastAccess
	{
		[Obsolete("Use AccessTools.MethodDelegate<Func<T, S>>(PropertyInfo.GetGetMethod(true))")]
		public static InstantiationHandler CreateInstantiationHandler(Type type)
		{
			var constructorInfo = type.GetConstructor(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null, new Type[0], null);
			if (constructorInfo is null)
				throw new ApplicationException(string.Format("The type {0} must declare an empty constructor (the constructor may be private, internal, protected, protected internal, or public).", type));
			var dynamicMethod = new DynamicMethodDefinition($"InstantiateObject_{type.Name}", type, null);
			var generator = dynamicMethod.GetILGenerator();
			generator.Emit(OpCodes.Newobj, constructorInfo);
			generator.Emit(OpCodes.Ret);
			return (InstantiationHandler)dynamicMethod.Generate().CreateDelegate(typeof(InstantiationHandler));
		}

		[Obsolete("Use AccessTools.MethodDelegate<Func<T, S>>(PropertyInfo.GetGetMethod(true))")]
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

		[Obsolete("Use AccessTools.FieldRefAccess<T, S>(fieldInfo)")]
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
			"AccessTools.MethodDelegate<Func<T, S>>(AccessTools.PropertyGetter(typeof(T), name)) for properties")]
		public static GetterHandler CreateFieldGetter(Type type, params string[] names)
		{
			foreach (var name in names)
			{
				var field = type.GetField(name, AccessTools.all);
				if (field is object)
					return CreateGetterHandler(field);
				var property = type.GetProperty(name, AccessTools.all);
				if (property is object)
					return CreateGetterHandler(property);
			}
			return null;
		}

		[Obsolete("Use AccessTools.MethodDelegate<Action<T, S>>(PropertyInfo.GetSetMethod(true))")]
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

		[Obsolete("Use AccessTools.FieldRefAccess<T, S>(fieldInfo)")]
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

		static DynamicMethodDefinition CreateGetDynamicMethod(Type type)
			=> new DynamicMethodDefinition($"DynamicGet_{type.Name}", typeof(object), new Type[] { typeof(object) });

		static DynamicMethodDefinition CreateSetDynamicMethod(Type type)
			=> new DynamicMethodDefinition($"DynamicSet_{type.Name}", typeof(void), new Type[] { typeof(object), typeof(object) });
	}
}