#pragma once
#include <string>
#include <Windows.h>

class Encoding
{
public:
    /// <summary>
    /// Convert system default encoding string to utf8
    /// </summary>
    /// <param name="osStr"></param>
    /// <returns></returns>
    static char* OsToUtf8(const char* osStr);

    /// <summary>
    /// Convert utf8 to system default encoding string 
    /// </summary>
    /// <param name="utf8Str"></param>
    /// <returns></returns>
    static char* Utf8ToOs(const char* utf8Str);
};

