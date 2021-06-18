using System.IO;
using System.Collections.Generic;
using MelonLoader.Tomlyn;
using MelonLoader.Tomlyn.Model;
using MelonLoader.Tomlyn.Syntax;

namespace MelonLoader.Il2CppAssemblyGenerator
{
    internal class Config
    {
        private static string FilePath = null;
        internal static string GameAssemblyHash = null;
        internal static string DeobfuscationMapHash = null;
        internal static string ObfuscationRegex = null;
        internal static string UnityVersion = null;
        internal static string DumperVersion = null;
        internal static string Il2CppAssemblyUnhollowerVersion = null;
        internal static List<string> OldFiles = new List<string>();

        static Config()
        {
            FilePath = Path.Combine(Core.BasePath, "Config.cfg");
            if (!File.Exists(FilePath))
            {
                Save();
                return;
            }
            string filestr = File.ReadAllText(FilePath);
            if (string.IsNullOrEmpty(filestr))
                return;
            DocumentSyntax docsyn = Toml.Parse(filestr);
            if (docsyn == null)
                return;
            TomlTable model = docsyn.ToModel();
            if (model.Count <= 0)
                return;
            TomlTable tbl = (TomlTable)model["AssemblyGenerator"];
            if (tbl == null)
                return;
            if (tbl.ContainsKey("UnityVersion"))
                UnityVersion = (string)tbl["UnityVersion"];
            if (tbl.ContainsKey("DeobfuscationMapHash"))
                DeobfuscationMapHash = (string)tbl["DeobfuscationMapHash"];
            if (tbl.ContainsKey("ObfuscationRegex"))
                ObfuscationRegex = (string)tbl["ObfuscationRegex"];
            if (tbl.ContainsKey("Dumper"))
                DumperVersion = (string)tbl["Dumper"];
            if (tbl.ContainsKey("Il2CppAssemblyUnhollower"))
                Il2CppAssemblyUnhollowerVersion = (string)tbl["Il2CppAssemblyUnhollower"];
            if (tbl.ContainsKey("GameAssemblyHash"))
                GameAssemblyHash = (string)tbl["GameAssemblyHash"];
            if (!tbl.ContainsKey("OldFiles"))
                return;
            TomlArray oldfilesarr = (TomlArray)tbl["OldFiles"];
            if (oldfilesarr.Count <= 0)
                return;
            for (int i = 0; i < oldfilesarr.Count; i++)
            {
                string file = (string)oldfilesarr[i];
                if (!string.IsNullOrEmpty(file))
                    OldFiles.Add(file);
            }
        }

        internal static void Save()
        {
            DocumentSyntax doc = new DocumentSyntax()
            {
                Tables =
                {
                    new TableSyntax("AssemblyGenerator")
                    {
                        Items =
                        {
                            {"UnityVersion", (UnityVersion == null) ? "" : UnityVersion},
                            {"DeobfuscationMapHash", (DeobfuscationMapHash == null) ? "" : DeobfuscationMapHash},
                            {"ObfuscationRegex", (ObfuscationRegex == null) ? "" : ObfuscationRegex},
                            {"Dumper", (DumperVersion == null) ? "" : DumperVersion},
                            {"Il2CppAssemblyUnhollower", (Il2CppAssemblyUnhollowerVersion == null) ? "" : Il2CppAssemblyUnhollowerVersion},
                            {"GameAssemblyHash", (GameAssemblyHash == null) ? "" : GameAssemblyHash},
                            {"OldFiles", OldFiles.ToArray()}
                        }
                    }
                }
            };
            File.WriteAllText(FilePath, doc.ToString());
        }
    }
}