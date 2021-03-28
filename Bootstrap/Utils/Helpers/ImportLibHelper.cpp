#include "../../pch.h"
#include "ImportLibHelper.h"
#include "../Assertion.h"
#include "../Console/Debug.h"

#ifdef __ANDROID__
#include <dlfcn.h>
#endif

#ifdef _WIN32
FARPROC ImportLibHelper::GetExport(HMODULE mod, const char* export_name)
{
	if (!ShouldContinue)
		return NULL;
	Debug::Msg(export_name);
	FARPROC returnval = GetProcAddress(mod, export_name);
	if (returnval == NULL)
		ThrowInternalFailure((std::string("Failed to GetExport ( ") + export_name + " )").c_str());
	return returnval;
}
#elif defined(__ANDROID__)
void* ImportLibHelper::GetExport(void* mod, const char* export_name)
{
	if (!Assertion::ShouldContinue)
		return NULL;
	
	Debug::Msg(export_name);
	
	void* fnPointer = dlsym(mod, export_name);

	// no need to return since already nullptr
	if (fnPointer == nullptr)
		Assertion::ThrowInternalFailure(dlerror());

	return fnPointer;
}

void* ImportLibHelper::GetInternalExport(void* mod, const char* ref_name, uint64_t ref_lib_addr, uint64_t dest_lib_addr)
{
	void* ref = GetExport(mod, ref_name);
	if (!Assertion::ShouldContinue)
		return NULL;

	Dl_info func_info;
	dladdr(ref, &func_info);

	size_t addr_named = (size_t)func_info.dli_saddr;
	uint64_t difference = addr_named - ref_lib_addr;
	size_t rawPtr = addr_named + difference;
	void* fnPointer = (void*)(rawPtr);

	if (fnPointer == NULL)
		Assertion::ThrowInternalFailure(dlerror());
	
	return fnPointer;
}
#endif