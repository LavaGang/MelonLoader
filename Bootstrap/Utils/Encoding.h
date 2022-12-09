#pragma once

class Encoding
{
public:
    /// <summary>
    /// Convert system default encoding string to utf8
    /// </summary>
    /// <param name="osStr"></param>
    /// <returns></returns>
    static char* OsToUtf8(const char* osStr);
};

