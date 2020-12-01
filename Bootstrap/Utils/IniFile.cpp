#include "IniFile.h"
#include <Windows.h>

IniFile::IniFile(std::string filepath) { FilePath = filepath; }
void IniFile::WriteValue(std::string Section, std::string Key, std::string Value) { WritePrivateProfileStringA(Section.c_str(), Key.c_str(), Value.c_str(), FilePath.c_str()); }
std::string IniFile::ReadValue(std::string Section, std::string Key) { TCHAR result[1023]; GetPrivateProfileStringA(Section.c_str(), Key.c_str(), " _", result, 1023, FilePath.c_str()); return result; }