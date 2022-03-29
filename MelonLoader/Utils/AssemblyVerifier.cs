#if NET6_0
using AsmResolver.DotNet;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MelonLoader.Utils
{
    internal static class AssemblyVerifier
    {
        private static bool IsNameValid(string name)
        {
            if (name is null) 
                return false;

            foreach (char c in name)
            {
                if (!(c >= 'a' && c <= 'z' || c >= 'A' && c <= 'Z' || c >= '0' && c <= '9' || c == '_' || c == '<' || c == '>'
                || c == '`' || c == '.' || c == '=' || c == '-' || c == '|' || c == ',' || c == '[' || c == ']' || c == '$'
                || c == ':' || c == '@' || c == ')' || c == '(' || c == '?' || c == '{' || c == '}'))
                    return false;
            }

            return true;
        }

        private static void CountChars(string str, ref Dictionary<char, int> map)
        {
            foreach (char c in str)
            {
                if (map.ContainsKey(c))
                    map[c]++;
                else
                    map.Add(c, 1);
            }
        }

        internal static bool CheckAssembly(ModuleDefinition image)
        {
            string imageName = image.Name;

            var moduleCount = image.Assembly.Modules.Count;

            if (moduleCount is not 1)
            {
                MelonDebug.Msg($"[AssemblyVerifier] {image.Name} Has an Invalid Module Count!");
                return false;
            }
            int numTypeDefs = image.GetAllTypes().Count();
            int numTypeRefs = image.GetImportedTypeReferences().Count();
            int numMethodDefs = 0;
            int numFieldDefs = 0;
            foreach(var type in image.GetAllTypes())
            {
                numMethodDefs += type.Methods.Count;
                numFieldDefs += type.Fields.Count;
            }

            int delegateIndex = -2;

            for (int i = 0; i < numTypeRefs; i++)
            {
                var type = image.GetImportedTypeReferences().ElementAt(i);

                var nsStr = type.Namespace;
                var nameStr = type.Name;

                if (nameStr.CompareTo("MulticastDelegate") == 0 && nsStr.CompareTo("System") == 0)
                {
                    delegateIndex = i;
                    break;
                }
            }

            var symbolCounts = new Dictionary<char, int>();
            for (int i = 0; i < numTypeDefs; i++)
            {
                var type = image.GetAllTypes().ElementAt(i);
                var typeNsStr = type.Namespace;
                var typeNameStr = type.Name;

                var a = type.BaseType;

                if (a is not null)
                {
                    var baseType = image.GetAllTypes().ToList().IndexOf((TypeDefinition)a);
                    if (baseType == delegateIndex)
                    {
                        var fieldIndex = i;
                        var nextFieldIndex = i == numTypeDefs - 1 ? (numFieldDefs + 1) : i + 1;
                        if (fieldIndex != nextFieldIndex)
                        {
                            MelonDebug.Msg($"[AssemblyVerifier] {image.Name} Has an Invalid field index!");
                            return false;
                        }
                    }
                }

                if (!IsNameValid(typeNsStr))
                {
                    MelonDebug.Msg($"[AssemblyVerifier] {image.Name} Has an Invalid Namespace String {typeNsStr}");
                    return false;
                }

                if (!IsNameValid(typeNameStr))
                {
                    MelonDebug.Msg($"[AssemblyVerifier] {image.Name} Has an Invalid Type Name String {typeNameStr}");
                    return false;
                }

                if (typeNameStr.Contains("<Module>"))
                {
                    //TODO: something about method indexes
                }

                CountChars(typeNameStr, ref symbolCounts);

            }

            foreach(var type in image.GetAllTypes())
            {
                foreach(var method in type.Methods)
                {
                    if (!IsNameValid(method.Name))
                    {
                        MelonDebug.Msg($"[AssemblyVerifier] {image.Name} Has an Invalid Method: {method.Name}!");
                        return false;
                    }
                    CountChars(method.Name, ref symbolCounts);
                }
            }

            if (numTypeDefs + numMethodDefs < 25)
                return true;

            double totalChars = 0;

            foreach(var pair in symbolCounts)
                totalChars += pair.Value;

            double totalEntropy = 0;

            foreach (var pair in symbolCounts)
                totalEntropy += pair.Value * Math.Log2(pair.Value / totalChars);

            totalEntropy /= -totalChars;

            if (totalEntropy < 4 || totalEntropy > 5.25)
            {
                MelonDebug.Msg($"[AssemblyVerifier] {image.Name} Has an Invalid Entropy: {totalEntropy}!");
                return false;
            }

            return true;

        }

        internal static (bool, string) LoadFromPatch(string assemblyFile)
        {
            if (assemblyFile is not null)
            {
                var module = ModuleDefinition.FromFile(assemblyFile);

                if (module is null)
                    return (false, "Failed to load assembly");

                bool checkResult = CheckAssembly(module);

                if (!checkResult)
                    return (false, "Invalid assembly");
            }

            return (true, null);
        }

        internal static (bool, string) LoadRawPatch(byte[] rawAssembly)
        {
            if (rawAssembly is not null)
            {
                var module = ModuleDefinition.FromBytes(rawAssembly);

                if (module is null)
                    return (false, "Failed to load assembly");

                bool checkResult = CheckAssembly(module);

                if (!checkResult)
                    return (false, "Invalid assembly");
            }
            return (true, null);
        }

        private static HarmonyMethod GetLocalPatch(string name)
        {
            return typeof(AssemblyVerifier).GetMethod(name, BindingFlags.NonPublic | BindingFlags.Static).ToNewHarmonyMethod();
        }

        internal static void InstallHooks()
        {
            /*var byteArray = new byte[] { };
            Type[] originalTypes = new Type[] { typeof(string) };
            MethodInfo callOriginalLoadFrom = typeof(Assembly).GetMethod(nameof(Assembly.LoadFrom), originalTypes);

            originalTypes = new Type[] { byteArray.GetType() };

            MethodInfo callOriginalLoadRaw = typeof(AppDomain).GetMethod(nameof(AppDomain.Load), originalTypes);

            Core.HarmonyInstance.Patch(callOriginalLoadFrom, GetLocalPatch(nameof(LoadFromPatch)), null);
            Core.HarmonyInstance.Patch(callOriginalLoadRaw, GetLocalPatch(nameof(LoadRawPatch)), null);*/

        }

        
    }
}
#endif