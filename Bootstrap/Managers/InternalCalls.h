#pragma once
#include "Mono.h"
#include "../Utils/Console.h"

class InternalCalls
{
public:
	static void Initialize();

	class MelonDebug
	{
	public:
		static void AddInternalCalls();
		static bool IsEnabled();
	};

	class MelonLogger
	{
	public:
		static void AddInternalCalls();
		static void Internal_PrintModName(Console::Color meloncolor, Console::Color authorcolor, Mono::String* name, Mono::String* author, Mono::String* version, Mono::String* id);
		static void Internal_Msg(Console::Color meloncolor, Console::Color txtcolor, Mono::String* namesection, Mono::String* txt);
	    static void Internal_Warning(Mono::String* namesection, Mono::String* txt);
		static void Internal_Error(Mono::String* namesection, Mono::String* txt);
		static void ThrowInternalFailure(Mono::String* msg);
		static void WriteSpacer();
		static void Flush();
	};

	class MelonUtils
	{
	public:
		static void AddInternalCalls();
		static bool IsGame32Bit();
		static bool IsGameIl2Cpp();
		static bool IsOldMono();
		static Mono::String* GetApplicationPath();
		static Mono::String* GetBaseDirectory();
		static Mono::String* GetGameDirectory();
		static Mono::String* GetGameDataDirectory();
		static Mono::String* GetManagedDirectory();
		static Mono::String* GetHashCode();
		static void SCT(Mono::String* title);
		static Mono::String* GetFileProductName(Mono::String* filepath);
		static void* GetLibPtr();
		static void* GetRootDomainPtr();
		static Mono::ReflectionAssembly* CastManagedAssemblyPtr(void* ptr);
	};

	class UnityInformationHandler
	{
	public:
		static void AddInternalCalls();
		static void SetDefaultConsoleTitleWithGameName(Mono::String* GameName, Mono::String* GameVersion);
	};

	class IIl2CppAssemblyGenerator
	{
	public:
		static void AddInternalCalls();

		class ExecutablePackageBase
		{
		public:
			static void AddInternalCalls();
			static void SetProcessId(int id);
		};
	};
};