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

void AssertionManager::ThrowError(std::string msg, const char* filepath)
{
	if (!Result)
	{
		Result = true;
		msg += ("for [ " + std::string(FileName) + " | " + Position + " ]");
		if (filepath != NULL)
			msg += " in { " + std::string(filepath) + "}";
		Logger::LogError(msg);
		if (MelonLoader::DebugMode)
			MessageBox(NULL, msg.c_str(), "MelonLoader - INTERNAL FAILURE", MB_OK | MB_ICONERROR);
		else
			MessageBox(NULL, "Please Post your Latest Log File\non #internal-failure in the MelonLoader Discord!", "MelonLoader - INTERNAL FAILURE!", MB_OK | MB_ICONERROR);
		MelonLoader::UNLOAD();
	}
}

void AssertionManager::Decide(void* thing, const char* name)
{
	if (!Result && (thing == NULL))
		ThrowError((std::string(name) + " is NULL for [ " + FileName + " | " + Position + " ]"));
}

HMODULE AssertionManager::LoadLib(const char* name, const char* filepath)
{
	HMODULE returnval = NULL;
	if (!Result)
	{
		returnval = LoadLibrary(filepath);
		if (returnval == NULL)
			ThrowError((std::string("Failed to LoadLib ( ") + name + " )"));
	}
	return returnval;
}

HMODULE AssertionManager::GetModuleHandlePtr(const char* name)
{
	HMODULE returnval = NULL;
	if (!Result)
	{
		returnval = GetModuleHandle(name);
		if (returnval == NULL)
			ThrowError((std::string("Failed to GetModuleHandlePtr ( ") + name + " )"));
	}
	return returnval;
}

FARPROC AssertionManager::GetExport(HMODULE mod, const char* export_name)
{
	FARPROC returnval = NULL;
	if (!Result)
	{
		returnval = GetProcAddress(mod, export_name);
		if (returnval == NULL)
			ThrowError((std::string("Failed to GetExport ( ") + export_name + " )"));
	}
	return returnval;
}

uintptr_t AssertionManager::FindPattern(HMODULE mod, const char* name, const char* target_pattern)
{
	uintptr_t returnval = NULL;
	if (!Result)
	{
		returnval = PointerUtils::FindPattern(mod, target_pattern);
		if (returnval == NULL)
			ThrowError((std::string("Failed to FindPattern ( ") + name + " )"));
	}
	return returnval;
}

std::vector<uintptr_t> AssertionManager::FindAllPattern(HMODULE mod, const char* name, const char* target_pattern)
{
	std::vector<uintptr_t> returnval;
	if (!Result)
	{
		returnval = PointerUtils::FindAllPattern(mod, target_pattern);
		if (returnval.size() < 1)
			ThrowError((std::string("Failed to FindAllPattern ( ") + name + " )"));
	}
	return returnval;
}