#include "ProjectModel.h"
#include "json.hpp"
#include <vector>
#include <map>
#include <string>
#include <fstream>
#include <algorithm>


namespace bhaptics
{
    bool utils::array_equals(std::vector<int> &a, std::vector<int> &b) {
        if (a.size() != b.size()) {
            return false;
        }

        for (int i = 0; i < int(a.size()); i++) {
            if (a[i] != b[i]) {
                return false;
            }
        }

        return true;
    }

    std::string utils::to_string(PositionType position) {
        switch (position) {
        case All:
            return "All";
        case Right:
            return "Right";
        case Left:
            return "Left";
        case Vest:
            return "Vest";
        case VestFront:
            return "VestFront";
        case VestBack:
            return "VestBack";
        case Head:
            return "Head";
        case Racket:
            return "Racket";
        case HandL:
            return "HandL";
        case HandR:
            return "HandR";
        case FootL:
            return "FootL";
        case FootR:
            return "FootR";
        case ForearmL:
            return "ForearmL";
        case ForearmR:
            return "ForearmR";
        default:
            return "Left";
        }
    }

    PositionType utils::to_position(std::string pos) {
        if (pos == "Left")
            return Left;

        if (pos == "Right")
            return Right;

        if (pos == "Vest")
            return Vest;

        if (pos == "VestFront")
            return VestFront;

        if (pos == "VestBack")
            return VestBack;

        if (pos == "ForearmL")
            return ForearmL;

        if (pos == "ForearmR")
            return ForearmR;

        if (pos == "Head")
            return Head;

        if (pos == "Racket")
            return Racket;

        if (pos == "HandL")
            return HandL;

        if (pos == "HandR")
            return HandR;

        if (pos == "FootL")
            return FootL;

        if (pos == "FootR")
            return FootR;

        if (pos == "All")
            return All;

        return Left;
    }

    PathMovingPattern utils::to_moving_pattern(std::string s) {
        if (s.compare("CONST_SPEED") == 0) {
            return CONST_SPEED;
        }

        return CONST_TDM;
    }

    std::string utils::to_string(PathMovingPattern pattern) {
        switch (pattern) {
        case CONST_SPEED:
            return "CONST_SPEED";
        default:
            return "CONST_TDM";
        }
    }

    FeedbackMode utils::to_feedback_mode(std::string s) {
        if (s.compare("PATH_MODE") == 0) {
            return PATH_MODE;
        }

        return DOT_MODE;
    }

    std::string utils::to_string(FeedbackMode mode) {
        switch (mode) {
        case PATH_MODE:
            return "PATH_MODE";
        default:
            return "DOT_MODE";
        }
    }

    PlaybackType utils:: to_playback_type(std::string s) {
        if (s.compare("FADE_IN") == 0) {
            return FADE_IN;
        }

        if (s.compare("FADE_OUT") == 0) {
            return FADE_OUT;
        }

        if (s.compare("FADE_IN_OUT") == 0) {
            return FADE_IN_OUT;
        }

        return NONE;
    }

    std::string utils::to_string(PlaybackType playback) {
        switch (playback) {
        case FADE_IN:
            return "FADE_IN";
        case FADE_IN_OUT:
            return "FADE_IN_OUT";
        case FADE_OUT:
            return "FADE_OUT";
        default:
            return "NONE";
        }
    }

    float utils::get_float(const nlohmann::json& j, std::string key) {
        auto val = j.at(key);

        if (val.type() == nlohmann::detail::value_t::string) {
            return std::stof(val.get<std::string>());
        }

        return val.get<float>();
    }

    int utils::clamps(int val, int bottom, int up) {
        if (val < bottom) {
            val = bottom;
        } else if (val > up) {
            val = up;
        }

        return val;
    }

    float utils::clamps(float val, float bottom, float up) {
        if (val < bottom) {
            val = bottom;
        } else if (val > up) {
            val = up;
        }

        return val;
    }


    PathModeObject::PathModeObject(const nlohmann::json& j) {
        x = utils::get_float(j, utils::get_key(j, "x"));
        y = utils::get_float(j, utils::get_key(j, "y"));//"y");
        intensity = utils::get_float(j, utils::get_key(j, "intensity")); //"intensity");
        time = j.at(utils::get_key(j, "time")).get<int>();
    }

    nlohmann::json PathModeObject::to_json() {
        nlohmann::json model;

        model["x"] = x;
        model["y"] = y;
        model["intensity"] = intensity;
        model["time"] = time;

        return model;
    }

    PathModeObjectCollection::PathModeObjectCollection(const nlohmann::json& j) {
        playbackType = utils::to_playback_type(j.at(utils::get_key(j, "playbackType")).get<std::string>());
        auto key = utils::get_key(j, "movingPattern");
        movingPattern = utils::to_moving_pattern(j.at(key).get<std::string>());

        auto feedbackJson = j.at(utils::get_key(j, "pointList")).get<std::vector<nlohmann::json>>();
        for (auto itemJson : feedbackJson) {
            PathModeObject pObject(itemJson);
            pointList.push_back(pObject);
        }
    }

    nlohmann::json PathModeObjectCollection::to_json() {
        nlohmann::json model;

        model["playbackType"] = utils::to_string(playbackType);
        model["movingPattern"] = utils::to_string(movingPattern);

        auto arr = nlohmann::json::array();
        for (auto obj : pointList) {
            arr.push_back(obj.to_json());
        }
        model["pointList"] = arr;

        return model;
    }

    PathMode::PathMode(const nlohmann::json& j) {
        auto feedbackKey = utils::get_key(j, "feedback");
        auto feedbackVal = j.at(feedbackKey);

        if (feedbackVal != nullptr) {
            auto feedbackJson = feedbackVal.get<std::vector<nlohmann::json>>();
            for (auto itemJson : feedbackJson) {
                PathModeObjectCollection collection(itemJson);
                feedback.push_back(collection);
            }
        }

    }

    nlohmann::json PathMode::to_json() {
        nlohmann::json model;
        auto arr = nlohmann::json::array();
        for (auto obj : feedback) {
            arr.push_back(obj.to_json());
        }
        model["feedback"] = arr;
        return model;
    }


    DotModeObject::DotModeObject(const nlohmann::json& j) {
        auto indexKey = utils::get_key(j, "index");

        index = utils::clamps(j.at(indexKey).get<int>(), 0, 19);

        auto intensityKey = utils::get_key(j, "intensity");
        intensity = utils::get_float(j, intensityKey);
    }

    nlohmann::json DotModeObject::to_json() {
        nlohmann::json model;
        model["index"] = index;
        model["intensity"] = intensity;

        return model;
    }

    DotModeObjectCollection::DotModeObjectCollection(const nlohmann::json& j) {
        auto startTimeKey = utils::get_key(j, "startTime");
        startTime = j.at(startTimeKey).get<int>();
        auto endTimeKey = utils::get_key(j, "endTime");
        endTime = j.at(endTimeKey).get<int>();

        auto playbackTypeKey = utils::get_key(j, "playbackType");
        playbackType = utils::to_playback_type(j.at(playbackTypeKey).get<std::string>());

        auto pointListKey = utils::get_key(j, "pointList");
        auto pointListJson = j.at(pointListKey).get<std::vector<nlohmann::json>>();

        for (auto dotModeObjectJson : pointListJson) {
            DotModeObject obj(dotModeObjectJson);
            pointList.push_back(obj);
        }
    }

    nlohmann::json DotModeObjectCollection::to_json() {
        nlohmann::json model;

        model["startTime"] = startTime;
        model["endTime"] = endTime;
        model["playbackType"] = utils::to_string(playbackType);

        auto arr = nlohmann::json::array();
        for (auto obj : pointList) {
            arr.push_back(obj.to_json());
        }
        model["pointList"] = arr;

        return model;
    }

    DotMode::DotMode(const nlohmann::json& j) {
        auto dotConnectedKey = utils::get_key(j, "dotConnected");
        dotConnected = j.at(dotConnectedKey).get<bool>();
        auto feedbackKey = utils::get_key(j, "feedback");
        auto feedbackJson = j.at(feedbackKey).get<std::vector<nlohmann::json>>();
        for (auto itemJson : feedbackJson) {
            DotModeObjectCollection collection(itemJson);
            feedback.push_back(collection);
        }
    }

    nlohmann::json DotMode::to_json() {
        nlohmann::json model;

        model["dotConnected"] = dotConnected;
        auto arr = nlohmann::json::array();
        for (auto collection : feedback) {
            nlohmann::json jsonObj = collection.to_json();
            arr.push_back(jsonObj);
        }
        model["feedback"] = arr;
        return model;
    }



    HapticEffectMode::HapticEffectMode(const nlohmann::json& j) {
        auto modeKey = utils::get_key(j, "mode");
        auto modeStr = j.at(modeKey).get<std::string>();
        mode = utils::to_feedback_mode(modeStr);

        auto dotModeKey = utils::get_key(j, "dotMode");
        auto pathModeKey = utils::get_key(j, "pathMode");
        DotMode dotModeObj(j.at(dotModeKey));
        PathMode pathModeObj(j.at(pathModeKey));

        dotMode = dotModeObj;
        pathMode = pathModeObj;
    }

    nlohmann::json HapticEffectMode::to_json() {
        nlohmann::json model;
        model["mode"] = utils::to_string(mode);
        model["dotMode"] = dotMode.to_json();
        model["pathMode"] = pathMode.to_json();

        return model;
    }

    HapticEffect::HapticEffect(const nlohmann::json& j) {
        auto startTimeKey = utils::get_key(j, "startTime");
        startTime = j.at(startTimeKey).get<int>();

        auto offsetTimeKey = utils::get_key(j, "offsetTime");
        offsetTime = j.at(offsetTimeKey).get<int>();

        auto modesKey = utils::get_key(j, "modes");
        for (auto item : j.at(modesKey).items()) {
            auto key = item.key();
            auto modeJson = item.value();
            HapticEffectMode hMode(modeJson);
            modes[key] = hMode;
        }
    }

    nlohmann::json HapticEffect::to_json() {
        nlohmann::json model;

        model["startTime"] = startTime;
        model["offsetTime"] = offsetTime;

        nlohmann::json modeJson;

        for (auto it = modes.begin(); it != modes.end(); ++it) {
            auto key = it->first;
            auto mode = it->second;
            modeJson[key] = mode.to_json();
        }

        model["modes"] = modeJson;

        return model;
    }


    LayoutObject::LayoutObject(const nlohmann::json& j) {
        auto indexKey = utils::get_key(j, "index");
        index = j.at(indexKey).get<int>();
        x = utils::get_float(j, utils::get_key(j, "x"));
        y = utils::get_float(j, utils::get_key(j, "y"));
    }

    nlohmann::json LayoutObject::to_json() {
        nlohmann::json model;
        model["index"] = index;
        model["x"] = x;
        model["y"] = y;

        return model;
    }

    Layout::Layout(const nlohmann::json& j) {
        auto typeKey = utils::get_key(j, "type");
        auto typeJson = j.at(typeKey).get<std::string>();
        type = typeJson;
        auto layoutsKey = utils::get_key(j, "layouts");

        for (const auto& x : j[layoutsKey].items()) {
            auto layoutObjectList = x.value().get<std::vector<nlohmann::json>>();
            auto key = x.key();
            std::vector<LayoutObject> arr;

            for (auto obj : layoutObjectList) {
                LayoutObject lObj(obj);
                arr.push_back(lObj);
            }
            layouts[key] = arr;
        }
    }

    nlohmann::json Layout::to_json() {
        nlohmann::json model;
        model["type"] = type;

        nlohmann::json layoutJson;

        for (const auto& kv : layouts) {
            auto key = kv.first;
            auto value = kv.second;

            auto arr = nlohmann::json::array();
            for (auto layoutObj : value) {
                arr.push_back(layoutObj.to_json());
            }
            layoutJson[key] = arr;
        }

        model["layouts"] = layoutJson;

        return model;
    }

    Track::Track(const nlohmann::json& j) {
        if (!utils::contains_key(j, "effects")) {
            return;
        }

        auto effectsKey = utils::get_key(j, "effects");

        auto effectsJson = j.at(effectsKey).get<std::vector<nlohmann::json>>();
        for (auto effectJson : effectsJson) {
            HapticEffect effect(effectJson);
            effects.push_back(effect);
        }
    }

    nlohmann::json Track::to_json() {
        nlohmann::json model;

        auto arr = nlohmann::json::array();
        for (auto effect : effects) {
            arr.push_back(effect.to_json());
        }
        model["effects"] = arr;
        return model;
    }

    Project::Project(const nlohmann::json& j) {
        if (!utils::contains_key(j, "tracks")) {
            return;
        }
        auto tracksKey = utils::get_key(j, "tracks");
        auto tracksJson = j.at(tracksKey).get<std::vector<nlohmann::json>>();

        for (auto trackJson : tracksJson) {
            Track track(trackJson);
            tracks.push_back(track);
        }

        auto layoutKey = utils::get_key(j, "layout");
        Layout layoutObj(j.at(layoutKey));
        layout = layoutObj;
    }

    nlohmann::json Project::to_json() {
        nlohmann::json model;

        auto arr = nlohmann::json::array();
        for (auto track : tracks) {
            nlohmann::json trackJson = track.to_json();
            arr.push_back(trackJson);
        }
        model["tracks"] = arr;
        model["layout"] = layout.to_json();

        return model;
    }


    ProjectFile::ProjectFile(const nlohmann::json& j) {
        intervalMillis = j.at("intervalMillis").get<int>();
        size = j.at("size").get<int>();
        durationMillis = j.at("durationMillis").get<int>();
        Project prj(j.at("project"));
        project = prj;
    }
    ///////////////////////////////////////////////////////////////////////////////////////


    DotPointInternal::DotPointInternal(int index, int intensity) {
        this->index = utils::clamps(index, 0, 19);
        this->intensity = utils::clamps(intensity, 0, 100);
    }

    DotPointInternal::DotPointInternal(const nlohmann::json& j) {
        this->index = j.at(utils::get_key(j, "index")).get<int>();
        this->intensity = j.at(utils::get_key(j, "intensity")).get<int>();
    }

    nlohmann::json DotPointInternal::to_json() {
        nlohmann::json model;
        model["index"] = index;
        model["intensity"] = intensity;
        return model;
    }

    PathPointInternal::PathPointInternal(float x, float y, int intensity, int motorCount) {
        this->intensity = intensity;
        this->motorCount = utils::clamps(motorCount, 1, 3);
        this->x = utils::clamps(x, 0.0, 1.0);
        this->y = utils::clamps(y, 0.0, 1.0);
    }

    PathPointInternal::PathPointInternal(nlohmann::json& j) {
        x = utils::get_float(j, utils::get_key(j, "x"));
        y = utils::get_float(j, utils::get_key(j, "y"));
        intensity = j.at(utils::get_key(j, "intensity")).get<int>();

        if (!utils::contains_key(j, "motorCount")) {
            motorCount = 3;
            return;
        }
        motorCount = j.at(utils::get_key(j, "motorCount")).get<int>();
    }

    nlohmann::json PathPointInternal::to_json() {
        nlohmann::json model;
        model["x"] = x;
        model["y"] = y;
        model["intensity"] = intensity;
        model["motorCount"] = motorCount;
        return model;
    }



    HapticFeedbackFrame::HapticFeedbackFrame(PositionType pos,
        const std::vector<PathPointInternal>& points) {
        position = pos;
        pathPoints = points;
    }

    HapticFeedbackFrame::HapticFeedbackFrame(PositionType pos) {
        position = pos;
    }

    HapticFeedbackFrame::HapticFeedbackFrame(PositionType pos,
        const std::vector<DotPointInternal>& points) {
        position = pos;
        dotPoints = points;
    }

    HapticFeedbackFrame HapticFeedbackFrame::Copy(const HapticFeedbackFrame& frame) {
        HapticFeedbackFrame cpy(frame.position);

        for (auto from : frame.dotPoints) {
            DotPointInternal point(from.index, from.intensity);
            cpy.dotPoints.push_back(point);
        }

        for (auto from : frame.pathPoints) {
            PathPointInternal point(from.x, from.y, from.intensity, from.motorCount);
            cpy.pathPoints.push_back(point);
        }

        return cpy;
    }



    Frame::Frame(std::vector<PathPointInternal> points,
                 PositionType position, int durationMillis) {
        this->position = position;
        this->pathPoints = points;
        this->durationMillis = durationMillis;
    }

    Frame::Frame(std::vector<DotPointInternal> points,
                 PositionType position, int durationMillis) {
        this->position = position;
        this->dotPoints = points;
        this->durationMillis = durationMillis;
    }

    Frame::Frame(nlohmann::json& j) {
        if (j.is_null()) {
//            logging::log("Fram() null json format");
            return;
        }
        durationMillis = j.at(utils::get_key(j, "durationMillis")).get<int>();
        auto posStr = j.at(utils::get_key(j, "position")).get<std::string>();
        position = utils::to_position(posStr);

        auto pathModesJson = j.at(utils::get_key(j, "pathPoints")).get<std::vector<nlohmann::json>>();
        for (auto itemJson : pathModesJson) {
            PathPointInternal po(itemJson);
            pathPoints.push_back(po);
        }

        auto dotModesJson = j.at(utils::get_key(j, "dotPoints")).get<std::vector<nlohmann::json>>();
        for (auto itemJson : dotModesJson) {
            DotPointInternal po(itemJson);
            dotPoints.push_back(po);
        }
    }

    nlohmann::json Frame::to_json() {
        nlohmann::json model;
        model["durationMillis"] = durationMillis;
        auto arr = nlohmann::json::array();
        for (auto obj : pathPoints) {
            arr.push_back(obj.to_json());
        }
        model["pathPoints"] = arr;

        auto dotPointsArr = nlohmann::json::array();
        for (auto obj : dotPoints) {
            dotPointsArr.push_back(obj.to_json());
        }
        model["dotPoints"] = dotPointsArr;
        return model;
    }


}
