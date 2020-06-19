using System;
using System.Net;
using LightJson;

namespace MelonLoader.AssemblyGenerator
{
    public static class Program
    {
        public static bool Force_Regenerate = false;
        internal static WebClient webClient = new WebClient();

        public static int Main(string[] args)
        {
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol |= (SecurityProtocolType)3072;
            webClient.Headers.Add("User-Agent", "Unity web player");

            if (args.Length < 4)
            {
                Logger.LogError("Bad arguments for generator process; expected arguments: <unityVersion> <gameRoot> <gameData> <regenerate> <force_version_unhollower>");
                return -1;
            }

            if ((args.Length >= 4) && !string.IsNullOrEmpty(args[3]) && args[3].Equals("true"))
                Force_Regenerate = true;

            if (args.Length >= 5)
            {
                try
                {
                    string Force_Version_Unhollower = args[4];
                    if (!string.IsNullOrEmpty(Force_Version_Unhollower))
                    {
                        JsonArray data = (JsonArray)JsonValue.Parse(webClient.DownloadString("https://api.github.com/repos/knah/Il2CppAssemblyUnhollower/releases")).AsJsonArray;
                        if (data.Count > 0)
                        {
                            foreach (var x in data)
                            {
                                string version = x["tag_name"].AsString;
                                if (!string.IsNullOrEmpty(version) && version.Equals("v" + Force_Version_Unhollower))
                                {
                                    ExternalToolVersions.Il2CppAssemblyUnhollowerVersion = Force_Version_Unhollower;
                                    ExternalToolVersions.Il2CppAssemblyUnhollowerUrl = "https://github.com/knah/Il2CppAssemblyUnhollower/releases/download/v" + Force_Version_Unhollower + "/Il2CppAssemblyUnhollower." + Force_Version_Unhollower + ".zip";
                                    break;
                                }
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Logger.LogError("Failed to Force Unhollower Version: " + e.Message);
                }
            }

            try
            {
                return AssemblyGenerator.Main.Initialize(args[0], args[1], args[2]) ? 0 : -2;
            }
            catch (Exception ex)
            {
                Logger.LogError("Failed to generate assemblies;");
                Logger.LogError(ex.ToString());
                
                return -3;
            }
        }
    }
}