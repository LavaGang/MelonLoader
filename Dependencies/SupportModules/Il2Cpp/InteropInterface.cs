﻿using Il2CppInterop.Common;
using Il2CppInterop.Common.Attributes;
using Il2CppInterop.Runtime;
using Il2CppInterop.Runtime.Injection;
using Il2CppInterop.Runtime.InteropTypes;
using System;
using System.Reflection;

namespace MelonLoader.Support;

internal class InteropInterface : InteropSupport.Interface
{
    public IntPtr CopyMethodInfoStruct(IntPtr ptr)
        => ptr;
    //=> UnityVersionHandler.CopyMethodInfoStruct(ptr);

    public int? GetIl2CppMethodCallerCount(MethodBase method)
    {
        var att = method.GetCustomAttribute<CallerCountAttribute>();
        return att?.Count;
    }

    public FieldInfo MethodBaseToIl2CppFieldInfo(MethodBase method)
        => Il2CppInteropUtils.GetIl2CppMethodInfoPointerFieldForGeneratedMethod(method);

    public void RegisterTypeInIl2CppDomain(Type type, bool logSuccess)
        => ClassInjector.RegisterTypeInIl2Cpp(type, new() { LogSuccess = logSuccess });
    public void RegisterTypeInIl2CppDomainWithInterfaces(Type type, Type[] interfaces, bool logSuccess)
        => ClassInjector.RegisterTypeInIl2Cpp(type, new() { LogSuccess = logSuccess, Interfaces = interfaces });

    public bool IsInheritedFromIl2CppObjectBase(Type type)
        => (type != null) && type.IsSubclassOf(typeof(Il2CppObjectBase));

    public bool IsInjectedType(Type type)
    {
        var ptr = GetClassPointerForType(type);
        return ptr != IntPtr.Zero && RuntimeSpecificsStore.IsInjected(ptr);
    }

    public IntPtr GetClassPointerForType(Type type)
    {
        return type == typeof(void)
            ? Il2CppClassPointerStore<Il2CppSystem.Void>.NativeClassPtr
            : (IntPtr)typeof(Il2CppClassPointerStore<>).MakeGenericType(type)
              .GetField(nameof(Il2CppClassPointerStore<int>.NativeClassPtr)).GetValue(null);
    }
}
