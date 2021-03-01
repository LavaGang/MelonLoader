using System;
using System.Text.RegularExpressions;
using MelonLoader.TinyJSON;
#pragma warning disable 0649

namespace MelonLoader.AssemblyGenerator
{
    internal static class SamboyAPI
    {
        private static string API_URL = "https://melon.samboy.dev/api";
        private static string API_VERSION = "v1";
        internal static SamboyAPI_Response LAST_RESPONSE = new SamboyAPI_Response();

        internal static void Setup()
        {
            string ContactURL = $"{API_URL}/{API_VERSION}/game/{Regex.Replace(Core.GameName, "[^a-zA-Z0-9_.]+", "-", RegexOptions.Compiled).ToLowerInvariant()}";
            Logger.Debug_Msg($"ContactURL = {ContactURL}");

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
                        Logger.Debug_Msg($"Game Not Found on API");
                        return;
                    }
                }
                Logger.Error($"Exception while Contacting API: {ex}");
                throw;
            }

            Logger.Debug_Msg($"Response = {(string.IsNullOrEmpty(Response) ? "null" : Response)}");

            Variant responsearr = null;
            try { responsearr = JSON.Load(Response); }
            catch (Exception ex)
            {
                Logger.Error($"Exception while Decoding Response to JSON Variant: {ex}");
                throw;
            }

            try { LAST_RESPONSE = responsearr.Make<SamboyAPI_Response>(); }
            catch (Exception ex)
            {
                Logger.Error($"Exception while Converting JSON Variant to SamboyAPI_Response: {ex}");
                throw;
            }

            Logger.Debug_Msg($"mappingUrl = {(string.IsNullOrEmpty(LAST_RESPONSE.mappingUrl) ? "null" : LAST_RESPONSE.mappingUrl)}");
            Logger.Debug_Msg($"mappingFileSHA512 = {(string.IsNullOrEmpty(LAST_RESPONSE.mappingFileSHA512) ? "null" : LAST_RESPONSE.mappingFileSHA512)}");
            Logger.Debug_Msg($"forceCpp2IlVersion = {(string.IsNullOrEmpty(LAST_RESPONSE.forceCpp2IlVersion) ? "null" : LAST_RESPONSE.forceCpp2IlVersion)}");
            Logger.Debug_Msg($"forceUnhollowerVersion = {(string.IsNullOrEmpty(LAST_RESPONSE.forceUnhollowerVersion) ? "null" : LAST_RESPONSE.forceUnhollowerVersion)}");
            Logger.Debug_Msg($"obfuscationRegex = {(string.IsNullOrEmpty(LAST_RESPONSE.obfuscationRegex) ? "null" : LAST_RESPONSE.obfuscationRegex)}");
        }
    }

    internal class SamboyAPI_Response
    {
        public string mappingUrl = null;
        public string mappingFileSHA512 = null;
        public string forceCpp2IlVersion = null;
        public string forceUnhollowerVersion = null;
        public string obfuscationRegex = null;
    }
}
