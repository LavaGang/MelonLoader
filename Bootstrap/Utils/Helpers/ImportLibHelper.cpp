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
#endif