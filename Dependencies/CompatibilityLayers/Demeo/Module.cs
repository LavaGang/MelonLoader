using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using MelonLoader.Modules;

[assembly: MelonLoader.PatchShield]

namespace MelonLoader.CompatibilityLayers
{
    internal class Demeo_Module : MelonModule
    {
        private Dictionary<MelonBase, object> ModInformation = new Dictionary<MelonBase, object>();
        private IList ModInfoList;

        private Type modInfoType;
        private FieldInfo name_field;
        private FieldInfo version_field;
        private FieldInfo author_field;
        private FieldInfo description_field;
        private FieldInfo isNetworkCompatible_field;

        public override void OnInitialize()
        {
            MelonEvents.OnApplicationStart.Subscribe(OnPreAppStart, int.MaxValue);
            MelonBase.OnMelonRegistered.Subscribe(ParseMelon, int.MaxValue);
            MelonBase.OnMelonUnregistered.Subscribe(OnUnregister, int.MaxValue);
        }

        private void OnPreAppStart()
        {
            try
            {
                Assembly assembly = Assembly.Load("Assembly-CSharp");
                Type moddingApi = assembly.GetType("Boardgame.Modding.ModdingAPI");

                modInfoType = moddingApi.GetNestedType("ModInformation");
                name_field = modInfoType.GetField("name", BindingFlags.Public | BindingFlags.Instance);
                version_field = modInfoType.GetField("version", BindingFlags.Public | BindingFlags.Instance);
                author_field = modInfoType.GetField("author", BindingFlags.Public | BindingFlags.Instance);
                description_field = modInfoType.GetField("description", BindingFlags.Public | BindingFlags.Instance);
                isNetworkCompatible_field = modInfoType.GetField("isNetworkCompatible", BindingFlags.Public | BindingFlags.Instance);

                FieldInfo externalModsField = moddingApi.GetField("ExternallyInstalledMods", BindingFlags.Public | BindingFlags.Static);
                if (externalModsField.GetValue(null) == null)
                {
                    var listType = typeof(List<>);
                    var constructedListType = listType.MakeGenericType(modInfoType);
                    ModInfoList = (IList)Activator.CreateInstance(constructedListType);
                    externalModsField.SetValue(null, ModInfoList);
                }

                foreach (var m in MelonPlugin.RegisteredMelons)
                {
                    try
                    {
                        ParseMelon(m);
                    }
                    catch (Exception ex)
                    {
                        MelonLogger.Error($"Demeo Integration has thrown an exception: {ex}");
                    }
                }

                foreach (var m in MelonMod.RegisteredMelons)
                {
                    try
                    {
                        ParseMelon(m);
                    }
                    catch (Exception ex)
                    {
                        MelonLogger.Error($"Demeo Integration has thrown an exception: {ex}");
                    }
                }
            }
            catch (Exception ex)
            {
                MelonLogger.Error($"Demeo Integration has thrown an exception: {ex}");
            }
        }

        private void OnUnregister(MelonBase melon)
        {
            if (melon == null)
                return;

            if (!ModInformation.ContainsKey(melon))
                return;

            try
            {
                object info = ModInformation[melon];
                ModInformation.Remove(melon);
                ModInfoList.Remove(info);
            }
            catch (Exception ex)
            {
                MelonLogger.Error($"Demeo Integration has thrown an exception: {ex}");
            }
        }

        private void ParseMelon<T>(T melon) where T : MelonBase
        {

            if (melon == null)
                return;

            if (ModInformation.ContainsKey(melon))
                return;

            try
            {
                object info = Activator.CreateInstance(modInfoType);

                name_field.SetValue(info, melon.Info.Name);
                version_field.SetValue(info, melon.Info.Version);
                author_field.SetValue(info, melon.Info.Author);
                description_field.SetValue(info, melon.Info.DownloadLink);
                isNetworkCompatible_field.SetValue(info, MelonUtils.PullAttributeFromAssembly<Demeo_LobbyRequirement>(melon.MelonAssembly.Assembly) == null);

                ModInformation.Add(melon, info);
                ModInfoList.Add(info);
            }
            catch (Exception ex)
            {
                MelonLogger.Error($"Demeo Integration has thrown an exception: {ex}");
            }
        }
    }
}