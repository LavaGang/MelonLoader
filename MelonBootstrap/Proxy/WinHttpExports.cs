using System.Runtime.InteropServices;

namespace MelonLoader.Bootstrap.Proxy;

internal static class WinHttpExports
{
    [UnmanagedCallersOnly(EntryPoint = "ImplDllCanUnloadNow")]
    public static void ImplDllCanUnloadNow() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplDllGetClassObject")]
    public static void ImplDllGetClassObject() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplPrivate1")]
    public static void ImplPrivate1() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplSvchostPushServiceGlobals")]
    public static void ImplSvchostPushServiceGlobals() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplWinHttpAddRequestHeaders")]
    public static void ImplWinHttpAddRequestHeaders() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplWinHttpAddRequestHeadersEx")]
    public static void ImplWinHttpAddRequestHeadersEx() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplWinHttpAutoProxySvcMain")]
    public static void ImplWinHttpAutoProxySvcMain() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplWinHttpCheckPlatform")]
    public static void ImplWinHttpCheckPlatform() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplWinHttpCloseHandle")]
    public static void ImplWinHttpCloseHandle() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplWinHttpConnect")]
    public static void ImplWinHttpConnect() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplWinHttpConnectionDeletePolicyEntries")]
    public static void ImplWinHttpConnectionDeletePolicyEntries() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplWinHttpConnectionDeleteProxyInfo")]
    public static void ImplWinHttpConnectionDeleteProxyInfo() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplWinHttpConnectionFreeNameList")]
    public static void ImplWinHttpConnectionFreeNameList() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplWinHttpConnectionFreeProxyInfo")]
    public static void ImplWinHttpConnectionFreeProxyInfo() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplWinHttpConnectionFreeProxyList")]
    public static void ImplWinHttpConnectionFreeProxyList() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplWinHttpConnectionGetNameList")]
    public static void ImplWinHttpConnectionGetNameList() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplWinHttpConnectionGetProxyInfo")]
    public static void ImplWinHttpConnectionGetProxyInfo() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplWinHttpConnectionGetProxyList")]
    public static void ImplWinHttpConnectionGetProxyList() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplWinHttpConnectionOnlyConvert")]
    public static void ImplWinHttpConnectionOnlyConvert() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplWinHttpConnectionOnlyReceive")]
    public static void ImplWinHttpConnectionOnlyReceive() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplWinHttpConnectionOnlySend")]
    public static void ImplWinHttpConnectionOnlySend() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplWinHttpConnectionSetPolicyEntries")]
    public static void ImplWinHttpConnectionSetPolicyEntries() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplWinHttpConnectionSetProxyInfo")]
    public static void ImplWinHttpConnectionSetProxyInfo() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplWinHttpConnectionUpdateIfIndexTable")]
    public static void ImplWinHttpConnectionUpdateIfIndexTable() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplWinHttpCrackUrl")]
    public static void ImplWinHttpCrackUrl() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplWinHttpCreateProxyList")]
    public static void ImplWinHttpCreateProxyList() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplWinHttpCreateProxyManager")]
    public static void ImplWinHttpCreateProxyManager() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplWinHttpCreateProxyResolver")]
    public static void ImplWinHttpCreateProxyResolver() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplWinHttpCreateProxyResult")]
    public static void ImplWinHttpCreateProxyResult() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplWinHttpCreateUiCompatibleProxyString")]
    public static void ImplWinHttpCreateUiCompatibleProxyString() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplWinHttpCreateUrl")]
    public static void ImplWinHttpCreateUrl() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplWinHttpDetectAutoProxyConfigUrl")]
    public static void ImplWinHttpDetectAutoProxyConfigUrl() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplWinHttpFreeProxyResult")]
    public static void ImplWinHttpFreeProxyResult() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplWinHttpFreeProxyResultEx")]
    public static void ImplWinHttpFreeProxyResultEx() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplWinHttpFreeProxySettings")]
    public static void ImplWinHttpFreeProxySettings() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplWinHttpFreeProxySettingsEx")]
    public static void ImplWinHttpFreeProxySettingsEx() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplWinHttpFreeQueryConnectionGroupResult")]
    public static void ImplWinHttpFreeQueryConnectionGroupResult() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplWinHttpGetDefaultProxyConfiguration")]
    public static void ImplWinHttpGetDefaultProxyConfiguration() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplWinHttpGetIEProxyConfigForCurrentUser")]
    public static void ImplWinHttpGetIEProxyConfigForCurrentUser() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplWinHttpGetProxyForUrl")]
    public static void ImplWinHttpGetProxyForUrl() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplWinHttpGetProxyForUrlEx")]
    public static void ImplWinHttpGetProxyForUrlEx() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplWinHttpGetProxyForUrlEx2")]
    public static void ImplWinHttpGetProxyForUrlEx2() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplWinHttpGetProxyForUrlHvsi")]
    public static void ImplWinHttpGetProxyForUrlHvsi() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplWinHttpGetProxyResult")]
    public static void ImplWinHttpGetProxyResult() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplWinHttpGetProxyResultEx")]
    public static void ImplWinHttpGetProxyResultEx() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplWinHttpGetProxySettingsEx")]
    public static void ImplWinHttpGetProxySettingsEx() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplWinHttpGetProxySettingsResultEx")]
    public static void ImplWinHttpGetProxySettingsResultEx() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplWinHttpGetProxySettingsVersion")]
    public static void ImplWinHttpGetProxySettingsVersion() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplWinHttpGetTunnelSocket")]
    public static void ImplWinHttpGetTunnelSocket() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplWinHttpOpen")]
    public static void ImplWinHttpOpen() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplWinHttpOpenRequest")]
    public static void ImplWinHttpOpenRequest() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplWinHttpPacJsWorkerMain")]
    public static void ImplWinHttpPacJsWorkerMain() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplWinHttpProbeConnectivity")]
    public static void ImplWinHttpProbeConnectivity() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplWinHttpProtocolCompleteUpgrade")]
    public static void ImplWinHttpProtocolCompleteUpgrade() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplWinHttpProtocolReceive")]
    public static void ImplWinHttpProtocolReceive() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplWinHttpProtocolSend")]
    public static void ImplWinHttpProtocolSend() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplWinHttpQueryAuthSchemes")]
    public static void ImplWinHttpQueryAuthSchemes() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplWinHttpQueryConnectionGroup")]
    public static void ImplWinHttpQueryConnectionGroup() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplWinHttpQueryDataAvailable")]
    public static void ImplWinHttpQueryDataAvailable() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplWinHttpQueryHeaders")]
    public static void ImplWinHttpQueryHeaders() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplWinHttpQueryHeadersEx")]
    public static void ImplWinHttpQueryHeadersEx() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplWinHttpQueryOption")]
    public static void ImplWinHttpQueryOption() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplWinHttpReadData")]
    public static void ImplWinHttpReadData() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplWinHttpReadDataEx")]
    public static void ImplWinHttpReadDataEx() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplWinHttpReadProxySettings")]
    public static void ImplWinHttpReadProxySettings() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplWinHttpReadProxySettingsHvsi")]
    public static void ImplWinHttpReadProxySettingsHvsi() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplWinHttpReceiveResponse")]
    public static void ImplWinHttpReceiveResponse() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplWinHttpRefreshProxySettings")]
    public static void ImplWinHttpRefreshProxySettings() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplWinHttpRegisterProxyChangeNotification")]
    public static void ImplWinHttpRegisterProxyChangeNotification() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplWinHttpResetAutoProxy")]
    public static void ImplWinHttpResetAutoProxy() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplWinHttpResolverGetProxyForUrl")]
    public static void ImplWinHttpResolverGetProxyForUrl() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplWinHttpSaveProxyCredentials")]
    public static void ImplWinHttpSaveProxyCredentials() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplWinHttpSendRequest")]
    public static void ImplWinHttpSendRequest() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplWinHttpSetCredentials")]
    public static void ImplWinHttpSetCredentials() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplWinHttpSetDefaultProxyConfiguration")]
    public static void ImplWinHttpSetDefaultProxyConfiguration() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplWinHttpSetOption")]
    public static void ImplWinHttpSetOption() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplWinHttpSetProxySettingsPerUser")]
    public static void ImplWinHttpSetProxySettingsPerUser() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplWinHttpSetSecureLegacyServersAppCompat")]
    public static void ImplWinHttpSetSecureLegacyServersAppCompat() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplWinHttpSetStatusCallback")]
    public static void ImplWinHttpSetStatusCallback() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplWinHttpSetTimeouts")]
    public static void ImplWinHttpSetTimeouts() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplWinHttpTimeFromSystemTime")]
    public static void ImplWinHttpTimeFromSystemTime() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplWinHttpTimeToSystemTime")]
    public static void ImplWinHttpTimeToSystemTime() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplWinHttpUnregisterProxyChangeNotification")]
    public static void ImplWinHttpUnregisterProxyChangeNotification() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplWinHttpWebSocketClose")]
    public static void ImplWinHttpWebSocketClose() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplWinHttpWebSocketCompleteUpgrade")]
    public static void ImplWinHttpWebSocketCompleteUpgrade() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplWinHttpWebSocketQueryCloseStatus")]
    public static void ImplWinHttpWebSocketQueryCloseStatus() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplWinHttpWebSocketReceive")]
    public static void ImplWinHttpWebSocketReceive() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplWinHttpWebSocketSend")]
    public static void ImplWinHttpWebSocketSend() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplWinHttpWebSocketShutdown")]
    public static void ImplWinHttpWebSocketShutdown() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplWinHttpWriteData")]
    public static void ImplWinHttpWriteData() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplWinHttpWriteProxySettings")]
    public static void ImplWinHttpWriteProxySettings() { }
}