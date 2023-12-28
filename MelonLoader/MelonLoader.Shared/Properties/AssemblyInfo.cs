using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using MelonLoader.Properties;

[assembly: AssemblyTitle(BuildInfo.Description)]
[assembly: AssemblyDescription(BuildInfo.Description)]
[assembly: AssemblyCompany(BuildInfo.Company)]
[assembly: AssemblyProduct(BuildInfo.Name)]
[assembly: AssemblyCopyright("Created by " + BuildInfo.Author)]
[assembly: AssemblyTrademark(BuildInfo.Company)]
[assembly: Guid("A662769A-B294-434F-83B5-176FC4795334")]
[assembly: AssemblyVersion(BuildInfo.Version)]
[assembly: AssemblyFileVersion(BuildInfo.Version)]

[assembly: InternalsVisibleTo("MelonLoader.Bootstrap")]
