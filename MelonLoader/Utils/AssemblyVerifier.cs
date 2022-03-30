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
        private static HashSet<char> AllowedSymbols = new()
        {
            '_',
            '<',
            '>',
            '`',
            '.',
            '=',
            '-',
            '|',
            ',',
            '[',
            ']',
            '$',
            ':',
            '@',
            '(',
            ')',
            '?',
            '{',
            '}'
        };

        private static bool IsNameValid(string name)
        {
            if (name is null) 
                return false;

            foreach (char c in name)
            {
                bool isOk = false;
                isOk |= c is >= 'a' and <= 'z';
                isOk |= c is >= 'A' and <= 'Z';
                isOk |= c is >= '0' and <= '9';
                isOk |= AllowedSymbols.Contains(c);

                if (!isOk)
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
                //MelonDebug.Msg($"[AssemblyVerifier] {image.Name} Has an Invalid Module Count!");
                return false;
            }
            var allTypes = image.GetAllTypes().ToList();
            int numTypeDefs = allTypes.Count();
            int numTypeRefs = image.GetImportedTypeReferences().Count();
            int numMethodDefs = allTypes.SelectMany(t => t.Methods).Count();
            int numFieldDefs = allTypes.SelectMany(t => t.Fields).Count();

            var symbolCounts = new Dictionary<char, int>();
            foreach (var type in allTypes)
            {
                var typeNsStr = type.Namespace;
                var typeNameStr = type.Name;

                var baseType = type.BaseType;

                if (baseType != null && baseType.IsTypeOf("System", "MulticastDelegate"))
                {
                    if (type.Fields.Count != 0)
                    {
                        //MelonDebug.Msg($"{type.FullName} inherits from MulticastDelegate and has fields!");
                        return false;
                    }
                }

                if ((string) typeNsStr != null && !IsNameValid(typeNsStr))
                {
                    //MelonDebug.Msg($"[AssemblyVerifier] {image.Name} Has an Invalid Namespace String \"{typeNsStr ?? "null"}\"");
                    return false;
                }

                if (!IsNameValid(typeNameStr))
                {
                    //MelonDebug.Msg($"[AssemblyVerifier] {image.Name} Has an Invalid Type Name String \"{typeNameStr ?? null}\"");
                    return false;
                }

                if (typeNameStr == "<Module>")
                {
                    if (type.Fields.Count + type.Methods.Count != 0)
                    {
                        //MelonDebug.Msg($"[AssemblyVerifier] {image.Name} Has an Invalid Module with Fields or Methods!");
                        return false;
                    }
                }

                CountChars(typeNameStr, ref symbolCounts);
            }

            foreach(var type in allTypes)
            {
                foreach(var method in type.Methods)
                {
                    if (!IsNameValid(method.Name))
                    {
                        //MelonDebug.Msg($"[AssemblyVerifier] {image.Name} Has an Invalid Method: {method.Name}!");
                        return false;
                    }
                    CountChars(method.Name, ref symbolCounts);
                }
            }

            if (numTypeDefs + numMethodDefs < 25)
            {
                //MelonDebug.Msg($"[AssemblyVerifier] {image.Name} has too few chars to check entropy");
                return true;
            }

            double totalChars = 0;

            foreach(var pair in symbolCounts)
                totalChars += pair.Value;

            double totalEntropy = 0;

            foreach (var pair in symbolCounts)
                totalEntropy += pair.Value * Math.Log2(pair.Value / totalChars);

            totalEntropy /= -totalChars;

            if (totalEntropy < 4 || totalEntropy > 5.25)
            {
                //MelonDebug.Msg($"[AssemblyVerifier] {image.Name} Has an Invalid Entropy: {totalEntropy}!");
                return false;
            }

            //MelonDebug.Msg($"[AssemblyVerifier] {image.Name} passes");

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