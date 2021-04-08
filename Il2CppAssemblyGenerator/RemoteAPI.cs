using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
#pragma warning disable 0649

namespace MelonLoader.Il2CppAssemblyGenerator
{
    public static class RemoteAPI
    {
        public class InfoStruct
        {
            public string ForceDumperVersion = null;
            public string ForceUnhollowerVersion = null;
            public string ObfuscationRegex = null;
            public string MappingURL = null;
            public string MappingFileSHA512 = null;
        }
        internal static InfoStruct ReturnedInfo = new InfoStruct();
        private static Dictionary<string, Func<string, InfoStruct, bool>> DefaultHosts = new Dictionary<string, Func<string, InfoStruct, bool>>();
        public static Dictionary<string, Func<string, InfoStruct, bool>> CustomHosts = new Dictionary<string, Func<string, InfoStruct, bool>>();

        static RemoteAPI()
        {
            DefaultHosts[$"{DefaultHostInfo.Melon.API_URL}{DefaultHostInfo.Melon.API_VERSION}/game/{Regex.Replace(Core.GameName, "[^a-zA-Z0-9_.]+", "-", RegexOptions.Compiled).ToLowerInvariant()}"] =
                DefaultHostInfo.Melon.Contact;

            DefaultHosts[$"{DefaultHostInfo.Melon1.API_URL}{DefaultHostInfo.Melon1.API_VERSION}/game/{Regex.Replace(Core.GameName, "[^a-zA-Z0-9_.]+", "-", RegexOptions.Compiled).ToLowerInvariant()}"] =
                DefaultHostInfo.Melon1.Contact;

            DefaultHosts[$"{DefaultHostInfo.Melon2.API_URL}{DefaultHostInfo.Melon2.API_VERSION}/game/{Regex.Replace(Core.GameName, "[^a-zA-Z0-9_.]+", "-", RegexOptions.Compiled).ToLowerInvariant()}"] =
                DefaultHostInfo.Melon2.Contact;

            DefaultHosts[$"{DefaultHostInfo.Ruby.API_URL}{Regex.Replace(Core.GameName, "[^a-zA-Z0-9_.]+", "-", RegexOptions.Compiled).ToLowerInvariant()}.json"] =
                DefaultHostInfo.Ruby.Contact;

            DefaultHosts[$"{DefaultHostInfo.Samboy.API_URL}{DefaultHostInfo.Samboy.API_VERSION}/game/{Regex.Replace(Core.GameName, "[^a-zA-Z0-9_.]+", "-", RegexOptions.Compiled).ToLowerInvariant()}"] =
                DefaultHostInfo.Samboy.Contact;
        }

        internal static void Contact()
        {
            MelonLogger.Msg("Contacting RemoteAPI...");

            bool ContactReturnValue = ContactHosts(CustomHosts);
            if (!ContactReturnValue)
                ContactReturnValue = ContactHosts(DefaultHosts);

            MelonDebug.Msg($"ContactReturnValue = {ContactReturnValue}");

            MelonLogger.Msg($"ForceDumperVersion = {(string.IsNullOrEmpty(ReturnedInfo.ForceDumperVersion) ? "null" : ReturnedInfo.ForceDumperVersion)}");
            MelonLogger.Msg($"ForceUnhollowerVersion = {(string.IsNullOrEmpty(ReturnedInfo.ForceUnhollowerVersion) ? "null" : ReturnedInfo.ForceUnhollowerVersion)}");
            MelonLogger.Msg($"ObfuscationRegex = {(string.IsNullOrEmpty(ReturnedInfo.ObfuscationRegex) ? "null" : ReturnedInfo.ObfuscationRegex)}");
            MelonLogger.Msg($"MappingURL = {(string.IsNullOrEmpty(ReturnedInfo.MappingURL) ? "null" : ReturnedInfo.MappingURL)}");
            MelonLogger.Msg($"MappingFileSHA512 = {(string.IsNullOrEmpty(ReturnedInfo.MappingFileSHA512) ? "null" : ReturnedInfo.MappingFileSHA512)}");
        }

        private static bool ContactHosts(Dictionary<string, Func<string, InfoStruct, bool>> hostsdict)
        {
            if (hostsdict.Count <= 0)
                return false;

            foreach (KeyValuePair<string, Func<string, InfoStruct, bool>> pair in hostsdict)
            {
                if (pair.Value == null)
                    continue;
                MelonDebug.Msg($"ContactURL = {pair.Key}");

                string Response = null;
                try { Response = Core.webClient.DownloadString(pair.Key); }
                catch (Exception ex)
                {
                    if (!(ex is System.Net.WebException))
                    {
                        MelonLogger.Error($"Exception while Contacting RemoteAPI Host ({pair.Key}): {ex}");
                        continue;
                    }

                    System.Net.WebException we = (System.Net.WebException)ex;
                    System.Net.HttpWebResponse response = (System.Net.HttpWebResponse)we.Response;
                    if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    {
                        MelonDebug.Msg($"Game Not Found on RemoteAPI Host ({pair.Key})");
                        continue;
                    }

                    MelonLogger.Error($"WebException ({Enum.GetName(typeof(System.Net.HttpStatusCode), response.StatusCode)}) while Contacting RemoteAPI Host ({pair.Key}): {ex}");
                    continue;
                }

                bool is_response_null = string.IsNullOrEmpty(Response);
                MelonDebug.Msg($"Response = {(is_response_null ? "null" : Response) }");
                if (is_response_null)
                    continue;

                if (pair.Value(Response, ReturnedInfo))
                    return true;
            }
            return false;
        }

        private class DefaultHostInfo
        {
            internal static class Melon
            {
                internal static string API_URL = "https://api.melonloader.com/api/";
                internal static string API_VERSION = "v1";

                internal static bool Contact(string response_str, InfoStruct returninfo)
                {
                    ResponseStruct responseobj = MelonUtils.ParseJSONStringtoStruct<ResponseStruct>(response_str);
                    if (responseobj == null)
                        return false;

                    //returninfo.ForceDumperVersion = responseobj.forceCpp2IlVersion;
                    returninfo.ForceUnhollowerVersion = responseobj.forceUnhollowerVersion;
                    returninfo.ObfuscationRegex = responseobj.obfuscationRegex;
                    returninfo.MappingURL = responseobj.mappingUrl;
                    returninfo.MappingFileSHA512 = responseobj.mappingFileSHA512;

                    return true;
                }

                private class ResponseStruct
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

            internal static class Melon1
            {
                internal static string API_URL = "https://api-1.melonloader.com/api/";
                internal static string API_VERSION = "v1";

                internal static bool Contact(string response_str, InfoStruct returninfo)
                {
                    ResponseStruct responseobj = MelonUtils.ParseJSONStringtoStruct<ResponseStruct>(response_str);
                    if (responseobj == null)
                        return false;

                    //returninfo.ForceDumperVersion = responseobj.forceCpp2IlVersion;
                    returninfo.ForceUnhollowerVersion = responseobj.forceUnhollowerVersion;
                    returninfo.ObfuscationRegex = responseobj.obfuscationRegex;
                    returninfo.MappingURL = responseobj.mappingUrl;
                    returninfo.MappingFileSHA512 = responseobj.mappingFileSHA512;

                    return true;
                }

                private class ResponseStruct
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

            internal static class Melon2
            {
                internal static string API_URL = "https://api-2.melonloader.com/api/";
                internal static string API_VERSION = "v1";

                internal static bool Contact(string response_str, InfoStruct returninfo)
                {
                    ResponseStruct responseobj = MelonUtils.ParseJSONStringtoStruct<ResponseStruct>(response_str);
                    if (responseobj == null)
                        return false;

                    //returninfo.ForceDumperVersion = responseobj.forceCpp2IlVersion;
                    returninfo.ForceUnhollowerVersion = responseobj.forceUnhollowerVersion;
                    returninfo.ObfuscationRegex = responseobj.obfuscationRegex;
                    returninfo.MappingURL = responseobj.mappingUrl;
                    returninfo.MappingFileSHA512 = responseobj.mappingFileSHA512;

                    return true;
                }

                private class ResponseStruct
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

                internal static bool Contact(string response_str, InfoStruct returninfo)
                {
                    ResponseStruct responseobj = MelonUtils.ParseJSONStringtoStruct<ResponseStruct>(response_str);
                    if (responseobj == null)
                        return false;

                    //returninfo.ForceDumperVersion = responseobj.forceDumperVersion;
                    returninfo.ForceUnhollowerVersion = responseobj.forceUnhollowerVersion;
                    returninfo.ObfuscationRegex = responseobj.obfuscationRegex;
                    returninfo.MappingURL = responseobj.mappingURL;
                    returninfo.MappingFileSHA512 = responseobj.mappingFileSHA512;

                    return true;
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

            internal static class Samboy
            {
                internal static string API_URL = "https://melon.samboy.dev/api/";
                internal static string API_VERSION = "v1";

                internal static bool Contact(string response_str, InfoStruct returninfo)
                {
                    ResponseStruct responseobj = MelonUtils.ParseJSONStringtoStruct<ResponseStruct>(response_str);
                    if (responseobj == null)
                        return false;

                    //returninfo.ForceDumperVersion = responseobj.forceCpp2IlVersion;
                    returninfo.ForceUnhollowerVersion = responseobj.forceUnhollowerVersion;
                    returninfo.ObfuscationRegex = responseobj.obfuscationRegex;
                    returninfo.MappingURL = responseobj.mappingUrl;
                    returninfo.MappingFileSHA512 = responseobj.mappingFileSHA512;

                    return true;
                }

                private class ResponseStruct
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
        }
    }
}
