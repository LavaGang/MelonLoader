.globl CloseDriver
.globl DefDriverProc
.globl DriverCallback
.globl DrvGetModuleHandle
.globl GetDriverModuleHandle
.globl OpenDriver
.globl PlaySound
.globl PlaySoundA
.globl PlaySoundW
.globl SendDriverMessage
.globl WOWAppExit
.globl auxGetDevCapsA
.globl auxGetDevCapsW
.globl auxGetNumDevs
.globl auxGetVolume
.globl auxOutMessage
.globl auxSetVolume
.globl joyConfigChanged
.globl joyGetDevCapsA
.globl joyGetDevCapsW
.globl joyGetNumDevs
.globl joyGetPos
.globl joyGetPosEx
.globl joyGetThreshold
.globl joyReleaseCapture
.globl joySetCapture
.globl joySetThreshold
.globl mciDriverNotify
.globl mciDriverYield
.globl mciExecute
.globl mciFreeCommandResource
.globl mciGetCreatorTask
.globl mciGetDeviceIDA
.globl mciGetDeviceIDFromElementIDA
.globl mciGetDeviceIDFromElementIDW
.globl mciGetDeviceIDW
.globl mciGetDriverData
.globl mciGetErrorStringA
.globl mciGetErrorStringW
.globl mciGetYieldProc
.globl mciLoadCommandResource
.globl mciSendCommandA
.globl mciSendCommandW
.globl mciSendStringA
.globl mciSendStringW
.globl mciSetDriverData
.globl mciSetYieldProc
.globl midiConnect
.globl midiDisconnect
.globl midiInAddBuffer
.globl midiInClose
.globl midiInGetDevCapsA
.globl midiInGetDevCapsW
.globl midiInGetErrorTextA
.globl midiInGetErrorTextW
.globl midiInGetID
.globl midiInGetNumDevs
.globl midiInMessage
.globl midiInOpen
.globl midiInPrepareHeader
.globl midiInReset
.globl midiInStart
.globl midiInStop
.globl midiInUnprepareHeader
.globl midiOutCacheDrumPatches
.globl midiOutCachePatches
.globl midiOutClose
.globl midiOutGetDevCapsA
.globl midiOutGetDevCapsW
.globl midiOutGetErrorTextA
.globl midiOutGetErrorTextW
.globl midiOutGetID
.globl midiOutGetNumDevs
.globl midiOutGetVolume
.globl midiOutLongMsg
.globl midiOutMessage
.globl midiOutOpen
.globl midiOutPrepareHeader
.globl midiOutReset
.globl midiOutSetVolume
.globl midiOutShortMsg
.globl midiOutUnprepareHeader
.globl midiStreamClose
.globl midiStreamOpen
.globl midiStreamOut
.globl midiStreamPause
.globl midiStreamPosition
.globl midiStreamProperty
.globl midiStreamRestart
.globl midiStreamStop
.globl mixerClose
.globl mixerGetControlDetailsA
.globl mixerGetControlDetailsW
.globl mixerGetDevCapsA
.globl mixerGetDevCapsW
.globl mixerGetID
.globl mixerGetLineControlsA
.globl mixerGetLineControlsW
.globl mixerGetLineInfoA
.globl mixerGetLineInfoW
.globl mixerGetNumDevs
.globl mixerMessage
.globl mixerOpen
.globl mixerSetControlDetails
.globl mmDrvInstall
.globl mmGetCurrentTask
.globl mmTaskBlock
.globl mmTaskCreate
.globl mmTaskSignal
.globl mmTaskYield
.globl mmioAdvance
.globl mmioAscend
.globl mmioClose
.globl mmioCreateChunk
.globl mmioDescend
.globl mmioFlush
.globl mmioGetInfo
.globl mmioInstallIOProcA
.globl mmioInstallIOProcW
.globl mmioOpenA
.globl mmioOpenW
.globl mmioRead
.globl mmioRenameA
.globl mmioRenameW
.globl mmioSeek
.globl mmioSendMessage
.globl mmioSetBuffer
.globl mmioSetInfo
.globl mmioStringToFOURCCA
.globl mmioStringToFOURCCW
.globl mmioWrite
.globl mmsystemGetVersion
.globl sndPlaySoundA
.globl sndPlaySoundW
.globl timeBeginPeriod
.globl timeEndPeriod
.globl timeGetDevCaps
.globl timeGetSystemTime
.globl timeGetTime
.globl timeKillEvent
.globl timeSetEvent
.globl waveInAddBuffer
.globl waveInClose
.globl waveInGetDevCapsA
.globl waveInGetDevCapsW
.globl waveInGetErrorTextA
.globl waveInGetErrorTextW
.globl waveInGetID
.globl waveInGetNumDevs
.globl waveInGetPosition
.globl waveInMessage
.globl waveInOpen
.globl waveInPrepareHeader
.globl waveInReset
.globl waveInStart
.globl waveInStop
.globl waveInUnprepareHeader
.globl waveOutBreakLoop
.globl waveOutClose
.globl waveOutGetDevCapsA
.globl waveOutGetDevCapsW
.globl waveOutGetErrorTextA
.globl waveOutGetErrorTextW
.globl waveOutGetID
.globl waveOutGetNumDevs
.globl waveOutGetPitch
.globl waveOutGetPlaybackRate
.globl waveOutGetPosition
.globl waveOutGetVolume
.globl waveOutMessage
.globl waveOutOpen
.globl waveOutPause
.globl waveOutPrepareHeader
.globl waveOutReset
.globl waveOutRestart
.globl waveOutSetPitch
.globl waveOutSetPlaybackRate
.globl waveOutSetVolume
.globl waveOutUnprepareHeader
.globl waveOutWrite
.globl ExportByOrdinal2

CloseDriver:
	jmp qword ptr [rip + OriginalFuncs_winmm + 0 * 8]

DefDriverProc:
	jmp qword ptr [rip + OriginalFuncs_winmm + 1 * 8]

DriverCallback:
	jmp qword ptr [rip + OriginalFuncs_winmm + 2 * 8]

DrvGetModuleHandle:
	jmp qword ptr [rip + OriginalFuncs_winmm + 3 * 8]

GetDriverModuleHandle:
	jmp qword ptr [rip + OriginalFuncs_winmm + 4 * 8]

OpenDriver:
	jmp qword ptr [rip + OriginalFuncs_winmm + 5 * 8]

PlaySound:
	jmp qword ptr [rip + OriginalFuncs_winmm + 6 * 8]

PlaySoundA:
	jmp qword ptr [rip + OriginalFuncs_winmm + 7 * 8]

PlaySoundW:
	jmp qword ptr [rip + OriginalFuncs_winmm + 8 * 8]

SendDriverMessage:
	jmp qword ptr [rip + OriginalFuncs_winmm + 9 * 8]

WOWAppExit:
	jmp qword ptr [rip + OriginalFuncs_winmm + 10 * 8]

auxGetDevCapsA:
	jmp qword ptr [rip + OriginalFuncs_winmm + 11 * 8]

auxGetDevCapsW:
	jmp qword ptr [rip + OriginalFuncs_winmm + 12 * 8]

auxGetNumDevs:
	jmp qword ptr [rip + OriginalFuncs_winmm + 13 * 8]

auxGetVolume:
	jmp qword ptr [rip + OriginalFuncs_winmm + 14 * 8]

auxOutMessage:
	jmp qword ptr [rip + OriginalFuncs_winmm + 15 * 8]

auxSetVolume:
	jmp qword ptr [rip + OriginalFuncs_winmm + 16 * 8]

joyConfigChanged:
	jmp qword ptr [rip + OriginalFuncs_winmm + 17 * 8]

joyGetDevCapsA:
	jmp qword ptr [rip + OriginalFuncs_winmm + 18 * 8]

joyGetDevCapsW:
	jmp qword ptr [rip + OriginalFuncs_winmm + 19 * 8]

joyGetNumDevs:
	jmp qword ptr [rip + OriginalFuncs_winmm + 20 * 8]

joyGetPos:
	jmp qword ptr [rip + OriginalFuncs_winmm + 21 * 8]

joyGetPosEx:
	jmp qword ptr [rip + OriginalFuncs_winmm + 22 * 8]

joyGetThreshold:
	jmp qword ptr [rip + OriginalFuncs_winmm + 23 * 8]

joyReleaseCapture:
	jmp qword ptr [rip + OriginalFuncs_winmm + 24 * 8]

joySetCapture:
	jmp qword ptr [rip + OriginalFuncs_winmm + 25 * 8]

joySetThreshold:
	jmp qword ptr [rip + OriginalFuncs_winmm + 26 * 8]

mciDriverNotify:
	jmp qword ptr [rip + OriginalFuncs_winmm + 27 * 8]

mciDriverYield:
	jmp qword ptr [rip + OriginalFuncs_winmm + 28 * 8]

mciExecute:
	jmp qword ptr [rip + OriginalFuncs_winmm + 29 * 8]

mciFreeCommandResource:
	jmp qword ptr [rip + OriginalFuncs_winmm + 30 * 8]

mciGetCreatorTask:
	jmp qword ptr [rip + OriginalFuncs_winmm + 31 * 8]

mciGetDeviceIDA:
	jmp qword ptr [rip + OriginalFuncs_winmm + 32 * 8]

mciGetDeviceIDFromElementIDA:
	jmp qword ptr [rip + OriginalFuncs_winmm + 33 * 8]

mciGetDeviceIDFromElementIDW:
	jmp qword ptr [rip + OriginalFuncs_winmm + 34 * 8]

mciGetDeviceIDW:
	jmp qword ptr [rip + OriginalFuncs_winmm + 35 * 8]

mciGetDriverData:
	jmp qword ptr [rip + OriginalFuncs_winmm + 36 * 8]

mciGetErrorStringA:
	jmp qword ptr [rip + OriginalFuncs_winmm + 37 * 8]

mciGetErrorStringW:
	jmp qword ptr [rip + OriginalFuncs_winmm + 38 * 8]

mciGetYieldProc:
	jmp qword ptr [rip + OriginalFuncs_winmm + 39 * 8]

mciLoadCommandResource:
	jmp qword ptr [rip + OriginalFuncs_winmm + 40 * 8]

mciSendCommandA:
	jmp qword ptr [rip + OriginalFuncs_winmm + 41 * 8]

mciSendCommandW:
	jmp qword ptr [rip + OriginalFuncs_winmm + 42 * 8]

mciSendStringA:
	jmp qword ptr [rip + OriginalFuncs_winmm + 43 * 8]

mciSendStringW:
	jmp qword ptr [rip + OriginalFuncs_winmm + 44 * 8]

mciSetDriverData:
	jmp qword ptr [rip + OriginalFuncs_winmm + 45 * 8]

mciSetYieldProc:
	jmp qword ptr [rip + OriginalFuncs_winmm + 46 * 8]

midiConnect:
	jmp qword ptr [rip + OriginalFuncs_winmm + 47 * 8]

midiDisconnect:
	jmp qword ptr [rip + OriginalFuncs_winmm + 48 * 8]

midiInAddBuffer:
	jmp qword ptr [rip + OriginalFuncs_winmm + 49 * 8]

midiInClose:
	jmp qword ptr [rip + OriginalFuncs_winmm + 50 * 8]

midiInGetDevCapsA:
	jmp qword ptr [rip + OriginalFuncs_winmm + 51 * 8]

midiInGetDevCapsW:
	jmp qword ptr [rip + OriginalFuncs_winmm + 52 * 8]

midiInGetErrorTextA:
	jmp qword ptr [rip + OriginalFuncs_winmm + 53 * 8]

midiInGetErrorTextW:
	jmp qword ptr [rip + OriginalFuncs_winmm + 54 * 8]

midiInGetID:
	jmp qword ptr [rip + OriginalFuncs_winmm + 55 * 8]

midiInGetNumDevs:
	jmp qword ptr [rip + OriginalFuncs_winmm + 56 * 8]

midiInMessage:
	jmp qword ptr [rip + OriginalFuncs_winmm + 57 * 8]

midiInOpen:
	jmp qword ptr [rip + OriginalFuncs_winmm + 58 * 8]

midiInPrepareHeader:
	jmp qword ptr [rip + OriginalFuncs_winmm + 59 * 8]

midiInReset:
	jmp qword ptr [rip + OriginalFuncs_winmm + 60 * 8]

midiInStart:
	jmp qword ptr [rip + OriginalFuncs_winmm + 61 * 8]

midiInStop:
	jmp qword ptr [rip + OriginalFuncs_winmm + 62 * 8]

midiInUnprepareHeader:
	jmp qword ptr [rip + OriginalFuncs_winmm + 63 * 8]

midiOutCacheDrumPatches:
	jmp qword ptr [rip + OriginalFuncs_winmm + 64 * 8]

midiOutCachePatches:
	jmp qword ptr [rip + OriginalFuncs_winmm + 65 * 8]

midiOutClose:
	jmp qword ptr [rip + OriginalFuncs_winmm + 66 * 8]

midiOutGetDevCapsA:
	jmp qword ptr [rip + OriginalFuncs_winmm + 67 * 8]

midiOutGetDevCapsW:
	jmp qword ptr [rip + OriginalFuncs_winmm + 68 * 8]

midiOutGetErrorTextA:
	jmp qword ptr [rip + OriginalFuncs_winmm + 69 * 8]

midiOutGetErrorTextW:
	jmp qword ptr [rip + OriginalFuncs_winmm + 70 * 8]

midiOutGetID:
	jmp qword ptr [rip + OriginalFuncs_winmm + 71 * 8]

midiOutGetNumDevs:
	jmp qword ptr [rip + OriginalFuncs_winmm + 72 * 8]

midiOutGetVolume:
	jmp qword ptr [rip + OriginalFuncs_winmm + 73 * 8]

midiOutLongMsg:
	jmp qword ptr [rip + OriginalFuncs_winmm + 74 * 8]

midiOutMessage:
	jmp qword ptr [rip + OriginalFuncs_winmm + 75 * 8]

midiOutOpen:
	jmp qword ptr [rip + OriginalFuncs_winmm + 76 * 8]

midiOutPrepareHeader:
	jmp qword ptr [rip + OriginalFuncs_winmm + 77 * 8]

midiOutReset:
	jmp qword ptr [rip + OriginalFuncs_winmm + 78 * 8]

midiOutSetVolume:
	jmp qword ptr [rip + OriginalFuncs_winmm + 79 * 8]

midiOutShortMsg:
	jmp qword ptr [rip + OriginalFuncs_winmm + 80 * 8]

midiOutUnprepareHeader:
	jmp qword ptr [rip + OriginalFuncs_winmm + 81 * 8]

midiStreamClose:
	jmp qword ptr [rip + OriginalFuncs_winmm + 82 * 8]

midiStreamOpen:
	jmp qword ptr [rip + OriginalFuncs_winmm + 83 * 8]

midiStreamOut:
	jmp qword ptr [rip + OriginalFuncs_winmm + 84 * 8]

midiStreamPause:
	jmp qword ptr [rip + OriginalFuncs_winmm + 85 * 8]

midiStreamPosition:
	jmp qword ptr [rip + OriginalFuncs_winmm + 86 * 8]

midiStreamProperty:
	jmp qword ptr [rip + OriginalFuncs_winmm + 87 * 8]

midiStreamRestart:
	jmp qword ptr [rip + OriginalFuncs_winmm + 88 * 8]

midiStreamStop:
	jmp qword ptr [rip + OriginalFuncs_winmm + 89 * 8]

mixerClose:
	jmp qword ptr [rip + OriginalFuncs_winmm + 90 * 8]

mixerGetControlDetailsA:
	jmp qword ptr [rip + OriginalFuncs_winmm + 91 * 8]

mixerGetControlDetailsW:
	jmp qword ptr [rip + OriginalFuncs_winmm + 92 * 8]

mixerGetDevCapsA:
	jmp qword ptr [rip + OriginalFuncs_winmm + 93 * 8]

mixerGetDevCapsW:
	jmp qword ptr [rip + OriginalFuncs_winmm + 94 * 8]

mixerGetID:
	jmp qword ptr [rip + OriginalFuncs_winmm + 95 * 8]

mixerGetLineControlsA:
	jmp qword ptr [rip + OriginalFuncs_winmm + 96 * 8]

mixerGetLineControlsW:
	jmp qword ptr [rip + OriginalFuncs_winmm + 97 * 8]

mixerGetLineInfoA:
	jmp qword ptr [rip + OriginalFuncs_winmm + 98 * 8]

mixerGetLineInfoW:
	jmp qword ptr [rip + OriginalFuncs_winmm + 99 * 8]

mixerGetNumDevs:
	jmp qword ptr [rip + OriginalFuncs_winmm + 100 * 8]

mixerMessage:
	jmp qword ptr [rip + OriginalFuncs_winmm + 101 * 8]

mixerOpen:
	jmp qword ptr [rip + OriginalFuncs_winmm + 102 * 8]

mixerSetControlDetails:
	jmp qword ptr [rip + OriginalFuncs_winmm + 103 * 8]

mmDrvInstall:
	jmp qword ptr [rip + OriginalFuncs_winmm + 104 * 8]

mmGetCurrentTask:
	jmp qword ptr [rip + OriginalFuncs_winmm + 105 * 8]

mmTaskBlock:
	jmp qword ptr [rip + OriginalFuncs_winmm + 106 * 8]

mmTaskCreate:
	jmp qword ptr [rip + OriginalFuncs_winmm + 107 * 8]

mmTaskSignal:
	jmp qword ptr [rip + OriginalFuncs_winmm + 108 * 8]

mmTaskYield:
	jmp qword ptr [rip + OriginalFuncs_winmm + 109 * 8]

mmioAdvance:
	jmp qword ptr [rip + OriginalFuncs_winmm + 110 * 8]

mmioAscend:
	jmp qword ptr [rip + OriginalFuncs_winmm + 111 * 8]

mmioClose:
	jmp qword ptr [rip + OriginalFuncs_winmm + 112 * 8]

mmioCreateChunk:
	jmp qword ptr [rip + OriginalFuncs_winmm + 113 * 8]

mmioDescend:
	jmp qword ptr [rip + OriginalFuncs_winmm + 114 * 8]

mmioFlush:
	jmp qword ptr [rip + OriginalFuncs_winmm + 115 * 8]

mmioGetInfo:
	jmp qword ptr [rip + OriginalFuncs_winmm + 116 * 8]

mmioInstallIOProcA:
	jmp qword ptr [rip + OriginalFuncs_winmm + 117 * 8]

mmioInstallIOProcW:
	jmp qword ptr [rip + OriginalFuncs_winmm + 118 * 8]

mmioOpenA:
	jmp qword ptr [rip + OriginalFuncs_winmm + 119 * 8]

mmioOpenW:
	jmp qword ptr [rip + OriginalFuncs_winmm + 120 * 8]

mmioRead:
	jmp qword ptr [rip + OriginalFuncs_winmm + 121 * 8]

mmioRenameA:
	jmp qword ptr [rip + OriginalFuncs_winmm + 122 * 8]

mmioRenameW:
	jmp qword ptr [rip + OriginalFuncs_winmm + 123 * 8]

mmioSeek:
	jmp qword ptr [rip + OriginalFuncs_winmm + 124 * 8]

mmioSendMessage:
	jmp qword ptr [rip + OriginalFuncs_winmm + 125 * 8]

mmioSetBuffer:
	jmp qword ptr [rip + OriginalFuncs_winmm + 126 * 8]

mmioSetInfo:
	jmp qword ptr [rip + OriginalFuncs_winmm + 127 * 8]

mmioStringToFOURCCA:
	jmp qword ptr [rip + OriginalFuncs_winmm + 128 * 8]

mmioStringToFOURCCW:
	jmp qword ptr [rip + OriginalFuncs_winmm + 129 * 8]

mmioWrite:
	jmp qword ptr [rip + OriginalFuncs_winmm + 130 * 8]

mmsystemGetVersion:
	jmp qword ptr [rip + OriginalFuncs_winmm + 131 * 8]

sndPlaySoundA:
	jmp qword ptr [rip + OriginalFuncs_winmm + 132 * 8]

sndPlaySoundW:
	jmp qword ptr [rip + OriginalFuncs_winmm + 133 * 8]

timeBeginPeriod:
	jmp qword ptr [rip + OriginalFuncs_winmm + 134 * 8]

timeEndPeriod:
	jmp qword ptr [rip + OriginalFuncs_winmm + 135 * 8]

timeGetDevCaps:
	jmp qword ptr [rip + OriginalFuncs_winmm + 136 * 8]

timeGetSystemTime:
	jmp qword ptr [rip + OriginalFuncs_winmm + 137 * 8]

timeGetTime:
	jmp qword ptr [rip + OriginalFuncs_winmm + 138 * 8]

timeKillEvent:
	jmp qword ptr [rip + OriginalFuncs_winmm + 139 * 8]

timeSetEvent:
	jmp qword ptr [rip + OriginalFuncs_winmm + 140 * 8]

waveInAddBuffer:
	jmp qword ptr [rip + OriginalFuncs_winmm + 141 * 8]

waveInClose:
	jmp qword ptr [rip + OriginalFuncs_winmm + 142 * 8]

waveInGetDevCapsA:
	jmp qword ptr [rip + OriginalFuncs_winmm + 143 * 8]

waveInGetDevCapsW:
	jmp qword ptr [rip + OriginalFuncs_winmm + 144 * 8]

waveInGetErrorTextA:
	jmp qword ptr [rip + OriginalFuncs_winmm + 145 * 8]

waveInGetErrorTextW:
	jmp qword ptr [rip + OriginalFuncs_winmm + 146 * 8]

waveInGetID:
	jmp qword ptr [rip + OriginalFuncs_winmm + 147 * 8]

waveInGetNumDevs:
	jmp qword ptr [rip + OriginalFuncs_winmm + 148 * 8]

waveInGetPosition:
	jmp qword ptr [rip + OriginalFuncs_winmm + 149 * 8]

waveInMessage:
	jmp qword ptr [rip + OriginalFuncs_winmm + 150 * 8]

waveInOpen:
	jmp qword ptr [rip + OriginalFuncs_winmm + 151 * 8]

waveInPrepareHeader:
	jmp qword ptr [rip + OriginalFuncs_winmm + 152 * 8]

waveInReset:
	jmp qword ptr [rip + OriginalFuncs_winmm + 153 * 8]

waveInStart:
	jmp qword ptr [rip + OriginalFuncs_winmm + 154 * 8]

waveInStop:
	jmp qword ptr [rip + OriginalFuncs_winmm + 155 * 8]

waveInUnprepareHeader:
	jmp qword ptr [rip + OriginalFuncs_winmm + 156 * 8]

waveOutBreakLoop:
	jmp qword ptr [rip + OriginalFuncs_winmm + 157 * 8]

waveOutClose:
	jmp qword ptr [rip + OriginalFuncs_winmm + 158 * 8]

waveOutGetDevCapsA:
	jmp qword ptr [rip + OriginalFuncs_winmm + 159 * 8]

waveOutGetDevCapsW:
	jmp qword ptr [rip + OriginalFuncs_winmm + 160 * 8]

waveOutGetErrorTextA:
	jmp qword ptr [rip + OriginalFuncs_winmm + 161 * 8]

waveOutGetErrorTextW:
	jmp qword ptr [rip + OriginalFuncs_winmm + 162 * 8]

waveOutGetID:
	jmp qword ptr [rip + OriginalFuncs_winmm + 163 * 8]

waveOutGetNumDevs:
	jmp qword ptr [rip + OriginalFuncs_winmm + 164 * 8]

waveOutGetPitch:
	jmp qword ptr [rip + OriginalFuncs_winmm + 165 * 8]

waveOutGetPlaybackRate:
	jmp qword ptr [rip + OriginalFuncs_winmm + 166 * 8]

waveOutGetPosition:
	jmp qword ptr [rip + OriginalFuncs_winmm + 167 * 8]

waveOutGetVolume:
	jmp qword ptr [rip + OriginalFuncs_winmm + 168 * 8]

waveOutMessage:
	jmp qword ptr [rip + OriginalFuncs_winmm + 169 * 8]

waveOutOpen:
	jmp qword ptr [rip + OriginalFuncs_winmm + 170 * 8]

waveOutPause:
	jmp qword ptr [rip + OriginalFuncs_winmm + 171 * 8]

waveOutPrepareHeader:
	jmp qword ptr [rip + OriginalFuncs_winmm + 172 * 8]

waveOutReset:
	jmp qword ptr [rip + OriginalFuncs_winmm + 173 * 8]

waveOutRestart:
	jmp qword ptr [rip + OriginalFuncs_winmm + 174 * 8]

waveOutSetPitch:
	jmp qword ptr [rip + OriginalFuncs_winmm + 175 * 8]

waveOutSetPlaybackRate:
	jmp qword ptr [rip + OriginalFuncs_winmm + 176 * 8]

waveOutSetVolume:
	jmp qword ptr [rip + OriginalFuncs_winmm + 177 * 8]

waveOutUnprepareHeader:
	jmp qword ptr [rip + OriginalFuncs_winmm + 178 * 8]

waveOutWrite:
	jmp qword ptr [rip + OriginalFuncs_winmm + 179 * 8]

ExportByOrdinal2:
	jmp qword ptr [rip + OriginalFuncs_winmm + 180 * 8]