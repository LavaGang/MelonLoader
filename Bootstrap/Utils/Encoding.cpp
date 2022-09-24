#include "Encoding.h"
#include <string>
#include <Windows.h>

char* Encoding::OsToUtf8(const char* osStr)
{
	// Convert to UTF16
	int len = MultiByteToWideChar(CP_ACP, 0, osStr, -1, NULL, 0);
	wchar_t* wstr = new wchar_t[len + 1];
	memset(wstr, 0, len + 1);
	MultiByteToWideChar(CP_ACP, 0, osStr, -1, wstr, len);

	// Convert UTF16 to UTF8
	len = WideCharToMultiByte(CP_UTF8, 0, wstr, -1, NULL, 0, NULL, NULL);
	char* str = new char[len + 1];
	memset(str, 0, len + 1);
	WideCharToMultiByte(CP_UTF8, 0, wstr, -1, str, len, NULL, NULL);
	
	delete[] wstr;
	return str;
	
}