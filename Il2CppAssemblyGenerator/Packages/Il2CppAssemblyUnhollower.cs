using System.Collections.Generic;
using System.IO;
using AssemblyUnhollower;
using AssemblyUnhollower.Contexts;
using AssemblyUnhollower.Passes;
using UnhollowerBaseLib;

namespace MelonLoader.Il2CppAssemblyGenerator
{
    internal class Il2CppAssemblyUnhollower : ExecutablePackageBase
    {
#if PORT_DISABLE
        internal Il2CppAssemblyUnhollower()
        {
            Version = MelonCommandLine.AssemblyGenerator.ForceVersion_Il2CppAssemblyUnhollower;
            if (string.IsNullOrEmpty(Version) || Version.Equals("0.0.0.0"))
                Version = RemoteAPI.LAST_RESPONSE.ForceUnhollowerVersion;
            if (string.IsNullOrEmpty(Version) || Version.Equals("0.0.0.0"))
                Version = "0.4.13.0";
            URL = "https://github.com/knah/Il2CppAssemblyUnhollower/releases/download/v" + Version + "/Il2CppAssemblyUnhollower." + Version + ".zip";
            Destination = Path.Combine(Core.BasePath, "Il2CppAssemblyUnhollower");
            Output = Path.Combine(Destination, "Managed");
            ExePath = Path.Combine(Destination, "AssemblyUnhollower.exe");
        }

        private void Save()
        {
            Config.Il2CppAssemblyUnhollowerVersion = Version;
            Config.Save();
        }

        private bool ShouldDownload() => (string.IsNullOrEmpty(Config.Il2CppAssemblyUnhollowerVersion) || !Config.Il2CppAssemblyUnhollowerVersion.Equals(Version));

        internal override bool Download()
        {
            if (!ShouldDownload())
            {
                MelonLogger.Msg("Il2CppAssemblyUnhollower is up to date. No Download Needed.");
                return true;
            }
            MelonLogger.Msg("Downloading Il2CppAssemblyUnhollower...");
            if (base.Download())
            {
                Save();
                return true;
            }
            return false;
        }

        internal bool Execute()
        {
            MelonLogger.Msg("Executing Il2CppAssemblyUnhollower...");
            List<string> parameters = new List<string>();
            parameters.Add($"--input={ Core.dumper.Output }");
            parameters.Add($"--output={ Output }");
            parameters.Add($"--mscorlib={ Path.Combine(Core.ManagedPath, "mscorlib.dll") }");
            parameters.Add($"--unity={ Core.unitydependencies.Destination }");
            parameters.Add($"--gameassembly={ Core.GameAssemblyPath }");
            if (!string.IsNullOrEmpty(Core.deobfuscationMap.Version))
                parameters.Add($"--rename-map={ Path.Combine(Core.deobfuscationMap.Destination, Core.deobfuscationMap.NewFileName) }");
            parameters.Add("--blacklist-assembly=Mono.Security");
            parameters.Add("--blacklist-assembly=Newtonsoft.Json");
            parameters.Add("--blacklist-assembly=Valve.Newtonsoft.Json");
            if (!string.IsNullOrEmpty(Core.deobfuscationMap.ObfuscationRegex))
                parameters.Add($"--obf-regex={ Core.deobfuscationMap.ObfuscationRegex }");
            return Execute(parameters.ToArray());
        }
#else
        internal Il2CppAssemblyUnhollower()
        {
            Destination = Path.Combine(Core.BasePath, "Il2CppAssemblyUnhollower");
            Output = Path.Combine(Destination);
        }

        internal bool Execute()
        {
            MelonLogger.Msg("Executing Il2CppAssemblyUnhollower...");

            InstallLogger();

            var options = GenerateOptions();

            return Main(options);
        }

        private bool Main(UnhollowerOptions options)
        {
            if (!Directory.Exists(options.OutputDir))
                Directory.CreateDirectory(options.OutputDir);

            RewriteGlobalContext rewriteContext;

            LogSupport.Info("Reading assemblies");
            rewriteContext = new RewriteGlobalContext(options, Directory.EnumerateFiles(options.SourceDir, "*.dll"));

            LogSupport.Info("Computing renames");
            Pass05CreateRenameGroups.DoPass(rewriteContext);
            LogSupport.Info("Creating typedefs");
            Pass10CreateTypedefs.DoPass(rewriteContext);
            LogSupport.Info("Computing struct blittability");
            Pass11ComputeTypeSpecifics.DoPass(rewriteContext);
            LogSupport.Info("Filling typedefs");
            Pass12FillTypedefs.DoPass(rewriteContext);
            LogSupport.Info("Filling generic constraints");
            Pass13FillGenericConstraints.DoPass(rewriteContext);
            LogSupport.Info("Creating members");
            Pass15GenerateMemberContexts.DoPass(rewriteContext);
            LogSupport.Info("Scanning method cross-references");
            Pass16ScanMethodRefs.DoPass(rewriteContext, options);
            LogSupport.Info("Finalizing method declarations");
            Pass18FinalizeMethodContexts.DoPass(rewriteContext);
            LogSupport.Info($"{Pass18FinalizeMethodContexts.TotalPotentiallyDeadMethods} total potentially dead methods");
            LogSupport.Info("Filling method parameters");
            Pass19CopyMethodParameters.DoPass(rewriteContext);

            LogSupport.Info("Creating static constructors");
            Pass20GenerateStaticConstructors.DoPass(rewriteContext);
            LogSupport.Info("Creating value type fields");
            Pass21GenerateValueTypeFields.DoPass(rewriteContext);
            LogSupport.Info("Creating enums");
            Pass22GenerateEnums.DoPass(rewriteContext);
            LogSupport.Info("Creating IntPtr constructors");
            Pass23GeneratePointerConstructors.DoPass(rewriteContext);
            LogSupport.Info("Creating type getters");
            Pass24GenerateTypeStaticGetters.DoPass(rewriteContext);
            LogSupport.Info("Creating non-blittable struct constructors");
            Pass25GenerateNonBlittableValueTypeDefaultCtors.DoPass(rewriteContext);

            LogSupport.Info("Creating generic method static constructors");
            Pass30GenerateGenericMethodStoreConstructors.DoPass(rewriteContext);
            LogSupport.Info("Creating field accessors");
            Pass40GenerateFieldAccessors.DoPass(rewriteContext);
            LogSupport.Info("Filling methods");
            Pass50GenerateMethods.DoPass(rewriteContext);
            LogSupport.Info("Generating implicit conversions");
            Pass60AddImplicitConversions.DoPass(rewriteContext);
            LogSupport.Info("Creating properties");
            Pass70GenerateProperties.DoPass(rewriteContext);

            if (options.UnityBaseLibsDir != null)
            {
                LogSupport.Info("Unstripping types");
                Pass79UnstripTypes.DoPass(rewriteContext);
                LogSupport.Info("Unstripping fields");
                Pass80UnstripFields.DoPass(rewriteContext);
                LogSupport.Info("Unstripping methods");
                Pass80UnstripMethods.DoPass(rewriteContext);
                LogSupport.Info("Unstripping method bodies");
                Pass81FillUnstrippedMethodBodies.DoPass(rewriteContext);
            }
            else
                LogSupport.Warning("Not performing unstripping as unity libs are not specified");

            LogSupport.Info("Generating forwarded types");
            Pass89GenerateForwarders.DoPass(rewriteContext);

            LogSupport.Info("Writing xref cache");
            Pass89GenerateMethodXrefCache.DoPass(rewriteContext, options);

            LogSupport.Info("Writing assemblies");
            Pass90WriteToDisk.DoPass(rewriteContext, options);

            LogSupport.Info("Writing method pointer map");
            Pass91GenerateMethodPointerMap.DoPass(rewriteContext, options);

            if (!options.NoCopyUnhollowerLibs)
            {
                File.Copy(typeof(IL2CPP).Assembly.Location, Path.Combine(options.OutputDir, typeof(IL2CPP).Assembly.GetName().Name + ".dll"), true);
                File.Copy(typeof(UnhollowerRuntimeLib.RuntimeLibMarker).Assembly.Location, Path.Combine(options.OutputDir, typeof(UnhollowerRuntimeLib.RuntimeLibMarker).Assembly.GetName().Name + ".dll"), true);
                //File.Copy(typeof(Iced.In).Assembly.Location, Path.Combine(options.OutputDir, typeof(Decoder).Assembly.GetName().Name + ".dll"), true);
            }

            LogSupport.Info("Done!");

            rewriteContext.Dispose();

            return true;
        }

        private void InstallLogger()
        {
            UnhollowerBaseLib.LogSupport.InfoHandler += (msg) => { MelonLogger.Msg(System.ConsoleColor.Magenta, $"[{nameof(AssemblyUnhollower)}] {msg}"); };
            UnhollowerBaseLib.LogSupport.WarningHandler += (msg) => { MelonLogger.Warning($"[{nameof(AssemblyUnhollower)}] {msg}"); };
            UnhollowerBaseLib.LogSupport.ErrorHandler += (msg) => { MelonLogger.Error($"[{nameof(AssemblyUnhollower)}] {msg}"); };

            //if (MelonDebug.IsEnabled())
            //UnhollowerBaseLib.LogSupport.TraceHandler += (msg) => { MelonLogger.Error($"[{nameof(AssemblyUnhollower)}] {msg}"); };
        }

        private AssemblyUnhollower.UnhollowerOptions GenerateOptions()
        {
            var options = new AssemblyUnhollower.UnhollowerOptions
            {
                Verbose = true,
                SourceDir = new Il2CppDumper().Output,
                OutputDir = Output,
                MscorlibPath = Path.Combine(Core.BasePath, "reference_assemblies", "mscorlib.dll"),
                UnityBaseLibsDir = Path.Combine(Core.BasePath, "unity"),
                GameAssemblyPath = Core.GameAssemblyPath
            };

            options.AdditionalAssembliesBlacklist.AddRange(new List<string> { "Mono.Security", "Newtonsoft.Json", "Valve.Newtonsoft.Json" });

            return options;
        }
#endif
    }
}