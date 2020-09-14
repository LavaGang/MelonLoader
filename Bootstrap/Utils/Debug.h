#pragma once

class Debug
{
public:
	static bool Enabled;
	
	static void Msg(const char* txt);

	static void Internal_Msg(const char* namesection, const char* txt);
};