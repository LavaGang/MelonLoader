using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using Semver;

namespace MelonLoader.Il2CppAssemblyGenerator
{
    internal static class RemoteAPI
    {
        internal class InfoStruct
        {
            internal string ForceDumperVersion = null;
            internal string ObfuscationRegex = null;
            internal string MappingURL = null;
            internal string MappingFileSHA512 = null;
        }

        internal static InfoStruct Info = new InfoStruct();
        internal static RemoteAPIContactResult? LastContactResult { get; private set; }

        internal static RemoteAPIContactResult Contact()
        {
            Core.Logger.Msg("Contacting RemoteAPI...");

            Info = new InfoStruct();
            RemoteAPIContactOutcome outcome = RemoteAPIClient.ContactHosts(
                Core.webClient,
                CreateHostList(),
                MelonDebug.Msg);
            LastContactResult = outcome.Result;

            switch (outcome.Result)
            {
                case RemoteAPIContactResult.Success:
                    Info = outcome.Info;
                    NormalizeInfo();
                    LogInfo();
                    break;

                case RemoteAPIContactResult.NotFound:
                    Core.Logger.Msg($"Game Not Found on RemoteAPI Host ({outcome.HostURL})");
                    LogInfo();
                    break;

                case RemoteAPIContactResult.Unavailable:
                    Core.Logger.Warning(
                        $"RemoteAPI is unavailable after trying {outcome.Failures.Count} hosts "
                        + $"({BuildFailureSummary(outcome.Failures)}). "
                        + "Continuing with locally cached generation settings when available.");
                    break;
            }

            return outcome.Result;
        }

        internal static string GetRemoteOrCachedValue(string remoteValue, string cachedValue)
            => LastContactResult == RemoteAPIContactResult.Unavailable
                && !string.IsNullOrEmpty(cachedValue)
                    ? cachedValue
                    : remoteValue;

        private static List<RemoteAPIHost> CreateHostList()
        {
            string gameName = Regex.Replace(
                InternalUtils.UnityInformationHandler.GameName,
                "[^a-zA-Z0-9_.]+",
                "-",
                RegexOptions.Compiled).ToLowerInvariant();

            return new List<RemoteAPIHost> {
                new RemoteAPIHost($"{DefaultHostInfo.Melon.API_URL}{gameName}", DefaultHostInfo.Melon.Contact),
                new RemoteAPIHost($"{DefaultHostInfo.Melon.API_URL_1}{gameName}", DefaultHostInfo.Melon.Contact),
                new RemoteAPIHost($"{DefaultHostInfo.Melon.API_URL_2}{gameName}", DefaultHostInfo.Melon.Contact),
                new RemoteAPIHost($"{DefaultHostInfo.Melon.API_URL_SAMBOY}{gameName}", DefaultHostInfo.Melon.Contact),
                new RemoteAPIHost($"{DefaultHostInfo.Melon.API_URL_DUBYADUDE}{gameName}", DefaultHostInfo.Melon.Contact),
            };
        }

        private static void NormalizeInfo()
        {
            if (string.IsNullOrEmpty(Info.ForceDumperVersion))
                return;

            if (!SemVersion.TryParse(Info.ForceDumperVersion, out SemVersion version))
            {
                MelonDebug.Msg($"RemoteAPI returned an invalid Cpp2IL version '{Info.ForceDumperVersion}'. Ignoring it.");
                Info.ForceDumperVersion = null;
                return;
            }

            if (version <= SemVersion.Parse("2022.0.2"))
                Info.ForceDumperVersion = null;
        }

        private static void LogInfo()
        {
            Core.Logger.Msg($"RemoteAPI.DumperVersion = {(string.IsNullOrEmpty(Info.ForceDumperVersion) ? "null" : Info.ForceDumperVersion)}");
            Core.Logger.Msg($"RemoteAPI.ObfuscationRegex = {(string.IsNullOrEmpty(Info.ObfuscationRegex) ? "null" : Info.ObfuscationRegex)}");
            Core.Logger.Msg($"RemoteAPI.MappingURL = {(string.IsNullOrEmpty(Info.MappingURL) ? "null" : Info.MappingURL)}");
            Core.Logger.Msg($"RemoteAPI.MappingFileSHA512 = {(string.IsNullOrEmpty(Info.MappingFileSHA512) ? "null" : Info.MappingFileSHA512)}");
        }

        private static string BuildFailureSummary(IReadOnlyList<RemoteAPIHostFailure> failures)
        {
            if (failures.Count <= 0)
                return "no hosts configured";

            Dictionary<string, int> reasonCounts = new Dictionary<string, int>();
            List<string> reasons = new List<string>();
            foreach (RemoteAPIHostFailure failure in failures)
            {
                if (reasonCounts.TryGetValue(failure.Reason, out int count))
                {
                    reasonCounts[failure.Reason] = count + 1;
                    continue;
                }

                reasonCounts.Add(failure.Reason, 1);
                reasons.Add(failure.Reason);
            }

            for (int i = 0; i < reasons.Count; i++)
            {
                string reason = reasons[i];
                int count = reasonCounts[reason];
                if (count > 1)
                    reasons[i] = $"{reason} x{count}";
            }

            return string.Join(", ", reasons);
        }

        private class DefaultHostInfo
        {
            internal static class Melon
            {
                internal static string API_VERSION = "v1";
                internal static string API_URL = $"https://api.melonloader.com/api/{API_VERSION}/game/";
                internal static string API_URL_1 = $"https://api-1.melonloader.com/api/{API_VERSION}/game/";
                internal static string API_URL_2 = $"https://api-2.melonloader.com/api/{API_VERSION}/game/";
                internal static string API_URL_SAMBOY = $"https://melon.samboy.dev/api/{API_VERSION}/game/";
                internal static string API_URL_DUBYADUDE = $"https://melon.dubyadu.de/api/{API_VERSION}/game/";

                internal static InfoStruct Contact(string response_str)
                {
                    ResponseStruct responseobj = JsonSerializer.Deserialize<ResponseStruct>(response_str);
                    if (responseobj == null)
                        return null;

                    InfoStruct returninfo = new InfoStruct();
                    returninfo.ForceDumperVersion = responseobj.ForceCpp2IlVersion;
                    returninfo.ObfuscationRegex = responseobj.ObfuscationRegex;
                    returninfo.MappingURL = responseobj.MappingUrl;
                    returninfo.MappingFileSHA512 = responseobj.MappingFileSHA512;
                    return returninfo;
                }

                internal class ResponseStruct
                {
                    [JsonPropertyName("gameSlug")]
                    public string GameSlug { get; set; }

                    [JsonPropertyName("gameName")]
                    public string GameName { get; set; }

                    [JsonPropertyName("mappingUrl")]
                    public string MappingUrl { get; set; }

                    [JsonPropertyName("mappingFileSHA512")]
                    public string MappingFileSHA512 { get; set; }

                    [JsonPropertyName("forceCpp2IlVersion")]
                    public string ForceCpp2IlVersion { get; set; }

                    [JsonPropertyName("forceUnhollowerVersion")]
                    public string ForceUnhollowerVersion { get; set; } //TODO: Remove this from the API

                    [JsonPropertyName("obfuscationRegex")]
                    public string ObfuscationRegex { get; set; }
                }
            }
        }
    }
}
