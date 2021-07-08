ifdef RAX
	.code
		extern OriginalFuncs_psapi:QWORD
		EmptyWorkingSet proc
			jmp QWORD ptr OriginalFuncs_psapi[0 * 8]
		EmptyWorkingSet endp
		EnumDeviceDrivers proc
			jmp QWORD ptr OriginalFuncs_psapi[1 * 8]
		EnumDeviceDrivers endp
		EnumPageFilesA proc
			jmp QWORD ptr OriginalFuncs_psapi[2 * 8]
		EnumPageFilesA endp
		EnumPageFilesW proc
			jmp QWORD ptr OriginalFuncs_psapi[3 * 8]
		EnumPageFilesW endp
		EnumProcessModules proc
			jmp QWORD ptr OriginalFuncs_psapi[4 * 8]
		EnumProcessModules endp
		EnumProcessModulesEx proc
			jmp QWORD ptr OriginalFuncs_psapi[5 * 8]
		EnumProcessModulesEx endp
		EnumProcesses proc
			jmp QWORD ptr OriginalFuncs_psapi[6 * 8]
		EnumProcesses endp
		GetDeviceDriverBaseNameA proc
			jmp QWORD ptr OriginalFuncs_psapi[7 * 8]
		GetDeviceDriverBaseNameA endp
		GetDeviceDriverBaseNameW proc
			jmp QWORD ptr OriginalFuncs_psapi[8 * 8]
		GetDeviceDriverBaseNameW endp
		GetDeviceDriverFileNameA proc
			jmp QWORD ptr OriginalFuncs_psapi[9 * 8]
		GetDeviceDriverFileNameA endp
		GetDeviceDriverFileNameW proc
			jmp QWORD ptr OriginalFuncs_psapi[10 * 8]
		GetDeviceDriverFileNameW endp
		GetMappedFileNameA proc
			jmp QWORD ptr OriginalFuncs_psapi[11 * 8]
		GetMappedFileNameA endp
		GetMappedFileNameW proc
			jmp QWORD ptr OriginalFuncs_psapi[12 * 8]
		GetMappedFileNameW endp
		GetModuleBaseNameA proc
			jmp QWORD ptr OriginalFuncs_psapi[13 * 8]
		GetModuleBaseNameA endp
		GetModuleBaseNameW proc
			jmp QWORD ptr OriginalFuncs_psapi[14 * 8]
		GetModuleBaseNameW endp
		GetModuleFileNameExA proc
			jmp QWORD ptr OriginalFuncs_psapi[15 * 8]
		GetModuleFileNameExA endp
		GetModuleFileNameExW proc
			jmp QWORD ptr OriginalFuncs_psapi[16 * 8]
		GetModuleFileNameExW endp
		GetModuleInformation proc
			jmp QWORD ptr OriginalFuncs_psapi[17 * 8]
		GetModuleInformation endp
		GetPerformanceInfo proc
			jmp QWORD ptr OriginalFuncs_psapi[18 * 8]
		GetPerformanceInfo endp
		GetProcessImageFileNameA proc
			jmp QWORD ptr OriginalFuncs_psapi[19 * 8]
		GetProcessImageFileNameA endp
		GetProcessImageFileNameW proc
			jmp QWORD ptr OriginalFuncs_psapi[20 * 8]
		GetProcessImageFileNameW endp
		GetProcessMemoryInfo proc
			jmp QWORD ptr OriginalFuncs_psapi[21 * 8]
		GetProcessMemoryInfo endp
		GetWsChanges proc
			jmp QWORD ptr OriginalFuncs_psapi[22 * 8]
		GetWsChanges endp
		GetWsChangesEx proc
			jmp QWORD ptr OriginalFuncs_psapi[23 * 8]
		GetWsChangesEx endp
		InitializeProcessForWsWatch proc
			jmp QWORD ptr OriginalFuncs_psapi[24 * 8]
		InitializeProcessForWsWatch endp
		QueryWorkingSet proc
			jmp QWORD ptr OriginalFuncs_psapi[25 * 8]
		QueryWorkingSet endp
		QueryWorkingSetEx proc
			jmp QWORD ptr OriginalFuncs_psapi[26 * 8]
		QueryWorkingSetEx endp
else
	.model flat, C
	.stack 4096
	.code
		extern OriginalFuncs_psapi:DWORD
		EmptyWorkingSet proc
			jmp DWORD ptr OriginalFuncs_psapi[0 * 4]
		EmptyWorkingSet endp
		EnumDeviceDrivers proc
			jmp DWORD ptr OriginalFuncs_psapi[1 * 4]
		EnumDeviceDrivers endp
		EnumPageFilesA proc
			jmp DWORD ptr OriginalFuncs_psapi[2 * 4]
		EnumPageFilesA endp
		EnumPageFilesW proc
			jmp DWORD ptr OriginalFuncs_psapi[3 * 4]
		EnumPageFilesW endp
		EnumProcessModules proc
			jmp DWORD ptr OriginalFuncs_psapi[4 * 4]
		EnumProcessModules endp
		EnumProcessModulesEx proc
			jmp DWORD ptr OriginalFuncs_psapi[5 * 4]
		EnumProcessModulesEx endp
		EnumProcesses proc
			jmp DWORD ptr OriginalFuncs_psapi[6 * 4]
		EnumProcesses endp
		GetDeviceDriverBaseNameA proc
			jmp DWORD ptr OriginalFuncs_psapi[7 * 4]
		GetDeviceDriverBaseNameA endp
		GetDeviceDriverBaseNameW proc
			jmp DWORD ptr OriginalFuncs_psapi[8 * 4]
		GetDeviceDriverBaseNameW endp
		GetDeviceDriverFileNameA proc
			jmp DWORD ptr OriginalFuncs_psapi[9 * 4]
		GetDeviceDriverFileNameA endp
		GetDeviceDriverFileNameW proc
			jmp DWORD ptr OriginalFuncs_psapi[10 * 4]
		GetDeviceDriverFileNameW endp
		GetMappedFileNameA proc
			jmp DWORD ptr OriginalFuncs_psapi[11 * 4]
		GetMappedFileNameA endp
		GetMappedFileNameW proc
			jmp DWORD ptr OriginalFuncs_psapi[12 * 4]
		GetMappedFileNameW endp
		GetModuleBaseNameA proc
			jmp DWORD ptr OriginalFuncs_psapi[13 * 4]
		GetModuleBaseNameA endp
		GetModuleBaseNameW proc
			jmp DWORD ptr OriginalFuncs_psapi[14 * 4]
		GetModuleBaseNameW endp
		GetModuleFileNameExA proc
			jmp DWORD ptr OriginalFuncs_psapi[15 * 4]
		GetModuleFileNameExA endp
		GetModuleFileNameExW proc
			jmp DWORD ptr OriginalFuncs_psapi[16 * 4]
		GetModuleFileNameExW endp
		GetModuleInformation proc
			jmp DWORD ptr OriginalFuncs_psapi[17 * 4]
		GetModuleInformation endp
		GetPerformanceInfo proc
			jmp DWORD ptr OriginalFuncs_psapi[18 * 4]
		GetPerformanceInfo endp
		GetProcessImageFileNameA proc
			jmp DWORD ptr OriginalFuncs_psapi[19 * 4]
		GetProcessImageFileNameA endp
		GetProcessImageFileNameW proc
			jmp DWORD ptr OriginalFuncs_psapi[20 * 4]
		GetProcessImageFileNameW endp
		GetProcessMemoryInfo proc
			jmp DWORD ptr OriginalFuncs_psapi[21 * 4]
		GetProcessMemoryInfo endp
		GetWsChanges proc
			jmp DWORD ptr OriginalFuncs_psapi[22 * 4]
		GetWsChanges endp
		GetWsChangesEx proc
			jmp DWORD ptr OriginalFuncs_psapi[23 * 4]
		GetWsChangesEx endp
		InitializeProcessForWsWatch proc
			jmp DWORD ptr OriginalFuncs_psapi[24 * 4]
		InitializeProcessForWsWatch endp
		QueryWorkingSet proc
			jmp DWORD ptr OriginalFuncs_psapi[25 * 4]
		QueryWorkingSet endp
		QueryWorkingSetEx proc
			jmp DWORD ptr OriginalFuncs_psapi[26 * 4]
		QueryWorkingSetEx endp
endif
end