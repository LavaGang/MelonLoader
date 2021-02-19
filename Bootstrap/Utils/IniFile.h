#pragma once
#ifndef PORT_TODO_DISABLE
#include <string>

class IniFile
{
public:
	IniFile(std::string filepath);
	void WriteValue(std::string Section, std::string Key, std::string Value);
	std::string ReadValue(std::string Section, std::string Key);
private:
	std::string FilePath;
};
#endif