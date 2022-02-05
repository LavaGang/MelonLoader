#pragma once
#ifndef PORT_DISABLE
#include <string>

class IniFile
{
public:
	IniFile(std::string filepath);
	void WriteValue(std::string Section, std::string Key, std::string Patch);
	std::string ReadValue(std::string Section, std::string Key);
private:
	std::string FilePath;
};
#endif