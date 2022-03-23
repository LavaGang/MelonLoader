#include "BaseAssembly.h"
#include "../Utils/Debug.h"
#include "../Core.h"
#include "Game.h"
#include <string>
#include "../Utils/Assertion.h"
#include "../Utils/Logging/Logger.h"
#include "../Utils/Il2CppAssemblyGenerator.h"
#include "DotnetRuntime.h"

char* BaseAssembly::PathMono = NULL;

bool BaseAssembly::Initialize()
{
	if (Game::IsIl2Cpp) 
	{
		DotnetRuntime::CallInitialize();
		return true;
	}

	return Mono::InvokeInitialize();
}

bool BaseAssembly::PreStart()
{
	if (Game::IsIl2Cpp)
	{
		DotnetRuntime::CallPreStart();
		return true;
	}

	return Mono::InvokePreStart();
}

void BaseAssembly::Start()
{
	if (Game::IsIl2Cpp)
	{
		DotnetRuntime::CallStart();
		return;
	}

	Mono::InvokeStart();
}