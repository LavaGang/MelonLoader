#pragma once
#ifdef PORT_DISABLE
#include "Mono.h"
#include "../Utils/Console.h"

class InternalCalls
{
public:
	static void Initialize();

	class MelonCore
	{
	public:
		static void AddInternalCalls();
		static bool QuitFix();
	};

	class MelonLogger
	{
	public:
		static void AddInternalCalls();
		static void Internal_PrintModName(Console::Color color, Mono::String* name, Mono::String* version);
		static void Internal_Msg(Console::Color color, Mono::String* namesection, Mono::String* txt);
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
		static Mono::String* GetGameName();
		static Mono::String* GetGameDeveloper();
		static Mono::String* GetGameDirectory();
		static Mono::String* GetGameDataDirectory();
		static Mono::String* GetUnityVersion();
		static Mono::String* GetManagedDirectory();
		static void SCT(Mono::String* title);
		static Mono::String* GetFileProductName(Mono::String* filepath);
	};

	class MelonHandler
	{
	public:
		static void AddInternalCalls();
		enum LoadMode
		{
			NORMAL,
			DEV,
			BOTH
		};
		static LoadMode LoadModeForPlugins;
		static LoadMode LoadModeForMods;
		static LoadMode GetLoadModeForPlugins();
		static LoadMode GetLoadModeForMods();
	};

	class MelonDebug
	{
	public:
		static void AddInternalCalls();
		static bool IsEnabled();
		static void Internal_Msg(Console::Color color, Mono::String* namesection, Mono::String* txt);
	};
};
#endif