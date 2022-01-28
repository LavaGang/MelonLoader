using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using AssetsTools.NET;
using AssetsTools.NET.Extra;

namespace MelonLoader.InternalUtils
{
    public static class UnityInformationHandler
    {
        public static string GameName { get; private set; } = "UNKNOWN";
        public static string GameDeveloper { get; private set; } = "UNKNOWN";
        public static UnityVersion EngineVersion { get; private set; }
        public static string GameVersion { get; private set; } = "0";

        internal static void Setup()
        {
            string gameDataPath = MelonUtils.GetGameDataDirectory();
            AssetsManager assetsManager = new AssetsManager();

            ReadGameManager(gameDataPath, assetsManager);
            SetDefaultConsoleTitleWithGameName(GameName, GameVersion);

            MelonLogger.Msg($"Game Name: {GameName}");
            MelonLogger.Msg($"Game Developer: {GameDeveloper}");
            MelonLogger.Msg($"Unity Version: {EngineVersion}");
            MelonLogger.Msg($"Game Version: {GameVersion}");

            assetsManager.UnloadAll();
        }

        private static void ReadGameManager(string gameDataPath, AssetsManager assetsManager)
        {
            string bundlePath = Path.Combine(gameDataPath, "globalgamemanagers");
            if (!File.Exists(bundlePath))
                bundlePath = Path.Combine(gameDataPath, "mainData");
            if (!File.Exists(bundlePath))
                return;
            AssetsFileInstance instance = assetsManager.LoadAssetsFile(bundlePath, true);
            if (instance == null)
                return;

            assetsManager.LoadClassPackageFromIncludedMemory();
            assetsManager.LoadClassDatabaseFromPackage(instance.file.typeTree.unityVersion);

            EngineVersion = UnityVersion.Parse(instance.file.typeTree.unityVersion);

            AssetFileInfoEx buildSettings = instance.table.GetAssetsOfType(129).First();
            AssetTypeValueField buildSettings_baseField = assetsManager.GetTypeInstance(instance, buildSettings).GetBaseField();
            GameVersion = buildSettings_baseField.Get("bundleVersion").GetValue().AsString();

            AssetFileInfoEx playerSettings = instance.table.GetAssetsOfType(129).First();
            AssetTypeValueField playerSettings_baseField = assetsManager.GetTypeInstance(instance, playerSettings).GetBaseField();
            GameDeveloper = playerSettings_baseField.Get("companyName").GetValue().AsString();
            GameName = playerSettings_baseField.Get("productName").GetValue().AsString();

            instance.file.Close();
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        [return: MarshalAs(UnmanagedType.LPStr)]
        private extern static void SetDefaultConsoleTitleWithGameName([MarshalAs(UnmanagedType.LPStr)] string GameName, [MarshalAs(UnmanagedType.LPStr)] string GameVersion = null);
    }
}
