using System;
using System.Text.RegularExpressions;
using MelonLoader.TinyJSON;

namespace MelonLoader.Il2CppAssemblyGenerator.RemoteAPIHosts
{
    internal static class Ruby
    {
        private static string API_URL = "https://ruby-core.com/api/ml/";
        internal static RemoteAPI.InfoStruct LAST_RESPONSE = null;

        internal static void Contact()
        {
            if (!RemoteAPI.ShouldMakeContact)
                return;

            string ContactURL = $"{API_URL}{Regex.Replace(Core.GameName, "[^a-zA-Z0-9_.]+", "-", RegexOptions.Compiled).ToLowerInvariant()}.json";
            MelonDebug.Msg($"ContactURL = {ContactURL}");

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
                        MelonDebug.Msg($"Game Not Found on RemoteAPI Host [Ruby]");
                        RemoteAPI.ShouldMakeContact = false;
                        return;
                    }
                }
                MelonLogger.Error($"Exception while Contacting RemoteAPI Host [Ruby]: {ex}");
                return;
            }

            bool is_response_null = string.IsNullOrEmpty(Response);
            MelonDebug.Msg($"[Ruby] Response = {(is_response_null ? "null" : Response) }");
            if (is_response_null)
                return;

            Variant responsearr = null;
            try { responsearr = JSON.Load(Response); }
            catch (Exception ex)
            {
                MelonLogger.Error($"Exception while Decoding RemoteAPI Host [Ruby] Response to JSON Variant: {ex}");
                return;
            }

            ResponseStruct responseobj = null;
            try { responseobj = responsearr.Make<ResponseStruct>(); }
            catch (Exception ex)
            {
                MelonLogger.Error($"Exception while Converting JSON Variant to RemoteAPI Host [Ruby] ResponseStruct: {ex}");
                return;
            }

            //RemoteAPI.LAST_RESPONSE.ForceDumperVersion = responseobj.forceDumperVersion;
            RemoteAPI.LAST_RESPONSE.ForceUnhollowerVersion = responseobj.forceUnhollowerVersion;
            RemoteAPI.LAST_RESPONSE.ObfuscationRegex = responseobj.obfuscationRegex;
            RemoteAPI.LAST_RESPONSE.MappingURL = responseobj.mappingURL;
            RemoteAPI.LAST_RESPONSE.MappingFileSHA512 = responseobj.mappingFileSHA512;

            RemoteAPI.ShouldMakeContact = false;
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
