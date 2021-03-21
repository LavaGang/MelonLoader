#include "AssemblyGenerator.h"

char* AssemblyGenerator::PathMono = NULL;
int AssemblyGenerator::ProcessId = 0;

void AssemblyGenerator::Cleanup()
{
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
}