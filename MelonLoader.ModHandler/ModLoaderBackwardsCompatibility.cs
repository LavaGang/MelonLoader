using System;
using System.Linq;
using System.Reflection;

namespace MelonLoader
{
    internal static class ModLoaderBackwardsCompatibility
    {
        public static void AddAssemblyResolveHandler() => AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(AssemblyResolveHandler);
        private static Assembly AssemblyResolveHandler(object sender, ResolveEventArgs args)
        {
            string requestedName = (args.Name.Contains(',') ? args.Name.Substring(0, args.Name.IndexOf(',')) : args.Name.Replace(".dll", "")).Replace(".", "_");
            MelonModLogger.Log(requestedName);
            if (requestedName.Equals("VRCModLoader"))
                return Assembly.GetExecutingAssembly();
            return null;
        }

        internal static void VRCModLoader(Type t, Assembly assembly)
        {
            try
            {
                MelonMod modInstance = Activator.CreateInstance(t) as MelonMod;
                VRCModLoader.VRCModInfoAttribute modInfoAttribute = modInstance.GetType().GetCustomAttributes(typeof(VRCModLoader.VRCModInfoAttribute), true).FirstOrDefault() as VRCModLoader.VRCModInfoAttribute;
                if (modInfoAttribute != null)
                {
                    modInstance.Name = modInfoAttribute.Name;
                    modInstance.Version = modInfoAttribute.Version;
                    modInstance.Author = modInfoAttribute.Author;
                    modInstance.DownloadLink = modInfoAttribute.DownloadLink;
                }
                Main.ModControllers.Add(new MelonModController(modInstance, t, assembly));
            }
            catch (Exception e)
            {
                MelonModLogger.LogError("Could not load mod " + t.FullName + " in " + assembly.GetName() + "! " + e);
            }
        }
    }
}

namespace VRCModLoader
{
    public abstract class VRCMod
    {
        public string Name { get; internal set; }
        public string Version { get; internal set; }
        public string Author { get; internal set; }
        public string DownloadLink { get; internal set; }
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class VRCModInfoAttribute : Attribute
    {
        public string Name { get; }
        public string Version { get; }
        public string Author { get; }
        public string DownloadLink { get; }
        public VRCModInfoAttribute(string name, string version, string author, string downloadLink = null, string modid = null)
        {
            Name = name;
            Version = version;
            Author = author;
            DownloadLink = downloadLink;
        }
    }

    public class VRCModLogger
    {
        public static void Log(string s) => MelonLoader.MelonModLogger.LogError(s);
        public static void Log(string s, params object[] args) => MelonLoader.MelonModLogger.LogError(s, args);
        public static void LogError(string s) => MelonLoader.MelonModLogger.LogError(s);
        public static void LogError(string s, params object[] args) => MelonLoader.MelonModLogger.LogError(s, args);
    }
}