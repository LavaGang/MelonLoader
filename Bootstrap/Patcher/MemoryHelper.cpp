#pragma once
#include <stdbool.h>
#include <unistd.h>
#include <sys/mman.h>
#include <string.h>

// possible bug where if a patch is longer than the page
bool patcher_memory_permissions(const void* address, size_t len, int prot)
{
	size_t pagesize = sysconf(_SC_PAGESIZE);
	uintptr_t start = (uintptr_t)address;
	uintptr_t end = start + len;
	uintptr_t pagestart = start & -pagesize;
	return mprotect(
		(void*)pagestart,
		end - pagestart, // pagesize may be sufficient
		prot) < 0;
}

bool patcher_memory_protect(const void* address, size_t len)
{
	return patcher_memory_permissions(address, len, PROT_READ | PROT_EXEC);
}

bool patcher_memory_unprotect(const void* address, size_t len)
{
	return patcher_memory_permissions(address, len, PROT_READ | PROT_WRITE | PROT_EXEC);
}

bool patcher_memory_write(void* address, const char* data, size_t len)
{
	if (!patcher_memory_unprotect(address, len))
		return false;

	memcpy(address, data, len);
	
	return patcher_memory_protect(address, len);
}

void patcher_memory_read(void* address, char** data, size_t len)
{
	memcpy(*data, address, len);
}