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
	jmp qword ptr [rip + OriginalFuncs + 0 * 8]

DefDriverProc:
	jmp qword ptr [rip + OriginalFuncs + 1 * 8]

DriverCallback:
	jmp qword ptr [rip + OriginalFuncs + 2 * 8]

DrvGetModuleHandle:
	jmp qword ptr [rip + OriginalFuncs + 3 * 8]

GetDriverModuleHandle:
	jmp qword ptr [rip + OriginalFuncs + 4 * 8]

OpenDriver:
	jmp qword ptr [rip + OriginalFuncs + 5 * 8]

PlaySound:
	jmp qword ptr [rip + OriginalFuncs + 6 * 8]

PlaySoundA:
	jmp qword ptr [rip + OriginalFuncs + 7 * 8]

PlaySoundW:
	jmp qword ptr [rip + OriginalFuncs + 8 * 8]

SendDriverMessage:
	jmp qword ptr [rip + OriginalFuncs + 9 * 8]

WOWAppExit:
	jmp qword ptr [rip + OriginalFuncs + 10 * 8]

auxGetDevCapsA:
	jmp qword ptr [rip + OriginalFuncs + 11 * 8]

auxGetDevCapsW:
	jmp qword ptr [rip + OriginalFuncs + 12 * 8]

auxGetNumDevs:
	jmp qword ptr [rip + OriginalFuncs + 13 * 8]

auxGetVolume:
	jmp qword ptr [rip + OriginalFuncs + 14 * 8]

auxOutMessage:
	jmp qword ptr [rip + OriginalFuncs + 15 * 8]

auxSetVolume:
	jmp qword ptr [rip + OriginalFuncs + 16 * 8]

joyConfigChanged:
	jmp qword ptr [rip + OriginalFuncs + 17 * 8]

joyGetDevCapsA:
	jmp qword ptr [rip + OriginalFuncs + 18 * 8]

joyGetDevCapsW:
	jmp qword ptr [rip + OriginalFuncs + 19 * 8]

joyGetNumDevs:
	jmp qword ptr [rip + OriginalFuncs + 20 * 8]

joyGetPos:
	jmp qword ptr [rip + OriginalFuncs + 21 * 8]

joyGetPosEx:
	jmp qword ptr [rip + OriginalFuncs + 22 * 8]

joyGetThreshold:
	jmp qword ptr [rip + OriginalFuncs + 23 * 8]

joyReleaseCapture:
	jmp qword ptr [rip + OriginalFuncs + 24 * 8]

joySetCapture:
	jmp qword ptr [rip + OriginalFuncs + 25 * 8]

joySetThreshold:
	jmp qword ptr [rip + OriginalFuncs + 26 * 8]

mciDriverNotify:
	jmp qword ptr [rip + OriginalFuncs + 27 * 8]

mciDriverYield:
	jmp qword ptr [rip + OriginalFuncs + 28 * 8]

mciExecute:
	jmp qword ptr [rip + OriginalFuncs + 29 * 8]

mciFreeCommandResource:
	jmp qword ptr [rip + OriginalFuncs + 30 * 8]

mciGetCreatorTask:
	jmp qword ptr [rip + OriginalFuncs + 31 * 8]

mciGetDeviceIDA:
	jmp qword ptr [rip + OriginalFuncs + 32 * 8]

mciGetDeviceIDFromElementIDA:
	jmp qword ptr [rip + OriginalFuncs + 33 * 8]

mciGetDeviceIDFromElementIDW:
	jmp qword ptr [rip + OriginalFuncs + 34 * 8]

mciGetDeviceIDW:
	jmp qword ptr [rip + OriginalFuncs + 35 * 8]

mciGetDriverData:
	jmp qword ptr [rip + OriginalFuncs + 36 * 8]

mciGetErrorStringA:
	jmp qword ptr [rip + OriginalFuncs + 37 * 8]

mciGetErrorStringW:
	jmp qword ptr [rip + OriginalFuncs + 38 * 8]

mciGetYieldProc:
	jmp qword ptr [rip + OriginalFuncs + 39 * 8]

mciLoadCommandResource:
	jmp qword ptr [rip + OriginalFuncs + 40 * 8]

mciSendCommandA:
	jmp qword ptr [rip + OriginalFuncs + 41 * 8]

mciSendCommandW:
	jmp qword ptr [rip + OriginalFuncs + 42 * 8]

mciSendStringA:
	jmp qword ptr [rip + OriginalFuncs + 43 * 8]

mciSendStringW:
	jmp qword ptr [rip + OriginalFuncs + 44 * 8]

mciSetDriverData:
	jmp qword ptr [rip + OriginalFuncs + 45 * 8]

mciSetYieldProc:
	jmp qword ptr [rip + OriginalFuncs + 46 * 8]

midiConnect:
	jmp qword ptr [rip + OriginalFuncs + 47 * 8]

midiDisconnect:
	jmp qword ptr [rip + OriginalFuncs + 48 * 8]

midiInAddBuffer:
	jmp qword ptr [rip + OriginalFuncs + 49 * 8]

midiInClose:
	jmp qword ptr [rip + OriginalFuncs + 50 * 8]

midiInGetDevCapsA:
	jmp qword ptr [rip + OriginalFuncs + 51 * 8]

midiInGetDevCapsW:
	jmp qword ptr [rip + OriginalFuncs + 52 * 8]

midiInGetErrorTextA:
	jmp qword ptr [rip + OriginalFuncs + 53 * 8]

midiInGetErrorTextW:
	jmp qword ptr [rip + OriginalFuncs + 54 * 8]

midiInGetID:
	jmp qword ptr [rip + OriginalFuncs + 55 * 8]

midiInGetNumDevs:
	jmp qword ptr [rip + OriginalFuncs + 56 * 8]

midiInMessage:
	jmp qword ptr [rip + OriginalFuncs + 57 * 8]

midiInOpen:
	jmp qword ptr [rip + OriginalFuncs + 58 * 8]

midiInPrepareHeader:
	jmp qword ptr [rip + OriginalFuncs + 59 * 8]

midiInReset:
	jmp qword ptr [rip + OriginalFuncs + 60 * 8]

midiInStart:
	jmp qword ptr [rip + OriginalFuncs + 61 * 8]

midiInStop:
	jmp qword ptr [rip + OriginalFuncs + 62 * 8]

midiInUnprepareHeader:
	jmp qword ptr [rip + OriginalFuncs + 63 * 8]

midiOutCacheDrumPatches:
	jmp qword ptr [rip + OriginalFuncs + 64 * 8]

midiOutCachePatches:
	jmp qword ptr [rip + OriginalFuncs + 65 * 8]

midiOutClose:
	jmp qword ptr [rip + OriginalFuncs + 66 * 8]

midiOutGetDevCapsA:
	jmp qword ptr [rip + OriginalFuncs + 67 * 8]

midiOutGetDevCapsW:
	jmp qword ptr [rip + OriginalFuncs + 68 * 8]

midiOutGetErrorTextA:
	jmp qword ptr [rip + OriginalFuncs + 69 * 8]

midiOutGetErrorTextW:
	jmp qword ptr [rip + OriginalFuncs + 70 * 8]

midiOutGetID:
	jmp qword ptr [rip + OriginalFuncs + 71 * 8]

midiOutGetNumDevs:
	jmp qword ptr [rip + OriginalFuncs + 72 * 8]

midiOutGetVolume:
	jmp qword ptr [rip + OriginalFuncs + 73 * 8]

midiOutLongMsg:
	jmp qword ptr [rip + OriginalFuncs + 74 * 8]

midiOutMessage:
	jmp qword ptr [rip + OriginalFuncs + 75 * 8]

midiOutOpen:
	jmp qword ptr [rip + OriginalFuncs + 76 * 8]

midiOutPrepareHeader:
	jmp qword ptr [rip + OriginalFuncs + 77 * 8]

midiOutReset:
	jmp qword ptr [rip + OriginalFuncs + 78 * 8]

midiOutSetVolume:
	jmp qword ptr [rip + OriginalFuncs + 79 * 8]

midiOutShortMsg:
	jmp qword ptr [rip + OriginalFuncs + 80 * 8]

midiOutUnprepareHeader:
	jmp qword ptr [rip + OriginalFuncs + 81 * 8]

midiStreamClose:
	jmp qword ptr [rip + OriginalFuncs + 82 * 8]

midiStreamOpen:
	jmp qword ptr [rip + OriginalFuncs + 83 * 8]

midiStreamOut:
	jmp qword ptr [rip + OriginalFuncs + 84 * 8]

midiStreamPause:
	jmp qword ptr [rip + OriginalFuncs + 85 * 8]

midiStreamPosition:
	jmp qword ptr [rip + OriginalFuncs + 86 * 8]

midiStreamProperty:
	jmp qword ptr [rip + OriginalFuncs + 87 * 8]

midiStreamRestart:
	jmp qword ptr [rip + OriginalFuncs + 88 * 8]

midiStreamStop:
	jmp qword ptr [rip + OriginalFuncs + 89 * 8]

mixerClose:
	jmp qword ptr [rip + OriginalFuncs + 90 * 8]

mixerGetControlDetailsA:
	jmp qword ptr [rip + OriginalFuncs + 91 * 8]

mixerGetControlDetailsW:
	jmp qword ptr [rip + OriginalFuncs + 92 * 8]

mixerGetDevCapsA:
	jmp qword ptr [rip + OriginalFuncs + 93 * 8]

mixerGetDevCapsW:
	jmp qword ptr [rip + OriginalFuncs + 94 * 8]

mixerGetID:
	jmp qword ptr [rip + OriginalFuncs + 95 * 8]

mixerGetLineControlsA:
	jmp qword ptr [rip + OriginalFuncs + 96 * 8]

mixerGetLineControlsW:
	jmp qword ptr [rip + OriginalFuncs + 97 * 8]

mixerGetLineInfoA:
	jmp qword ptr [rip + OriginalFuncs + 98 * 8]

mixerGetLineInfoW:
	jmp qword ptr [rip + OriginalFuncs + 99 * 8]

mixerGetNumDevs:
	jmp qword ptr [rip + OriginalFuncs + 100 * 8]

mixerMessage:
	jmp qword ptr [rip + OriginalFuncs + 101 * 8]

mixerOpen:
	jmp qword ptr [rip + OriginalFuncs + 102 * 8]

mixerSetControlDetails:
	jmp qword ptr [rip + OriginalFuncs + 103 * 8]

mmDrvInstall:
	jmp qword ptr [rip + OriginalFuncs + 104 * 8]

mmGetCurrentTask:
	jmp qword ptr [rip + OriginalFuncs + 105 * 8]

mmTaskBlock:
	jmp qword ptr [rip + OriginalFuncs + 106 * 8]

mmTaskCreate:
	jmp qword ptr [rip + OriginalFuncs + 107 * 8]

mmTaskSignal:
	jmp qword ptr [rip + OriginalFuncs + 108 * 8]

mmTaskYield:
	jmp qword ptr [rip + OriginalFuncs + 109 * 8]

mmioAdvance:
	jmp qword ptr [rip + OriginalFuncs + 110 * 8]

mmioAscend:
	jmp qword ptr [rip + OriginalFuncs + 111 * 8]

mmioClose:
	jmp qword ptr [rip + OriginalFuncs + 112 * 8]

mmioCreateChunk:
	jmp qword ptr [rip + OriginalFuncs + 113 * 8]

mmioDescend:
	jmp qword ptr [rip + OriginalFuncs + 114 * 8]

mmioFlush:
	jmp qword ptr [rip + OriginalFuncs + 115 * 8]

mmioGetInfo:
	jmp qword ptr [rip + OriginalFuncs + 116 * 8]

mmioInstallIOProcA:
	jmp qword ptr [rip + OriginalFuncs + 117 * 8]

mmioInstallIOProcW:
	jmp qword ptr [rip + OriginalFuncs + 118 * 8]

mmioOpenA:
	jmp qword ptr [rip + OriginalFuncs + 119 * 8]

mmioOpenW:
	jmp qword ptr [rip + OriginalFuncs + 120 * 8]

mmioRead:
	jmp qword ptr [rip + OriginalFuncs + 121 * 8]

mmioRenameA:
	jmp qword ptr [rip + OriginalFuncs + 122 * 8]

mmioRenameW:
	jmp qword ptr [rip + OriginalFuncs + 123 * 8]

mmioSeek:
	jmp qword ptr [rip + OriginalFuncs + 124 * 8]

mmioSendMessage:
	jmp qword ptr [rip + OriginalFuncs + 125 * 8]

mmioSetBuffer:
	jmp qword ptr [rip + OriginalFuncs + 126 * 8]

mmioSetInfo:
	jmp qword ptr [rip + OriginalFuncs + 127 * 8]

mmioStringToFOURCCA:
	jmp qword ptr [rip + OriginalFuncs + 128 * 8]

mmioStringToFOURCCW:
	jmp qword ptr [rip + OriginalFuncs + 129 * 8]

mmioWrite:
	jmp qword ptr [rip + OriginalFuncs + 130 * 8]

mmsystemGetVersion:
	jmp qword ptr [rip + OriginalFuncs + 131 * 8]

sndPlaySoundA:
	jmp qword ptr [rip + OriginalFuncs + 132 * 8]

sndPlaySoundW:
	jmp qword ptr [rip + OriginalFuncs + 133 * 8]

timeBeginPeriod:
	jmp qword ptr [rip + OriginalFuncs + 134 * 8]

timeEndPeriod:
	jmp qword ptr [rip + OriginalFuncs + 135 * 8]

timeGetDevCaps:
	jmp qword ptr [rip + OriginalFuncs + 136 * 8]

timeGetSystemTime:
	jmp qword ptr [rip + OriginalFuncs + 137 * 8]

timeGetTime:
	jmp qword ptr [rip + OriginalFuncs + 138 * 8]

timeKillEvent:
	jmp qword ptr [rip + OriginalFuncs + 139 * 8]

timeSetEvent:
	jmp qword ptr [rip + OriginalFuncs + 140 * 8]

waveInAddBuffer:
	jmp qword ptr [rip + OriginalFuncs + 141 * 8]

waveInClose:
	jmp qword ptr [rip + OriginalFuncs + 142 * 8]

waveInGetDevCapsA:
	jmp qword ptr [rip + OriginalFuncs + 143 * 8]

waveInGetDevCapsW:
	jmp qword ptr [rip + OriginalFuncs + 144 * 8]

waveInGetErrorTextA:
	jmp qword ptr [rip + OriginalFuncs + 145 * 8]

waveInGetErrorTextW:
	jmp qword ptr [rip + OriginalFuncs + 146 * 8]

waveInGetID:
	jmp qword ptr [rip + OriginalFuncs + 147 * 8]

waveInGetNumDevs:
	jmp qword ptr [rip + OriginalFuncs + 148 * 8]

waveInGetPosition:
	jmp qword ptr [rip + OriginalFuncs + 149 * 8]

waveInMessage:
	jmp qword ptr [rip + OriginalFuncs + 150 * 8]

waveInOpen:
	jmp qword ptr [rip + OriginalFuncs + 151 * 8]

waveInPrepareHeader:
	jmp qword ptr [rip + OriginalFuncs + 152 * 8]

waveInReset:
	jmp qword ptr [rip + OriginalFuncs + 153 * 8]

waveInStart:
	jmp qword ptr [rip + OriginalFuncs + 154 * 8]

waveInStop:
	jmp qword ptr [rip + OriginalFuncs + 155 * 8]

waveInUnprepareHeader:
	jmp qword ptr [rip + OriginalFuncs + 156 * 8]

waveOutBreakLoop:
	jmp qword ptr [rip + OriginalFuncs + 157 * 8]

waveOutClose:
	jmp qword ptr [rip + OriginalFuncs + 158 * 8]

waveOutGetDevCapsA:
	jmp qword ptr [rip + OriginalFuncs + 159 * 8]

waveOutGetDevCapsW:
	jmp qword ptr [rip + OriginalFuncs + 160 * 8]

waveOutGetErrorTextA:
	jmp qword ptr [rip + OriginalFuncs + 161 * 8]

waveOutGetErrorTextW:
	jmp qword ptr [rip + OriginalFuncs + 162 * 8]

waveOutGetID:
	jmp qword ptr [rip + OriginalFuncs + 163 * 8]

waveOutGetNumDevs:
	jmp qword ptr [rip + OriginalFuncs + 164 * 8]

waveOutGetPitch:
	jmp qword ptr [rip + OriginalFuncs + 165 * 8]

waveOutGetPlaybackRate:
	jmp qword ptr [rip + OriginalFuncs + 166 * 8]

waveOutGetPosition:
	jmp qword ptr [rip + OriginalFuncs + 167 * 8]

waveOutGetVolume:
	jmp qword ptr [rip + OriginalFuncs + 168 * 8]

waveOutMessage:
	jmp qword ptr [rip + OriginalFuncs + 169 * 8]

waveOutOpen:
	jmp qword ptr [rip + OriginalFuncs + 170 * 8]

waveOutPause:
	jmp qword ptr [rip + OriginalFuncs + 171 * 8]

waveOutPrepareHeader:
	jmp qword ptr [rip + OriginalFuncs + 172 * 8]

waveOutReset:
	jmp qword ptr [rip + OriginalFuncs + 173 * 8]

waveOutRestart:
	jmp qword ptr [rip + OriginalFuncs + 174 * 8]

waveOutSetPitch:
	jmp qword ptr [rip + OriginalFuncs + 175 * 8]

waveOutSetPlaybackRate:
	jmp qword ptr [rip + OriginalFuncs + 176 * 8]

waveOutSetVolume:
	jmp qword ptr [rip + OriginalFuncs + 177 * 8]

waveOutUnprepareHeader:
	jmp qword ptr [rip + OriginalFuncs + 178 * 8]

waveOutWrite:
	jmp qword ptr [rip + OriginalFuncs + 179 * 8]

ExportByOrdinal2:
	jmp qword ptr [rip + OriginalFuncs + 180 * 8]