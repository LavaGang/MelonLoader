#include "InternalCalls.h"
#include "../Utils/Debug.h"
#include "../Utils/Logging/Logger.h"
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
	MelonDebug::AddInternalCalls();
	MelonLogger::AddInternalCalls();
	MelonUtils::AddInternalCalls();
	UnityInformationHandler::AddInternalCalls();
	IIl2CppAssemblyGenerator::AddInternalCalls();
}

#pragma region MelonDebug
bool InternalCalls::MelonDebug::IsEnabled() { return Debug::Enabled; }
void InternalCalls::MelonDebug::AddInternalCalls()
{
	Mono::AddInternalCall("MelonLoader.MelonDebug::IsEnabled", IsEnabled);
}
#pragma endregion

#pragma region MelonLogger
void InternalCalls::MelonLogger::Internal_Msg(Console::Color meloncolor, Console::Color txtcolor, Mono::String* namesection, Mono::String* txt)
{
	auto txtStr = Mono::Exports::mono_string_to_utf8(txt);
	auto txtStrOs = Encoding::Utf8ToOs(txtStr);
	Mono::Free(txtStr);

	char* nsStrOs = NULL;
	if (namesection != NULL)
	{
		auto nsStr = Mono::Exports::mono_string_to_utf8(namesection);
		nsStrOs = Encoding::Utf8ToOs(nsStr);
		Mono::Free(nsStr); 
	}

	Logger::Internal_Msg(meloncolor, txtcolor, nsStrOs, txtStrOs);

	delete[] txtStrOs;
	delete[] nsStrOs;
}

void InternalCalls::MelonLogger::Internal_PrintModName(Console::Color meloncolor, Console::Color authorcolor, Mono::String* name, Mono::String* author, Mono::String* version, Mono::String* id)
{
	auto nameStr = Mono::Exports::mono_string_to_utf8(name);
	auto nameStrOs = Encoding::Utf8ToOs(nameStr);
	Mono::Free(nameStr);

	auto versionStr = Mono::Exports::mono_string_to_utf8(version);
	auto versionStrOs = Encoding::Utf8ToOs(versionStr);
	Mono::Free(versionStr);

	char* idStrOs = NULL;
	if (id != NULL)
	{
		auto idStr = Mono::Exports::mono_string_to_utf8(id);
		idStrOs = Encoding::Utf8ToOs(idStr);
		Mono::Free(idStr);
	}

	char* authorStrOs = NULL;
	if (author != NULL)
	{
		auto authorStr = Mono::Exports::mono_string_to_utf8(author);
		authorStrOs = Encoding::Utf8ToOs(authorStr);
		Mono::Free(authorStr);
	}

	Logger::Internal_PrintModName(meloncolor, authorcolor, nameStrOs, authorStrOs, versionStrOs, idStrOs);

	delete[] nameStrOs;
	delete[] versionStrOs;
	if (idStrOs != NULL)
		delete[] idStrOs;
	if (authorStrOs != NULL)
		delete[] authorStrOs;
}

void InternalCalls::MelonLogger::Internal_Warning(Mono::String* namesection, Mono::String* txt)
{
	auto txtStr = Mono::Exports::mono_string_to_utf8(txt);
	auto txtStrOs = Encoding::Utf8ToOs(txtStr);
	Mono::Free(txtStr);

	char* nsStrOs = NULL;
	if (namesection != NULL)
	{
		auto nsStr = Mono::Exports::mono_string_to_utf8(namesection);
		nsStrOs = Encoding::Utf8ToOs(nsStr);
		Mono::Free(nsStr);
	}

	Logger::Internal_Warning(nsStrOs, txtStrOs);

	delete[] nsStrOs;
	delete[] txtStrOs;
}

void InternalCalls::MelonLogger::Internal_Error(Mono::String* namesection, Mono::String* txt)
{
	auto txtStr = Mono::Exports::mono_string_to_utf8(txt);
	auto txtStrOs = Encoding::Utf8ToOs(txtStr);
	Mono::Free(txtStr);

	char* nsStrOs = NULL;
	if (namesection != NULL)
	{
		auto nsStr = Mono::Exports::mono_string_to_utf8(namesection);
		nsStrOs = Encoding::Utf8ToOs(nsStr);
		Mono::Free(nsStr);
	}

	Logger::Internal_Error(nsStrOs, txtStrOs);

	delete[] nsStrOs;
	delete[] txtStrOs;
}

void InternalCalls::MelonLogger::ThrowInternalFailure(Mono::String* msg)
{
	auto str = Mono::Exports::mono_string_to_utf8(msg);
	auto strOs = Encoding::Utf8ToOs(str);
	Mono::Free(str);

	Assertion::ThrowInternalFailure(strOs);

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
Mono::String* InternalCalls::MelonUtils::GetBaseDirectory() { return Mono::Exports::mono_string_new(Mono::domain, Core::BasePathMono); }
Mono::String* InternalCalls::MelonUtils::GetGameDirectory() { return Mono::Exports::mono_string_new(Mono::domain, Game::BasePathMono); }
Mono::String* InternalCalls::MelonUtils::GetGameDataDirectory() { return Mono::Exports::mono_string_new(Mono::domain, Game::DataPathMono); }
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

void* InternalCalls::MelonUtils::GetLibPtr() { return Mono::Module; }
void* InternalCalls::MelonUtils::GetRootDomainPtr() { return Mono::domain; }
Mono::ReflectionAssembly* InternalCalls::MelonUtils::CastManagedAssemblyPtr(void* ptr) { return (Mono::ReflectionAssembly*)ptr; }

void InternalCalls::MelonUtils::AddInternalCalls()
{
	Mono::AddInternalCall("MelonLoader.MelonUtils::IsGame32Bit", IsGame32Bit);
	Mono::AddInternalCall("MelonLoader.MelonUtils::IsGameIl2Cpp", IsGameIl2Cpp);
	Mono::AddInternalCall("MelonLoader.MelonUtils::IsOldMono", IsOldMono);
	Mono::AddInternalCall("MelonLoader.MelonUtils::IsUnderWineOrSteamProton", Core::IsRunningInWine);
	
	Mono::AddInternalCall("MelonLoader.MelonUtils::GetApplicationPath", GetApplicationPath);
	Mono::AddInternalCall("MelonLoader.MelonUtils::GetGameDataDirectory", GetGameDataDirectory);
	Mono::AddInternalCall("MelonLoader.MelonUtils::GetManagedDirectory", GetManagedDirectory);
	Mono::AddInternalCall("MelonLoader.MelonUtils::SetConsoleTitle", SCT);
	Mono::AddInternalCall("MelonLoader.MelonUtils::GetFileProductName", GetFileProductName);
	Mono::AddInternalCall("MelonLoader.MelonUtils::NativeHookAttach", Hook::Attach);
	Mono::AddInternalCall("MelonLoader.MelonUtils::NativeHookDetach", Hook::Detach);

	Mono::AddInternalCall("MelonLoader.MelonUtils::Internal_GetBaseDirectory", GetBaseDirectory);
	Mono::AddInternalCall("MelonLoader.MelonUtils::Internal_GetGameDirectory", GetGameDirectory);
	Mono::AddInternalCall("MelonLoader.MelonUtils::Internal_GetHashCode", GetHashCode);

	Mono::AddInternalCall("MelonLoader.Support.Preload::GetManagedDirectory", GetManagedDirectory);

	Mono::AddInternalCall("MelonLoader.MonoInternals.MonoLibrary::GetLibPtr", GetLibPtr);
	Mono::AddInternalCall("MelonLoader.MonoInternals.MonoLibrary::GetRootDomainPtr", GetRootDomainPtr);
	Mono::AddInternalCall("MelonLoader.MonoInternals.MonoLibrary::CastManagedAssemblyPtr", CastManagedAssemblyPtr);
	Mono::AddInternalCall("MelonLoader.MonoInternals.ResolveInternals.AssemblyManager::InstallHooks", Mono::InstallAssemblyHooks);
}
#pragma endregion

#pragma region UnityInformationHandler
void InternalCalls::UnityInformationHandler::SetDefaultConsoleTitleWithGameName(Mono::String* GameName, Mono::String* GameVersion)
{
	if (GameName == NULL)
		return;
	Console::SetDefaultTitleWithGameName(Mono::Exports::mono_string_to_utf8(GameName),
		(GameVersion != NULL ? Mono::Exports::mono_string_to_utf8(GameVersion) : NULL)); 
}
void InternalCalls::UnityInformationHandler::AddInternalCalls()
{
	Mono::AddInternalCall("MelonLoader.InternalUtils.UnityInformationHandler::SetDefaultConsoleTitleWithGameName", SetDefaultConsoleTitleWithGameName);
}
#pragma endregion

#pragma region AssemblyGenerator
void InternalCalls::IIl2CppAssemblyGenerator::AddInternalCalls()
{
	Mono::AddInternalCall("MelonLoader.InternalUtils.Il2CppAssemblyGenerator::EnableCloseButton", Console::EnableCloseButton);
	Mono::AddInternalCall("MelonLoader.InternalUtils.Il2CppAssemblyGenerator::DisableCloseButton", Console::DisableCloseButton);
	ExecutablePackageBase::AddInternalCalls();
}

#pragma region ExecutablePackageBase
void InternalCalls::IIl2CppAssemblyGenerator::ExecutablePackageBase::SetProcessId(int id) { Il2CppAssemblyGenerator::ProcessId = id; }
void InternalCalls::IIl2CppAssemblyGenerator::ExecutablePackageBase::AddInternalCalls()
{
	Mono::AddInternalCall("MelonLoader.Il2CppAssemblyGenerator.Packages.Models.PackageBase::ThrowInternalFailure", InternalCalls::MelonLogger::ThrowInternalFailure);
	Mono::AddInternalCall("MelonLoader.Il2CppAssemblyGenerator.Packages.Models.ExecutablePackage::SetProcessId", SetProcessId);
}
#pragma endregion

#pragma endregion
