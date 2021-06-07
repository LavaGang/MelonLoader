using System;
using System.Reflection;

namespace MelonLoader
{
    [AttributeUsage(AttributeTargets.Class)]
    public class RegisterTypeInIl2Cpp : Attribute
    {
        internal bool LogSuccess = true;
        public RegisterTypeInIl2Cpp() { }
        public RegisterTypeInIl2Cpp(bool logSuccess) { LogSuccess = logSuccess; }

        public static void RegisterAssembly(Assembly asm)
        {
            if (!MelonUtils.IsGameIl2Cpp())
                return;
            Type[] typeTbl = asm.GetTypes();
            if ((typeTbl == null) || (typeTbl.Length <= 0))
                return;
            foreach (Type type in typeTbl)
            {
                object[] attTbl = type.GetCustomAttributes(typeof(RegisterTypeInIl2Cpp), false);
                if ((attTbl == null) || (attTbl.Length <= 0))
                    continue;
                RegisterTypeInIl2Cpp att = (RegisterTypeInIl2Cpp)attTbl[0];
                if (att == null)
                    continue;
                UnhollowerSupport.RegisterTypeInIl2CppDomain(type, att.LogSuccess);
            }
        }
    }
}