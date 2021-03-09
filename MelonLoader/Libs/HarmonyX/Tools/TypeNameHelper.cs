// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Text;

namespace HarmonyLib.Tools
{
	// Adapted from https://github.com/benaadams/Ben.Demystifier/blob/master/src/Ben.Demystifier/TypeNameHelper.cs
	internal static class TypeNameHelper
	{
		private static readonly Dictionary<Type, string> BuiltInTypeNames = new Dictionary<Type, string>
		{
			{typeof(void), "void"},
			{typeof(bool), "bool"},
			{typeof(byte), "byte"},
			{typeof(char), "char"},
			{typeof(decimal), "decimal"},
			{typeof(double), "double"},
			{typeof(float), "float"},
			{typeof(int), "int"},
			{typeof(long), "long"},
			{typeof(object), "object"},
			{typeof(sbyte), "sbyte"},
			{typeof(short), "short"},
			{typeof(string), "string"},
			{typeof(uint), "uint"},
			{typeof(ulong), "ulong"},
			{typeof(ushort), "ushort"}
		};

		private static readonly Dictionary<string, string> FSharpTypeNames = new Dictionary<string, string>
		{
			{"Unit", "void"},
			{"FSharpOption", "Option"},
			{"FSharpAsync", "Async"},
			{"FSharpOption`1", "Option"},
			{"FSharpAsync`1", "Async"}
		};

		/// <summary>
		///    Pretty print a full type name.
		/// </summary>
		/// <param name="type">The <see cref="Type" />.</param>
		/// <returns>The pretty printed full type name.</returns>
		///
		public static string GetTypeDisplayName(Type type)
		{
			var builder = new StringBuilder();
			ProcessType(builder, type);
			return builder.ToString();
		}

		private static void ProcessType(StringBuilder builder, Type type)
		{
			if (type.IsGenericType)
			{
				var underlyingType = Nullable.GetUnderlyingType(type);
				if (underlyingType != null)
				{
					ProcessType(builder, underlyingType);
					builder.Append('?');
				}
				else
				{
					var genericArguments = type.GetGenericArguments();
					ProcessGenericType(builder, type, genericArguments, genericArguments.Length);
				}
			}
			else if (type.IsArray)
				ProcessArrayType(builder, type);
			else if (BuiltInTypeNames.TryGetValue(type, out var builtInName))
				builder.Append(builtInName);
			else if (type.Namespace == nameof(System))
				builder.Append(type.Name);
			else if (type.Assembly.ManifestModule.Name == "FSharp.Core.dll"
			      && FSharpTypeNames.TryGetValue(type.Name, out builtInName))
				builder.Append(builtInName);
			else if (type.IsGenericParameter)
				builder.Append(type.Name);
			else
				builder.Append(type.FullName ?? type.Name);
		}

		private static void ProcessArrayType(StringBuilder builder, Type type)
		{
			var innerType = type;
			while (innerType.IsArray) innerType = innerType.GetElementType();

			ProcessType(builder, innerType);

			while (type.IsArray)
			{
				builder.Append('[');
				builder.Append(',', type.GetArrayRank() - 1);
				builder.Append(']');
				type = type.GetElementType();
			}
		}

		private static void ProcessGenericType(StringBuilder builder, Type type, Type[] genericArguments, int length)
		{
			var offset = 0;
			if (type.IsNested) offset = type.DeclaringType.GetGenericArguments().Length;

			if (type.IsNested)
			{
				ProcessGenericType(builder, type.DeclaringType, genericArguments, offset);
				builder.Append('+');
			}
			else if (!string.IsNullOrEmpty(type.Namespace))
			{
				builder.Append(type.Namespace);
				builder.Append('.');
			}

			var genericPartIndex = type.Name.IndexOf('`');
			if (genericPartIndex <= 0)
			{
				builder.Append(type.Name);
				return;
			}

			if (type.Assembly.ManifestModule.Name == "FSharp.Core.dll"
			 && FSharpTypeNames.TryGetValue(type.Name, out var builtInName))
				builder.Append(builtInName);
			else
				builder.Append(type.Name, 0, genericPartIndex);

			builder.Append('<');
			for (var i = offset; i < length; i++)
			{
				ProcessType(builder, genericArguments[i]);
				if (i + 1 == length) continue;

				builder.Append(',');
				if (!genericArguments[i + 1].IsGenericParameter) builder.Append(' ');
			}

			builder.Append('>');
		}
	}
}
