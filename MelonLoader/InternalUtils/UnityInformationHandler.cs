using System;
using System.Collections.Generic;
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
        public static UnityVersion EngineVersion { get; private set; } = new UnityVersion();
        public static string GameVersion { get; private set; } = "0";

        internal static void Setup()
        {
            AssetsManager assetsManager = new AssetsManager();
            ReadGameInfo(assetsManager);
            assetsManager.UnloadAll();

            SetDefaultConsoleTitleWithGameName(GameName, GameVersion);

            MelonLogger.Msg("------------------------------");
            MelonLogger.Msg($"Game Name: {GameName}");
            MelonLogger.Msg($"Game Developer: {GameDeveloper}");
            MelonLogger.Msg($"Unity Version: {EngineVersion}");
            MelonLogger.Msg($"Game Version: {GameVersion}");
            MelonLogger.Msg("------------------------------");
        }

        private static void ReadGameInfo(AssetsManager assetsManager)
        {
            string gameDataPath = MelonUtils.GetGameDataDirectory();
            AssetsFileInstance instance = null;
            try
            {
                string bundlePath = Path.Combine(gameDataPath, "globalgamemanagers");
                if (!File.Exists(bundlePath))
                    bundlePath = Path.Combine(gameDataPath, "mainData");

                if (!File.Exists(bundlePath))
                    return;

                instance = assetsManager.LoadAssetsFile(bundlePath, true);
                if (instance == null)
                    return;

                assetsManager.LoadIncludedClassPackage();
                assetsManager.LoadClassDatabaseFromPackage(instance.file.typeTree.unityVersion);
                EngineVersion = UnityVersion.Parse(instance.file.typeTree.unityVersion);

                List<AssetFileInfoEx> assetFiles = instance.table.GetAssetsOfType(129);
                if (assetFiles.Count > 0)
                {
                    AssetFileInfoEx playerSettings = assetFiles.First();

                    AssetTypeInstance assetTypeInstance = null;
                    try
                    {
                        assetTypeInstance = assetsManager.GetTypeInstance(instance, playerSettings);
                    }
                    catch (Exception ex)
                    {
                        MelonLogger.Error(ex);
                        MelonLogger.Warning("Attempting to use Large Class Package...");
                        assetsManager.LoadIncludedLargeClassPackage();
                        assetsManager.LoadClassDatabaseFromPackage(instance.file.typeTree.unityVersion);
                        assetTypeInstance = assetsManager.GetTypeInstance(instance, playerSettings);
                    }

                    if (assetTypeInstance != null)
                    {
                        AssetTypeValueField playerSettings_baseField = assetTypeInstance.GetBaseField();

                        AssetTypeValueField bundleVersion = playerSettings_baseField.Get("bundleVersion");
                        if (bundleVersion != null)
                            GameVersion = bundleVersion.GetValue().AsString();

                        AssetTypeValueField companyName = playerSettings_baseField.Get("companyName");
                        if (companyName != null)
                            GameDeveloper = companyName.GetValue().AsString();

                        AssetTypeValueField productName = playerSettings_baseField.Get("productName");
                        if (productName != null)
                            GameName = productName.GetValue().AsString();
                    }
                }
            }
            catch(Exception ex) { MelonLogger.Error(ex); }
            if (instance != null)
                instance.file.Close();
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        [return: MarshalAs(UnmanagedType.LPStr)]
        private extern static void SetDefaultConsoleTitleWithGameName([MarshalAs(UnmanagedType.LPStr)] string GameName, [MarshalAs(UnmanagedType.LPStr)] string GameVersion = null);
    }
}
