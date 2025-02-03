using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using MelonLoader.Properties;
using Semver;

namespace MelonLoader.Engine.Unity
{
    internal static class RemoteAPI
    {
        internal class InfoStruct
        {
            internal string ForceDumperVersion = null;
            internal string ObfuscationRegex = null;
            internal string MappingURL = null;
            internal string MappingFileSHA512 = null;
        }
        internal static InfoStruct Info = new InfoStruct();

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
        private static List<HostInfo> HostList = null;

        static RemoteAPI()
        {
            string gamename = Regex.Replace(UnityInformationHandler.GameName, "[^a-zA-Z0-9_.]+", "-", RegexOptions.Compiled).ToLowerInvariant();

            HostList = new List<HostInfo> {
                new HostInfo($"{DefaultHostInfo.Melon.API_URL}{gamename}", DefaultHostInfo.Melon.Contact),
                new HostInfo($"{DefaultHostInfo.Melon.API_URL_1}{gamename}", DefaultHostInfo.Melon.Contact),
                new HostInfo($"{DefaultHostInfo.Melon.API_URL_2}{gamename}", DefaultHostInfo.Melon.Contact),
                new HostInfo($"{DefaultHostInfo.Melon.API_URL_SAMBOY}{gamename}", DefaultHostInfo.Melon.Contact),
                new HostInfo($"{DefaultHostInfo.Melon.API_URL_DUBYADUDE}{gamename}", DefaultHostInfo.Melon.Contact),
            };
        }

        internal static void Contact()
        {
            AssemblyGenerator.Logger.Msg("Contacting RemoteAPI...");

            ContactHosts();

            AssemblyGenerator.Logger.Msg($"RemoteAPI.DumperVersion = {(string.IsNullOrEmpty(Info.ForceDumperVersion) ? "null" : Info.ForceDumperVersion)}");
            AssemblyGenerator.Logger.Msg($"RemoteAPI.ObfuscationRegex = {(string.IsNullOrEmpty(Info.ObfuscationRegex) ? "null" : Info.ObfuscationRegex)}");
            AssemblyGenerator.Logger.Msg($"RemoteAPI.MappingURL = {(string.IsNullOrEmpty(Info.MappingURL) ? "null" : Info.MappingURL)}");
            AssemblyGenerator.Logger.Msg($"RemoteAPI.MappingFileSHA512 = {(string.IsNullOrEmpty(Info.MappingFileSHA512) ? "null" : Info.MappingFileSHA512)}");
        }

        private static void ContactHosts()
        {
            if ((HostList == null) || (HostList.Count <= 0))
                return;
            foreach (HostInfo info in HostList)
            {
                if (string.IsNullOrEmpty(info.URL) || (info.Func == null))
                    continue;

                MelonDebug.Msg($"ContactURL = {info.URL}");

                string Response = null;
                try
                {
                    var result = AssemblyGenerator.webClient.GetAsync(info.URL).Result;
                    result.EnsureSuccessStatusCode();
                    Response = result.Content.ReadAsStringAsync().Result;
                }
                catch (Exception ex)
                {
                    if (ex is not HttpRequestException {StatusCode: {}} hre)
                    {
                        AssemblyGenerator.Logger.Error($"Exception while Contacting RemoteAPI Host ({info.URL}): {ex}");
                        continue;
                    }

                    if (hre.StatusCode == System.Net.HttpStatusCode.NotFound)
                    {
                        AssemblyGenerator.Logger.Msg($"Game Not Found on RemoteAPI Host ({info.URL})");
                        break;
                    }

                    AssemblyGenerator.Logger.Error($"WebException ({hre.StatusCode}) while Contacting RemoteAPI Host ({info.URL}): {ex}");
                    continue;
                }

                var isResponseNull = string.IsNullOrEmpty(Response);
                MelonDebug.Msg($"Response = {(isResponseNull ? "null" : Response) }");
                if (isResponseNull)
                    break;

                InfoStruct returnInfo = info.Func(Response);
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
                    ResponseStruct responseobj = JsonSerializer.Deserialize<ResponseStruct>(response_str);
                    if (responseobj == null)
                        return null;

                    InfoStruct returninfo = new InfoStruct();
                    returninfo.ForceDumperVersion = responseobj.ForceCpp2IlVersion;
                    returninfo.ObfuscationRegex = responseobj.ObfuscationRegex;
                    returninfo.MappingURL = responseobj.MappingUrl;
                    returninfo.MappingFileSHA512 = responseobj.MappingFileSHA512;
                    return returninfo;
                }

                internal class ResponseStruct
                {
                    [JsonPropertyName("gameSlug")]
                    public string GameSlug { get; set; }

                    [JsonPropertyName("gameName")]
                    public string GameName { get; set; }

                    [JsonPropertyName("mappingUrl")]
                    public string MappingUrl { get; set; }

                    [JsonPropertyName("mappingFileSHA512")]
                    public string MappingFileSHA512 { get; set; }

                    [JsonPropertyName("forceCpp2IlVersion")]
                    public string ForceCpp2IlVersion { get; set; }

                    [JsonPropertyName("forceUnhollowerVersion")]
                    public string ForceUnhollowerVersion { get; set; } //TODO: Remove this from the API

                    [JsonPropertyName("obfuscationRegex")]
                    public string ObfuscationRegex { get; set; }
                }
            }
        }
    }
}
