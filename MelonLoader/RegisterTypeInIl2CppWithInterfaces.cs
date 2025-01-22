using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MelonLoader;

[AttributeUsage(AttributeTargets.Class)]
public class RegisterTypeInIl2CppWithInterfaces : Attribute //Naming violation?
{
    internal static List<Assembly> registrationQueue = [];
    internal static bool ready;
    internal bool LogSuccess = true;

    internal Type[] Interfaces;
    internal bool GetInterfacesFromType;

    public RegisterTypeInIl2CppWithInterfaces()
    {
        GetInterfacesFromType = true;
    }

    public RegisterTypeInIl2CppWithInterfaces(bool logSuccess)
    {
        LogSuccess = logSuccess;
        GetInterfacesFromType = true;
    }

    public RegisterTypeInIl2CppWithInterfaces(params Type[] interfaces)
    {
        Interfaces = interfaces;
    }

    public RegisterTypeInIl2CppWithInterfaces(bool logSuccess, params Type[] interfaces)
    {
        LogSuccess = logSuccess;
        Interfaces = interfaces;
    }

    public static void RegisterAssembly(Assembly asm)
    {
        if (!MelonUtils.IsGameIl2Cpp())
            return;

        if (!ready)
        {
            registrationQueue.Add(asm);
            return;
        }

        var typeTbl = asm.GetValidTypes();
        if ((typeTbl == null) || (!typeTbl.Any()))
            return;

        foreach (var type in typeTbl)
        {
            var attTbl = type.GetCustomAttributes(typeof(RegisterTypeInIl2CppWithInterfaces), false);
            if ((attTbl == null) || (attTbl.Length <= 0))
                continue;

            var att = (RegisterTypeInIl2CppWithInterfaces)attTbl[0];
            if (att == null)
                continue;

            var interfaceArr = att.GetInterfacesFromType
                ? type.GetInterfaces()
                : att.Interfaces;

            InteropSupport.RegisterTypeInIl2CppDomainWithInterfaces(type,
                interfaceArr,
                att.LogSuccess);
        }
    }

    internal static void SetReady()
    {
        ready = true;

        if (registrationQueue == null)
            return;

        foreach (var asm in registrationQueue)
            RegisterAssembly(asm);

        registrationQueue = null;
    }
}