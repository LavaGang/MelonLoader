#include "BaseAssembly.h"
#include "Game.h"
#include "DotnetRuntime.h"
#include "Mono.h"

bool BaseAssembly::Initialize()
{
	if (Game::IsIl2Cpp)
	{
		DotnetRuntime::LoadDotNet();
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