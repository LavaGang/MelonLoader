using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

[assembly: AssemblyTitle(MelonLoader.BuildInfo.Description)]
[assembly: AssemblyDescription(MelonLoader.BuildInfo.Description)]
[assembly: AssemblyCompany(MelonLoader.BuildInfo.Company)]
[assembly: AssemblyProduct(MelonLoader.BuildInfo.Name)]
[assembly: AssemblyCopyright("Created by " + MelonLoader.BuildInfo.Author)]
[assembly: AssemblyTrademark(MelonLoader.BuildInfo.Company)]
[assembly: Guid("A662769A-B294-434F-83B5-176FC4795334")]
[assembly: AssemblyVersion(MelonLoader.BuildInfo.Version)]
[assembly: AssemblyFileVersion(MelonLoader.BuildInfo.Version)]
[assembly: MelonLoader.PatchShield]

[assembly: InternalsVisibleTo("MelonLoader.NativeHost")]
[assembly: InternalsVisibleTo("Il2CppAssemblyGenerator")]
[assembly: InternalsVisibleTo("Il2CppUnityTls")]
[assembly: InternalsVisibleTo("Il2Cpp")]
[assembly: InternalsVisibleTo("MelonStartScreen")] 