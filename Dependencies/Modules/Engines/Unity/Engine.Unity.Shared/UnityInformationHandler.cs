﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using AssetsTools.NET;
using AssetsTools.NET.Extra;
using MelonLoader.Utils;
using UnityVersion = AssetRipper.Primitives.UnityVersion;

namespace MelonLoader.Engine.Unity
{
    public static class UnityInformationHandler
    {
        private const string DefaultInfo = "UNKNOWN";

        public static string GameName { get; private set; }
        public static string GameDeveloper { get; private set; }
        public static UnityVersion EngineVersion { get; private set; }
        public static string GameVersion { get; private set; }

        private static UnityVersion TryParse(string version)
        {
            UnityVersion returnval = UnityVersion.MinVersion;
            try 
            {
                returnval = UnityVersion.Parse(version); 
            }
            catch (Exception ex)
            {
                if (MelonDebug.IsEnabled())
                    MelonLogger.Error(ex);
                returnval = UnityVersion.MinVersion;
            }
            return returnval;
        }

        internal static void Setup(string gameDataPath, string unityPlayerPath)
        {
            //if (!string.IsNullOrEmpty(LoaderConfig.Current.UnityEngine.VersionOverride))
            //    EngineVersion = TryParse(LoaderConfig.Current.UnityEngine.VersionOverride);

            AssetsManager assetsManager = new AssetsManager();
            ReadGameInfo(assetsManager, gameDataPath);
            assetsManager.UnloadAll();

            if (string.IsNullOrEmpty(GameDeveloper)
                || string.IsNullOrEmpty(GameName))
                ReadGameInfoFallback(gameDataPath);

            if (EngineVersion == UnityVersion.MinVersion)
                EngineVersion = ReadVersionFallback(gameDataPath, unityPlayerPath);

            if (string.IsNullOrEmpty(GameDeveloper))
                GameDeveloper = DefaultInfo;
            if (string.IsNullOrEmpty(GameName))
                GameName = DefaultInfo;
            if (string.IsNullOrEmpty(GameVersion))
                GameVersion = DefaultInfo;
        }

        private static void ReadGameInfo(AssetsManager assetsManager, string gameDataPath)
        {
            AssetsFileInstance instance = null;
            try
            {
                string bundlePath = Path.Combine(gameDataPath, "globalgamemanagers");
                if (!File.Exists(bundlePath))
                    bundlePath = Path.Combine(gameDataPath, "mainData");

                if (!File.Exists(bundlePath))
                {
                    bundlePath = Path.Combine(gameDataPath, "data.unity3d");
                    if (!File.Exists(bundlePath))
                        return;

                    BundleFileInstance bundleFile = assetsManager.LoadBundleFile(bundlePath);
                    instance = assetsManager.LoadAssetsFileFromBundle(bundleFile, "globalgamemanagers");
                }
                else
                    instance = assetsManager.LoadAssetsFile(bundlePath, true);
                if (instance == null)
                    return;

                assetsManager.LoadIncludedClassPackage();
                if (!instance.file.Metadata.TypeTreeEnabled)
                    assetsManager.LoadClassDatabaseFromPackage(instance.file.Metadata.UnityVersion);

                if (EngineVersion == UnityVersion.MinVersion)
                    EngineVersion = TryParse(instance.file.Metadata.UnityVersion);

                List<AssetFileInfo> assetFiles = instance.file.GetAssetsOfType(AssetClassID.PlayerSettings);
                if (assetFiles.Count > 0)
                {
                    AssetFileInfo playerSettings = assetFiles.First();

                    AssetTypeValueField playerSettings_baseField = assetsManager.GetBaseField(instance, playerSettings);
                    if (playerSettings_baseField != null)
                    {
                        AssetTypeValueField bundleVersion = playerSettings_baseField.Get("bundleVersion");
                        if (bundleVersion != null)
                            GameVersion = bundleVersion.AsString;

                        AssetTypeValueField companyName = playerSettings_baseField.Get("companyName");
                        if (companyName != null)
                            GameDeveloper = companyName.AsString;

                        AssetTypeValueField productName = playerSettings_baseField.Get("productName");
                        if (productName != null)
                            GameName = productName.AsString;
                    }
                }
            }
            catch(Exception ex)
            {
                if (MelonDebug.IsEnabled())
                    MelonLogger.Error(ex);
            }
            if (instance != null)
                instance.file.Close();
        }

        private static void ReadGameInfoFallback(string gameDataPath)
        {
            try
            {
                string appInfoFilePath = Path.Combine(gameDataPath, "app.info");
                if (!File.Exists(appInfoFilePath))
                    return;

                string[] filestr = File.ReadAllLines(appInfoFilePath);
                if ((filestr == null) || (filestr.Length < 2))
                    return;

                if (string.IsNullOrEmpty(GameDeveloper) && !string.IsNullOrEmpty(filestr[0]))
                    GameDeveloper = filestr[0];

                if (string.IsNullOrEmpty(GameName) && !string.IsNullOrEmpty(filestr[1]))
                    GameName = filestr[1];

            }
            catch (Exception ex)
            {
                if (MelonDebug.IsEnabled())
                    MelonLogger.Error(ex);
            }
        }

        private static UnityVersion ReadVersionFallback(string gameDataPath, string unityPlayerPath)
        {
            if (!File.Exists(unityPlayerPath))
                unityPlayerPath = MelonEnvironment.ApplicationExecutablePath;

            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                var unityVer = FileVersionInfo.GetVersionInfo(unityPlayerPath);
                return TryParse(unityVer.FileVersion);
            }

            try
            {
                var globalgamemanagersPath = Path.Combine(gameDataPath, "globalgamemanagers");
                if (File.Exists(globalgamemanagersPath))
                    return GetVersionFromGlobalGameManagers(File.ReadAllBytes(globalgamemanagersPath));
            }
            catch (Exception ex)
            {
                if (MelonDebug.IsEnabled())
                    MelonLogger.Error(ex);
            }

            try
            {
                var dataPath = Path.Combine(gameDataPath, "data.unity3d");
                if (File.Exists(dataPath))
                    return GetVersionFromDataUnity3D(File.OpenRead(dataPath));
            }
            catch (Exception ex)
            {
                if (MelonDebug.IsEnabled())
                    MelonLogger.Error(ex);
            }

            return UnityVersion.MinVersion;
        }

        private static UnityVersion GetVersionFromGlobalGameManagers(byte[] ggmBytes)
        {
            var verString = new StringBuilder();
            var idx = 0x14;
            while (ggmBytes[idx] != 0)
            {
                verString.Append(Convert.ToChar(ggmBytes[idx]));
                idx++;
            }

            Regex UnityVersionRegex = new Regex(@"^[0-9]+\.[0-9]+\.[0-9]+[abcfx][0-9]+$", RegexOptions.Compiled);
            string unityVer = verString.ToString();
            if (!UnityVersionRegex.IsMatch(unityVer))
            {
                idx = 0x30;
                verString = new StringBuilder();
                while (ggmBytes[idx] != 0)
                {
                    verString.Append(Convert.ToChar(ggmBytes[idx]));
                    idx++;
                }

                unityVer = verString.ToString().Trim();
            }

            return TryParse(unityVer);
        }

        private static UnityVersion GetVersionFromDataUnity3D(Stream fileStream)
        {
            var verString = new StringBuilder();

            if (fileStream.CanSeek)
                fileStream.Seek(0x12, SeekOrigin.Begin);
            else
            {
                if (fileStream.Read(new byte[0x12], 0, 0x12) != 0x12)
                    throw new("Failed to seek to 0x12 in data.unity3d");
            }

            while (true)
            {
                var read = fileStream.ReadByte();
                if (read == 0)
                    break;
                verString.Append(Convert.ToChar(read));
            }

            return TryParse(verString.ToString().Trim());
        }
    }
}
