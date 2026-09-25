using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MelonLoader
{
    [AttributeUsage(AttributeTargets.Class)]
    public class RegisterTypeInIl2Cpp : Attribute //Naming violation?
    {
        internal static List<Assembly> registrationQueue = new List<Assembly>();
        internal static bool ready;
        internal bool LogSuccess = true;

        public RegisterTypeInIl2Cpp() { }
        public RegisterTypeInIl2Cpp(bool logSuccess) { LogSuccess = logSuccess; }

        public static bool TryRegisterAssembly(Assembly asm)
        {
            try
            {
                RegisterAssembly(asm);
                return true;
            }
            catch (Exception e)
            {
                MelonLogger.Error(e.ToString());
            }
            return false;
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
            
            ProcessAssembly(asm);
        }

        internal static void SetReady()
        {
            ready = true;

            if (registrationQueue == null)
                return;

            foreach (var asm in registrationQueue)
                TryRegisterAssembly(asm);

            registrationQueue = null;
        }
        
        private static void ProcessAssembly(Assembly asm)
        {
            IEnumerable<Type> typeTbl = asm.GetValidTypes();
            if ((typeTbl == null) || (typeTbl.Count() <= 0))
                return;
            
            foreach (Type type in typeTbl)
            {
                object[] attTbl = type.GetCustomAttributes(typeof(RegisterTypeInIl2Cpp), true);
                if ((attTbl == null) || (attTbl.Length <= 0))
                    continue;
                
                RegisterTypeInIl2Cpp att = (RegisterTypeInIl2Cpp)attTbl[0];
                if (att == null)
                    continue;

                bool shouldLogSuccess = MelonDebug.IsEnabled() 
                    || att.LogSuccess;

                InteropSupport.RegisterTypeInIl2CppDomain(type, 
                    shouldLogSuccess);
            }
        }
    }
}