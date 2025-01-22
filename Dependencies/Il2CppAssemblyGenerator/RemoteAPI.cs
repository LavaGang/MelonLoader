using Semver;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.RegularExpressions;

#pragma warning disable 0649

namespace MelonLoader.Il2CppAssemblyGenerator;

internal static class RemoteAPI
{
    internal class InfoStruct
    {
        internal string ForceDumperVersion = null;
        internal string ObfuscationRegex = null;
        internal string MappingURL = null;
        internal string MappingFileSHA512 = null;
    }
    internal static InfoStruct Info = new();

    private class HostInfo
    {
        internal string URL = null;
        internal LemonFunc<string, InfoStruct> Func = null;
        internal HostInfo(string url, LemonFunc<string, InfoStruct> func)
        {
            URL = url;
            Func = func;
        }
    }
    private static readonly List<HostInfo> HostList = null;

    static RemoteAPI()
    {
        var gamename = Regex.Replace(InternalUtils.UnityInformationHandler.GameName, "[^a-zA-Z0-9_.]+", "-", RegexOptions.Compiled).ToLowerInvariant();

        HostList = [
            new HostInfo($"{DefaultHostInfo.Melon.API_URL}{gamename}", DefaultHostInfo.Melon.Contact),
            new HostInfo($"{DefaultHostInfo.Melon.API_URL_1}{gamename}", DefaultHostInfo.Melon.Contact),
            new HostInfo($"{DefaultHostInfo.Melon.API_URL_2}{gamename}", DefaultHostInfo.Melon.Contact),
            new HostInfo($"{DefaultHostInfo.Melon.API_URL_SAMBOY}{gamename}", DefaultHostInfo.Melon.Contact),
            new HostInfo($"{DefaultHostInfo.Melon.API_URL_DUBYADUDE}{gamename}", DefaultHostInfo.Melon.Contact),
        ];
    }

    internal static void Contact()
    {
        Core.Logger.Msg("Contacting RemoteAPI...");

        ContactHosts();

        Core.Logger.Msg($"RemoteAPI.DumperVersion = {(string.IsNullOrEmpty(Info.ForceDumperVersion) ? "null" : Info.ForceDumperVersion)}");
        Core.Logger.Msg($"RemoteAPI.ObfuscationRegex = {(string.IsNullOrEmpty(Info.ObfuscationRegex) ? "null" : Info.ObfuscationRegex)}");
        Core.Logger.Msg($"RemoteAPI.MappingURL = {(string.IsNullOrEmpty(Info.MappingURL) ? "null" : Info.MappingURL)}");
        Core.Logger.Msg($"RemoteAPI.MappingFileSHA512 = {(string.IsNullOrEmpty(Info.MappingFileSHA512) ? "null" : Info.MappingFileSHA512)}");
    }

    private static void ContactHosts()
    {
        if ((HostList == null) || (HostList.Count <= 0))
            return;
        foreach (var info in HostList)
        {
            if (string.IsNullOrEmpty(info.URL) || (info.Func == null))
                continue;

            MelonDebug.Msg($"ContactURL = {info.URL}");

            string Response;
            try
            {
                var result = Core.webClient.GetAsync(info.URL).Result;
                result.EnsureSuccessStatusCode();
                Response = result.Content.ReadAsStringAsync().Result;
            }
            catch (Exception ex)
            {
                if (ex is not HttpRequestException { StatusCode: { } } hre)
                {
                    Core.Logger.Error($"Exception while Contacting RemoteAPI Host ({info.URL}): {ex}");
                    continue;
                }

                if (hre.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    Core.Logger.Msg($"Game Not Found on RemoteAPI Host ({info.URL})");
                    break;
                }

                Core.Logger.Error($"WebException ({hre.StatusCode}) while Contacting RemoteAPI Host ({info.URL}): {ex}");
                continue;
            }

            var isResponseNull = string.IsNullOrEmpty(Response);
            MelonDebug.Msg($"Response = {(isResponseNull ? "null" : Response)}");
            if (isResponseNull)
                break;

            var returnInfo = info.Func(Response);
            if (returnInfo == null)
                continue;

            if (returnInfo.ForceDumperVersion != null && SemVersion.Parse(returnInfo.ForceDumperVersion) <= SemVersion.Parse("2022.0.2"))
                returnInfo.ForceDumperVersion = null;

            Info = returnInfo;
            break;
        }
    }

    private class DefaultHostInfo
    {
        internal static class Melon
        {
            internal static string API_VERSION = "v1";
            internal static string API_URL = $"https://api.melonloader.com/api/{API_VERSION}/game/";
            internal static string API_URL_1 = $"https://api-1.melonloader.com/api/{API_VERSION}/game/";
            internal static string API_URL_2 = $"https://api-2.melonloader.com/api/{API_VERSION}/game/";
            internal static string API_URL_SAMBOY = $"https://melon.samboy.dev/api/{API_VERSION}/game/";
            internal static string API_URL_DUBYADUDE = $"https://melon.dubyadu.de/api/{API_VERSION}/game/";

            internal static InfoStruct Contact(string response_str)
            {
                var responseobj = MelonUtils.ParseJSONStringtoStruct<ResponseStruct>(response_str);
                if (responseobj == null)
                    return null;

                var returninfo = new InfoStruct
                {
                    ForceDumperVersion = responseobj.forceCpp2IlVersion,
                    ObfuscationRegex = responseobj.obfuscationRegex,
                    MappingURL = responseobj.mappingUrl,
                    MappingFileSHA512 = responseobj.mappingFileSHA512
                };
                return returninfo;
            }

            internal class ResponseStruct
            {
                public string gameSlug = null;
                public string gameName = null;
                public string mappingUrl = null;
                public string mappingFileSHA512 = null;
                public string forceCpp2IlVersion = null;
                public string forceUnhollowerVersion = null; //TODO: Remove this from the API
                public string obfuscationRegex = null;
            }
        }
    }
}
