//Copyright bHaptics Inc. 2017-2019
#ifndef BHAPTICS_UTIL
#define BHAPTICS_UTIL

#include "shared/model.h"
    class Util
    {
    public:
        static std::string readFile(const std::string& path);
        static bhaptics::HapticFile parse(const std::string& path);
        static bhaptics::HapticFile parseFromTactFileString(const std::string& jsonStr);
    };

#endif