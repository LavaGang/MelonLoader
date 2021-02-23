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

        internal static string Response_MappingURL = null;
        internal static string Response_MappingFileSHA512 = null;
        internal static string Response_ForceCpp2ILVersion = null;
        internal static string Response_ForceUnhollowerVersion = null;
        internal static string Response_ObfuscationRegex = null;

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
                Logger.Warning($"Exception while Contacting API: {ex}");
                return;
            }

            if (string.IsNullOrEmpty(Response))
            {
                Logger.Debug_Msg($"Response = null");
                return;
            }
            Logger.Debug_Msg($"Response = {Response}");

            Variant responsearr = null;
            try { responsearr = JSON.Load(Response); } catch (Exception ex) { Logger.Warning($"Exception while Parsing Response to JSON Variant: {ex}"); responsearr = null; }
            if (responsearr == null)
                return;

            try { Response_MappingURL = responsearr["mappingUrl"]; } catch (Exception ex) { Logger.Warning($"Exception while reading mappingUrl: {ex}"); Response_MappingURL = null; }
            try { Response_MappingFileSHA512 = responsearr["mappingFileSHA512"]; } catch (Exception ex) { Logger.Warning($"Exception while reading mappingFileSHA512: {ex}"); Response_MappingFileSHA512 = null; };
            try { Response_ForceCpp2ILVersion = responsearr["forceCpp2IlVersion"]; } catch (Exception ex) { Logger.Warning($"Exception while reading forceCpp2IlVersion: {ex}"); Response_ForceCpp2ILVersion = null; }
            try { Response_ForceUnhollowerVersion = responsearr["forceUnhollowerVersion"]; } catch (Exception ex) { Logger.Warning($"Exception while reading forceUnhollowerVersion: {ex}"); Response_ForceUnhollowerVersion = null; }
            try { Response_ObfuscationRegex = responsearr["obfuscationRegex"]; } catch (Exception ex) { Logger.Warning($"Exception while reading obfuscationRegex: {ex}"); Response_ObfuscationRegex = null; }

            Logger.Debug_Msg($"Response_MappingURL = {(string.IsNullOrEmpty(Response_MappingURL) ? "null" : Response_MappingURL)}");
            Logger.Debug_Msg($"Response_MappingFileSHA512 = {(string.IsNullOrEmpty(Response_MappingFileSHA512) ? "null" : Response_MappingFileSHA512)}");
            Logger.Debug_Msg($"Response_ForceCpp2ILVersion = {(string.IsNullOrEmpty(Response_ForceCpp2ILVersion) ? "null" : Response_ForceCpp2ILVersion)}");
            Logger.Debug_Msg($"Response_ForceUnhollowerVersion = {(string.IsNullOrEmpty(Response_ForceUnhollowerVersion) ? "null" : Response_ForceUnhollowerVersion)}");
            Logger.Debug_Msg($"Response_ObfuscationRegex = {(string.IsNullOrEmpty(Response_ObfuscationRegex) ? "null" : Response_ObfuscationRegex)}");
        }
    }
}
