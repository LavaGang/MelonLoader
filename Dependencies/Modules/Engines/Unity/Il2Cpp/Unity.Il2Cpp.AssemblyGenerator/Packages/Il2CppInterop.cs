using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Il2CppInterop.Common;
using Il2CppInterop.Generator;
using Il2CppInterop.Generator.Runners;
using Microsoft.Extensions.Logging;
using AsmResolver.DotNet;
using AsmResolver.DotNet.Serialized;

namespace MelonLoader.Engine.Unity.Packages
{
    internal class Il2CppInterop : Models.ExecutablePackage
    {
        internal Il2CppInterop(string BasePath)
        {
            Version = typeof(Il2CppInteropGenerator).Assembly.CustomAttributes
                .Where(x => x.AttributeType.Name == "AssemblyInformationalVersionAttribute")
                .Select(x => x.ConstructorArguments[0].Value.ToString())
                .FirstOrDefault();

            Name = nameof(Il2CppInterop);
            Destination = Path.Combine(BasePath, Name);
            OutputFolder = Path.Combine(Destination, "Il2CppAssemblies");
        }

        internal override bool ShouldSetup()
            => false;

        internal bool Execute(string GameAssemblyPath)
        {
            AssemblyGenerator.Logger.Msg("Reading dumped assemblies for interop generation...");

            var resolver = new InteropResolver();
            var inputAssemblies = Directory.GetFiles(AssemblyGenerator.cpp2il.OutputFolder)
                .Where(f => f.EndsWith(".dll"))
                .Select(f => ModuleDefinition.FromFile(f, new ModuleReaderParameters() { ModuleResolver = resolver }))
                .Select(f => { resolver.Add(f); return f; })
                .Select(f => f.Assembly)
                .ToList();

            var opts = new GeneratorOptions()
            {
                GameAssemblyPath = GameAssemblyPath,
                Source = inputAssemblies,
                OutputDir = OutputFolder,
                UnityBaseLibsDir = AssemblyGenerator.unitydependencies.Destination,
                ObfuscatedNamesRegex = string.IsNullOrEmpty(AssemblyGenerator.deobfuscationRegex.Regex) ? null : new Regex(AssemblyGenerator.deobfuscationRegex.Regex),
                Parallel = true,
                Il2CppPrefixMode = GeneratorOptions.PrefixMode.OptOut,
            };

            //Inform cecil of the unity base libs
            var trusted = (string)AppDomain.CurrentDomain.GetData("TRUSTED_PLATFORM_ASSEMBLIES");
            var allUnityDlls = string.Join(Path.PathSeparator, Directory.GetFiles(AssemblyGenerator.unitydependencies.Destination, "*.dll", SearchOption.TopDirectoryOnly));
            // var allDumpedDlls = string.Join(Path.PathSeparator, Directory.GetFiles(AssemblyGenerator.dumper.OutputFolder, "*.dll", SearchOption.TopDirectoryOnly));
            AppDomain.CurrentDomain.SetData("TRUSTED_PLATFORM_ASSEMBLIES", trusted + Path.PathSeparator + allUnityDlls);

            if (!string.IsNullOrEmpty(AssemblyGenerator.deobfuscationMap.Version))
            {
                AssemblyGenerator.Logger.Msg("Loading Deobfuscation Map...");
                opts.ReadRenameMap(AssemblyGenerator.deobfuscationMap.Destination);
            }

            AssemblyGenerator.Logger.Msg("Generating Interop Assemblies...");

#if !DEBUG
            try
#endif
            {
                Il2CppInteropGenerator.Create(opts)
                    .AddLogger(new InteropLogger())
                    .AddInteropAssemblyGenerator()
                    .Run();
            }
#if !DEBUG
            catch (Exception e)
            {
                AssemblyGenerator.Logger.Error("Error Generating Interop Assemblies!", e);
                return false;
            }
#endif

            AssemblyGenerator.Logger.Msg("Cleaning up...");
            AppDomain.CurrentDomain.SetData("TRUSTED_PLATFORM_ASSEMBLIES", trusted);
            //inputAssemblies.ForEach(a => a.Dispose());

            AssemblyGenerator.Logger.Msg("Interop Generation Complete!");
            return true;
        }
    }
    
    internal class InteropLogger : ILogger
    {
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (logLevel is LogLevel.Debug or LogLevel.Trace)
            {
                MelonDebug.Msg(formatter(state, exception));
                return;
            }
            
            AssemblyGenerator.Logger.Msg(formatter(state, exception));
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return logLevel switch
            {
                LogLevel.Debug or LogLevel.Trace => MelonDebug.IsEnabled(),
                _ => true
            };
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            throw new NotImplementedException();
        }
    }

    internal class InteropResolver : INetModuleResolver
    {
        private readonly Dictionary<string, ModuleDefinition> _cache = new();
        
        public void Dispose()
        {
            _cache.Clear();
        }
        
        internal void Add(ModuleDefinition module)
        {
            _cache[module.Name] = module;
        }

        public ModuleDefinition Resolve(string name)
        {
            return _cache.GetValueOrDefault(name);
        }
    }
}
