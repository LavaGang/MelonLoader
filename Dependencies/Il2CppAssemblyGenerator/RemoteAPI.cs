using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Semver;

#pragma warning disable 0649

namespace MelonLoader.Il2CppAssemblyGenerator
{
    internal static class RemoteAPI
    {
        internal class InfoStruct
        {
            internal string ForceDumperVersion = null;
            internal string ForceUnhollowerVersion = null;
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
            string gamename = Regex.Replace(InternalUtils.UnityInformationHandler.GameName, "[^a-zA-Z0-9_.]+", "-", RegexOptions.Compiled).ToLowerInvariant();

            HostList = new List<HostInfo> {
                new HostInfo($"{DefaultHostInfo.Melon.API_URL}{gamename}", DefaultHostInfo.Melon.Contact),
                new HostInfo($"{DefaultHostInfo.Melon.API_URL_1}{gamename}", DefaultHostInfo.Melon.Contact),
                new HostInfo($"{DefaultHostInfo.Melon.API_URL_2}{gamename}", DefaultHostInfo.Melon.Contact),
                new HostInfo($"{DefaultHostInfo.Melon.API_URL_SAMBOY}{gamename}", DefaultHostInfo.Melon.Contact),
                new HostInfo($"{DefaultHostInfo.Ruby.API_URL}{gamename}.json", DefaultHostInfo.Ruby.Contact),
            };
        }

        internal static void Contact()
        {
            Core.Logger.Msg("Contacting RemoteAPI...");

            ContactHosts();

            Core.Logger.Msg($"RemoteAPI.DumperVersion = {(string.IsNullOrEmpty(Info.ForceDumperVersion) ? "null" : Info.ForceDumperVersion)}");
            Core.Logger.Msg($"RemoteAPI.UnhollowerVersion = {(string.IsNullOrEmpty(Info.ForceUnhollowerVersion) ? "null" : Info.ForceUnhollowerVersion)}");
            Core.Logger.Msg($"RemoteAPI.ObfuscationRegex = {(string.IsNullOrEmpty(Info.ObfuscationRegex) ? "null" : Info.ObfuscationRegex)}");
            Core.Logger.Msg($"RemoteAPI.MappingURL = {(string.IsNullOrEmpty(Info.MappingURL) ? "null" : Info.MappingURL)}");
            Core.Logger.Msg($"RemoteAPI.MappingFileSHA512 = {(string.IsNullOrEmpty(Info.MappingFileSHA512) ? "null" : Info.MappingFileSHA512)}");
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
                try { Response = Core.webClient.DownloadString(info.URL); }
                catch (Exception ex)
                {
                    if (!(ex is System.Net.WebException) || ((System.Net.WebException) ex).Response == null)
                    {
                        Core.Logger.Error($"Exception while Contacting RemoteAPI Host ({info.URL}): {ex}");
                        continue;
                    }

                    System.Net.WebException we = (System.Net.WebException)ex;
                    System.Net.HttpWebResponse response = (System.Net.HttpWebResponse)we.Response;
                    if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    {
                        Core.Logger.Msg($"Game Not Found on RemoteAPI Host ({info.URL})");
                        break;
                    }

                    Core.Logger.Error($"WebException ({Enum.GetName(typeof(System.Net.HttpStatusCode), response.StatusCode)}) while Contacting RemoteAPI Host ({info.URL}): {ex}");
                    continue;
                }

                bool is_response_null = string.IsNullOrEmpty(Response);
                MelonDebug.Msg($"Response = {(is_response_null ? "null" : Response) }");
                if (is_response_null)
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

                internal static InfoStruct Contact(string response_str)
                {
                    ResponseStruct responseobj = MelonUtils.ParseJSONStringtoStruct<ResponseStruct>(response_str);
                    if (responseobj == null)
                        return null;

                    InfoStruct returninfo = new InfoStruct();
                    returninfo.ForceDumperVersion = responseobj.forceCpp2IlVersion;
                    returninfo.ForceUnhollowerVersion = responseobj.forceUnhollowerVersion;
                    returninfo.ObfuscationRegex = responseobj.obfuscationRegex;
                    returninfo.MappingURL = responseobj.mappingUrl;
                    returninfo.MappingFileSHA512 = responseobj.mappingFileSHA512;
                    return returninfo;
                }

                internal class ResponseStruct
                {
                    public string gameSlug = null;
                    public string gameName = null;
                    public string mappingUrl = null;
                    public string mappingFileSHA512 = null;
                    public string forceCpp2IlVersion = null;
                    public string forceUnhollowerVersion = null;
                    public string obfuscationRegex = null;
                }
            }

            internal static class Ruby
            {
                internal static string API_URL = "https://ruby-core.com/api/ml/";

                internal static InfoStruct Contact(string response_str)
                {
                    ResponseStruct responseobj = MelonUtils.ParseJSONStringtoStruct<ResponseStruct>(response_str);
                    if (responseobj == null)
                        return null;

                    InfoStruct returninfo = new InfoStruct();
                    //returninfo.ForceDumperVersion = responseobj.forceDumperVersion;
                    returninfo.ForceUnhollowerVersion = responseobj.forceUnhollowerVersion;
                    returninfo.ObfuscationRegex = responseobj.obfuscationRegex;
                    returninfo.MappingURL = responseobj.mappingURL;
                    returninfo.MappingFileSHA512 = responseobj.mappingFileSHA512;
                    return returninfo;
                }

                private class ResponseStruct
                {
                    public string forceDumperVersion = null;
                    public string forceUnhollowerVersion = null;
                    public string obfuscationRegex = null;
                    public string mappingURL = null;
                    public string mappingFileSHA512 = null;
                }
            }
        }
    }
}
