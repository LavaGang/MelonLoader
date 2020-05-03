using System;
using System.Collections.Generic;
using System.Reflection;

namespace Harmony
{
	public class AccessCache
	{
		Dictionary<Type, Dictionary<string, FieldInfo>> fields = new Dictionary<Type, Dictionary<string, FieldInfo>>();
		Dictionary<Type, Dictionary<string, PropertyInfo>> properties = new Dictionary<Type, Dictionary<string, PropertyInfo>>();
		readonly Dictionary<Type, Dictionary<string, Dictionary<int, MethodBase>>> methods = new Dictionary<Type, Dictionary<string, Dictionary<int, MethodBase>>>();

		[UpgradeToLatestVersion(1)]
		public FieldInfo GetFieldInfo(Type type, string name)
		{
			Dictionary<string, FieldInfo> fieldsByType = null;
			if (fields.TryGetValue(type, out fieldsByType) == false)
			{
				fieldsByType = new Dictionary<string, FieldInfo>();
				fields.Add(type, fieldsByType);
			}

			FieldInfo field = null;
			if (fieldsByType.TryGetValue(name, out field) == false)
			{
				field = AccessTools.Field(type, name);
				fieldsByType.Add(name, field);
			}
			return field;
		}

		public PropertyInfo GetPropertyInfo(Type type, string name)
		{
			Dictionary<string, PropertyInfo> propertiesByType = null;
			if (properties.TryGetValue(type, out propertiesByType) == false)
			{
				propertiesByType = new Dictionary<string, PropertyInfo>();
				properties.Add(type, propertiesByType);
			}

			PropertyInfo property = null;
			if (propertiesByType.TryGetValue(name, out property) == false)
			{
				property = AccessTools.Property(type, name);
				propertiesByType.Add(name, property);
			}
			return property;
		}

		static int CombinedHashCode(IEnumerable<object> objects)
		{
			var hash1 = (5381 << 16) + 5381;
			var hash2 = hash1;
			var i = 0;
			foreach (var obj in objects)
			{
				if (i % 2 == 0)
					hash1 = ((hash1 << 5) + hash1 + (hash1 >> 27)) ^ obj.GetHashCode();
				else
					hash2 = ((hash2 << 5) + hash2 + (hash2 >> 27)) ^ obj.GetHashCode();
				++i;
			}
			return hash1 + (hash2 * 1566083941);
		}

		public MethodBase GetMethodInfo(Type type, string name, Type[] arguments)
		{
			Dictionary<string, Dictionary<int, MethodBase>> methodsByName = null;
			methods.TryGetValue(type, out methodsByName);
			if (methodsByName == null)
			{
				methodsByName = new Dictionary<string, Dictionary<int, MethodBase>>();
				methods.Add(type, methodsByName);
			}

			Dictionary<int, MethodBase> methodsByArguments = null;
			methodsByName.TryGetValue(name, out methodsByArguments);
			if (methodsByArguments == null)
			{
				methodsByArguments = new Dictionary<int, MethodBase>();
				methodsByName.Add(name, methodsByArguments);
			}

			MethodBase method = null;
			var argumentsHash = CombinedHashCode(arguments);
			methodsByArguments.TryGetValue(argumentsHash, out method);
			if (method == null)
			{
				method = AccessTools.Method(type, name, arguments);
				methodsByArguments.Add(argumentsHash, method);
			}

			return method;
		}
	}
}