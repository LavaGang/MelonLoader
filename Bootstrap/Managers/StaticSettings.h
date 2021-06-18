#pragma once
#include <stdint.h>

class StaticSettings
{
public:
    static struct Settings_t
    {
        bool safeMode;
    } Settings;

    static bool Initialize();
private:
    static void ReadSettings(char* &buffer, size_t &len);
};
