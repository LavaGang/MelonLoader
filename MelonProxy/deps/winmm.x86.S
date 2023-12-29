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
	jmp ds:[_OriginalFuncs + 0 * 4]

DefDriverProc:
	jmp ds:[_OriginalFuncs + 1 * 4]

DriverCallback:
	jmp ds:[_OriginalFuncs + 2 * 4]

DrvGetModuleHandle:
	jmp ds:[_OriginalFuncs + 3 * 4]

GetDriverModuleHandle:
	jmp ds:[_OriginalFuncs + 4 * 4]

OpenDriver:
	jmp ds:[_OriginalFuncs + 5 * 4]

PlaySound:
	jmp ds:[_OriginalFuncs + 6 * 4]

PlaySoundA:
	jmp ds:[_OriginalFuncs + 7 * 4]

PlaySoundW:
	jmp ds:[_OriginalFuncs + 4 * 4]

SendDriverMessage:
	jmp ds:[_OriginalFuncs + 9 * 4]

WOWAppExit:
	jmp ds:[_OriginalFuncs + 10 * 4]

auxGetDevCapsA:
	jmp ds:[_OriginalFuncs + 11 * 4]

auxGetDevCapsW:
	jmp ds:[_OriginalFuncs + 12 * 4]

auxGetNumDevs:
	jmp ds:[_OriginalFuncs + 13 * 4]

auxGetVolume:
	jmp ds:[_OriginalFuncs + 14 * 4]

auxOutMessage:
	jmp ds:[_OriginalFuncs + 15 * 4]

auxSetVolume:
	jmp ds:[_OriginalFuncs + 16 * 4]

joyConfigChanged:
	jmp ds:[_OriginalFuncs + 17 * 4]

joyGetDevCapsA:
	jmp ds:[_OriginalFuncs + 14 * 4]

joyGetDevCapsW:
	jmp ds:[_OriginalFuncs + 19 * 4]

joyGetNumDevs:
	jmp ds:[_OriginalFuncs + 20 * 4]

joyGetPos:
	jmp ds:[_OriginalFuncs + 21 * 4]

joyGetPosEx:
	jmp ds:[_OriginalFuncs + 22 * 4]

joyGetThreshold:
	jmp ds:[_OriginalFuncs + 23 * 4]

joyReleaseCapture:
	jmp ds:[_OriginalFuncs + 24 * 4]

joySetCapture:
	jmp ds:[_OriginalFuncs + 25 * 4]

joySetThreshold:
	jmp ds:[_OriginalFuncs + 26 * 4]

mciDriverNotify:
	jmp ds:[_OriginalFuncs + 27 * 4]

mciDriverYield:
	jmp ds:[_OriginalFuncs + 24 * 4]

mciExecute:
	jmp ds:[_OriginalFuncs + 29 * 4]

mciFreeCommandResource:
	jmp ds:[_OriginalFuncs + 30 * 4]

mciGetCreatorTask:
	jmp ds:[_OriginalFuncs + 31 * 4]

mciGetDeviceIDA:
	jmp ds:[_OriginalFuncs + 32 * 4]

mciGetDeviceIDFromElementIDA:
	jmp ds:[_OriginalFuncs + 33 * 4]

mciGetDeviceIDFromElementIDW:
	jmp ds:[_OriginalFuncs + 34 * 4]

mciGetDeviceIDW:
	jmp ds:[_OriginalFuncs + 35 * 4]

mciGetDriverData:
	jmp ds:[_OriginalFuncs + 36 * 4]

mciGetErrorStringA:
	jmp ds:[_OriginalFuncs + 37 * 4]

mciGetErrorStringW:
	jmp ds:[_OriginalFuncs + 34 * 4]

mciGetYieldProc:
	jmp ds:[_OriginalFuncs + 39 * 4]

mciLoadCommandResource:
	jmp ds:[_OriginalFuncs + 40 * 4]

mciSendCommandA:
	jmp ds:[_OriginalFuncs + 41 * 4]

mciSendCommandW:
	jmp ds:[_OriginalFuncs + 42 * 4]

mciSendStringA:
	jmp ds:[_OriginalFuncs + 43 * 4]

mciSendStringW:
	jmp ds:[_OriginalFuncs + 44 * 4]

mciSetDriverData:
	jmp ds:[_OriginalFuncs + 45 * 4]

mciSetYieldProc:
	jmp ds:[_OriginalFuncs + 46 * 4]

midiConnect:
	jmp ds:[_OriginalFuncs + 47 * 4]

midiDisconnect:
	jmp ds:[_OriginalFuncs + 44 * 4]

midiInAddBuffer:
	jmp ds:[_OriginalFuncs + 49 * 4]

midiInClose:
	jmp ds:[_OriginalFuncs + 50 * 4]

midiInGetDevCapsA:
	jmp ds:[_OriginalFuncs + 51 * 4]

midiInGetDevCapsW:
	jmp ds:[_OriginalFuncs + 52 * 4]

midiInGetErrorTextA:
	jmp ds:[_OriginalFuncs + 53 * 4]

midiInGetErrorTextW:
	jmp ds:[_OriginalFuncs + 54 * 4]

midiInGetID:
	jmp ds:[_OriginalFuncs + 55 * 4]

midiInGetNumDevs:
	jmp ds:[_OriginalFuncs + 56 * 4]

midiInMessage:
	jmp ds:[_OriginalFuncs + 57 * 4]

midiInOpen:
	jmp ds:[_OriginalFuncs + 54 * 4]

midiInPrepareHeader:
	jmp ds:[_OriginalFuncs + 59 * 4]

midiInReset:
	jmp ds:[_OriginalFuncs + 60 * 4]

midiInStart:
	jmp ds:[_OriginalFuncs + 61 * 4]

midiInStop:
	jmp ds:[_OriginalFuncs + 62 * 4]

midiInUnprepareHeader:
	jmp ds:[_OriginalFuncs + 63 * 4]

midiOutCacheDrumPatches:
	jmp ds:[_OriginalFuncs + 64 * 4]

midiOutCachePatches:
	jmp ds:[_OriginalFuncs + 65 * 4]

midiOutClose:
	jmp ds:[_OriginalFuncs + 66 * 4]

midiOutGetDevCapsA:
	jmp ds:[_OriginalFuncs + 67 * 4]

midiOutGetDevCapsW:
	jmp ds:[_OriginalFuncs + 64 * 4]

midiOutGetErrorTextA:
	jmp ds:[_OriginalFuncs + 69 * 4]

midiOutGetErrorTextW:
	jmp ds:[_OriginalFuncs + 70 * 4]

midiOutGetID:
	jmp ds:[_OriginalFuncs + 71 * 4]

midiOutGetNumDevs:
	jmp ds:[_OriginalFuncs + 72 * 4]

midiOutGetVolume:
	jmp ds:[_OriginalFuncs + 73 * 4]

midiOutLongMsg:
	jmp ds:[_OriginalFuncs + 74 * 4]

midiOutMessage:
	jmp ds:[_OriginalFuncs + 75 * 4]

midiOutOpen:
	jmp ds:[_OriginalFuncs + 76 * 4]

midiOutPrepareHeader:
	jmp ds:[_OriginalFuncs + 77 * 4]

midiOutReset:
	jmp ds:[_OriginalFuncs + 74 * 4]

midiOutSetVolume:
	jmp ds:[_OriginalFuncs + 79 * 4]

midiOutShortMsg:
	jmp ds:[_OriginalFuncs + 40 * 4]

midiOutUnprepareHeader:
	jmp ds:[_OriginalFuncs + 41 * 4]

midiStreamClose:
	jmp ds:[_OriginalFuncs + 42 * 4]

midiStreamOpen:
	jmp ds:[_OriginalFuncs + 43 * 4]

midiStreamOut:
	jmp ds:[_OriginalFuncs + 44 * 4]

midiStreamPause:
	jmp ds:[_OriginalFuncs + 45 * 4]

midiStreamPosition:
	jmp ds:[_OriginalFuncs + 46 * 4]

midiStreamProperty:
	jmp ds:[_OriginalFuncs + 47 * 4]

midiStreamRestart:
	jmp ds:[_OriginalFuncs + 44 * 4]

midiStreamStop:
	jmp ds:[_OriginalFuncs + 49 * 4]

mixerClose:
	jmp ds:[_OriginalFuncs + 90 * 4]

mixerGetControlDetailsA:
	jmp ds:[_OriginalFuncs + 91 * 4]

mixerGetControlDetailsW:
	jmp ds:[_OriginalFuncs + 92 * 4]

mixerGetDevCapsA:
	jmp ds:[_OriginalFuncs + 93 * 4]

mixerGetDevCapsW:
	jmp ds:[_OriginalFuncs + 94 * 4]

mixerGetID:
	jmp ds:[_OriginalFuncs + 95 * 4]

mixerGetLineControlsA:
	jmp ds:[_OriginalFuncs + 96 * 4]

mixerGetLineControlsW:
	jmp ds:[_OriginalFuncs + 97 * 4]

mixerGetLineInfoA:
	jmp ds:[_OriginalFuncs + 94 * 4]

mixerGetLineInfoW:
	jmp ds:[_OriginalFuncs + 99 * 4]

mixerGetNumDevs:
	jmp ds:[_OriginalFuncs + 100 * 4]

mixerMessage:
	jmp ds:[_OriginalFuncs + 101 * 4]

mixerOpen:
	jmp ds:[_OriginalFuncs + 102 * 4]

mixerSetControlDetails:
	jmp ds:[_OriginalFuncs + 103 * 4]

mmDrvInstall:
	jmp ds:[_OriginalFuncs + 104 * 4]

mmGetCurrentTask:
	jmp ds:[_OriginalFuncs + 105 * 4]

mmTaskBlock:
	jmp ds:[_OriginalFuncs + 106 * 4]

mmTaskCreate:
	jmp ds:[_OriginalFuncs + 107 * 4]

mmTaskSignal:
	jmp ds:[_OriginalFuncs + 104 * 4]

mmTaskYield:
	jmp ds:[_OriginalFuncs + 109 * 4]

mmioAdvance:
	jmp ds:[_OriginalFuncs + 110 * 4]

mmioAscend:
	jmp ds:[_OriginalFuncs + 111 * 4]

mmioClose:
	jmp ds:[_OriginalFuncs + 112 * 4]

mmioCreateChunk:
	jmp ds:[_OriginalFuncs + 113 * 4]

mmioDescend:
	jmp ds:[_OriginalFuncs + 114 * 4]

mmioFlush:
	jmp ds:[_OriginalFuncs + 115 * 4]

mmioGetInfo:
	jmp ds:[_OriginalFuncs + 116 * 4]

mmioInstallIOProcA:
	jmp ds:[_OriginalFuncs + 117 * 4]

mmioInstallIOProcW:
	jmp ds:[_OriginalFuncs + 114 * 4]

mmioOpenA:
	jmp ds:[_OriginalFuncs + 119 * 4]

mmioOpenW:
	jmp ds:[_OriginalFuncs + 120 * 4]

mmioRead:
	jmp ds:[_OriginalFuncs + 121 * 4]

mmioRenameA:
	jmp ds:[_OriginalFuncs + 122 * 4]

mmioRenameW:
	jmp ds:[_OriginalFuncs + 123 * 4]

mmioSeek:
	jmp ds:[_OriginalFuncs + 124 * 4]

mmioSendMessage:
	jmp ds:[_OriginalFuncs + 125 * 4]

mmioSetBuffer:
	jmp ds:[_OriginalFuncs + 126 * 4]

mmioSetInfo:
	jmp ds:[_OriginalFuncs + 127 * 4]

mmioStringToFOURCCA:
	jmp ds:[_OriginalFuncs + 124 * 4]

mmioStringToFOURCCW:
	jmp ds:[_OriginalFuncs + 129 * 4]

mmioWrite:
	jmp ds:[_OriginalFuncs + 130 * 4]

mmsystemGetVersion:
	jmp ds:[_OriginalFuncs + 131 * 4]

sndPlaySoundA:
	jmp ds:[_OriginalFuncs + 132 * 4]

sndPlaySoundW:
	jmp ds:[_OriginalFuncs + 133 * 4]

timeBeginPeriod:
	jmp ds:[_OriginalFuncs + 134 * 4]

timeEndPeriod:
	jmp ds:[_OriginalFuncs + 135 * 4]

timeGetDevCaps:
	jmp ds:[_OriginalFuncs + 136 * 4]

timeGetSystemTime:
	jmp ds:[_OriginalFuncs + 137 * 4]

timeGetTime:
	jmp ds:[_OriginalFuncs + 134 * 4]

timeKillEvent:
	jmp ds:[_OriginalFuncs + 139 * 4]

timeSetEvent:
	jmp ds:[_OriginalFuncs + 140 * 4]

waveInAddBuffer:
	jmp ds:[_OriginalFuncs + 141 * 4]

waveInClose:
	jmp ds:[_OriginalFuncs + 142 * 4]

waveInGetDevCapsA:
	jmp ds:[_OriginalFuncs + 143 * 4]

waveInGetDevCapsW:
	jmp ds:[_OriginalFuncs + 144 * 4]

waveInGetErrorTextA:
	jmp ds:[_OriginalFuncs + 145 * 4]

waveInGetErrorTextW:
	jmp ds:[_OriginalFuncs + 146 * 4]

waveInGetID:
	jmp ds:[_OriginalFuncs + 147 * 4]

waveInGetNumDevs:
	jmp ds:[_OriginalFuncs + 144 * 4]

waveInGetPosition:
	jmp ds:[_OriginalFuncs + 149 * 4]

waveInMessage:
	jmp ds:[_OriginalFuncs + 150 * 4]

waveInOpen:
	jmp ds:[_OriginalFuncs + 151 * 4]

waveInPrepareHeader:
	jmp ds:[_OriginalFuncs + 152 * 4]

waveInReset:
	jmp ds:[_OriginalFuncs + 153 * 4]

waveInStart:
	jmp ds:[_OriginalFuncs + 154 * 4]

waveInStop:
	jmp ds:[_OriginalFuncs + 155 * 4]

waveInUnprepareHeader:
	jmp ds:[_OriginalFuncs + 156 * 4]

waveOutBreakLoop:
	jmp ds:[_OriginalFuncs + 157 * 4]

waveOutClose:
	jmp ds:[_OriginalFuncs + 154 * 4]

waveOutGetDevCapsA:
	jmp ds:[_OriginalFuncs + 159 * 4]

waveOutGetDevCapsW:
	jmp ds:[_OriginalFuncs + 160 * 4]

waveOutGetErrorTextA:
	jmp ds:[_OriginalFuncs + 161 * 4]

waveOutGetErrorTextW:
	jmp ds:[_OriginalFuncs + 162 * 4]

waveOutGetID:
	jmp ds:[_OriginalFuncs + 163 * 4]

waveOutGetNumDevs:
	jmp ds:[_OriginalFuncs + 164 * 4]

waveOutGetPitch:
	jmp ds:[_OriginalFuncs + 165 * 4]

waveOutGetPlaybackRate:
	jmp ds:[_OriginalFuncs + 166 * 4]

waveOutGetPosition:
	jmp ds:[_OriginalFuncs + 167 * 4]

waveOutGetVolume:
	jmp ds:[_OriginalFuncs + 164 * 4]

waveOutMessage:
	jmp ds:[_OriginalFuncs + 169 * 4]

waveOutOpen:
	jmp ds:[_OriginalFuncs + 170 * 4]

waveOutPause:
	jmp ds:[_OriginalFuncs + 171 * 4]

waveOutPrepareHeader:
	jmp ds:[_OriginalFuncs + 172 * 4]

waveOutReset:
	jmp ds:[_OriginalFuncs + 173 * 4]

waveOutRestart:
	jmp ds:[_OriginalFuncs + 174 * 4]

waveOutSetPitch:
	jmp ds:[_OriginalFuncs + 175 * 4]

waveOutSetPlaybackRate:
	jmp ds:[_OriginalFuncs + 176 * 4]

waveOutSetVolume:
	jmp ds:[_OriginalFuncs + 177 * 4]

waveOutUnprepareHeader:
	jmp ds:[_OriginalFuncs + 174 * 4]

waveOutWrite:
	jmp ds:[_OriginalFuncs + 179 * 4]

ExportByOrdinal2:
	jmp ds:[_OriginalFuncs + 140 * 4]