ifdef RAX
	.code
		extern OriginalFuncs_winhttp:QWORD
		Private1 proc
			jmp QWORD ptr OriginalFuncs_winhttp[0 * 8]
		Private1 endp
		SvchostPushServiceGlobals proc
			jmp QWORD ptr OriginalFuncs_winhttp[1 * 8]
		SvchostPushServiceGlobals endp
		WinHttpAddRequestHeaders proc
			jmp QWORD ptr OriginalFuncs_winhttp[2 * 8]
		WinHttpAddRequestHeaders endp
		WinHttpAutoProxySvcMain proc
			jmp QWORD ptr OriginalFuncs_winhttp[3 * 8]
		WinHttpAutoProxySvcMain endp
		WinHttpCheckPlatform proc
			jmp QWORD ptr OriginalFuncs_winhttp[4 * 8]
		WinHttpCheckPlatform endp
		WinHttpCloseHandle proc
			jmp QWORD ptr OriginalFuncs_winhttp[5 * 8]
		WinHttpCloseHandle endp
		WinHttpConnect proc
			jmp QWORD ptr OriginalFuncs_winhttp[6 * 8]
		WinHttpConnect endp
		WinHttpConnectionDeletePolicyEntries proc
			jmp QWORD ptr OriginalFuncs_winhttp[7 * 8]
		WinHttpConnectionDeletePolicyEntries endp
		WinHttpConnectionDeleteProxyInfo proc
			jmp QWORD ptr OriginalFuncs_winhttp[8 * 8]
		WinHttpConnectionDeleteProxyInfo endp
		WinHttpConnectionFreeNameList proc
			jmp QWORD ptr OriginalFuncs_winhttp[9 * 8]
		WinHttpConnectionFreeNameList endp
		WinHttpConnectionFreeProxyInfo proc
			jmp QWORD ptr OriginalFuncs_winhttp[10 * 8]
		WinHttpConnectionFreeProxyInfo endp
		WinHttpConnectionFreeProxyList proc
			jmp QWORD ptr OriginalFuncs_winhttp[11 * 8]
		WinHttpConnectionFreeProxyList endp
		WinHttpConnectionGetNameList proc
			jmp QWORD ptr OriginalFuncs_winhttp[12 * 8]
		WinHttpConnectionGetNameList endp
		WinHttpConnectionGetProxyInfo proc
			jmp QWORD ptr OriginalFuncs_winhttp[13 * 8]
		WinHttpConnectionGetProxyInfo endp
		WinHttpConnectionGetProxyList proc
			jmp QWORD ptr OriginalFuncs_winhttp[14 * 8]
		WinHttpConnectionGetProxyList endp
		WinHttpConnectionSetPolicyEntries proc
			jmp QWORD ptr OriginalFuncs_winhttp[15 * 8]
		WinHttpConnectionSetPolicyEntries endp
		WinHttpConnectionSetProxyInfo proc
			jmp QWORD ptr OriginalFuncs_winhttp[16 * 8]
		WinHttpConnectionSetProxyInfo endp
		WinHttpConnectionUpdateIfIndexTable proc
			jmp QWORD ptr OriginalFuncs_winhttp[17 * 8]
		WinHttpConnectionUpdateIfIndexTable endp
		WinHttpCrackUrl proc
			jmp QWORD ptr OriginalFuncs_winhttp[18 * 8]
		WinHttpCrackUrl endp
		WinHttpCreateProxyResolver proc
			jmp QWORD ptr OriginalFuncs_winhttp[19 * 8]
		WinHttpCreateProxyResolver endp
		WinHttpCreateUrl proc
			jmp QWORD ptr OriginalFuncs_winhttp[20 * 8]
		WinHttpCreateUrl endp
		WinHttpDetectAutoProxyConfigUrl proc
			jmp QWORD ptr OriginalFuncs_winhttp[21 * 8]
		WinHttpDetectAutoProxyConfigUrl endp
		WinHttpFreeProxyResult proc
			jmp QWORD ptr OriginalFuncs_winhttp[22 * 8]
		WinHttpFreeProxyResult endp
		WinHttpFreeProxyResultEx proc
			jmp QWORD ptr OriginalFuncs_winhttp[23 * 8]
		WinHttpFreeProxyResultEx endp
		WinHttpFreeProxySettings proc
			jmp QWORD ptr OriginalFuncs_winhttp[24 * 8]
		WinHttpFreeProxySettings endp
		WinHttpGetDefaultProxyConfiguration proc
			jmp QWORD ptr OriginalFuncs_winhttp[25 * 8]
		WinHttpGetDefaultProxyConfiguration endp
		WinHttpGetIEProxyConfigForCurrentUser proc
			jmp QWORD ptr OriginalFuncs_winhttp[26 * 8]
		WinHttpGetIEProxyConfigForCurrentUser endp
		WinHttpGetProxyForUrl proc
			jmp QWORD ptr OriginalFuncs_winhttp[27 * 8]
		WinHttpGetProxyForUrl endp
		WinHttpGetProxyForUrlEx proc
			jmp QWORD ptr OriginalFuncs_winhttp[28 * 8]
		WinHttpGetProxyForUrlEx endp
		WinHttpGetProxyForUrlEx2 proc
			jmp QWORD ptr OriginalFuncs_winhttp[29 * 8]
		WinHttpGetProxyForUrlEx2 endp
		WinHttpGetProxyForUrlHvsi proc
			jmp QWORD ptr OriginalFuncs_winhttp[30 * 8]
		WinHttpGetProxyForUrlHvsi endp
		WinHttpGetProxyResult proc
			jmp QWORD ptr OriginalFuncs_winhttp[31 * 8]
		WinHttpGetProxyResult endp
		WinHttpGetProxyResultEx proc
			jmp QWORD ptr OriginalFuncs_winhttp[32 * 8]
		WinHttpGetProxyResultEx endp
		WinHttpGetProxySettingsVersion proc
			jmp QWORD ptr OriginalFuncs_winhttp[33 * 8]
		WinHttpGetProxySettingsVersion endp
		WinHttpGetTunnelSocket proc
			jmp QWORD ptr OriginalFuncs_winhttp[34 * 8]
		WinHttpGetTunnelSocket endp
		WinHttpOpen proc
			jmp QWORD ptr OriginalFuncs_winhttp[35 * 8]
		WinHttpOpen endp
		WinHttpOpenRequest proc
			jmp QWORD ptr OriginalFuncs_winhttp[36 * 8]
		WinHttpOpenRequest endp
		WinHttpPacJsWorkerMain proc
			jmp QWORD ptr OriginalFuncs_winhttp[37 * 8]
		WinHttpPacJsWorkerMain endp
		WinHttpProbeConnectivity proc
			jmp QWORD ptr OriginalFuncs_winhttp[38 * 8]
		WinHttpProbeConnectivity endp
		WinHttpQueryAuthSchemes proc
			jmp QWORD ptr OriginalFuncs_winhttp[39 * 8]
		WinHttpQueryAuthSchemes endp
		WinHttpQueryDataAvailable proc
			jmp QWORD ptr OriginalFuncs_winhttp[40 * 8]
		WinHttpQueryDataAvailable endp
		WinHttpQueryHeaders proc
			jmp QWORD ptr OriginalFuncs_winhttp[41 * 8]
		WinHttpQueryHeaders endp
		WinHttpQueryOption proc
			jmp QWORD ptr OriginalFuncs_winhttp[42 * 8]
		WinHttpQueryOption endp
		WinHttpReadData proc
			jmp QWORD ptr OriginalFuncs_winhttp[43 * 8]
		WinHttpReadData endp
		WinHttpReadProxySettings proc
			jmp QWORD ptr OriginalFuncs_winhttp[44 * 8]
		WinHttpReadProxySettings endp
		WinHttpReadProxySettingsHvsi proc
			jmp QWORD ptr OriginalFuncs_winhttp[45 * 8]
		WinHttpReadProxySettingsHvsi endp
		WinHttpReceiveResponse proc
			jmp QWORD ptr OriginalFuncs_winhttp[46 * 8]
		WinHttpReceiveResponse endp
		WinHttpResetAutoProxy proc
			jmp QWORD ptr OriginalFuncs_winhttp[47 * 8]
		WinHttpResetAutoProxy endp
		WinHttpSaveProxyCredentials proc
			jmp QWORD ptr OriginalFuncs_winhttp[48 * 8]
		WinHttpSaveProxyCredentials endp
		WinHttpSendRequest proc
			jmp QWORD ptr OriginalFuncs_winhttp[49 * 8]
		WinHttpSendRequest endp
		WinHttpSetCredentials proc
			jmp QWORD ptr OriginalFuncs_winhttp[50 * 8]
		WinHttpSetCredentials endp
		WinHttpSetDefaultProxyConfiguration proc
			jmp QWORD ptr OriginalFuncs_winhttp[51 * 8]
		WinHttpSetDefaultProxyConfiguration endp
		WinHttpSetOption proc
			jmp QWORD ptr OriginalFuncs_winhttp[52 * 8]
		WinHttpSetOption endp
		WinHttpSetStatusCallback proc
			jmp QWORD ptr OriginalFuncs_winhttp[53 * 8]
		WinHttpSetStatusCallback endp
		WinHttpSetTimeouts proc
			jmp QWORD ptr OriginalFuncs_winhttp[54 * 8]
		WinHttpSetTimeouts endp
		WinHttpTimeFromSystemTime proc
			jmp QWORD ptr OriginalFuncs_winhttp[55 * 8]
		WinHttpTimeFromSystemTime endp
		WinHttpTimeToSystemTime proc
			jmp QWORD ptr OriginalFuncs_winhttp[56 * 8]
		WinHttpTimeToSystemTime endp
		WinHttpWebSocketClose proc
			jmp QWORD ptr OriginalFuncs_winhttp[57 * 8]
		WinHttpWebSocketClose endp
		WinHttpWebSocketCompleteUpgrade proc
			jmp QWORD ptr OriginalFuncs_winhttp[58 * 8]
		WinHttpWebSocketCompleteUpgrade endp
		WinHttpWebSocketQueryCloseStatus proc
			jmp QWORD ptr OriginalFuncs_winhttp[59 * 8]
		WinHttpWebSocketQueryCloseStatus endp
		WinHttpWebSocketReceive proc
			jmp QWORD ptr OriginalFuncs_winhttp[60 * 8]
		WinHttpWebSocketReceive endp
		WinHttpWebSocketSend proc
			jmp QWORD ptr OriginalFuncs_winhttp[61 * 8]
		WinHttpWebSocketSend endp
		WinHttpWebSocketShutdown proc
			jmp QWORD ptr OriginalFuncs_winhttp[62 * 8]
		WinHttpWebSocketShutdown endp
		WinHttpWriteData proc
			jmp QWORD ptr OriginalFuncs_winhttp[63 * 8]
		WinHttpWriteData endp
		WinHttpWriteProxySettings proc
			jmp QWORD ptr OriginalFuncs_winhttp[64 * 8]
		WinHttpWriteProxySettings endp
else
	.model flat, C
	.stack 4096
	.code
		extern OriginalFuncs_winhttp:DWORD
				Private1 proc
			jmp DWORD ptr OriginalFuncs_winhttp[0 * 4]
		Private1 endp
		SvchostPushServiceGlobals proc
			jmp DWORD ptr OriginalFuncs_winhttp[1 * 4]
		SvchostPushServiceGlobals endp
		WinHttpAddRequestHeaders proc
			jmp DWORD ptr OriginalFuncs_winhttp[2 * 4]
		WinHttpAddRequestHeaders endp
		WinHttpAutoProxySvcMain proc
			jmp DWORD ptr OriginalFuncs_winhttp[3 * 4]
		WinHttpAutoProxySvcMain endp
		WinHttpCheckPlatform proc
			jmp DWORD ptr OriginalFuncs_winhttp[4 * 4]
		WinHttpCheckPlatform endp
		WinHttpCloseHandle proc
			jmp DWORD ptr OriginalFuncs_winhttp[5 * 4]
		WinHttpCloseHandle endp
		WinHttpConnect proc
			jmp DWORD ptr OriginalFuncs_winhttp[6 * 4]
		WinHttpConnect endp
		WinHttpConnectionDeletePolicyEntries proc
			jmp DWORD ptr OriginalFuncs_winhttp[7 * 4]
		WinHttpConnectionDeletePolicyEntries endp
		WinHttpConnectionDeleteProxyInfo proc
			jmp DWORD ptr OriginalFuncs_winhttp[8 * 4]
		WinHttpConnectionDeleteProxyInfo endp
		WinHttpConnectionFreeNameList proc
			jmp DWORD ptr OriginalFuncs_winhttp[9 * 4]
		WinHttpConnectionFreeNameList endp
		WinHttpConnectionFreeProxyInfo proc
			jmp DWORD ptr OriginalFuncs_winhttp[10 * 4]
		WinHttpConnectionFreeProxyInfo endp
		WinHttpConnectionFreeProxyList proc
			jmp DWORD ptr OriginalFuncs_winhttp[11 * 4]
		WinHttpConnectionFreeProxyList endp
		WinHttpConnectionGetNameList proc
			jmp DWORD ptr OriginalFuncs_winhttp[12 * 4]
		WinHttpConnectionGetNameList endp
		WinHttpConnectionGetProxyInfo proc
			jmp DWORD ptr OriginalFuncs_winhttp[13 * 4]
		WinHttpConnectionGetProxyInfo endp
		WinHttpConnectionGetProxyList proc
			jmp DWORD ptr OriginalFuncs_winhttp[14 * 4]
		WinHttpConnectionGetProxyList endp
		WinHttpConnectionSetPolicyEntries proc
			jmp DWORD ptr OriginalFuncs_winhttp[15 * 4]
		WinHttpConnectionSetPolicyEntries endp
		WinHttpConnectionSetProxyInfo proc
			jmp DWORD ptr OriginalFuncs_winhttp[16 * 4]
		WinHttpConnectionSetProxyInfo endp
		WinHttpConnectionUpdateIfIndexTable proc
			jmp DWORD ptr OriginalFuncs_winhttp[17 * 4]
		WinHttpConnectionUpdateIfIndexTable endp
		WinHttpCrackUrl proc
			jmp DWORD ptr OriginalFuncs_winhttp[18 * 4]
		WinHttpCrackUrl endp
		WinHttpCreateProxyResolver proc
			jmp DWORD ptr OriginalFuncs_winhttp[19 * 4]
		WinHttpCreateProxyResolver endp
		WinHttpCreateUrl proc
			jmp DWORD ptr OriginalFuncs_winhttp[20 * 4]
		WinHttpCreateUrl endp
		WinHttpDetectAutoProxyConfigUrl proc
			jmp DWORD ptr OriginalFuncs_winhttp[21 * 4]
		WinHttpDetectAutoProxyConfigUrl endp
		WinHttpFreeProxyResult proc
			jmp DWORD ptr OriginalFuncs_winhttp[22 * 4]
		WinHttpFreeProxyResult endp
		WinHttpFreeProxyResultEx proc
			jmp DWORD ptr OriginalFuncs_winhttp[23 * 4]
		WinHttpFreeProxyResultEx endp
		WinHttpFreeProxySettings proc
			jmp DWORD ptr OriginalFuncs_winhttp[24 * 4]
		WinHttpFreeProxySettings endp
		WinHttpGetDefaultProxyConfiguration proc
			jmp DWORD ptr OriginalFuncs_winhttp[25 * 4]
		WinHttpGetDefaultProxyConfiguration endp
		WinHttpGetIEProxyConfigForCurrentUser proc
			jmp DWORD ptr OriginalFuncs_winhttp[26 * 4]
		WinHttpGetIEProxyConfigForCurrentUser endp
		WinHttpGetProxyForUrl proc
			jmp DWORD ptr OriginalFuncs_winhttp[27 * 4]
		WinHttpGetProxyForUrl endp
		WinHttpGetProxyForUrlEx proc
			jmp DWORD ptr OriginalFuncs_winhttp[28 * 4]
		WinHttpGetProxyForUrlEx endp
		WinHttpGetProxyForUrlEx2 proc
			jmp DWORD ptr OriginalFuncs_winhttp[29 * 4]
		WinHttpGetProxyForUrlEx2 endp
		WinHttpGetProxyForUrlHvsi proc
			jmp DWORD ptr OriginalFuncs_winhttp[30 * 4]
		WinHttpGetProxyForUrlHvsi endp
		WinHttpGetProxyResult proc
			jmp DWORD ptr OriginalFuncs_winhttp[31 * 4]
		WinHttpGetProxyResult endp
		WinHttpGetProxyResultEx proc
			jmp DWORD ptr OriginalFuncs_winhttp[32 * 4]
		WinHttpGetProxyResultEx endp
		WinHttpGetProxySettingsVersion proc
			jmp DWORD ptr OriginalFuncs_winhttp[33 * 4]
		WinHttpGetProxySettingsVersion endp
		WinHttpGetTunnelSocket proc
			jmp DWORD ptr OriginalFuncs_winhttp[34 * 4]
		WinHttpGetTunnelSocket endp
		WinHttpOpen proc
			jmp DWORD ptr OriginalFuncs_winhttp[35 * 4]
		WinHttpOpen endp
		WinHttpOpenRequest proc
			jmp DWORD ptr OriginalFuncs_winhttp[36 * 4]
		WinHttpOpenRequest endp
		WinHttpPacJsWorkerMain proc
			jmp DWORD ptr OriginalFuncs_winhttp[37 * 4]
		WinHttpPacJsWorkerMain endp
		WinHttpProbeConnectivity proc
			jmp DWORD ptr OriginalFuncs_winhttp[38 * 4]
		WinHttpProbeConnectivity endp
		WinHttpQueryAuthSchemes proc
			jmp DWORD ptr OriginalFuncs_winhttp[39 * 4]
		WinHttpQueryAuthSchemes endp
		WinHttpQueryDataAvailable proc
			jmp DWORD ptr OriginalFuncs_winhttp[40 * 4]
		WinHttpQueryDataAvailable endp
		WinHttpQueryHeaders proc
			jmp DWORD ptr OriginalFuncs_winhttp[41 * 4]
		WinHttpQueryHeaders endp
		WinHttpQueryOption proc
			jmp DWORD ptr OriginalFuncs_winhttp[42 * 4]
		WinHttpQueryOption endp
		WinHttpReadData proc
			jmp DWORD ptr OriginalFuncs_winhttp[43 * 4]
		WinHttpReadData endp
		WinHttpReadProxySettings proc
			jmp DWORD ptr OriginalFuncs_winhttp[44 * 4]
		WinHttpReadProxySettings endp
		WinHttpReadProxySettingsHvsi proc
			jmp DWORD ptr OriginalFuncs_winhttp[45 * 4]
		WinHttpReadProxySettingsHvsi endp
		WinHttpReceiveResponse proc
			jmp DWORD ptr OriginalFuncs_winhttp[46 * 4]
		WinHttpReceiveResponse endp
		WinHttpResetAutoProxy proc
			jmp DWORD ptr OriginalFuncs_winhttp[47 * 4]
		WinHttpResetAutoProxy endp
		WinHttpSaveProxyCredentials proc
			jmp DWORD ptr OriginalFuncs_winhttp[48 * 4]
		WinHttpSaveProxyCredentials endp
		WinHttpSendRequest proc
			jmp DWORD ptr OriginalFuncs_winhttp[49 * 4]
		WinHttpSendRequest endp
		WinHttpSetCredentials proc
			jmp DWORD ptr OriginalFuncs_winhttp[50 * 4]
		WinHttpSetCredentials endp
		WinHttpSetDefaultProxyConfiguration proc
			jmp DWORD ptr OriginalFuncs_winhttp[51 * 4]
		WinHttpSetDefaultProxyConfiguration endp
		WinHttpSetOption proc
			jmp DWORD ptr OriginalFuncs_winhttp[52 * 4]
		WinHttpSetOption endp
		WinHttpSetStatusCallback proc
			jmp DWORD ptr OriginalFuncs_winhttp[53 * 4]
		WinHttpSetStatusCallback endp
		WinHttpSetTimeouts proc
			jmp DWORD ptr OriginalFuncs_winhttp[54 * 4]
		WinHttpSetTimeouts endp
		WinHttpTimeFromSystemTime proc
			jmp DWORD ptr OriginalFuncs_winhttp[55 * 4]
		WinHttpTimeFromSystemTime endp
		WinHttpTimeToSystemTime proc
			jmp DWORD ptr OriginalFuncs_winhttp[56 * 4]
		WinHttpTimeToSystemTime endp
		WinHttpWebSocketClose proc
			jmp DWORD ptr OriginalFuncs_winhttp[57 * 4]
		WinHttpWebSocketClose endp
		WinHttpWebSocketCompleteUpgrade proc
			jmp DWORD ptr OriginalFuncs_winhttp[58 * 4]
		WinHttpWebSocketCompleteUpgrade endp
		WinHttpWebSocketQueryCloseStatus proc
			jmp DWORD ptr OriginalFuncs_winhttp[59 * 4]
		WinHttpWebSocketQueryCloseStatus endp
		WinHttpWebSocketReceive proc
			jmp DWORD ptr OriginalFuncs_winhttp[60 * 4]
		WinHttpWebSocketReceive endp
		WinHttpWebSocketSend proc
			jmp DWORD ptr OriginalFuncs_winhttp[61 * 4]
		WinHttpWebSocketSend endp
		WinHttpWebSocketShutdown proc
			jmp DWORD ptr OriginalFuncs_winhttp[62 * 4]
		WinHttpWebSocketShutdown endp
		WinHttpWriteData proc
			jmp DWORD ptr OriginalFuncs_winhttp[63 * 4]
		WinHttpWriteData endp
		WinHttpWriteProxySettings proc
			jmp DWORD ptr OriginalFuncs_winhttp[64 * 4]
		WinHttpWriteProxySettings endp
endif
end