using System;
using System.Text.RegularExpressions;
using MelonLoader.TinyJSON;

namespace MelonLoader.AssemblyGenerator.RemoteAPIHosts
{
    internal static class Samboy
    {
        private static string API_URL = "https://melon.samboy.dev/api";
        private static string API_VERSION = "v1";
        internal static RemoteAPI.InfoStruct LAST_RESPONSE = null;

        internal static void Contact()
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
                        Logger.Debug_Msg($"Game Not Found on RemoteAPI Host [Samboy]");
                        return;
                    }
                }
                Logger.Error($"Exception while Contacting RemoteAPI Host [Samboy]: {ex}");
                throw;
            }

            if (string.IsNullOrEmpty(Response))
                throw new ArgumentNullException("Response");

            Logger.Debug_Msg($"Response = {Response}");

            Variant responsearr = null;
            try { responsearr = JSON.Load(Response); }
            catch (Exception ex)
            {
                Logger.Error($"Exception while Decoding Response to JSON Variant: {ex}");
                throw;
            }

            ResponseStruct responseobj = null;
            try { responseobj = responsearr.Make<ResponseStruct>(); }
            catch (Exception ex)
            {
                Logger.Error($"Exception while Converting JSON Variant to ResponseStruct: {ex}");
                throw;
            }

            LAST_RESPONSE = new RemoteAPI.InfoStruct();
            //LAST_RESPONSE.ForceDumperVersion = responseobj.forceCpp2IlVersion;
            LAST_RESPONSE.ForceUnhollowerVersion = responseobj.forceUnhollowerVersion;
            LAST_RESPONSE.ObfuscationRegex = responseobj.obfuscationRegex;
            LAST_RESPONSE.MappingURL = responseobj.mappingUrl;
            LAST_RESPONSE.MappingFileSHA512 = responseobj.mappingFileSHA512;
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
