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
            string response = null;
            try
            {
                response = Core.webClient.DownloadString(API_URL 
                + "/" 
                + API_VERSION 
                + "/game/" 
                + Regex.Replace(Core.GameName, "[^a-zA-Z0-9_.]+", "-", RegexOptions.Compiled));
            }
            catch { return; }
            if (string.IsNullOrEmpty(response))
                return;
            try
            {
                Variant responsearr = JSON.Load(response);
                if (responsearr == null)
                    return;
                Response_MappingURL = responsearr["mappingUrl"];
                Response_MappingFileSHA512 = responsearr["mappingFileSHA512"];
                Response_ForceCpp2ILVersion = responsearr["forceCpp2IlVersion"];
                Response_ForceUnhollowerVersion = responsearr["forceUnhollowerVersion"];
                Response_ObfuscationRegex = responsearr["obfuscationRegex"];
            }
            catch { }
        }
    }
}
