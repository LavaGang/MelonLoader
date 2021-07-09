#include "InternalCalls.h"
#include "../Utils/Debug.h"
#include "../Utils/Logger.h"
#include "Game.h"
#include "Hook.h"
#include "../Utils/Assertion.h"
#include "../Core.h"
#include "../Utils/HashCode.h"
#include "Il2Cpp.h"
#include "../Utils/Il2CppAssemblyGenerator.h"
#include "../Utils/Encoding.h"

void InternalCalls::Initialize()
{
	Debug::Msg("Initializing Internal Calls...");
	MelonLogger::AddInternalCalls();
	MelonUtils::AddInternalCalls();
	MelonDebug::AddInternalCalls();
	SupportModules::AddInternalCalls();
	IIl2CppAssemblyGenerator::AddInternalCalls();
	IIl2CppAssemblyGenerator::ExecutablePackageBase::AddInternalCalls();
}

#pragma region MelonLogger
void InternalCalls::MelonLogger::Internal_Msg(Console::Color meloncolor, Console::Color txtcolor, Mono::String* namesection, Mono::String* txt)
{
	auto nsStr = namesection != NULL ? Mono::Exports::mono_string_to_utf8(namesection) : NULL;
	auto txtStr = Mono::Exports::mono_string_to_utf8(txt);

	char* nsStrOs = NULL;
	if (nsStr != NULL) 
	{
		nsStrOs = Encoding::Utf8ToOs(nsStr);
		Mono::Free(nsStr); 
	}
	auto txtStrOs = Encoding::Utf8ToOs(txtStr);
	Mono::Free(txtStr);
	Logger::Internal_Msg(meloncolor, txtcolor, nsStrOs, txtStrOs);

	delete[] txtStrOs;
	delete[] nsStrOs;
}

void InternalCalls::MelonLogger::Internal_PrintModName(Console::Color meloncolor, Mono::String* name, Mono::String* version)
{
	auto nameStr = Mono::Exports::mono_string_to_utf8(name);
	auto versionStr = Mono::Exports::mono_string_to_utf8(version);

	auto nameStrOs = Encoding::Utf8ToOs(nameStr);
	auto versionStrOs = Encoding::Utf8ToOs(versionStr);

	Mono::Free(nameStr);
	Mono::Free(versionStr);

	Logger::Internal_PrintModName(meloncolor, nameStrOs, versionStrOs);

	delete[] nameStrOs;
	delete[] versionStrOs;
}

void InternalCalls::MelonLogger::Internal_Warning(Mono::String* namesection, Mono::String* txt)
{
	auto nsStr = namesection != NULL ? Mono::Exports::mono_string_to_utf8(namesection) : NULL;
	auto txtStr = Mono::Exports::mono_string_to_utf8(txt);

	char* nsStrOs = NULL;
	if (nsStr != NULL) 
	{
		nsStrOs = Encoding::Utf8ToOs(nsStr);
		Mono::Free(nsStr);
	}
	auto txtStrOs = Encoding::Utf8ToOs(txtStr);
	Mono::Free(txtStr);

	Logger::Internal_Warning(nsStrOs, txtStrOs);

	delete[] nsStrOs;
	delete[] txtStrOs;
}

void InternalCalls::MelonLogger::Internal_Error(Mono::String* namesection, Mono::String* txt)
{
	auto nsStr = namesection != NULL ? Mono::Exports::mono_string_to_utf8(namesection) : NULL;
	auto txtStr = Mono::Exports::mono_string_to_utf8(txt);

	char* nsStrOs = NULL;
	if (nsStr != NULL)
	{
		nsStrOs = Encoding::Utf8ToOs(nsStr);
		Mono::Free(nsStr);
	}
	auto txtStrOs = Encoding::Utf8ToOs(txtStr);
	Mono::Free(txtStr);

	Logger::Internal_Error(nsStrOs, txtStrOs);

	delete[] nsStrOs;
	delete[] txtStrOs;
}

void InternalCalls::MelonLogger::ThrowInternalFailure(Mono::String* msg)
{
	auto str = Mono::Exports::mono_string_to_utf8(msg);
	auto strOs = Encoding::Utf8ToOs(str);
	Assertion::ThrowInternalFailure(strOs);
	Mono::Free(str);
	delete[] strOs;
}

void InternalCalls::MelonLogger::WriteSpacer() { Logger::WriteSpacer(); }
void InternalCalls::MelonLogger::Flush() { Logger::Flush(); Console::Flush(); }
void InternalCalls::MelonLogger::AddInternalCalls()
{
	Mono::AddInternalCall("MelonLoader.MelonLogger::Internal_PrintModName", Internal_PrintModName);
	Mono::AddInternalCall("MelonLoader.MelonLogger::Internal_Msg", Internal_Msg);
	Mono::AddInternalCall("MelonLoader.MelonLogger::Internal_Warning", Internal_Warning);
	Mono::AddInternalCall("MelonLoader.MelonLogger::Internal_Error", Internal_Error);
	Mono::AddInternalCall("MelonLoader.MelonLogger::ThrowInternalFailure", ThrowInternalFailure);
	Mono::AddInternalCall("MelonLoader.MelonLogger::WriteSpacer", WriteSpacer);
	Mono::AddInternalCall("MelonLoader.MelonLogger::Flush", Flush);
}
#pragma endregion

#pragma region MelonUtils
bool InternalCalls::MelonUtils::IsGame32Bit()
{
#ifdef _WIN64
	return false;
#else
	return true;
#endif
}
bool InternalCalls::MelonUtils::IsGameIl2Cpp() { return Game::IsIl2Cpp; }
bool InternalCalls::MelonUtils::IsOldMono() { return Mono::IsOldMono; }
Mono::String* InternalCalls::MelonUtils::GetApplicationPath() { return Mono::Exports::mono_string_new(Mono::domain, Game::ApplicationPathMono); }
Mono::String* InternalCalls::MelonUtils::GetGameName() { return Mono::Exports::mono_string_new(Mono::domain, Game::Name); }
Mono::String* InternalCalls::MelonUtils::GetGameDeveloper() { return Mono::Exports::mono_string_new(Mono::domain, Game::Developer); }
Mono::String* InternalCalls::MelonUtils::GetGameDirectory() { return Mono::Exports::mono_string_new(Mono::domain, Game::BasePathMono); }
Mono::String* InternalCalls::MelonUtils::GetGameDataDirectory() { return Mono::Exports::mono_string_new(Mono::domain, Game::DataPathMono); }
Mono::String* InternalCalls::MelonUtils::GetUnityVersion() { return Mono::Exports::mono_string_new(Mono::domain, Game::UnityVersion); }
Mono::String* InternalCalls::MelonUtils::GetManagedDirectory() { return Mono::Exports::mono_string_new(Mono::domain, Mono::ManagedPathMono); }
Mono::String* InternalCalls::MelonUtils::GetHashCode() { return Mono::Exports::mono_string_new(Mono::domain, HashCode::Hash.c_str()); }
void InternalCalls::MelonUtils::SCT(Mono::String* title)
{
	if (title == NULL) return;
	auto str = Mono::Exports::mono_string_to_utf8(title);
	Console::SetTitle(str);
	Mono::Free(str);
}
Mono::String* InternalCalls::MelonUtils::GetFileProductName(Mono::String* filepath)
{
	char* filepathstr = Mono::Exports::mono_string_to_utf8(filepath);
	if (filepathstr == NULL)
		return NULL;
	const char* info = Core::GetFileInfoProductName(filepathstr);
	Mono::Free(filepathstr);
	if (info == NULL)
		return NULL;
	return Mono::Exports::mono_string_new(Mono::domain, info);
}

void InternalCalls::MelonUtils::AddInternalCalls()
{
	Mono::AddInternalCall("MelonLoader.MelonUtils::IsGame32Bit", IsGame32Bit);
	Mono::AddInternalCall("MelonLoader.MelonUtils::IsGameIl2Cpp", IsGameIl2Cpp);
	Mono::AddInternalCall("MelonLoader.MelonUtils::IsOldMono", IsOldMono);
	Mono::AddInternalCall("MelonLoader.MelonUtils::GetApplicationPath", GetApplicationPath);
	Mono::AddInternalCall("MelonLoader.MelonUtils::GetGameDataDirectory", GetGameDataDirectory);
	Mono::AddInternalCall("MelonLoader.MelonUtils::GetUnityVersion", GetUnityVersion);
	Mono::AddInternalCall("MelonLoader.MelonUtils::GetManagedDirectory", GetManagedDirectory);
	Mono::AddInternalCall("MelonLoader.MelonUtils::SetConsoleTitle", SCT);
	Mono::AddInternalCall("MelonLoader.MelonUtils::GetFileProductName", GetFileProductName);
	Mono::AddInternalCall("MelonLoader.MelonUtils::NativeHookAttach", Hook::Attach);
	Mono::AddInternalCall("MelonLoader.MelonUtils::NativeHookDetach", Hook::Detach);

	Mono::AddInternalCall("MelonLoader.MelonUtils::Internal_GetGameName", GetGameName);
	Mono::AddInternalCall("MelonLoader.MelonUtils::Internal_GetGameDeveloper", GetGameDeveloper);
	Mono::AddInternalCall("MelonLoader.MelonUtils::Internal_GetGameDirectory", GetGameDirectory);
	Mono::AddInternalCall("MelonLoader.MelonUtils::Internal_GetHashCode", GetHashCode);
}
#pragma endregion

#pragma region MelonDebug
void InternalCalls::MelonDebug::Internal_Msg(Console::Color meloncolor, Console::Color txtcolor, Mono::String* namesection, Mono::String* txt)
{
	auto nsStr = namesection != NULL ? Mono::Exports::mono_string_to_utf8(namesection) : NULL;
	auto txtStr = Mono::Exports::mono_string_to_utf8(txt);

	char* nsStrOs = NULL;
	if (nsStr != NULL) 
	{
		nsStrOs = Encoding::Utf8ToOs(nsStr);
		Mono::Free(nsStr); 
	}
	auto txtStrOs = Encoding::Utf8ToOs(txtStr);
	Mono::Free(txtStr);
	Debug::Internal_Msg(meloncolor, txtcolor, nsStrOs, txtStrOs);

	delete[] txtStrOs;
	delete[] nsStrOs;
}
void InternalCalls::MelonDebug::AddInternalCalls()
{
	Mono::AddInternalCall("MelonLoader.MelonDebug::Internal_Msg", Internal_Msg);
}
#pragma endregion

#pragma region SupportModules
void InternalCalls::SupportModules::SetDefaultConsoleTitleWithGameName(Mono::String* GameVersion) { Console::SetDefaultTitleWithGameName(GameVersion != NULL ? Mono::Exports::mono_string_to_utf8(GameVersion) : NULL); }
void InternalCalls::SupportModules::AddInternalCalls()
{
	Mono::AddInternalCall("MelonLoader.Support.Preload::GetManagedDirectory", MelonUtils::GetManagedDirectory);
	Mono::AddInternalCall("MelonLoader.Support.Main::SetDefaultConsoleTitleWithGameName", SetDefaultConsoleTitleWithGameName);
}
#pragma endregion

#pragma region AssemblyGenerator
void InternalCalls::IIl2CppAssemblyGenerator::AddInternalCalls()
{
	Mono::AddInternalCall("MelonLoader.Il2CppAssemblyGenerator::EnableCloseButton", Console::EnableCloseButton);
	Mono::AddInternalCall("MelonLoader.Il2CppAssemblyGenerator::DisableCloseButton", Console::DisableCloseButton);
	ExecutablePackageBase::AddInternalCalls();
}

#pragma region ExecutablePackageBase
void InternalCalls::IIl2CppAssemblyGenerator::ExecutablePackageBase::SetProcessId(int id) { Il2CppAssemblyGenerator::ProcessId = id; }
void InternalCalls::IIl2CppAssemblyGenerator::ExecutablePackageBase::AddInternalCalls()
{
	Mono::AddInternalCall("MelonLoader.Il2CppAssemblyGenerator.ExecutablePackageBase::SetProcessId", SetProcessId);
}
#pragma endregion

#pragma endregion
