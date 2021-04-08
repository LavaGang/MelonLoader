ifdef RAX
	.code
		extern OriginalFuncs_winmm:QWORD
		CloseDriver proc
			jmp QWORD ptr OriginalFuncs_winmm[0 * 8]
		CloseDriver endp
		DefDriverProc proc
			jmp QWORD ptr OriginalFuncs_winmm[1 * 8]
		DefDriverProc endp
		DriverCallback proc
			jmp QWORD ptr OriginalFuncs_winmm[2 * 8]
		DriverCallback endp
		DrvGetModuleHandle proc
			jmp QWORD ptr OriginalFuncs_winmm[3 * 8]
		DrvGetModuleHandle endp
		GetDriverModuleHandle proc
			jmp QWORD ptr OriginalFuncs_winmm[4 * 8]
		GetDriverModuleHandle endp
		OpenDriver proc
			jmp QWORD ptr OriginalFuncs_winmm[5 * 8]
		OpenDriver endp
		PlaySound proc
			jmp QWORD ptr OriginalFuncs_winmm[6 * 8]
		PlaySound endp
		PlaySoundA proc
			jmp QWORD ptr OriginalFuncs_winmm[7 * 8]
		PlaySoundA endp
		PlaySoundW proc
			jmp QWORD ptr OriginalFuncs_winmm[8 * 8]
		PlaySoundW endp
		SendDriverMessage proc
			jmp QWORD ptr OriginalFuncs_winmm[9 * 8]
		SendDriverMessage endp
		WOWAppExit proc
			jmp QWORD ptr OriginalFuncs_winmm[10 * 8]
		WOWAppExit endp
		auxGetDevCapsA proc
			jmp QWORD ptr OriginalFuncs_winmm[11 * 8]
		auxGetDevCapsA endp
		auxGetDevCapsW proc
			jmp QWORD ptr OriginalFuncs_winmm[12 * 8]
		auxGetDevCapsW endp
		auxGetNumDevs proc
			jmp QWORD ptr OriginalFuncs_winmm[13 * 8]
		auxGetNumDevs endp
		auxGetVolume proc
			jmp QWORD ptr OriginalFuncs_winmm[14 * 8]
		auxGetVolume endp
		auxOutMessage proc
			jmp QWORD ptr OriginalFuncs_winmm[15 * 8]
		auxOutMessage endp
		auxSetVolume proc
			jmp QWORD ptr OriginalFuncs_winmm[16 * 8]
		auxSetVolume endp
		joyConfigChanged proc
			jmp QWORD ptr OriginalFuncs_winmm[17 * 8]
		joyConfigChanged endp
		joyGetDevCapsA proc
			jmp QWORD ptr OriginalFuncs_winmm[18 * 8]
		joyGetDevCapsA endp
		joyGetDevCapsW proc
			jmp QWORD ptr OriginalFuncs_winmm[19 * 8]
		joyGetDevCapsW endp
		joyGetNumDevs proc
			jmp QWORD ptr OriginalFuncs_winmm[20 * 8]
		joyGetNumDevs endp
		joyGetPos proc
			jmp QWORD ptr OriginalFuncs_winmm[21 * 8]
		joyGetPos endp
		joyGetPosEx proc
			jmp QWORD ptr OriginalFuncs_winmm[22 * 8]
		joyGetPosEx endp
		joyGetThreshold proc
			jmp QWORD ptr OriginalFuncs_winmm[23 * 8]
		joyGetThreshold endp
		joyReleaseCapture proc
			jmp QWORD ptr OriginalFuncs_winmm[24 * 8]
		joyReleaseCapture endp
		joySetCapture proc
			jmp QWORD ptr OriginalFuncs_winmm[25 * 8]
		joySetCapture endp
		joySetThreshold proc
			jmp QWORD ptr OriginalFuncs_winmm[26 * 8]
		joySetThreshold endp
		mciDriverNotify proc
			jmp QWORD ptr OriginalFuncs_winmm[27 * 8]
		mciDriverNotify endp
		mciDriverYield proc
			jmp QWORD ptr OriginalFuncs_winmm[28 * 8]
		mciDriverYield endp
		mciExecute proc
			jmp QWORD ptr OriginalFuncs_winmm[29 * 8]
		mciExecute endp
		mciFreeCommandResource proc
			jmp QWORD ptr OriginalFuncs_winmm[30 * 8]
		mciFreeCommandResource endp
		mciGetCreatorTask proc
			jmp QWORD ptr OriginalFuncs_winmm[31 * 8]
		mciGetCreatorTask endp
		mciGetDeviceIDA proc
			jmp QWORD ptr OriginalFuncs_winmm[32 * 8]
		mciGetDeviceIDA endp
		mciGetDeviceIDFromElementIDA proc
			jmp QWORD ptr OriginalFuncs_winmm[33 * 8]
		mciGetDeviceIDFromElementIDA endp
		mciGetDeviceIDFromElementIDW proc
			jmp QWORD ptr OriginalFuncs_winmm[34 * 8]
		mciGetDeviceIDFromElementIDW endp
		mciGetDeviceIDW proc
			jmp QWORD ptr OriginalFuncs_winmm[35 * 8]
		mciGetDeviceIDW endp
		mciGetDriverData proc
			jmp QWORD ptr OriginalFuncs_winmm[36 * 8]
		mciGetDriverData endp
		mciGetErrorStringA proc
			jmp QWORD ptr OriginalFuncs_winmm[37 * 8]
		mciGetErrorStringA endp
		mciGetErrorStringW proc
			jmp QWORD ptr OriginalFuncs_winmm[38 * 8]
		mciGetErrorStringW endp
		mciGetYieldProc proc
			jmp QWORD ptr OriginalFuncs_winmm[39 * 8]
		mciGetYieldProc endp
		mciLoadCommandResource proc
			jmp QWORD ptr OriginalFuncs_winmm[40 * 8]
		mciLoadCommandResource endp
		mciSendCommandA proc
			jmp QWORD ptr OriginalFuncs_winmm[41 * 8]
		mciSendCommandA endp
		mciSendCommandW proc
			jmp QWORD ptr OriginalFuncs_winmm[42 * 8]
		mciSendCommandW endp
		mciSendStringA proc
			jmp QWORD ptr OriginalFuncs_winmm[43 * 8]
		mciSendStringA endp
		mciSendStringW proc
			jmp QWORD ptr OriginalFuncs_winmm[44 * 8]
		mciSendStringW endp
		mciSetDriverData proc
			jmp QWORD ptr OriginalFuncs_winmm[45 * 8]
		mciSetDriverData endp
		mciSetYieldProc proc
			jmp QWORD ptr OriginalFuncs_winmm[46 * 8]
		mciSetYieldProc endp
		midiConnect proc
			jmp QWORD ptr OriginalFuncs_winmm[47 * 8]
		midiConnect endp
		midiDisconnect proc
			jmp QWORD ptr OriginalFuncs_winmm[48 * 8]
		midiDisconnect endp
		midiInAddBuffer proc
			jmp QWORD ptr OriginalFuncs_winmm[49 * 8]
		midiInAddBuffer endp
		midiInClose proc
			jmp QWORD ptr OriginalFuncs_winmm[50 * 8]
		midiInClose endp
		midiInGetDevCapsA proc
			jmp QWORD ptr OriginalFuncs_winmm[51 * 8]
		midiInGetDevCapsA endp
		midiInGetDevCapsW proc
			jmp QWORD ptr OriginalFuncs_winmm[52 * 8]
		midiInGetDevCapsW endp
		midiInGetErrorTextA proc
			jmp QWORD ptr OriginalFuncs_winmm[53 * 8]
		midiInGetErrorTextA endp
		midiInGetErrorTextW proc
			jmp QWORD ptr OriginalFuncs_winmm[54 * 8]
		midiInGetErrorTextW endp
		midiInGetID proc
			jmp QWORD ptr OriginalFuncs_winmm[55 * 8]
		midiInGetID endp
		midiInGetNumDevs proc
			jmp QWORD ptr OriginalFuncs_winmm[56 * 8]
		midiInGetNumDevs endp
		midiInMessage proc
			jmp QWORD ptr OriginalFuncs_winmm[57 * 8]
		midiInMessage endp
		midiInOpen proc
			jmp QWORD ptr OriginalFuncs_winmm[58 * 8]
		midiInOpen endp
		midiInPrepareHeader proc
			jmp QWORD ptr OriginalFuncs_winmm[59 * 8]
		midiInPrepareHeader endp
		midiInReset proc
			jmp QWORD ptr OriginalFuncs_winmm[60 * 8]
		midiInReset endp
		midiInStart proc
			jmp QWORD ptr OriginalFuncs_winmm[61 * 8]
		midiInStart endp
		midiInStop proc
			jmp QWORD ptr OriginalFuncs_winmm[62 * 8]
		midiInStop endp
		midiInUnprepareHeader proc
			jmp QWORD ptr OriginalFuncs_winmm[63 * 8]
		midiInUnprepareHeader endp
		midiOutCacheDrumPatches proc
			jmp QWORD ptr OriginalFuncs_winmm[64 * 8]
		midiOutCacheDrumPatches endp
		midiOutCachePatches proc
			jmp QWORD ptr OriginalFuncs_winmm[65 * 8]
		midiOutCachePatches endp
		midiOutClose proc
			jmp QWORD ptr OriginalFuncs_winmm[66 * 8]
		midiOutClose endp
		midiOutGetDevCapsA proc
			jmp QWORD ptr OriginalFuncs_winmm[67 * 8]
		midiOutGetDevCapsA endp
		midiOutGetDevCapsW proc
			jmp QWORD ptr OriginalFuncs_winmm[68 * 8]
		midiOutGetDevCapsW endp
		midiOutGetErrorTextA proc
			jmp QWORD ptr OriginalFuncs_winmm[69 * 8]
		midiOutGetErrorTextA endp
		midiOutGetErrorTextW proc
			jmp QWORD ptr OriginalFuncs_winmm[70 * 8]
		midiOutGetErrorTextW endp
		midiOutGetID proc
			jmp QWORD ptr OriginalFuncs_winmm[71 * 8]
		midiOutGetID endp
		midiOutGetNumDevs proc
			jmp QWORD ptr OriginalFuncs_winmm[72 * 8]
		midiOutGetNumDevs endp
		midiOutGetVolume proc
			jmp QWORD ptr OriginalFuncs_winmm[73 * 8]
		midiOutGetVolume endp
		midiOutLongMsg proc
			jmp QWORD ptr OriginalFuncs_winmm[74 * 8]
		midiOutLongMsg endp
		midiOutMessage proc
			jmp QWORD ptr OriginalFuncs_winmm[75 * 8]
		midiOutMessage endp
		midiOutOpen proc
			jmp QWORD ptr OriginalFuncs_winmm[76 * 8]
		midiOutOpen endp
		midiOutPrepareHeader proc
			jmp QWORD ptr OriginalFuncs_winmm[77 * 8]
		midiOutPrepareHeader endp
		midiOutReset proc
			jmp QWORD ptr OriginalFuncs_winmm[78 * 8]
		midiOutReset endp
		midiOutSetVolume proc
			jmp QWORD ptr OriginalFuncs_winmm[79 * 8]
		midiOutSetVolume endp
		midiOutShortMsg proc
			jmp QWORD ptr OriginalFuncs_winmm[80 * 8]
		midiOutShortMsg endp
		midiOutUnprepareHeader proc
			jmp QWORD ptr OriginalFuncs_winmm[81 * 8]
		midiOutUnprepareHeader endp
		midiStreamClose proc
			jmp QWORD ptr OriginalFuncs_winmm[82 * 8]
		midiStreamClose endp
		midiStreamOpen proc
			jmp QWORD ptr OriginalFuncs_winmm[83 * 8]
		midiStreamOpen endp
		midiStreamOut proc
			jmp QWORD ptr OriginalFuncs_winmm[84 * 8]
		midiStreamOut endp
		midiStreamPause proc
			jmp QWORD ptr OriginalFuncs_winmm[85 * 8]
		midiStreamPause endp
		midiStreamPosition proc
			jmp QWORD ptr OriginalFuncs_winmm[86 * 8]
		midiStreamPosition endp
		midiStreamProperty proc
			jmp QWORD ptr OriginalFuncs_winmm[87 * 8]
		midiStreamProperty endp
		midiStreamRestart proc
			jmp QWORD ptr OriginalFuncs_winmm[88 * 8]
		midiStreamRestart endp
		midiStreamStop proc
			jmp QWORD ptr OriginalFuncs_winmm[89 * 8]
		midiStreamStop endp
		mixerClose proc
			jmp QWORD ptr OriginalFuncs_winmm[90 * 8]
		mixerClose endp
		mixerGetControlDetailsA proc
			jmp QWORD ptr OriginalFuncs_winmm[91 * 8]
		mixerGetControlDetailsA endp
		mixerGetControlDetailsW proc
			jmp QWORD ptr OriginalFuncs_winmm[92 * 8]
		mixerGetControlDetailsW endp
		mixerGetDevCapsA proc
			jmp QWORD ptr OriginalFuncs_winmm[93 * 8]
		mixerGetDevCapsA endp
		mixerGetDevCapsW proc
			jmp QWORD ptr OriginalFuncs_winmm[94 * 8]
		mixerGetDevCapsW endp
		mixerGetID proc
			jmp QWORD ptr OriginalFuncs_winmm[95 * 8]
		mixerGetID endp
		mixerGetLineControlsA proc
			jmp QWORD ptr OriginalFuncs_winmm[96 * 8]
		mixerGetLineControlsA endp
		mixerGetLineControlsW proc
			jmp QWORD ptr OriginalFuncs_winmm[97 * 8]
		mixerGetLineControlsW endp
		mixerGetLineInfoA proc
			jmp QWORD ptr OriginalFuncs_winmm[98 * 8]
		mixerGetLineInfoA endp
		mixerGetLineInfoW proc
			jmp QWORD ptr OriginalFuncs_winmm[99 * 8]
		mixerGetLineInfoW endp
		mixerGetNumDevs proc
			jmp QWORD ptr OriginalFuncs_winmm[100 * 8]
		mixerGetNumDevs endp
		mixerMessage proc
			jmp QWORD ptr OriginalFuncs_winmm[101 * 8]
		mixerMessage endp
		mixerOpen proc
			jmp QWORD ptr OriginalFuncs_winmm[102 * 8]
		mixerOpen endp
		mixerSetControlDetails proc
			jmp QWORD ptr OriginalFuncs_winmm[103 * 8]
		mixerSetControlDetails endp
		mmDrvInstall proc
			jmp QWORD ptr OriginalFuncs_winmm[104 * 8]
		mmDrvInstall endp
		mmGetCurrentTask proc
			jmp QWORD ptr OriginalFuncs_winmm[105 * 8]
		mmGetCurrentTask endp
		mmTaskBlock proc
			jmp QWORD ptr OriginalFuncs_winmm[106 * 8]
		mmTaskBlock endp
		mmTaskCreate proc
			jmp QWORD ptr OriginalFuncs_winmm[107 * 8]
		mmTaskCreate endp
		mmTaskSignal proc
			jmp QWORD ptr OriginalFuncs_winmm[108 * 8]
		mmTaskSignal endp
		mmTaskYield proc
			jmp QWORD ptr OriginalFuncs_winmm[109 * 8]
		mmTaskYield endp
		mmioAdvance proc
			jmp QWORD ptr OriginalFuncs_winmm[110 * 8]
		mmioAdvance endp
		mmioAscend proc
			jmp QWORD ptr OriginalFuncs_winmm[111 * 8]
		mmioAscend endp
		mmioClose proc
			jmp QWORD ptr OriginalFuncs_winmm[112 * 8]
		mmioClose endp
		mmioCreateChunk proc
			jmp QWORD ptr OriginalFuncs_winmm[113 * 8]
		mmioCreateChunk endp
		mmioDescend proc
			jmp QWORD ptr OriginalFuncs_winmm[114 * 8]
		mmioDescend endp
		mmioFlush proc
			jmp QWORD ptr OriginalFuncs_winmm[115 * 8]
		mmioFlush endp
		mmioGetInfo proc
			jmp QWORD ptr OriginalFuncs_winmm[116 * 8]
		mmioGetInfo endp
		mmioInstallIOProcA proc
			jmp QWORD ptr OriginalFuncs_winmm[117 * 8]
		mmioInstallIOProcA endp
		mmioInstallIOProcW proc
			jmp QWORD ptr OriginalFuncs_winmm[118 * 8]
		mmioInstallIOProcW endp
		mmioOpenA proc
			jmp QWORD ptr OriginalFuncs_winmm[119 * 8]
		mmioOpenA endp
		mmioOpenW proc
			jmp QWORD ptr OriginalFuncs_winmm[120 * 8]
		mmioOpenW endp
		mmioRead proc
			jmp QWORD ptr OriginalFuncs_winmm[121 * 8]
		mmioRead endp
		mmioRenameA proc
			jmp QWORD ptr OriginalFuncs_winmm[122 * 8]
		mmioRenameA endp
		mmioRenameW proc
			jmp QWORD ptr OriginalFuncs_winmm[123 * 8]
		mmioRenameW endp
		mmioSeek proc
			jmp QWORD ptr OriginalFuncs_winmm[124 * 8]
		mmioSeek endp
		mmioSendMessage proc
			jmp QWORD ptr OriginalFuncs_winmm[125 * 8]
		mmioSendMessage endp
		mmioSetBuffer proc
			jmp QWORD ptr OriginalFuncs_winmm[126 * 8]
		mmioSetBuffer endp
		mmioSetInfo proc
			jmp QWORD ptr OriginalFuncs_winmm[127 * 8]
		mmioSetInfo endp
		mmioStringToFOURCCA proc
			jmp QWORD ptr OriginalFuncs_winmm[128 * 8]
		mmioStringToFOURCCA endp
		mmioStringToFOURCCW proc
			jmp QWORD ptr OriginalFuncs_winmm[129 * 8]
		mmioStringToFOURCCW endp
		mmioWrite proc
			jmp QWORD ptr OriginalFuncs_winmm[130 * 8]
		mmioWrite endp
		mmsystemGetVersion proc
			jmp QWORD ptr OriginalFuncs_winmm[131 * 8]
		mmsystemGetVersion endp
		sndPlaySoundA proc
			jmp QWORD ptr OriginalFuncs_winmm[132 * 8]
		sndPlaySoundA endp
		sndPlaySoundW proc
			jmp QWORD ptr OriginalFuncs_winmm[133 * 8]
		sndPlaySoundW endp
		timeBeginPeriod proc
			jmp QWORD ptr OriginalFuncs_winmm[134 * 8]
		timeBeginPeriod endp
		timeEndPeriod proc
			jmp QWORD ptr OriginalFuncs_winmm[135 * 8]
		timeEndPeriod endp
		timeGetDevCaps proc
			jmp QWORD ptr OriginalFuncs_winmm[136 * 8]
		timeGetDevCaps endp
		timeGetSystemTime proc
			jmp QWORD ptr OriginalFuncs_winmm[137 * 8]
		timeGetSystemTime endp
		timeGetTime proc
			jmp QWORD ptr OriginalFuncs_winmm[138 * 8]
		timeGetTime endp
		timeKillEvent proc
			jmp QWORD ptr OriginalFuncs_winmm[139 * 8]
		timeKillEvent endp
		timeSetEvent proc
			jmp QWORD ptr OriginalFuncs_winmm[140 * 8]
		timeSetEvent endp
		waveInAddBuffer proc
			jmp QWORD ptr OriginalFuncs_winmm[141 * 8]
		waveInAddBuffer endp
		waveInClose proc
			jmp QWORD ptr OriginalFuncs_winmm[142 * 8]
		waveInClose endp
		waveInGetDevCapsA proc
			jmp QWORD ptr OriginalFuncs_winmm[143 * 8]
		waveInGetDevCapsA endp
		waveInGetDevCapsW proc
			jmp QWORD ptr OriginalFuncs_winmm[144 * 8]
		waveInGetDevCapsW endp
		waveInGetErrorTextA proc
			jmp QWORD ptr OriginalFuncs_winmm[145 * 8]
		waveInGetErrorTextA endp
		waveInGetErrorTextW proc
			jmp QWORD ptr OriginalFuncs_winmm[146 * 8]
		waveInGetErrorTextW endp
		waveInGetID proc
			jmp QWORD ptr OriginalFuncs_winmm[147 * 8]
		waveInGetID endp
		waveInGetNumDevs proc
			jmp QWORD ptr OriginalFuncs_winmm[148 * 8]
		waveInGetNumDevs endp
		waveInGetPosition proc
			jmp QWORD ptr OriginalFuncs_winmm[149 * 8]
		waveInGetPosition endp
		waveInMessage proc
			jmp QWORD ptr OriginalFuncs_winmm[150 * 8]
		waveInMessage endp
		waveInOpen proc
			jmp QWORD ptr OriginalFuncs_winmm[151 * 8]
		waveInOpen endp
		waveInPrepareHeader proc
			jmp QWORD ptr OriginalFuncs_winmm[152 * 8]
		waveInPrepareHeader endp
		waveInReset proc
			jmp QWORD ptr OriginalFuncs_winmm[153 * 8]
		waveInReset endp
		waveInStart proc
			jmp QWORD ptr OriginalFuncs_winmm[154 * 8]
		waveInStart endp
		waveInStop proc
			jmp QWORD ptr OriginalFuncs_winmm[155 * 8]
		waveInStop endp
		waveInUnprepareHeader proc
			jmp QWORD ptr OriginalFuncs_winmm[156 * 8]
		waveInUnprepareHeader endp
		waveOutBreakLoop proc
			jmp QWORD ptr OriginalFuncs_winmm[157 * 8]
		waveOutBreakLoop endp
		waveOutClose proc
			jmp QWORD ptr OriginalFuncs_winmm[158 * 8]
		waveOutClose endp
		waveOutGetDevCapsA proc
			jmp QWORD ptr OriginalFuncs_winmm[159 * 8]
		waveOutGetDevCapsA endp
		waveOutGetDevCapsW proc
			jmp QWORD ptr OriginalFuncs_winmm[160 * 8]
		waveOutGetDevCapsW endp
		waveOutGetErrorTextA proc
			jmp QWORD ptr OriginalFuncs_winmm[161 * 8]
		waveOutGetErrorTextA endp
		waveOutGetErrorTextW proc
			jmp QWORD ptr OriginalFuncs_winmm[162 * 8]
		waveOutGetErrorTextW endp
		waveOutGetID proc
			jmp QWORD ptr OriginalFuncs_winmm[163 * 8]
		waveOutGetID endp
		waveOutGetNumDevs proc
			jmp QWORD ptr OriginalFuncs_winmm[164 * 8]
		waveOutGetNumDevs endp
		waveOutGetPitch proc
			jmp QWORD ptr OriginalFuncs_winmm[165 * 8]
		waveOutGetPitch endp
		waveOutGetPlaybackRate proc
			jmp QWORD ptr OriginalFuncs_winmm[166 * 8]
		waveOutGetPlaybackRate endp
		waveOutGetPosition proc
			jmp QWORD ptr OriginalFuncs_winmm[167 * 8]
		waveOutGetPosition endp
		waveOutGetVolume proc
			jmp QWORD ptr OriginalFuncs_winmm[168 * 8]
		waveOutGetVolume endp
		waveOutMessage proc
			jmp QWORD ptr OriginalFuncs_winmm[169 * 8]
		waveOutMessage endp
		waveOutOpen proc
			jmp QWORD ptr OriginalFuncs_winmm[170 * 8]
		waveOutOpen endp
		waveOutPause proc
			jmp QWORD ptr OriginalFuncs_winmm[171 * 8]
		waveOutPause endp
		waveOutPrepareHeader proc
			jmp QWORD ptr OriginalFuncs_winmm[172 * 8]
		waveOutPrepareHeader endp
		waveOutReset proc
			jmp QWORD ptr OriginalFuncs_winmm[173 * 8]
		waveOutReset endp
		waveOutRestart proc
			jmp QWORD ptr OriginalFuncs_winmm[174 * 8]
		waveOutRestart endp
		waveOutSetPitch proc
			jmp QWORD ptr OriginalFuncs_winmm[175 * 8]
		waveOutSetPitch endp
		waveOutSetPlaybackRate proc
			jmp QWORD ptr OriginalFuncs_winmm[176 * 8]
		waveOutSetPlaybackRate endp
		waveOutSetVolume proc
			jmp QWORD ptr OriginalFuncs_winmm[177 * 8]
		waveOutSetVolume endp
		waveOutUnprepareHeader proc
			jmp QWORD ptr OriginalFuncs_winmm[178 * 8]
		waveOutUnprepareHeader endp
		waveOutWrite proc
			jmp QWORD ptr OriginalFuncs_winmm[179 * 8]
		waveOutWrite endp
		ExportByOrdinal2 proc
			jmp QWORD ptr OriginalFuncs_winmm[180 * 8]
		ExportByOrdinal2 endp
else
	.model flat, C
	.stack 4096
	.code
		extern OriginalFuncs_winmm:DWORD
		CloseDriver proc
			jmp DWORD ptr OriginalFuncs_winmm[0 * 4]
		CloseDriver endp
		DefDriverProc proc
			jmp DWORD ptr OriginalFuncs_winmm[1 * 4]
		DefDriverProc endp
		DriverCallback proc
			jmp DWORD ptr OriginalFuncs_winmm[2 * 4]
		DriverCallback endp
		DrvGetModuleHandle proc
			jmp DWORD ptr OriginalFuncs_winmm[3 * 4]
		DrvGetModuleHandle endp
		GetDriverModuleHandle proc
			jmp DWORD ptr OriginalFuncs_winmm[4 * 4]
		GetDriverModuleHandle endp
		OpenDriver proc
			jmp DWORD ptr OriginalFuncs_winmm[5 * 4]
		OpenDriver endp
		PlaySound proc
			jmp DWORD ptr OriginalFuncs_winmm[6 * 4]
		PlaySound endp
		PlaySoundA proc
			jmp DWORD ptr OriginalFuncs_winmm[7 * 4]
		PlaySoundA endp
		PlaySoundW proc
			jmp DWORD ptr OriginalFuncs_winmm[8 * 4]
		PlaySoundW endp
		SendDriverMessage proc
			jmp DWORD ptr OriginalFuncs_winmm[9 * 4]
		SendDriverMessage endp
		WOWAppExit proc
			jmp DWORD ptr OriginalFuncs_winmm[10 * 4]
		WOWAppExit endp
		auxGetDevCapsA proc
			jmp DWORD ptr OriginalFuncs_winmm[11 * 4]
		auxGetDevCapsA endp
		auxGetDevCapsW proc
			jmp DWORD ptr OriginalFuncs_winmm[12 * 4]
		auxGetDevCapsW endp
		auxGetNumDevs proc
			jmp DWORD ptr OriginalFuncs_winmm[13 * 4]
		auxGetNumDevs endp
		auxGetVolume proc
			jmp DWORD ptr OriginalFuncs_winmm[14 * 4]
		auxGetVolume endp
		auxOutMessage proc
			jmp DWORD ptr OriginalFuncs_winmm[15 * 4]
		auxOutMessage endp
		auxSetVolume proc
			jmp DWORD ptr OriginalFuncs_winmm[16 * 4]
		auxSetVolume endp
		joyConfigChanged proc
			jmp DWORD ptr OriginalFuncs_winmm[17 * 4]
		joyConfigChanged endp
		joyGetDevCapsA proc
			jmp DWORD ptr OriginalFuncs_winmm[18 * 4]
		joyGetDevCapsA endp
		joyGetDevCapsW proc
			jmp DWORD ptr OriginalFuncs_winmm[19 * 4]
		joyGetDevCapsW endp
		joyGetNumDevs proc
			jmp DWORD ptr OriginalFuncs_winmm[20 * 4]
		joyGetNumDevs endp
		joyGetPos proc
			jmp DWORD ptr OriginalFuncs_winmm[21 * 4]
		joyGetPos endp
		joyGetPosEx proc
			jmp DWORD ptr OriginalFuncs_winmm[22 * 4]
		joyGetPosEx endp
		joyGetThreshold proc
			jmp DWORD ptr OriginalFuncs_winmm[23 * 4]
		joyGetThreshold endp
		joyReleaseCapture proc
			jmp DWORD ptr OriginalFuncs_winmm[24 * 4]
		joyReleaseCapture endp
		joySetCapture proc
			jmp DWORD ptr OriginalFuncs_winmm[25 * 4]
		joySetCapture endp
		joySetThreshold proc
			jmp DWORD ptr OriginalFuncs_winmm[26 * 4]
		joySetThreshold endp
		mciDriverNotify proc
			jmp DWORD ptr OriginalFuncs_winmm[27 * 4]
		mciDriverNotify endp
		mciDriverYield proc
			jmp DWORD ptr OriginalFuncs_winmm[28 * 4]
		mciDriverYield endp
		mciExecute proc
			jmp DWORD ptr OriginalFuncs_winmm[29 * 4]
		mciExecute endp
		mciFreeCommandResource proc
			jmp DWORD ptr OriginalFuncs_winmm[30 * 4]
		mciFreeCommandResource endp
		mciGetCreatorTask proc
			jmp DWORD ptr OriginalFuncs_winmm[31 * 4]
		mciGetCreatorTask endp
		mciGetDeviceIDA proc
			jmp DWORD ptr OriginalFuncs_winmm[32 * 4]
		mciGetDeviceIDA endp
		mciGetDeviceIDFromElementIDA proc
			jmp DWORD ptr OriginalFuncs_winmm[33 * 4]
		mciGetDeviceIDFromElementIDA endp
		mciGetDeviceIDFromElementIDW proc
			jmp DWORD ptr OriginalFuncs_winmm[34 * 4]
		mciGetDeviceIDFromElementIDW endp
		mciGetDeviceIDW proc
			jmp DWORD ptr OriginalFuncs_winmm[35 * 4]
		mciGetDeviceIDW endp
		mciGetDriverData proc
			jmp DWORD ptr OriginalFuncs_winmm[36 * 4]
		mciGetDriverData endp
		mciGetErrorStringA proc
			jmp DWORD ptr OriginalFuncs_winmm[37 * 4]
		mciGetErrorStringA endp
		mciGetErrorStringW proc
			jmp DWORD ptr OriginalFuncs_winmm[38 * 4]
		mciGetErrorStringW endp
		mciGetYieldProc proc
			jmp DWORD ptr OriginalFuncs_winmm[39 * 4]
		mciGetYieldProc endp
		mciLoadCommandResource proc
			jmp DWORD ptr OriginalFuncs_winmm[40 * 4]
		mciLoadCommandResource endp
		mciSendCommandA proc
			jmp DWORD ptr OriginalFuncs_winmm[41 * 4]
		mciSendCommandA endp
		mciSendCommandW proc
			jmp DWORD ptr OriginalFuncs_winmm[42 * 4]
		mciSendCommandW endp
		mciSendStringA proc
			jmp DWORD ptr OriginalFuncs_winmm[43 * 4]
		mciSendStringA endp
		mciSendStringW proc
			jmp DWORD ptr OriginalFuncs_winmm[44 * 4]
		mciSendStringW endp
		mciSetDriverData proc
			jmp DWORD ptr OriginalFuncs_winmm[45 * 4]
		mciSetDriverData endp
		mciSetYieldProc proc
			jmp DWORD ptr OriginalFuncs_winmm[46 * 4]
		mciSetYieldProc endp
		midiConnect proc
			jmp DWORD ptr OriginalFuncs_winmm[47 * 4]
		midiConnect endp
		midiDisconnect proc
			jmp DWORD ptr OriginalFuncs_winmm[48 * 4]
		midiDisconnect endp
		midiInAddBuffer proc
			jmp DWORD ptr OriginalFuncs_winmm[49 * 4]
		midiInAddBuffer endp
		midiInClose proc
			jmp DWORD ptr OriginalFuncs_winmm[50 * 4]
		midiInClose endp
		midiInGetDevCapsA proc
			jmp DWORD ptr OriginalFuncs_winmm[51 * 4]
		midiInGetDevCapsA endp
		midiInGetDevCapsW proc
			jmp DWORD ptr OriginalFuncs_winmm[52 * 4]
		midiInGetDevCapsW endp
		midiInGetErrorTextA proc
			jmp DWORD ptr OriginalFuncs_winmm[53 * 4]
		midiInGetErrorTextA endp
		midiInGetErrorTextW proc
			jmp DWORD ptr OriginalFuncs_winmm[54 * 4]
		midiInGetErrorTextW endp
		midiInGetID proc
			jmp DWORD ptr OriginalFuncs_winmm[55 * 4]
		midiInGetID endp
		midiInGetNumDevs proc
			jmp DWORD ptr OriginalFuncs_winmm[56 * 4]
		midiInGetNumDevs endp
		midiInMessage proc
			jmp DWORD ptr OriginalFuncs_winmm[57 * 4]
		midiInMessage endp
		midiInOpen proc
			jmp DWORD ptr OriginalFuncs_winmm[58 * 4]
		midiInOpen endp
		midiInPrepareHeader proc
			jmp DWORD ptr OriginalFuncs_winmm[59 * 4]
		midiInPrepareHeader endp
		midiInReset proc
			jmp DWORD ptr OriginalFuncs_winmm[60 * 4]
		midiInReset endp
		midiInStart proc
			jmp DWORD ptr OriginalFuncs_winmm[61 * 4]
		midiInStart endp
		midiInStop proc
			jmp DWORD ptr OriginalFuncs_winmm[62 * 4]
		midiInStop endp
		midiInUnprepareHeader proc
			jmp DWORD ptr OriginalFuncs_winmm[63 * 4]
		midiInUnprepareHeader endp
		midiOutCacheDrumPatches proc
			jmp DWORD ptr OriginalFuncs_winmm[64 * 4]
		midiOutCacheDrumPatches endp
		midiOutCachePatches proc
			jmp DWORD ptr OriginalFuncs_winmm[65 * 4]
		midiOutCachePatches endp
		midiOutClose proc
			jmp DWORD ptr OriginalFuncs_winmm[66 * 4]
		midiOutClose endp
		midiOutGetDevCapsA proc
			jmp DWORD ptr OriginalFuncs_winmm[67 * 4]
		midiOutGetDevCapsA endp
		midiOutGetDevCapsW proc
			jmp DWORD ptr OriginalFuncs_winmm[68 * 4]
		midiOutGetDevCapsW endp
		midiOutGetErrorTextA proc
			jmp DWORD ptr OriginalFuncs_winmm[69 * 4]
		midiOutGetErrorTextA endp
		midiOutGetErrorTextW proc
			jmp DWORD ptr OriginalFuncs_winmm[70 * 4]
		midiOutGetErrorTextW endp
		midiOutGetID proc
			jmp DWORD ptr OriginalFuncs_winmm[71 * 4]
		midiOutGetID endp
		midiOutGetNumDevs proc
			jmp DWORD ptr OriginalFuncs_winmm[72 * 4]
		midiOutGetNumDevs endp
		midiOutGetVolume proc
			jmp DWORD ptr OriginalFuncs_winmm[73 * 4]
		midiOutGetVolume endp
		midiOutLongMsg proc
			jmp DWORD ptr OriginalFuncs_winmm[74 * 4]
		midiOutLongMsg endp
		midiOutMessage proc
			jmp DWORD ptr OriginalFuncs_winmm[75 * 4]
		midiOutMessage endp
		midiOutOpen proc
			jmp DWORD ptr OriginalFuncs_winmm[76 * 4]
		midiOutOpen endp
		midiOutPrepareHeader proc
			jmp DWORD ptr OriginalFuncs_winmm[77 * 4]
		midiOutPrepareHeader endp
		midiOutReset proc
			jmp DWORD ptr OriginalFuncs_winmm[78 * 4]
		midiOutReset endp
		midiOutSetVolume proc
			jmp DWORD ptr OriginalFuncs_winmm[79 * 4]
		midiOutSetVolume endp
		midiOutShortMsg proc
			jmp DWORD ptr OriginalFuncs_winmm[80 * 4]
		midiOutShortMsg endp
		midiOutUnprepareHeader proc
			jmp DWORD ptr OriginalFuncs_winmm[81 * 4]
		midiOutUnprepareHeader endp
		midiStreamClose proc
			jmp DWORD ptr OriginalFuncs_winmm[82 * 4]
		midiStreamClose endp
		midiStreamOpen proc
			jmp DWORD ptr OriginalFuncs_winmm[83 * 4]
		midiStreamOpen endp
		midiStreamOut proc
			jmp DWORD ptr OriginalFuncs_winmm[84 * 4]
		midiStreamOut endp
		midiStreamPause proc
			jmp DWORD ptr OriginalFuncs_winmm[85 * 4]
		midiStreamPause endp
		midiStreamPosition proc
			jmp DWORD ptr OriginalFuncs_winmm[86 * 4]
		midiStreamPosition endp
		midiStreamProperty proc
			jmp DWORD ptr OriginalFuncs_winmm[87 * 4]
		midiStreamProperty endp
		midiStreamRestart proc
			jmp DWORD ptr OriginalFuncs_winmm[88 * 4]
		midiStreamRestart endp
		midiStreamStop proc
			jmp DWORD ptr OriginalFuncs_winmm[89 * 4]
		midiStreamStop endp
		mixerClose proc
			jmp DWORD ptr OriginalFuncs_winmm[90 * 4]
		mixerClose endp
		mixerGetControlDetailsA proc
			jmp DWORD ptr OriginalFuncs_winmm[91 * 4]
		mixerGetControlDetailsA endp
		mixerGetControlDetailsW proc
			jmp DWORD ptr OriginalFuncs_winmm[92 * 4]
		mixerGetControlDetailsW endp
		mixerGetDevCapsA proc
			jmp DWORD ptr OriginalFuncs_winmm[93 * 4]
		mixerGetDevCapsA endp
		mixerGetDevCapsW proc
			jmp DWORD ptr OriginalFuncs_winmm[94 * 4]
		mixerGetDevCapsW endp
		mixerGetID proc
			jmp DWORD ptr OriginalFuncs_winmm[95 * 4]
		mixerGetID endp
		mixerGetLineControlsA proc
			jmp DWORD ptr OriginalFuncs_winmm[96 * 4]
		mixerGetLineControlsA endp
		mixerGetLineControlsW proc
			jmp DWORD ptr OriginalFuncs_winmm[97 * 4]
		mixerGetLineControlsW endp
		mixerGetLineInfoA proc
			jmp DWORD ptr OriginalFuncs_winmm[98 * 4]
		mixerGetLineInfoA endp
		mixerGetLineInfoW proc
			jmp DWORD ptr OriginalFuncs_winmm[99 * 4]
		mixerGetLineInfoW endp
		mixerGetNumDevs proc
			jmp DWORD ptr OriginalFuncs_winmm[100 * 4]
		mixerGetNumDevs endp
		mixerMessage proc
			jmp DWORD ptr OriginalFuncs_winmm[101 * 4]
		mixerMessage endp
		mixerOpen proc
			jmp DWORD ptr OriginalFuncs_winmm[102 * 4]
		mixerOpen endp
		mixerSetControlDetails proc
			jmp DWORD ptr OriginalFuncs_winmm[103 * 4]
		mixerSetControlDetails endp
		mmDrvInstall proc
			jmp DWORD ptr OriginalFuncs_winmm[104 * 4]
		mmDrvInstall endp
		mmGetCurrentTask proc
			jmp DWORD ptr OriginalFuncs_winmm[105 * 4]
		mmGetCurrentTask endp
		mmTaskBlock proc
			jmp DWORD ptr OriginalFuncs_winmm[106 * 4]
		mmTaskBlock endp
		mmTaskCreate proc
			jmp DWORD ptr OriginalFuncs_winmm[107 * 4]
		mmTaskCreate endp
		mmTaskSignal proc
			jmp DWORD ptr OriginalFuncs_winmm[108 * 4]
		mmTaskSignal endp
		mmTaskYield proc
			jmp DWORD ptr OriginalFuncs_winmm[109 * 4]
		mmTaskYield endp
		mmioAdvance proc
			jmp DWORD ptr OriginalFuncs_winmm[110 * 4]
		mmioAdvance endp
		mmioAscend proc
			jmp DWORD ptr OriginalFuncs_winmm[111 * 4]
		mmioAscend endp
		mmioClose proc
			jmp DWORD ptr OriginalFuncs_winmm[112 * 4]
		mmioClose endp
		mmioCreateChunk proc
			jmp DWORD ptr OriginalFuncs_winmm[113 * 4]
		mmioCreateChunk endp
		mmioDescend proc
			jmp DWORD ptr OriginalFuncs_winmm[114 * 4]
		mmioDescend endp
		mmioFlush proc
			jmp DWORD ptr OriginalFuncs_winmm[115 * 4]
		mmioFlush endp
		mmioGetInfo proc
			jmp DWORD ptr OriginalFuncs_winmm[116 * 4]
		mmioGetInfo endp
		mmioInstallIOProcA proc
			jmp DWORD ptr OriginalFuncs_winmm[117 * 4]
		mmioInstallIOProcA endp
		mmioInstallIOProcW proc
			jmp DWORD ptr OriginalFuncs_winmm[118 * 4]
		mmioInstallIOProcW endp
		mmioOpenA proc
			jmp DWORD ptr OriginalFuncs_winmm[119 * 4]
		mmioOpenA endp
		mmioOpenW proc
			jmp DWORD ptr OriginalFuncs_winmm[120 * 4]
		mmioOpenW endp
		mmioRead proc
			jmp DWORD ptr OriginalFuncs_winmm[121 * 4]
		mmioRead endp
		mmioRenameA proc
			jmp DWORD ptr OriginalFuncs_winmm[122 * 4]
		mmioRenameA endp
		mmioRenameW proc
			jmp DWORD ptr OriginalFuncs_winmm[123 * 4]
		mmioRenameW endp
		mmioSeek proc
			jmp DWORD ptr OriginalFuncs_winmm[124 * 4]
		mmioSeek endp
		mmioSendMessage proc
			jmp DWORD ptr OriginalFuncs_winmm[125 * 4]
		mmioSendMessage endp
		mmioSetBuffer proc
			jmp DWORD ptr OriginalFuncs_winmm[126 * 4]
		mmioSetBuffer endp
		mmioSetInfo proc
			jmp DWORD ptr OriginalFuncs_winmm[127 * 4]
		mmioSetInfo endp
		mmioStringToFOURCCA proc
			jmp DWORD ptr OriginalFuncs_winmm[128 * 4]
		mmioStringToFOURCCA endp
		mmioStringToFOURCCW proc
			jmp DWORD ptr OriginalFuncs_winmm[129 * 4]
		mmioStringToFOURCCW endp
		mmioWrite proc
			jmp DWORD ptr OriginalFuncs_winmm[130 * 4]
		mmioWrite endp
		mmsystemGetVersion proc
			jmp DWORD ptr OriginalFuncs_winmm[131 * 4]
		mmsystemGetVersion endp
		sndPlaySoundA proc
			jmp DWORD ptr OriginalFuncs_winmm[132 * 4]
		sndPlaySoundA endp
		sndPlaySoundW proc
			jmp DWORD ptr OriginalFuncs_winmm[133 * 4]
		sndPlaySoundW endp
		timeBeginPeriod proc
			jmp DWORD ptr OriginalFuncs_winmm[134 * 4]
		timeBeginPeriod endp
		timeEndPeriod proc
			jmp DWORD ptr OriginalFuncs_winmm[135 * 4]
		timeEndPeriod endp
		timeGetDevCaps proc
			jmp DWORD ptr OriginalFuncs_winmm[136 * 4]
		timeGetDevCaps endp
		timeGetSystemTime proc
			jmp DWORD ptr OriginalFuncs_winmm[137 * 4]
		timeGetSystemTime endp
		timeGetTime proc
			jmp DWORD ptr OriginalFuncs_winmm[138 * 4]
		timeGetTime endp
		timeKillEvent proc
			jmp DWORD ptr OriginalFuncs_winmm[139 * 4]
		timeKillEvent endp
		timeSetEvent proc
			jmp DWORD ptr OriginalFuncs_winmm[140 * 4]
		timeSetEvent endp
		waveInAddBuffer proc
			jmp DWORD ptr OriginalFuncs_winmm[141 * 4]
		waveInAddBuffer endp
		waveInClose proc
			jmp DWORD ptr OriginalFuncs_winmm[142 * 4]
		waveInClose endp
		waveInGetDevCapsA proc
			jmp DWORD ptr OriginalFuncs_winmm[143 * 4]
		waveInGetDevCapsA endp
		waveInGetDevCapsW proc
			jmp DWORD ptr OriginalFuncs_winmm[144 * 4]
		waveInGetDevCapsW endp
		waveInGetErrorTextA proc
			jmp DWORD ptr OriginalFuncs_winmm[145 * 4]
		waveInGetErrorTextA endp
		waveInGetErrorTextW proc
			jmp DWORD ptr OriginalFuncs_winmm[146 * 4]
		waveInGetErrorTextW endp
		waveInGetID proc
			jmp DWORD ptr OriginalFuncs_winmm[147 * 4]
		waveInGetID endp
		waveInGetNumDevs proc
			jmp DWORD ptr OriginalFuncs_winmm[148 * 4]
		waveInGetNumDevs endp
		waveInGetPosition proc
			jmp DWORD ptr OriginalFuncs_winmm[149 * 4]
		waveInGetPosition endp
		waveInMessage proc
			jmp DWORD ptr OriginalFuncs_winmm[150 * 4]
		waveInMessage endp
		waveInOpen proc
			jmp DWORD ptr OriginalFuncs_winmm[151 * 4]
		waveInOpen endp
		waveInPrepareHeader proc
			jmp DWORD ptr OriginalFuncs_winmm[152 * 4]
		waveInPrepareHeader endp
		waveInReset proc
			jmp DWORD ptr OriginalFuncs_winmm[153 * 4]
		waveInReset endp
		waveInStart proc
			jmp DWORD ptr OriginalFuncs_winmm[154 * 4]
		waveInStart endp
		waveInStop proc
			jmp DWORD ptr OriginalFuncs_winmm[155 * 4]
		waveInStop endp
		waveInUnprepareHeader proc
			jmp DWORD ptr OriginalFuncs_winmm[156 * 4]
		waveInUnprepareHeader endp
		waveOutBreakLoop proc
			jmp DWORD ptr OriginalFuncs_winmm[157 * 4]
		waveOutBreakLoop endp
		waveOutClose proc
			jmp DWORD ptr OriginalFuncs_winmm[158 * 4]
		waveOutClose endp
		waveOutGetDevCapsA proc
			jmp DWORD ptr OriginalFuncs_winmm[159 * 4]
		waveOutGetDevCapsA endp
		waveOutGetDevCapsW proc
			jmp DWORD ptr OriginalFuncs_winmm[160 * 4]
		waveOutGetDevCapsW endp
		waveOutGetErrorTextA proc
			jmp DWORD ptr OriginalFuncs_winmm[161 * 4]
		waveOutGetErrorTextA endp
		waveOutGetErrorTextW proc
			jmp DWORD ptr OriginalFuncs_winmm[162 * 4]
		waveOutGetErrorTextW endp
		waveOutGetID proc
			jmp DWORD ptr OriginalFuncs_winmm[163 * 4]
		waveOutGetID endp
		waveOutGetNumDevs proc
			jmp DWORD ptr OriginalFuncs_winmm[164 * 4]
		waveOutGetNumDevs endp
		waveOutGetPitch proc
			jmp DWORD ptr OriginalFuncs_winmm[165 * 4]
		waveOutGetPitch endp
		waveOutGetPlaybackRate proc
			jmp DWORD ptr OriginalFuncs_winmm[166 * 4]
		waveOutGetPlaybackRate endp
		waveOutGetPosition proc
			jmp DWORD ptr OriginalFuncs_winmm[167 * 4]
		waveOutGetPosition endp
		waveOutGetVolume proc
			jmp DWORD ptr OriginalFuncs_winmm[168 * 4]
		waveOutGetVolume endp
		waveOutMessage proc
			jmp DWORD ptr OriginalFuncs_winmm[169 * 4]
		waveOutMessage endp
		waveOutOpen proc
			jmp DWORD ptr OriginalFuncs_winmm[170 * 4]
		waveOutOpen endp
		waveOutPause proc
			jmp DWORD ptr OriginalFuncs_winmm[171 * 4]
		waveOutPause endp
		waveOutPrepareHeader proc
			jmp DWORD ptr OriginalFuncs_winmm[172 * 4]
		waveOutPrepareHeader endp
		waveOutReset proc
			jmp DWORD ptr OriginalFuncs_winmm[173 * 4]
		waveOutReset endp
		waveOutRestart proc
			jmp DWORD ptr OriginalFuncs_winmm[174 * 4]
		waveOutRestart endp
		waveOutSetPitch proc
			jmp DWORD ptr OriginalFuncs_winmm[175 * 4]
		waveOutSetPitch endp
		waveOutSetPlaybackRate proc
			jmp DWORD ptr OriginalFuncs_winmm[176 * 4]
		waveOutSetPlaybackRate endp
		waveOutSetVolume proc
			jmp DWORD ptr OriginalFuncs_winmm[177 * 4]
		waveOutSetVolume endp
		waveOutUnprepareHeader proc
			jmp DWORD ptr OriginalFuncs_winmm[178 * 4]
		waveOutUnprepareHeader endp
		waveOutWrite proc
			jmp DWORD ptr OriginalFuncs_winmm[179 * 4]
		waveOutWrite endp
		ExportByOrdinal2 proc
			jmp DWORD ptr OriginalFuncs_winmm[180 * 4]
		ExportByOrdinal2 endp
endif
end