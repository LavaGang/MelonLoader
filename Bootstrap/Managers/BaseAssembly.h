#pragma once

class BaseAssembly
{
public:
	static char* PathMono;
	static bool Initialize();
	static void Preload();
	static bool PreStart();
	static void Start();
};