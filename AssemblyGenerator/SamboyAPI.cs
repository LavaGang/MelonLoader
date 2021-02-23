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
            try
            {
                string Response = Core.webClient.DownloadString(ContactURL);
                if (string.IsNullOrEmpty(Response))
                    return;
                Logger.Debug_Msg($"Response = {Response}");
                Variant responsearr = JSON.Load(Response);
                if (responsearr == null)
                    return;
                try { Response_MappingURL = responsearr["mappingUrl"]; } catch { Response_MappingURL = null; }
                try { Response_MappingFileSHA512 = responsearr["mappingFileSHA512"]; } catch { Response_MappingFileSHA512 = null; }
                try { Response_ForceCpp2ILVersion = responsearr["forceCpp2IlVersion"]; } catch { Response_ForceCpp2ILVersion = null; }
                try { Response_ForceUnhollowerVersion = responsearr["forceUnhollowerVersion"]; } catch { Response_ForceUnhollowerVersion = null; }
                try { Response_ObfuscationRegex = responsearr["obfuscationRegex"]; } catch { Response_ObfuscationRegex = null; }
                if (!string.IsNullOrEmpty(Response_MappingURL))
                    Logger.Debug_Msg($"Response_MappingURL = {Response_MappingURL}");
                if (!string.IsNullOrEmpty(Response_MappingFileSHA512))
                    Logger.Debug_Msg($"Response_MappingFileSHA512 = {Response_MappingFileSHA512}");
                if (!string.IsNullOrEmpty(Response_ForceCpp2ILVersion))
                    Logger.Debug_Msg($"Response_ForceCpp2ILVersion = {Response_ForceCpp2ILVersion}");
                if (!string.IsNullOrEmpty(Response_ForceUnhollowerVersion))
                    Logger.Debug_Msg($"Response_ForceUnhollowerVersion = {Response_ForceUnhollowerVersion}");
                if (!string.IsNullOrEmpty(Response_ObfuscationRegex))
                    Logger.Debug_Msg($"Response_ObfuscationRegex = {Response_ObfuscationRegex}");
            }
            catch { }
        }
    }
}
