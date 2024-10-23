using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace MelonBootstrap.Proxy;

internal static class WinHttpExports
{
    [UnmanagedCallersOnly(EntryPoint = "DllCanUnloadNow")]
    public static void DllCanUnloadNow() { }

    [UnmanagedCallersOnly(EntryPoint = "DllGetClassObject")]
    public static void DllGetClassObject() { }

    [UnmanagedCallersOnly(EntryPoint = "Private1")]
    public static void Private1() { }

    [UnmanagedCallersOnly(EntryPoint = "SvchostPushServiceGlobals")]
    public static void SvchostPushServiceGlobals() { }

    [UnmanagedCallersOnly(EntryPoint = "WinHttpAddRequestHeaders")]
    public static void WinHttpAddRequestHeaders() { }

    [UnmanagedCallersOnly(EntryPoint = "WinHttpAddRequestHeadersEx")]
    public static void WinHttpAddRequestHeadersEx() { }

    [UnmanagedCallersOnly(EntryPoint = "WinHttpAutoProxySvcMain")]
    public static void WinHttpAutoProxySvcMain() { }

    [UnmanagedCallersOnly(EntryPoint = "WinHttpCheckPlatform")]
    public static void WinHttpCheckPlatform() { }

    [UnmanagedCallersOnly(EntryPoint = "WinHttpCloseHandle")]
    public static void WinHttpCloseHandle() { }

    [UnmanagedCallersOnly(EntryPoint = "WinHttpConnect")]
    public static void WinHttpConnect() { }

    [UnmanagedCallersOnly(EntryPoint = "WinHttpConnectionDeletePolicyEntries")]
    public static void WinHttpConnectionDeletePolicyEntries() { }

    [UnmanagedCallersOnly(EntryPoint = "WinHttpConnectionDeleteProxyInfo")]
    public static void WinHttpConnectionDeleteProxyInfo() { }

    [UnmanagedCallersOnly(EntryPoint = "WinHttpConnectionFreeNameList")]
    public static void WinHttpConnectionFreeNameList() { }

    [UnmanagedCallersOnly(EntryPoint = "WinHttpConnectionFreeProxyInfo")]
    public static void WinHttpConnectionFreeProxyInfo() { }

    [UnmanagedCallersOnly(EntryPoint = "WinHttpConnectionFreeProxyList")]
    public static void WinHttpConnectionFreeProxyList() { }

    [UnmanagedCallersOnly(EntryPoint = "WinHttpConnectionGetNameList")]
    public static void WinHttpConnectionGetNameList() { }

    [UnmanagedCallersOnly(EntryPoint = "WinHttpConnectionGetProxyInfo")]
    public static void WinHttpConnectionGetProxyInfo() { }

    [UnmanagedCallersOnly(EntryPoint = "WinHttpConnectionGetProxyList")]
    public static void WinHttpConnectionGetProxyList() { }

    [UnmanagedCallersOnly(EntryPoint = "WinHttpConnectionOnlyConvert")]
    public static void WinHttpConnectionOnlyConvert() { }

    [UnmanagedCallersOnly(EntryPoint = "WinHttpConnectionOnlyReceive")]
    public static void WinHttpConnectionOnlyReceive() { }

    [UnmanagedCallersOnly(EntryPoint = "WinHttpConnectionOnlySend")]
    public static void WinHttpConnectionOnlySend() { }

    [UnmanagedCallersOnly(EntryPoint = "WinHttpConnectionSetPolicyEntries")]
    public static void WinHttpConnectionSetPolicyEntries() { }

    [UnmanagedCallersOnly(EntryPoint = "WinHttpConnectionSetProxyInfo")]
    public static void WinHttpConnectionSetProxyInfo() { }

    [UnmanagedCallersOnly(EntryPoint = "WinHttpConnectionUpdateIfIndexTable")]
    public static void WinHttpConnectionUpdateIfIndexTable() { }

    [UnmanagedCallersOnly(EntryPoint = "WinHttpCrackUrl")]
    public static void WinHttpCrackUrl() { }

    [UnmanagedCallersOnly(EntryPoint = "WinHttpCreateProxyList")]
    public static void WinHttpCreateProxyList() { }

    [UnmanagedCallersOnly(EntryPoint = "WinHttpCreateProxyManager")]
    public static void WinHttpCreateProxyManager() { }

    [UnmanagedCallersOnly(EntryPoint = "WinHttpCreateProxyResolver")]
    public static void WinHttpCreateProxyResolver() { }

    [UnmanagedCallersOnly(EntryPoint = "WinHttpCreateProxyResult")]
    public static void WinHttpCreateProxyResult() { }

    [UnmanagedCallersOnly(EntryPoint = "WinHttpCreateUiCompatibleProxyString")]
    public static void WinHttpCreateUiCompatibleProxyString() { }

    [UnmanagedCallersOnly(EntryPoint = "WinHttpCreateUrl")]
    public static void WinHttpCreateUrl() { }

    [UnmanagedCallersOnly(EntryPoint = "WinHttpDetectAutoProxyConfigUrl")]
    public static void WinHttpDetectAutoProxyConfigUrl() { }

    [UnmanagedCallersOnly(EntryPoint = "WinHttpFreeProxyResult")]
    public static void WinHttpFreeProxyResult() { }

    [UnmanagedCallersOnly(EntryPoint = "WinHttpFreeProxyResultEx")]
    public static void WinHttpFreeProxyResultEx() { }

    [UnmanagedCallersOnly(EntryPoint = "WinHttpFreeProxySettings")]
    public static void WinHttpFreeProxySettings() { }

    [UnmanagedCallersOnly(EntryPoint = "WinHttpFreeProxySettingsEx")]
    public static void WinHttpFreeProxySettingsEx() { }

    [UnmanagedCallersOnly(EntryPoint = "WinHttpFreeQueryConnectionGroupResult")]
    public static void WinHttpFreeQueryConnectionGroupResult() { }

    [UnmanagedCallersOnly(EntryPoint = "WinHttpGetDefaultProxyConfiguration")]
    public static void WinHttpGetDefaultProxyConfiguration() { }

    [UnmanagedCallersOnly(EntryPoint = "WinHttpGetIEProxyConfigForCurrentUser")]
    public static void WinHttpGetIEProxyConfigForCurrentUser() { }

    [UnmanagedCallersOnly(EntryPoint = "WinHttpGetProxyForUrl")]
    public static void WinHttpGetProxyForUrl() { }

    [UnmanagedCallersOnly(EntryPoint = "WinHttpGetProxyForUrlEx")]
    public static void WinHttpGetProxyForUrlEx() { }

    [UnmanagedCallersOnly(EntryPoint = "WinHttpGetProxyForUrlEx2")]
    public static void WinHttpGetProxyForUrlEx2() { }

    [UnmanagedCallersOnly(EntryPoint = "WinHttpGetProxyForUrlHvsi")]
    public static void WinHttpGetProxyForUrlHvsi() { }

    [UnmanagedCallersOnly(EntryPoint = "WinHttpGetProxyResult")]
    public static void WinHttpGetProxyResult() { }

    [UnmanagedCallersOnly(EntryPoint = "WinHttpGetProxyResultEx")]
    public static void WinHttpGetProxyResultEx() { }

    [UnmanagedCallersOnly(EntryPoint = "WinHttpGetProxySettingsEx")]
    public static void WinHttpGetProxySettingsEx() { }

    [UnmanagedCallersOnly(EntryPoint = "WinHttpGetProxySettingsResultEx")]
    public static void WinHttpGetProxySettingsResultEx() { }

    [UnmanagedCallersOnly(EntryPoint = "WinHttpGetProxySettingsVersion")]
    public static void WinHttpGetProxySettingsVersion() { }

    [UnmanagedCallersOnly(EntryPoint = "WinHttpGetTunnelSocket")]
    public static void WinHttpGetTunnelSocket() { }

    [UnmanagedCallersOnly(EntryPoint = "WinHttpOpen")]
    public static void WinHttpOpen() { }

    [UnmanagedCallersOnly(EntryPoint = "WinHttpOpenRequest")]
    public static void WinHttpOpenRequest() { }

    [UnmanagedCallersOnly(EntryPoint = "WinHttpPacJsWorkerMain")]
    public static void WinHttpPacJsWorkerMain() { }

    [UnmanagedCallersOnly(EntryPoint = "WinHttpProbeConnectivity")]
    public static void WinHttpProbeConnectivity() { }

    [UnmanagedCallersOnly(EntryPoint = "WinHttpProtocolCompleteUpgrade")]
    public static void WinHttpProtocolCompleteUpgrade() { }

    [UnmanagedCallersOnly(EntryPoint = "WinHttpProtocolReceive")]
    public static void WinHttpProtocolReceive() { }

    [UnmanagedCallersOnly(EntryPoint = "WinHttpProtocolSend")]
    public static void WinHttpProtocolSend() { }

    [UnmanagedCallersOnly(EntryPoint = "WinHttpQueryAuthSchemes")]
    public static void WinHttpQueryAuthSchemes() { }

    [UnmanagedCallersOnly(EntryPoint = "WinHttpQueryConnectionGroup")]
    public static void WinHttpQueryConnectionGroup() { }

    [UnmanagedCallersOnly(EntryPoint = "WinHttpQueryDataAvailable")]
    public static void WinHttpQueryDataAvailable() { }

    [UnmanagedCallersOnly(EntryPoint = "WinHttpQueryHeaders")]
    public static void WinHttpQueryHeaders() { }

    [UnmanagedCallersOnly(EntryPoint = "WinHttpQueryHeadersEx")]
    public static void WinHttpQueryHeadersEx() { }

    [UnmanagedCallersOnly(EntryPoint = "WinHttpQueryOption")]
    public static void WinHttpQueryOption() { }

    [UnmanagedCallersOnly(EntryPoint = "WinHttpReadData")]
    public static void WinHttpReadData() { }

    [UnmanagedCallersOnly(EntryPoint = "WinHttpReadDataEx")]
    public static void WinHttpReadDataEx() { }

    [UnmanagedCallersOnly(EntryPoint = "WinHttpReadProxySettings")]
    public static void WinHttpReadProxySettings() { }

    [UnmanagedCallersOnly(EntryPoint = "WinHttpReadProxySettingsHvsi")]
    public static void WinHttpReadProxySettingsHvsi() { }

    [UnmanagedCallersOnly(EntryPoint = "WinHttpReceiveResponse")]
    public static void WinHttpReceiveResponse() { }

    [UnmanagedCallersOnly(EntryPoint = "WinHttpRefreshProxySettings")]
    public static void WinHttpRefreshProxySettings() { }

    [UnmanagedCallersOnly(EntryPoint = "WinHttpRegisterProxyChangeNotification")]
    public static void WinHttpRegisterProxyChangeNotification() { }

    [UnmanagedCallersOnly(EntryPoint = "WinHttpResetAutoProxy")]
    public static void WinHttpResetAutoProxy() { }

    [UnmanagedCallersOnly(EntryPoint = "WinHttpResolverGetProxyForUrl")]
    public static void WinHttpResolverGetProxyForUrl() { }

    [UnmanagedCallersOnly(EntryPoint = "WinHttpSaveProxyCredentials")]
    public static void WinHttpSaveProxyCredentials() { }

    [UnmanagedCallersOnly(EntryPoint = "WinHttpSendRequest")]
    public static void WinHttpSendRequest() { }

    [UnmanagedCallersOnly(EntryPoint = "WinHttpSetCredentials")]
    public static void WinHttpSetCredentials() { }

    [UnmanagedCallersOnly(EntryPoint = "WinHttpSetDefaultProxyConfiguration")]
    public static void WinHttpSetDefaultProxyConfiguration() { }

    [UnmanagedCallersOnly(EntryPoint = "WinHttpSetOption")]
    public static void WinHttpSetOption() { }

    [UnmanagedCallersOnly(EntryPoint = "WinHttpSetProxySettingsPerUser")]
    public static void WinHttpSetProxySettingsPerUser() { }

    [UnmanagedCallersOnly(EntryPoint = "WinHttpSetSecureLegacyServersAppCompat")]
    public static void WinHttpSetSecureLegacyServersAppCompat() { }

    [UnmanagedCallersOnly(EntryPoint = "WinHttpSetStatusCallback")]
    public static void WinHttpSetStatusCallback() { }

    [UnmanagedCallersOnly(EntryPoint = "WinHttpSetTimeouts")]
    public static void WinHttpSetTimeouts() { }

    [UnmanagedCallersOnly(EntryPoint = "WinHttpTimeFromSystemTime")]
    public static void WinHttpTimeFromSystemTime() { }

    [UnmanagedCallersOnly(EntryPoint = "WinHttpTimeToSystemTime")]
    public static void WinHttpTimeToSystemTime() { }

    [UnmanagedCallersOnly(EntryPoint = "WinHttpUnregisterProxyChangeNotification")]
    public static void WinHttpUnregisterProxyChangeNotification() { }

    [UnmanagedCallersOnly(EntryPoint = "WinHttpWebSocketClose")]
    public static void WinHttpWebSocketClose() { }

    [UnmanagedCallersOnly(EntryPoint = "WinHttpWebSocketCompleteUpgrade")]
    public static void WinHttpWebSocketCompleteUpgrade() { }

    [UnmanagedCallersOnly(EntryPoint = "WinHttpWebSocketQueryCloseStatus")]
    public static void WinHttpWebSocketQueryCloseStatus() { }

    [UnmanagedCallersOnly(EntryPoint = "WinHttpWebSocketReceive")]
    public static void WinHttpWebSocketReceive() { }

    [UnmanagedCallersOnly(EntryPoint = "WinHttpWebSocketSend")]
    public static void WinHttpWebSocketSend() { }

    [UnmanagedCallersOnly(EntryPoint = "WinHttpWebSocketShutdown")]
    public static void WinHttpWebSocketShutdown() { }

    [UnmanagedCallersOnly(EntryPoint = "WinHttpWriteData")]
    public static void WinHttpWriteData() { }

    [UnmanagedCallersOnly(EntryPoint = "WinHttpWriteProxySettings")]
    public static void WinHttpWriteProxySettings() { }
}