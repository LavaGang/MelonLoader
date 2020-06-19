using System.Resources;
using System.Reflection;
using System.Runtime.InteropServices;
using MelonLoader;

[assembly: AssemblyTitle(TestPlugin.BuildInfo.Name)]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany(TestPlugin.BuildInfo.Company)]
[assembly: AssemblyProduct(TestPlugin.BuildInfo.Name)]
[assembly: AssemblyCopyright("Created by " + TestPlugin.BuildInfo.Author)]
[assembly: AssemblyTrademark(TestPlugin.BuildInfo.Company)]
[assembly: AssemblyCulture("")]
[assembly: ComVisible(false)]
//[assembly: Guid("")]
[assembly: AssemblyVersion(TestPlugin.BuildInfo.Version)]
[assembly: AssemblyFileVersion(TestPlugin.BuildInfo.Version)]
[assembly: NeutralResourcesLanguage("en")]
[assembly: MelonModInfo(typeof(TestPlugin.TestPlugin), TestPlugin.BuildInfo.Name, TestPlugin.BuildInfo.Version, TestPlugin.BuildInfo.Author, TestPlugin.BuildInfo.DownloadLink)]


// Create and Setup a MelonModGame to mark a Mod as Universal or Compatible with specific Games.
// If no MelonModGameAttribute is found or any of the Values for any MelonModGame on the Mod is null or empty it will be assumed the Mod is Universal.
// Values for MelonModGame can be found in the Game's app.info file or printed at the top of every log directly beneath the Unity version.
[assembly: MelonModGame(null, null)]