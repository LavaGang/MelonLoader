#if NET6_0
using AsmResolver.DotNet;
using AsmResolver.PE.DotNet.Metadata.Strings;
using AsmResolver.PE.DotNet.Metadata.Tables;
using AsmResolver.PE.DotNet.Metadata.Tables.Rows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

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

        [MethodImpl(MethodImplOptions.NoInlining)]
        internal static void EnsureInitialized()
        {
            var dummyListToEnsureThisCodeDoesntGetNuked = new List<object>();

            //Force load AsmResolver
            dummyListToEnsureThisCodeDoesntGetNuked.Add(new Constant(AsmResolver.PE.DotNet.Metadata.Tables.Rows.ElementType.Class, null));
            dummyListToEnsureThisCodeDoesntGetNuked.Add(typeof(AsmResolver.PE.File.PEFile));
        }

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
            // string imageName = image.Name;

            var moduleCount = image.Assembly!.Modules.Count;

            if (moduleCount is not 1)
            {
                //MelonDebug.Msg($"[AssemblyVerifier] {image.Name} Has an Invalid Module Count!");
                return false;
            }
            var tableStream = image.DotNetDirectory!.Metadata!.GetStream<TablesStream>();
            var stringStream = image.DotNetDirectory.Metadata.GetStream<StringsStream>();

            var allTypes = image.GetAllTypes().ToList();
            var numTypeDefs = allTypes.Count;

            var methodTable = (MetadataTable<MethodDefinitionRow>) tableStream.GetTable(TableIndex.Method);
            var numMethodDefs = methodTable.Count;

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

            foreach(var method in methodTable)
            {
                var methodName = stringStream.GetStringByIndex(method.Name);

                if(!IsNameValid(methodName))
                {
                    //MelonDebug.Msg($"[AssemblyVerifier] {image.Name} Has an Invalid Method: {method.Name}!");
                    return false;
                }

                CountChars(methodName, ref symbolCounts);
            }

            if (numTypeDefs + numMethodDefs < 25)
            {
                //MelonDebug.Msg($"[AssemblyVerifier] {image.Name} has too few chars to check entropy");
                return true;
            }

            var totalChars = symbolCounts.Aggregate(0.0, (current, pair) => current + pair.Value);

            var totalEntropy = symbolCounts.Sum(pair => pair.Value * Math.Log2(pair.Value / totalChars));

            totalEntropy /= -totalChars;

            if (totalEntropy is < 4 or > 5.5)
            {
                //MelonDebug.Msg($"[AssemblyVerifier] {image.Name} Has an Invalid Entropy: {totalEntropy}!");
                return false;
            }

            //MelonDebug.Msg($"[AssemblyVerifier] {image.Name} passes");

            return true;

        }

        internal static (bool, string) VerifyFile(string assemblyFile)
        {
            if (assemblyFile is not null)
            {
                var module = ModuleDefinition.FromFile(assemblyFile);

                var checkResult = CheckAssembly(module);

                if (!checkResult)
                    return (false, "Invalid assembly");
            }

            return (true, null);
        }

        internal static (bool, string) VerifyByteArray(byte[] rawAssembly)
        {
            if (rawAssembly is not null)
            {
                var module = ModuleDefinition.FromBytes(rawAssembly);

                var checkResult = CheckAssembly(module);

                if (!checkResult)
                    return (false, "Invalid assembly");
            }
            return (true, null);
        }
    }
}
#endif
