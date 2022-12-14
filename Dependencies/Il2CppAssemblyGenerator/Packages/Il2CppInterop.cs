using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Il2CppInterop.Common;
using Il2CppInterop.Generator;
using Il2CppInterop.Generator.Runners;
using Microsoft.Extensions.Logging;
using Mono.Cecil;

namespace MelonLoader.Il2CppAssemblyGenerator.Packages
{
    internal class Il2CppInterop : Models.ExecutablePackage
    {
        internal Il2CppInterop()
        {
            Version = typeof(Il2CppInteropGenerator).Assembly.CustomAttributes
                .Where(x => x.AttributeType.Name == "AssemblyInformationalVersionAttribute")
                .Select(x => x.ConstructorArguments[0].Value.ToString())
                .FirstOrDefault();

            Name = nameof(Il2CppInterop);
            Destination = Path.Combine(Core.BasePath, Name);
            OutputFolder = Path.Combine(Destination, "Il2CppAssemblies");
        }

        internal override bool ShouldSetup()
            => !Config.Values.UseInterop;

        internal override bool Execute()
        {
            if (Config.Values.UseInterop)
            {
                return ExecuteInterop();
            }
            
            if (Execute(new string[] {
                $"--input={ Core.dumper.OutputFolder }",
                $"--output={ OutputFolder }",
                $"--mscorlib={ Path.Combine(Core.ManagedPath, "mscorlib.dll") }",
                $"--unity={ Core.unitydependencies.Destination }",
                $"--gameassembly={ Core.GameAssemblyPath }",
                string.IsNullOrEmpty(Core.deobfuscationMap.Version) ? string.Empty : $"--rename-map={ Core.deobfuscationMap.Destination }",
                string.IsNullOrEmpty(Core.deobfuscationRegex.Regex) ? string.Empty : $"--obf-regex={ Core.deobfuscationRegex.Regex }",
                "--add-prefix-to=ICSharpCode",
                "--add-prefix-to=Newtonsoft",
                "--add-prefix-to=TinyJson",
                "--add-prefix-to=Valve.Newtonsoft"
            }))
                return true;
            return false;
        }

        private bool ExecuteInterop()
        {
            Core.Logger.Msg("Reading dumped assemblies for interop generation...");

            var resolver = new InteropResolver();
            var inputAssemblies = Directory.GetFiles(Core.dumper.OutputFolder)
                .Where(f => f.EndsWith(".dll"))
                .Select(f => ModuleDefinition.ReadModule(f, new ReaderParameters() {AssemblyResolver = resolver}))
                .Select(m => m.Assembly)
                .ToList();
            
            inputAssemblies.ForEach(resolver.Add);
            
            var opts = new GeneratorOptions()
            {
                GameAssemblyPath = Core.GameAssemblyPath,
                Source = inputAssemblies,
                OutputDir = OutputFolder,
                UnityBaseLibsDir = Core.unitydependencies.Destination,
                ObfuscatedNamesRegex = string.IsNullOrEmpty(Core.deobfuscationRegex.Regex) ? null : new Regex(Core.deobfuscationRegex.Regex),
                Parallel = true,
                Il2CppPrefixMode = GeneratorOptions.PrefixMode.OptOut,
            };
            
            //Inform cecil of the unity base libs
            var trusted = (string) AppDomain.CurrentDomain.GetData("TRUSTED_PLATFORM_ASSEMBLIES");
            var allUnityDlls = string.Join(Path.PathSeparator, Directory.GetFiles(Core.unitydependencies.Destination, "*.dll", SearchOption.TopDirectoryOnly));
            // var allDumpedDlls = string.Join(Path.PathSeparator, Directory.GetFiles(Core.dumper.OutputFolder, "*.dll", SearchOption.TopDirectoryOnly));
            AppDomain.CurrentDomain.SetData("TRUSTED_PLATFORM_ASSEMBLIES", trusted + Path.PathSeparator + allUnityDlls);

            if (!string.IsNullOrEmpty(Core.deobfuscationMap.Version))
            {
                Core.Logger.Msg("Loading Deobfuscation Map...");
                opts.ReadRenameMap(Core.deobfuscationMap.Destination);
            }

            Core.Logger.Msg("Generating Interop Assemblies...");

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
                Core.Logger.Error("Error Generating Interop Assemblies!", e);
                return false;
            }
#endif

            Core.Logger.Msg("Cleaning up...");
            AppDomain.CurrentDomain.SetData("TRUSTED_PLATFORM_ASSEMBLIES", trusted);
            inputAssemblies.ForEach(a => a.Dispose());
            
            Core.Logger.Msg("Interop Generation Complete!");
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
            
            Core.Logger.Msg(formatter(state, exception));
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

    internal class InteropResolver : IAssemblyResolver
    {
        private readonly Dictionary<string, AssemblyDefinition> _cache = new();
        
        public void Dispose()
        {
            _cache.Clear();
        }
        
        internal void Add(AssemblyDefinition assembly)
        {
            _cache[assembly.Name.Name] = assembly;
        }

        public AssemblyDefinition Resolve(AssemblyNameReference name)
        {
            return _cache.GetValueOrDefault(name.Name);
        }

        public AssemblyDefinition Resolve(AssemblyNameReference name, ReaderParameters parameters)
        {
            return Resolve(name);
        }
    }
}
