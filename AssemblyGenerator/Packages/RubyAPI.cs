using System;
using System.Text.RegularExpressions;
using MelonLoader.TinyJSON;
#pragma warning disable 0649

namespace MelonLoader.AssemblyGenerator
{
    internal static class RubyAPI
    {
        private static string API_URL = "https://ruby-core.com/api/ml";
        internal static RubyAPI_Response LAST_RESPONSE = new RubyAPI_Response();

        internal static void Contact()
        {
            Logger.Msg("Contacting RubyAPI...");
            string ContactURL = $"{API_URL}/{Regex.Replace(Core.GameName, "[^a-zA-Z0-9_.]+", "-", RegexOptions.Compiled).ToLowerInvariant()}.json";
            //Logger.Debug_Msg($"ContactURL = {ContactURL}");
            Logger.Msg($"ContactURL = {ContactURL}");

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
                        //Logger.Debug_Msg($"Game Not Found on RubyAPI");
                        Logger.Msg($"Game Not Found on RubyAPI");
                        return;
                    }
                }
                Logger.Error($"Exception while Contacting RubyAPI: {ex}");
                return;
            }

            if (string.IsNullOrEmpty(Response))
                throw new ArgumentNullException("Response");

            //Logger.Debug_Msg($"Response = {Response}");
            Logger.Msg($"Response = {Response}");

            Variant responsearr = null;
            try { responsearr = JSON.Load(Response); }
            catch (Exception ex)
            {
                Logger.Error($"Exception while Decoding Response to JSON Variant: {ex}");
                return;
            }

            try { LAST_RESPONSE = responsearr.Make<RubyAPI_Response>(); }
            catch (Exception ex)
            {
                Logger.Error($"Exception while Converting JSON Variant to RubyAPI_Response: {ex}");
                return;
            }

            //Logger.Debug_Msg($"forceCpp2IlVersion = {(string.IsNullOrEmpty(LAST_RESPONSE.forceCpp2IlVersion) ? "null" : LAST_RESPONSE.forceCpp2IlVersion)}");
            //Logger.Debug_Msg($"forceUnhollowerVersion = {(string.IsNullOrEmpty(LAST_RESPONSE.forceUnhollowerVersion) ? "null" : LAST_RESPONSE.forceUnhollowerVersion)}");
            // Logger.Debug_Msg($"obfuscationRegex = {(string.IsNullOrEmpty(LAST_RESPONSE.obfuscationRegex) ? "null" : LAST_RESPONSE.obfuscationRegex)}");
            //Logger.Debug_Msg($"mappingUrl = {(string.IsNullOrEmpty(LAST_RESPONSE.mappingUrl) ? "null" : LAST_RESPONSE.mappingUrl)}");
            //Logger.Debug_Msg($"mappingFileSHA512 = {(string.IsNullOrEmpty(LAST_RESPONSE.mappingFileSHA512) ? "null" : LAST_RESPONSE.mappingFileSHA512)}");

            Logger.Msg($"obfuscationRegex = {(string.IsNullOrEmpty(LAST_RESPONSE.obfuscationRegex) ? "null" : LAST_RESPONSE.obfuscationRegex)}");
            Logger.Msg($"mappingURL = {(string.IsNullOrEmpty(LAST_RESPONSE.mappingURL) ? "null" : LAST_RESPONSE.mappingURL)}");
            Logger.Msg($"mappingFileSHA512 = {(string.IsNullOrEmpty(LAST_RESPONSE.mappingFileSHA512) ? "null" : LAST_RESPONSE.mappingFileSHA512)}");
        }
    }

    internal class RubyAPI_Response
    {
        public string forceDumperVersion = null;
        public string forceUnhollowerVersion = null;
        public string obfuscationRegex = null;
        public string mappingURL = null;
        public string mappingFileSHA512 = null;
    }
}
