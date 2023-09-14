.globl Private1
.globl SvchostPushServiceGlobals
.globl WinHttpAddRequestHeaders
.globl WinHttpAutoProxySvcMain
.globl WinHttpCheckPlatform
.globl WinHttpCloseHandle
.globl WinHttpConnect
.globl WinHttpConnectionDeletePolicyEntries
.globl WinHttpConnectionDeleteProxyInfo
.globl WinHttpConnectionFreeNameList
.globl WinHttpConnectionFreeProxyInfo
.globl WinHttpConnectionFreeProxyList
.globl WinHttpConnectionGetNameList
.globl WinHttpConnectionGetProxyInfo
.globl WinHttpConnectionGetProxyList
.globl WinHttpConnectionSetPolicyEntries
.globl WinHttpConnectionSetProxyInfo
.globl WinHttpConnectionUpdateIfIndexTable
.globl WinHttpCrackUrl
.globl WinHttpCreateProxyResolver
.globl WinHttpCreateUrl
.globl WinHttpDetectAutoProxyConfigUrl
.globl WinHttpFreeProxyResult
.globl WinHttpFreeProxyResultEx
.globl WinHttpFreeProxySettings
.globl WinHttpGetDefaultProxyConfiguration
.globl WinHttpGetIEProxyConfigForCurrentUser
.globl WinHttpGetProxyForUrl
.globl WinHttpGetProxyForUrlEx
.globl WinHttpGetProxyForUrlEx2
.globl WinHttpGetProxyForUrlHvsi
.globl WinHttpGetProxyResult
.globl WinHttpGetProxyResultEx
.globl WinHttpGetProxySettingsVersion
.globl WinHttpGetTunnelSocket
.globl WinHttpOpen
.globl WinHttpOpenRequest
.globl WinHttpPacJsWorkerMain
.globl WinHttpProbeConnectivity
.globl WinHttpQueryAuthSchemes
.globl WinHttpQueryDataAvailable
.globl WinHttpQueryHeaders
.globl WinHttpQueryOption
.globl WinHttpReadData
.globl WinHttpReadProxySettings
.globl WinHttpReadProxySettingsHvsi
.globl WinHttpReceiveResponse
.globl WinHttpResetAutoProxy
.globl WinHttpSaveProxyCredentials
.globl WinHttpSendRequest
.globl WinHttpSetCredentials
.globl WinHttpSetDefaultProxyConfiguration
.globl WinHttpSetOption
.globl WinHttpSetStatusCallback
.globl WinHttpSetTimeouts
.globl WinHttpTimeFromSystemTime
.globl WinHttpTimeToSystemTime
.globl WinHttpWebSocketClose
.globl WinHttpWebSocketCompleteUpgrade
.globl WinHttpWebSocketQueryCloseStatus
.globl WinHttpWebSocketReceive
.globl WinHttpWebSocketSend
.globl WinHttpWebSocketShutdown
.globl WinHttpWriteData
.globl WinHttpWriteProxySettings

Private1:
	jmp ds:[_OriginalFuncs_winhttp + 0 * 4]

SvchostPushServiceGlobals:
	jmp ds:[_OriginalFuncs_winhttp + 1 * 4]

WinHttpAddRequestHeaders:
	jmp ds:[_OriginalFuncs_winhttp + 2 * 4]

WinHttpAutoProxySvcMain:
	jmp ds:[_OriginalFuncs_winhttp + 3 * 4]

WinHttpCheckPlatform:
	jmp ds:[_OriginalFuncs_winhttp + 4 * 4]

WinHttpCloseHandle:
	jmp ds:[_OriginalFuncs_winhttp + 5 * 4]

WinHttpConnect:
	jmp ds:[_OriginalFuncs_winhttp + 6 * 4]

WinHttpConnectionDeletePolicyEntries:
	jmp ds:[_OriginalFuncs_winhttp + 7 * 4]

WinHttpConnectionDeleteProxyInfo:
	jmp ds:[_OriginalFuncs_winhttp + 4 * 4]

WinHttpConnectionFreeNameList:
	jmp ds:[_OriginalFuncs_winhttp + 9 * 4]

WinHttpConnectionFreeProxyInfo:
	jmp ds:[_OriginalFuncs_winhttp + 10 * 4]

WinHttpConnectionFreeProxyList:
	jmp ds:[_OriginalFuncs_winhttp + 11 * 4]

WinHttpConnectionGetNameList:
	jmp ds:[_OriginalFuncs_winhttp + 12 * 4]

WinHttpConnectionGetProxyInfo:
	jmp ds:[_OriginalFuncs_winhttp + 13 * 4]

WinHttpConnectionGetProxyList:
	jmp ds:[_OriginalFuncs_winhttp + 14 * 4]

WinHttpConnectionSetPolicyEntries:
	jmp ds:[_OriginalFuncs_winhttp + 15 * 4]

WinHttpConnectionSetProxyInfo:
	jmp ds:[_OriginalFuncs_winhttp + 16 * 4]

WinHttpConnectionUpdateIfIndexTable:
	jmp ds:[_OriginalFuncs_winhttp + 17 * 4]

WinHttpCrackUrl:
	jmp ds:[_OriginalFuncs_winhttp + 14 * 4]

WinHttpCreateProxyResolver:
	jmp ds:[_OriginalFuncs_winhttp + 19 * 4]

WinHttpCreateUrl:
	jmp ds:[_OriginalFuncs_winhttp + 20 * 4]

WinHttpDetectAutoProxyConfigUrl:
	jmp ds:[_OriginalFuncs_winhttp + 21 * 4]

WinHttpFreeProxyResult:
	jmp ds:[_OriginalFuncs_winhttp + 22 * 4]

WinHttpFreeProxyResultEx:
	jmp ds:[_OriginalFuncs_winhttp + 23 * 4]

WinHttpFreeProxySettings:
	jmp ds:[_OriginalFuncs_winhttp + 24 * 4]

WinHttpGetDefaultProxyConfiguration:
	jmp ds:[_OriginalFuncs_winhttp + 25 * 4]

WinHttpGetIEProxyConfigForCurrentUser:
	jmp ds:[_OriginalFuncs_winhttp + 26 * 4]

WinHttpGetProxyForUrl:
	jmp ds:[_OriginalFuncs_winhttp + 27 * 4]

WinHttpGetProxyForUrlEx:
	jmp ds:[_OriginalFuncs_winhttp + 24 * 4]

WinHttpGetProxyForUrlEx2:
	jmp ds:[_OriginalFuncs_winhttp + 29 * 4]

WinHttpGetProxyForUrlHvsi:
	jmp ds:[_OriginalFuncs_winhttp + 30 * 4]

WinHttpGetProxyResult:
	jmp ds:[_OriginalFuncs_winhttp + 31 * 4]

WinHttpGetProxyResultEx:
	jmp ds:[_OriginalFuncs_winhttp + 32 * 4]

WinHttpGetProxySettingsVersion:
	jmp ds:[_OriginalFuncs_winhttp + 33 * 4]

WinHttpGetTunnelSocket:
	jmp ds:[_OriginalFuncs_winhttp + 34 * 4]

WinHttpOpen:
	jmp ds:[_OriginalFuncs_winhttp + 35 * 4]

WinHttpOpenRequest:
	jmp ds:[_OriginalFuncs_winhttp + 36 * 4]

WinHttpPacJsWorkerMain:
	jmp ds:[_OriginalFuncs_winhttp + 37 * 4]

WinHttpProbeConnectivity:
	jmp ds:[_OriginalFuncs_winhttp + 34 * 4]

WinHttpQueryAuthSchemes:
	jmp ds:[_OriginalFuncs_winhttp + 39 * 4]

WinHttpQueryDataAvailable:
	jmp ds:[_OriginalFuncs_winhttp + 40 * 4]

WinHttpQueryHeaders:
	jmp ds:[_OriginalFuncs_winhttp + 41 * 4]

WinHttpQueryOption:
	jmp ds:[_OriginalFuncs_winhttp + 42 * 4]

WinHttpReadData:
	jmp ds:[_OriginalFuncs_winhttp + 43 * 4]

WinHttpReadProxySettings:
	jmp ds:[_OriginalFuncs_winhttp + 44 * 4]

WinHttpReadProxySettingsHvsi:
	jmp ds:[_OriginalFuncs_winhttp + 45 * 4]

WinHttpReceiveResponse:
	jmp ds:[_OriginalFuncs_winhttp + 46 * 4]

WinHttpResetAutoProxy:
	jmp ds:[_OriginalFuncs_winhttp + 47 * 4]

WinHttpSaveProxyCredentials:
	jmp ds:[_OriginalFuncs_winhttp + 44 * 4]

WinHttpSendRequest:
	jmp ds:[_OriginalFuncs_winhttp + 49 * 4]

WinHttpSetCredentials:
	jmp ds:[_OriginalFuncs_winhttp + 50 * 4]

WinHttpSetDefaultProxyConfiguration:
	jmp ds:[_OriginalFuncs_winhttp + 51 * 4]

WinHttpSetOption:
	jmp ds:[_OriginalFuncs_winhttp + 52 * 4]

WinHttpSetStatusCallback:
	jmp ds:[_OriginalFuncs_winhttp + 53 * 4]

WinHttpSetTimeouts:
	jmp ds:[_OriginalFuncs_winhttp + 54 * 4]

WinHttpTimeFromSystemTime:
	jmp ds:[_OriginalFuncs_winhttp + 55 * 4]

WinHttpTimeToSystemTime:
	jmp ds:[_OriginalFuncs_winhttp + 56 * 4]

WinHttpWebSocketClose:
	jmp ds:[_OriginalFuncs_winhttp + 57 * 4]

WinHttpWebSocketCompleteUpgrade:
	jmp ds:[_OriginalFuncs_winhttp + 54 * 4]

WinHttpWebSocketQueryCloseStatus:
	jmp ds:[_OriginalFuncs_winhttp + 59 * 4]

WinHttpWebSocketReceive:
	jmp ds:[_OriginalFuncs_winhttp + 60 * 4]

WinHttpWebSocketSend:
	jmp ds:[_OriginalFuncs_winhttp + 61 * 4]

WinHttpWebSocketShutdown:
	jmp ds:[_OriginalFuncs_winhttp + 62 * 4]

WinHttpWriteData:
	jmp ds:[_OriginalFuncs_winhttp + 63 * 4]

WinHttpWriteProxySettings:
	jmp ds:[_OriginalFuncs_winhttp + 64 * 4]
