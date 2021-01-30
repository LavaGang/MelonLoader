using System.Text.RegularExpressions;
using MelonLoader.TinyJSON;
#pragma warning disable 0649

namespace MelonLoader.AssemblyGenerator
{
    internal static class SamboyAPI
    {
        private static string API_URL = "https://melon.samboy.dev/api";
        private static string API_VERSION = "v1";

        private static string GetGameURL() => API_URL + "/" + API_VERSION + "/game/" + Regex.Replace(Core.GameName, "[^a-zA-Z0-9_.]+", "-", RegexOptions.Compiled);

        internal static Variant GetResponse()
        {
            string url = GetGameURL();
            string response;
            try { response = Core.webClient.DownloadString(GetGameURL()); } catch { return null; }
            if (string.IsNullOrEmpty(response))
                return null;
            Variant responsearr = null;
            try { responsearr = JSON.Load(response); } catch { return null; }
            if (responsearr == null)
                return null;
            return responsearr;
        }
    }
}
