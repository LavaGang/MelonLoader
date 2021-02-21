//
// Created by westside on 2019-04-02.
//

#ifndef HAPTIC_LIBRARY_COMMONUTILS_H
#define HAPTIC_LIBRARY_COMMONUTILS_H


#include "hapticsManager.h"
#include "json.hpp"

namespace bhaptics {
    struct commonutils {
        static void from_json(const nlohmann::json& j, PlayerResponse& p);
        static std::string posToString(const PositionType positionType);
        static	bool TryParseToPosition(const std::string deviceName, PositionType &pos);
    };
}

#endif //HAPTIC_LIBRARY_COMMONUTILS_H
