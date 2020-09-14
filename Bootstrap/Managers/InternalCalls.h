#pragma once
#include "Mono.h"

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
		static void Internal_Msg(Mono::String* namesection, Mono::String* txt);
	    static void Internal_Warning(Mono::String* namesection, Mono::String* txt);
		static void Internal_Error(Mono::String* namesection, Mono::String* txt);
		static void ThrowInternalFailure(Mono::String* msg);
		static void WriteSpacer();
	};

	class MelonUtils
	{
	public:
		static void AddInternalCalls();
		static bool IsGameIl2Cpp();
		static Mono::String* GetApplicationPath();
		static Mono::String* GetGameName();
		static Mono::String* GetGameDeveloper();
		static Mono::String* GetGameDirectory();
		static Mono::String* GetGameDataDirectory();
		static Mono::String* GetUnityVersion();
		static Mono::String* GetManagedDirectory();
		static void SCT(Mono::String* title);

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
	};
};