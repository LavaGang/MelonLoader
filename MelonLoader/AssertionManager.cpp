#include "AssertionManager.h"
#include "MelonLoader.h"
#include "PointerUtils.h"
#include "Logger.h"

bool AssertionManager::Result = false;
const char* AssertionManager::FileName = NULL;
const char* AssertionManager::Position = NULL;

void AssertionManager::Start(const char* filename, const char* position)
{
	FileName = filename;
	Position = position;
}

void AssertionManager::Decide(void* thing, const char* name)
{
	if (!Result)
	{
		if (thing == NULL)
		{
			Result = true;
			Logger::LogError((std::string(name) + " is NULL for [ " + FileName + " | " + Position + " ]"));

			if (MelonLoader::DebugMode)
				MessageBox(NULL, (std::string(name) + " is NULL for [ " + FileName + " | " + Position + " ]").c_str(), "MelonLoader - INTERNAL FAILURE", MB_OK | MB_ICONERROR);
			else
				MessageBox(NULL, "Please Post your Latest Log File\non #internal-failure in the MelonLoader Discord!", "INTERNAL FAILURE!", MB_OK | MB_ICONERROR);

			MelonLoader::UNLOAD();
		}
	}
}

HMODULE AssertionManager::LoadLib(const char* name, const char* filepath)
{
	if (!Result)
	{
		HMODULE returnval = LoadLibrary(filepath);
		if (returnval == NULL)
		{
			Result = true;
			Logger::LogError((std::string("Failed to LoadLib ( ") + name + " ) for [ " + FileName + " | " + Position + " ] in { " + filepath + "}"));

			if (MelonLoader::DebugMode)
				MessageBox(NULL, (std::string("Failed to LoadLib ( ") + name + " ) for [ " + FileName + " | " + Position + " ] in { " + filepath + "}").c_str(), "MelonLoader - INTERNAL FAILURE", MB_OK | MB_ICONERROR);
			else
				MessageBox(NULL, "Please Post your Latest Log File\non #internal-failure in the MelonLoader Discord!", "INTERNAL FAILURE!", MB_OK | MB_ICONERROR);

			MelonLoader::UNLOAD();
		}
		return returnval;
	}
	return NULL;
}

HMODULE AssertionManager::GetModuleHandlePtr(const char* name)
{
	if (!Result)
	{
		HMODULE returnval = GetModuleHandle(name);
		if (returnval == NULL)
		{
			Result = true;
			Logger::LogError((std::string("Failed to LoadLib ( ") + name + " ) for [ " + FileName + " | " + Position + " ]"));

			if (MelonLoader::DebugMode)
				MessageBox(NULL, (std::string("Failed to LoadLib ( ") + name + " ) for [ " + FileName + " | " + Position + " ]").c_str(), "MelonLoader - INTERNAL FAILURE", MB_OK | MB_ICONERROR);
			else
				MessageBox(NULL, "Please Post your Latest Log File\non #internal-failure in the MelonLoader Discord!", "INTERNAL FAILURE!", MB_OK | MB_ICONERROR);

			MelonLoader::UNLOAD();
		}
		return returnval;
	}
	return NULL;
}

FARPROC AssertionManager::GetExport(HMODULE mod, const char* export_name)
{
	if (!Result)
	{
		FARPROC returnval = GetProcAddress(mod, export_name);
		if (returnval == NULL)
		{
			Result = true;
			Logger::LogError((std::string("Failed to GetExport ( ") + export_name + " ) for [ " + FileName + " | " + Position + " ]"));

			if (MelonLoader::DebugMode)
				MessageBox(NULL, (std::string("Failed to GetExport ( ") + export_name + " ) for [ " + FileName + " | " + Position + " ]").c_str(), "MelonLoader - INTERNAL FAILURE", MB_OK | MB_ICONERROR);
			else
				MessageBox(NULL, "Please Post your Latest Log File\non #internal-failure in the MelonLoader Discord!", "INTERNAL FAILURE!", MB_OK | MB_ICONERROR);

			MelonLoader::UNLOAD();
		}
		return returnval;
	}
	return NULL;
}

uintptr_t AssertionManager::FindPattern(HMODULE mod, const char* name, const char* target_pattern)
{
	if (!Result)
	{
		uintptr_t returnval = PointerUtils::FindPattern(mod, target_pattern);
		if (returnval == NULL)
		{
			Result = true;
			Logger::LogError((std::string("Failed to FindPattern ( ") + name + " ) for [ " + FileName + " | " + Position + " ]"));

			if (MelonLoader::DebugMode)
				MessageBox(NULL, (std::string("Failed to FindPattern ( ") + name + " ) for [ " + FileName + " | " + Position + " ]").c_str(), "MelonLoader - INTERNAL FAILURE!", MB_OK | MB_ICONERROR);
			else
				MessageBox(NULL, "Please Post your Latest Log File\non #internal-failure in the MelonLoader Discord!", "INTERNAL FAILURE!", MB_OK | MB_ICONERROR);

			MelonLoader::UNLOAD();
		}
		return returnval;
	}
	return NULL;
}