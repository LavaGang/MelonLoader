using System;
using System.IO;
using MelonLoader.Tomlyn;
using MelonLoader.Tomlyn.Model;
using MelonLoader.Tomlyn.Syntax;

namespace MelonLoader
{
    internal static class Config
    {
        private static string FilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "MelonLoader.Installer.cfg");

        private static int _theme = 0;
        internal static int Theme { get { return _theme; } set { _theme = value; Save(); } }

        private static bool _autoupdateinstaller = true;
        internal static bool AutoUpdateInstaller { get { return _autoupdateinstaller; } set { _autoupdateinstaller = value; Save(); } }

        private static bool _closeaftercompletion = true;
        internal static bool CloseAfterCompletion { get { return _closeaftercompletion; } set { _closeaftercompletion = value; Save(); } }

        private static bool _showalphareleases = false;
        internal static bool ShowAlphaReleases { get { return _showalphareleases; } set { _showalphareleases = false; } } // set { _showalphareleases = value; Save(); } }

        internal static void Load()
        {
            if (!File.Exists(FilePath))
                return;
            string filestr = File.ReadAllText(FilePath);
            if (string.IsNullOrEmpty(filestr))
                return;
            DocumentSyntax doc = Toml.Parse(filestr);
            if ((doc == null) || doc.HasErrors)
                return;
            TomlTable tbl = doc.ToModel();
            if ((tbl.Count <= 0) || !tbl.ContainsKey("Installer"))
                return;
            TomlTable installertbl = (TomlTable)tbl["Installer"];
            if ((installertbl == null) || (installertbl.Count <= 0))
                return;
            if (installertbl.ContainsKey("Theme"))
                Int32.TryParse(installertbl["Theme"].ToString(), out _theme);
            if (installertbl.ContainsKey("AutoUpdateInstaller"))
                Boolean.TryParse(installertbl["AutoUpdateInstaller"].ToString(), out _autoupdateinstaller);
            if (installertbl.ContainsKey("CloseAfterCompletion"))
                Boolean.TryParse(installertbl["CloseAfterCompletion"].ToString(), out _closeaftercompletion);
            //if (installertbl.ContainsKey("ShowAlphaReleases"))
            //    Boolean.TryParse(installertbl["ShowAlphaReleases"].ToString(), out _showalphareleases);
        }   

        internal static void Save()
        {
            DocumentSyntax doc = new DocumentSyntax();
            TableSyntax tbl = new TableSyntax("Installer");
            tbl.Items.Add(new KeyValueSyntax("Theme", new IntegerValueSyntax(_theme)));
            tbl.Items.Add(new KeyValueSyntax("AutoUpdateInstaller", new BooleanValueSyntax(_autoupdateinstaller)));
            tbl.Items.Add(new KeyValueSyntax("CloseAfterCompletion", new BooleanValueSyntax(_closeaftercompletion)));
            //tbl.Items.Add(new KeyValueSyntax("ShowAlphaReleases", new BooleanValueSyntax(_showalphareleases)));
            doc.Tables.Add(tbl);
            File.WriteAllText(FilePath, doc.ToString());
        }
    }
}
