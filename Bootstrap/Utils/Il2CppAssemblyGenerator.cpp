#include "Il2CppAssemblyGenerator.h"

#ifdef _WIN32
#include <Windows.h>
#else
#include <cstddef>
#endif

char* Il2CppAssemblyGenerator::PathMono = NULL;
int Il2CppAssemblyGenerator::ProcessId = 0;

void Il2CppAssemblyGenerator::Cleanup()
{
#if _WIN32
	if (ProcessId != 0)
	{
		HANDLE hProcess = OpenProcess(PROCESS_TERMINATE, FALSE, ProcessId);
		if (hProcess != NULL)
		{
			BOOL result = TerminateProcess(hProcess, 0);
			CloseHandle(hProcess);
		}
		ProcessId = 0;
	}
#endif
}