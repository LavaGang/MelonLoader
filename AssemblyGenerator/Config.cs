using System.IO;
using System.Collections.Generic;
using MelonLoader.Tomlyn;
using MelonLoader.Tomlyn.Model;
using MelonLoader.Tomlyn.Syntax;

namespace MelonLoader.AssemblyGenerator
{
    internal class Config
    {
        private static string FilePath = null;
        internal static string GameAssemblyHash = null;
        internal static string UnityVersion = null;
        internal static string Cpp2ILVersion = null;
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
            if (tbl.ContainsKey("Cpp2IL"))
                Cpp2ILVersion = (string)tbl["Cpp2IL"];
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
                            {"Cpp2IL", (Cpp2ILVersion == null) ? "" : Cpp2ILVersion },
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