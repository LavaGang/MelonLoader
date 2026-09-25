using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MelonLoader
{
    [AttributeUsage(AttributeTargets.Class)]
    public class RegisterTypeInIl2CppWithInterfaces : Attribute //Naming violation?
    {
        internal static List<Assembly> registrationQueue = new List<Assembly>();
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
            
            try { ProcessAssembly(asm); }
            catch { }
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
        
        private static void ProcessAssembly(Assembly asm)
        {
            IEnumerable<Type> typeTbl = asm.GetValidTypes();
            if ((typeTbl == null) || (typeTbl.Count() <= 0))
                return;

            foreach (Type type in typeTbl)
            {
                object[] attTbl = [];
                try { attTbl = type.GetCustomAttributes(typeof(RegisterTypeInIl2Cpp), true); }
                catch { continue; }
                if (attTbl.Length <= 0)
                    continue;

                RegisterTypeInIl2CppWithInterfaces att = (RegisterTypeInIl2CppWithInterfaces)attTbl[0];
                if (att == null)
                    continue;

                Type[] interfaceArr = att.GetInterfacesFromType
                    ? type.GetInterfaces()
                    : att.Interfaces;

                bool shouldLogSuccess = MelonDebug.IsEnabled()
                                        || att.LogSuccess;
                
                try
                {
                    InteropSupport.RegisterTypeInIl2CppDomainWithInterfaces(type,
                        interfaceArr,
                        shouldLogSuccess);
                }
                catch (Exception e)
                {
                    MelonLogger.Error($"Failed to register custom type {type.FullName}");
                    MelonLogger.Error(e.ToString());
                }
            }
        }
    }
}