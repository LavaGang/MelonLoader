#pragma once
#include <Windows.h>

class Core
{
public:
	static void Load(HINSTANCE hinstDLL);

private:
	static void ApplicationCheck();
	static void KillItDead();
	static void LoadExports_version(HMODULE originaldll);
	static void LoadExports_winmm(HMODULE originaldll);
	static void LoadExports_winhttp(HMODULE originaldll);
};