//
// Created by westside on 2019-04-02.
//

#include "commonutils.h"

namespace bhaptics
{

    void commonutils::from_json(const nlohmann::json &j, PlayerResponse &p) {
        p.RegisteredKeys = j.at("RegisteredKeys").get<std::vector<std::string>>();
        p.ActiveKeys = j.at("ActiveKeys").get<std::vector<std::string>>();
        p.ConnectedDeviceCount = j.at("ConnectedDeviceCount").get<int>();

        std::vector<std::string> positionStr = j.at("ConnectedPositions").get<std::vector<std::string>>();

        for (size_t i = 0; i < positionStr.size(); i++)
        {
            std::string DeviceValue = positionStr[i];

            PositionType  pos;
            bool result = commonutils::TryParseToPosition(DeviceValue, pos);

            if (result) {
                p.ConnectedPositions.push_back(pos);
            }
        }

        const nlohmann::json& rh = j["Status"];

        for (auto& element : rh.items()) {
            std::string key = element.key();
            std::vector<int> motors = element.value().get<std::vector<int>>();
            p.Status[key] = motors;
        }
    }

    std::string commonutils::posToString(const PositionType positionType) {
        if (positionType == Left) {
            return "Left";
        } else if (positionType == Right) {
            return "Right";
        } else if (positionType == ForearmL) {
            return "ForearmL";
        } else if (positionType == ForearmR) {
            return "ForearmR";
        } else if (positionType == Vest) {
            return "Vest";
        } else if (positionType == VestFront) {
            return "VestFront";
        } else if (positionType == VestBack) {
            return "VestBack";
        } else if (positionType == Head) {
            return "Head";
        } else if (positionType == HandL) {
            return "HandL";
        } else if (positionType == HandR) {
            return "HandR";
        } else if (positionType == FootL) {
            return "FootL";
        } else if (positionType == FootR) {
            return "FootR";
        } else if (positionType == All) {
            return "All";
        }

        // TODO
        return "ForearmL";

    }

    bool commonutils::TryParseToPosition(const std::string deviceName, PositionType &pos) {
        if (deviceName == "Left")
        {
            pos = PositionType::Left;
        }
        else if (deviceName == "Right")
        {
            pos = PositionType::Right;
        }
        else if (deviceName == "ForearmL")
        {
            pos = PositionType::ForearmL;
        }
        else if (deviceName == "ForearmR")
        {
            pos = PositionType::ForearmR;
        }
        else if (deviceName == "Vest")
        {
            pos = PositionType::Vest;
        }
        else if (deviceName == "VestFront")
        {
            pos = PositionType::VestFront;
        }
        else if (deviceName == "VestBack")
        {
            pos = PositionType::VestBack;
        }
        else if (deviceName == "Head")
        {
            pos = PositionType::Head;
        }
        else if (deviceName == "Racket")
        {
            pos = PositionType::Racket;
        }
        else if (deviceName == "HandL")
        {
            pos = PositionType::HandL;
        }
        else if (deviceName == "HandR")
        {
            pos = PositionType::HandR;
        }
        else if (deviceName == "FootL")
        {
            pos = PositionType::FootL;
        }
        else if (deviceName == "FootR")
        {
            pos = PositionType::FootR;
        }  else
        {
            return false;
        }

        return true;
    }
}
