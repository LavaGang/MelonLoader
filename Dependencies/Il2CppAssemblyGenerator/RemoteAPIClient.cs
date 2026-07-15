using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;

namespace MelonLoader.Il2CppAssemblyGenerator
{
    internal enum RemoteAPIContactResult
    {
        Success,
        NotFound,
        Unavailable,
    }

    internal sealed class RemoteAPIHost
    {
        internal string URL { get; }
        internal Func<string, RemoteAPI.InfoStruct> ParseResponse { get; }

        internal RemoteAPIHost(string url, Func<string, RemoteAPI.InfoStruct> parseResponse)
        {
            URL = url;
            ParseResponse = parseResponse;
        }
    }

    internal sealed class RemoteAPIHostFailure
    {
        internal string URL { get; }
        internal string Reason { get; }

        internal RemoteAPIHostFailure(string url, string reason)
        {
            URL = url;
            Reason = reason;
        }
    }

    internal sealed class RemoteAPIContactOutcome
    {
        internal RemoteAPIContactResult Result { get; }
        internal RemoteAPI.InfoStruct Info { get; }
        internal string HostURL { get; }
        internal IReadOnlyList<RemoteAPIHostFailure> Failures { get; }

        internal RemoteAPIContactOutcome(
            RemoteAPIContactResult result,
            RemoteAPI.InfoStruct info,
            string hostURL,
            IReadOnlyList<RemoteAPIHostFailure> failures)
        {
            Result = result;
            Info = info;
            HostURL = hostURL;
            Failures = failures;
        }
    }

    internal static class RemoteAPIClient
    {
        internal static RemoteAPIContactOutcome ContactHosts(
            HttpClient client,
            IReadOnlyList<RemoteAPIHost> hosts,
            Action<string> debugLog = null)
        {
            if (client == null)
                throw new ArgumentNullException(nameof(client));
            if (hosts == null)
                throw new ArgumentNullException(nameof(hosts));

            List<RemoteAPIHostFailure> failures = new List<RemoteAPIHostFailure>();

            foreach (RemoteAPIHost host in hosts)
            {
                if (host == null || string.IsNullOrEmpty(host.URL) || host.ParseResponse == null)
                {
                    AddFailure(failures, host?.URL, "invalid host configuration", null, debugLog);
                    continue;
                }

                debugLog?.Invoke($"ContactURL = {host.URL}");

                HttpResponseMessage response;
                try
                {
                    response = client.GetAsync(host.URL).GetAwaiter().GetResult();
                }
                catch (Exception ex)
                {
                    AddFailure(failures, host.URL, ex.GetType().Name, ex, debugLog);
                    continue;
                }

                using (response)
                {
                    if (response.StatusCode == HttpStatusCode.NotFound)
                    {
                        return new RemoteAPIContactOutcome(
                            RemoteAPIContactResult.NotFound,
                            null,
                            host.URL,
                            failures);
                    }

                    if (!response.IsSuccessStatusCode)
                    {
                        string reason = $"{(int)response.StatusCode} {response.ReasonPhrase}".TrimEnd();
                        AddFailure(failures, host.URL, reason, null, debugLog);
                        continue;
                    }

                    string responseBody;
                    try
                    {
                        responseBody = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                    }
                    catch (Exception ex)
                    {
                        AddFailure(failures, host.URL, ex.GetType().Name, ex, debugLog);
                        continue;
                    }

                    if (string.IsNullOrWhiteSpace(responseBody))
                    {
                        AddFailure(failures, host.URL, "empty response", null, debugLog);
                        continue;
                    }

                    debugLog?.Invoke($"Response = {responseBody}");

                    RemoteAPI.InfoStruct info;
                    try
                    {
                        info = host.ParseResponse(responseBody);
                    }
                    catch (Exception ex)
                    {
                        AddFailure(failures, host.URL, $"invalid response ({ex.GetType().Name})", ex, debugLog);
                        continue;
                    }

                    if (info == null)
                    {
                        AddFailure(failures, host.URL, "invalid response", null, debugLog);
                        continue;
                    }

                    return new RemoteAPIContactOutcome(
                        RemoteAPIContactResult.Success,
                        info,
                        host.URL,
                        failures);
                }
            }

            return new RemoteAPIContactOutcome(
                RemoteAPIContactResult.Unavailable,
                null,
                null,
                failures);
        }

        private static void AddFailure(
            ICollection<RemoteAPIHostFailure> failures,
            string url,
            string reason,
            Exception exception,
            Action<string> debugLog)
        {
            string displayURL = string.IsNullOrEmpty(url) ? "unknown" : url;
            failures.Add(new RemoteAPIHostFailure(displayURL, reason));

            string details = exception == null ? reason : $"{reason}: {exception}";
            debugLog?.Invoke($"RemoteAPI Host ({displayURL}) failed: {details}");
        }
    }
}
