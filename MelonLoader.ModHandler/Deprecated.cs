using System;

namespace MelonLoader
{
    [Obsolete("MelonModGame is obsolete. Please use MelonGame instead.")]
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
    public class MelonModGameAttribute : Attribute
    {
        public string Developer { get; }
        public string GameName { get; }
        public MelonModGameAttribute(string developer = null, string gameName = null)
        {
            Developer = developer;
            GameName = gameName;
        }
        internal MelonGameAttribute Convert() => new MelonGameAttribute(Developer, GameName);
    }
    [Obsolete("MelonModInfo is obsolete. Please use MelonInfo instead.")]
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false)]
    public class MelonModInfoAttribute : Attribute
    {
        public Type SystemType { get; }
        public string Name { get; }
        public string Version { get; }
        public string Author { get; }
        public string DownloadLink { get; }

        public MelonModInfoAttribute(Type type, string name, string version, string author, string downloadLink = null)
        {
            SystemType = type;
            Name = name;
            Version = version;
            Author = author;
            DownloadLink = downloadLink;
        }
        internal MelonInfoAttribute Convert() => new MelonInfoAttribute(SystemType, Name, Version, Author, DownloadLink);
    }
    [Obsolete("MelonPluginGame is obsolete. Please use MelonGame instead.")]
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
    public class MelonPluginGameAttribute : Attribute
    {
        public string Developer { get; }
        public string GameName { get; }
        public MelonPluginGameAttribute(string developer = null, string gameName = null)
        {
            Developer = developer;
            GameName = gameName;
        }
        public MelonGameAttribute Convert() => new MelonGameAttribute(Developer, GameName);
    }
    [Obsolete("MelonPluginInfo is obsolete. Please use MelonInfo instead.")]
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false)]
    public class MelonPluginInfoAttribute : Attribute
    {
        public Type SystemType { get; }
        public string Name { get; }
        public string Version { get; }
        public string Author { get; }
        public string DownloadLink { get; }

        public MelonPluginInfoAttribute(Type type, string name, string version, string author, string downloadLink = null)
        {
            SystemType = type;
            Name = name;
            Version = version;
            Author = author;
            DownloadLink = downloadLink;
        }
        public MelonInfoAttribute Convert() => new MelonInfoAttribute(SystemType, Name, Version, Author, DownloadLink);
    }
    [Obsolete("MelonModLogger is obsolete. Please use MelonLogger instead.")]
    public class MelonModLogger
    {
        public static void Log(string s) => MelonLogger.Native_Log((MelonLogger.GetNameSection() + s));
        public static void Log(ConsoleColor color, string s) => MelonLogger.Native_LogColor((MelonLogger.GetNameSection() + s), color);
        public static void Log(string s, params object[] args) => MelonLogger.Native_Log((MelonLogger.GetNameSection() + string.Format(s, args)));
        public static void Log(ConsoleColor color, string s, params object[] args) => MelonLogger.Native_LogColor((MelonLogger.GetNameSection() + string.Format(s, args)), color);
        public static void LogWarning(string s) => MelonLogger.Native_LogWarning(MelonLogger.GetNameSection(), s);
        public static void LogWarning(string s, params object[] args) => MelonLogger.Native_LogWarning(MelonLogger.GetNameSection(), string.Format(s, args));
        public static void LogError(string s) => MelonLogger.Native_LogError(MelonLogger.GetNameSection(), s);
        public static void LogError(string s, params object[] args) => MelonLogger.Native_LogError(MelonLogger.GetNameSection(), string.Format(s, args));
    }
 }