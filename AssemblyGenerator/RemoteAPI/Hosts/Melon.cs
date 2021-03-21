using System;
using System.Text.RegularExpressions;
using MelonLoader.TinyJSON;

namespace MelonLoader.AssemblyGenerator.RemoteAPIHosts
{
    internal static class Melon
    {
        private static string API_URL = "https://api.melonloader.com/api/";
        private static string API_VERSION = "v1";

        internal static void Contact()
        {
            if (!RemoteAPI.ShouldMakeContact)
                return;

            string ContactURL = $"{API_URL}{API_VERSION}/game/{Regex.Replace(Core.GameName, "[^a-zA-Z0-9_.]+", "-", RegexOptions.Compiled).ToLowerInvariant()}";
            MelonDebug.Msg($"[Melon] ContactURL = {ContactURL}");

            string Response = null;
            try { Response = Core.webClient.DownloadString(ContactURL); }
            catch (Exception ex)
            {
                if (ex is System.Net.WebException)
                {
                    System.Net.WebException we = (System.Net.WebException)ex;
                    System.Net.HttpWebResponse response = (System.Net.HttpWebResponse)we.Response;
                    if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    {
                        MelonDebug.Msg($"Game Not Found on RemoteAPI Host [Melon]");
                        RemoteAPI.ShouldMakeContact = false;
                        return;
                    }
                }
                MelonLogger.Error($"Exception while Contacting RemoteAPI Host [Melon]: {ex}");
                return;
            }

            bool is_response_null = string.IsNullOrEmpty(Response);
            MelonDebug.Msg($"[Melon] Response = {(is_response_null ? "null" : Response) }");
            if (is_response_null)
                return;

            Variant responsearr = null;
            try { responsearr = JSON.Load(Response); }
            catch (Exception ex)
            {
                MelonLogger.Error($"Exception while Decoding RemoteAPI Host [Melon] Response to JSON Variant: {ex}");
                return;
            }

            ResponseStruct responseobj = null;
            try { responseobj = responsearr.Make<ResponseStruct>(); }
            catch (Exception ex)
            {
                MelonLogger.Error($"Exception while Converting JSON Variant to RemoteAPI Host [Melon] ResponseStruct: {ex}");
                return;
            }

            //RemoteAPI.LAST_RESPONSE.ForceDumperVersion = responseobj.forceCpp2IlVersion;
            RemoteAPI.LAST_RESPONSE.ForceUnhollowerVersion = responseobj.forceUnhollowerVersion;
            RemoteAPI.LAST_RESPONSE.ObfuscationRegex = responseobj.obfuscationRegex;
            RemoteAPI.LAST_RESPONSE.MappingURL = responseobj.mappingUrl;
            RemoteAPI.LAST_RESPONSE.MappingFileSHA512 = responseobj.mappingFileSHA512;

            RemoteAPI.ShouldMakeContact = false;
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
