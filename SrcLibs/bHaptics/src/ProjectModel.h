//
// Created by westside on 2018-05-31.
//


#pragma once
#ifndef SAMPLEAPP_PROJECT_MODEL_H
#define SAMPLEAPP_PROJECT_MODEL_H

// independent
#include "json.hpp"
#include <sstream>
#include "shared/model.h"

namespace bhaptics
{
    // Common
    enum PathMovingPattern
    {
        CONST_SPEED, CONST_TDM
    };

    enum PlaybackType
    {
        NONE, FADE_IN, FADE_OUT, FADE_IN_OUT
    };

//    enum FeedbackMode
//    {
//        DOT_MODE, PATH_MODE
//    };

//    enum Position {
//        All = 0, Left = 1, Right = 2,
//        Vest = 3,
//        Head = 4,
//        Racket = 5,
//        HandL = 6, HandR = 7,
//        FootL = 8, FootR = 9,
//        ForearmL = 10, ForearmR = 11,
//        VestFront = 201, VestBack = 202,
//        GloveLeft = 203, GloveRight = 204,
//        Custom1 = 251, Custom2 = 252, Custom3 = 253, Custom4 = 254
//    };




    // PathMode
    class PathModeObject {
    public:
        float x = 0;
        float y = 0;
        float intensity = 0;
        int time = 0;

        PathModeObject(){}
        PathModeObject(const nlohmann::json& j);
        nlohmann::json to_json();
    };

    class PathModeObjectCollection {
    public:
        PlaybackType playbackType = NONE;
        PathMovingPattern movingPattern = CONST_TDM;
        std::vector<PathModeObject> pointList;

        PathModeObjectCollection(){}
        PathModeObjectCollection(const nlohmann::json& j);
        nlohmann::json to_json();
    };

    struct PathMode {
        std::vector<PathModeObjectCollection> feedback;

        PathMode(){}
        PathMode(const nlohmann::json& j);
        nlohmann::json to_json();
    };


    // DotMode
    struct DotModeObject {
        int index;
        float intensity;

        DotModeObject(){}
        DotModeObject(const nlohmann::json& j);
        nlohmann::json to_json();
    };

    struct DotModeObjectCollection {
        int startTime = 0;
        int endTime = 0;
        PlaybackType playbackType = NONE;
        std::vector<DotModeObject> pointList;

        DotModeObjectCollection(){}
        DotModeObjectCollection(const nlohmann::json& j);
        nlohmann::json to_json();
    };

    struct DotMode {
        bool dotConnected = false;
        std::vector<DotModeObjectCollection> feedback;

        DotMode() {}
        DotMode(const nlohmann::json& j);
        nlohmann::json to_json();
    };

    struct HapticEffectMode {
        FeedbackMode mode = DOT_MODE;
        DotMode dotMode;
        PathMode pathMode;

        HapticEffectMode() {}
        HapticEffectMode(const nlohmann::json& j);
        nlohmann::json to_json();
    };


    struct HapticEffect {
        int startTime = 0;
        int offsetTime = 0;
        std::map<std::string, HapticEffectMode> modes;

        HapticEffect(){}
        HapticEffect(const nlohmann::json& j);
        nlohmann::json to_json();
    };


    // Layout
    struct LayoutObject {
        int index = 0;
        float x = 0;
        float y = 0;

        LayoutObject() {}
        LayoutObject(const nlohmann::json& j);
        nlohmann::json to_json();
    };

    struct Layout {
        std::string type;
        std::map<std::string, std::vector<LayoutObject>> layouts;

        Layout() {}
        Layout(const nlohmann::json& j);
        nlohmann::json to_json();
    };

    struct Track {
        std::vector<HapticEffect> effects;

        Track(){}
        Track(const nlohmann::json& j);
        nlohmann::json to_json();
    };

    struct Project {
        std::vector<Track> tracks;
        Layout layout;

        Project() {}
        Project(const nlohmann::json& j);
        nlohmann::json to_json();
    };

    struct ProjectFile {
        int intervalMillis = 0;
        int size = 0;
        int durationMillis = 0;
        Project project;

        ProjectFile(const nlohmann::json& j);
        ProjectFile(){}
    };





    //// protocol
    struct DotPointInternal {
        int index;
        int intensity;

        DotPointInternal(){}
        DotPointInternal(int index, int intensity);
        DotPointInternal(const nlohmann::json& j);
        nlohmann::json to_json();
    };

    struct PathPointInternal {
        float x;
        float y;
        int intensity;
        int motorCount;

        PathPointInternal(){}
        PathPointInternal(float x, float y, int intensity, int motorCount = 3);
        PathPointInternal(nlohmann::json& j);
        nlohmann::json to_json();

    };

    struct HapticFeedbackFrame {
        PositionType position = Left;
        std::vector<PathPointInternal> pathPoints;
        std::vector<DotPointInternal> dotPoints;

        HapticFeedbackFrame(){}
        HapticFeedbackFrame(PositionType pos,
                            const std::vector<PathPointInternal>& points);
        HapticFeedbackFrame(PositionType pos);
        HapticFeedbackFrame(PositionType pos,
                            const std::vector<DotPointInternal>& points);
        static HapticFeedbackFrame Copy(const HapticFeedbackFrame& frame);

    };

    class Frame {
    public:
        int durationMillis = 0;
        PositionType position = Left;
        std::vector<PathPointInternal> pathPoints;
        std::vector<DotPointInternal> dotPoints;

        Frame(std::vector<PathPointInternal> points,
              PositionType position, int durationMillis);

        Frame(std::vector<DotPointInternal> points,
              PositionType position, int durationMillis);

        Frame(){}

        Frame(nlohmann::json& j);

        nlohmann::json to_json();
    };




    class utils {
    public:
        static bool array_equals(std::vector<int> &a, std::vector<int> &b);
        static std::string to_string(PositionType position);
        static PositionType to_position(std::string pos);
        static PathMovingPattern to_moving_pattern(std::string s);

        static std::string to_string(PathMovingPattern pattern);
        static FeedbackMode to_feedback_mode(std::string s);
        static std::string to_string(FeedbackMode mode);
        static PlaybackType to_playback_type(std::string s);
        static std::string to_string(PlaybackType playback);
        static float get_float(const nlohmann::json& j, std::string key);
        static int clamps(int val, int bottom, int up);
        static float clamps(float val, float bottom, float up);

        static std::string get_key(const nlohmann::json j, std::string key1) {
            if (j.count(key1) > 0) {
                return key1;
            }

            key1[0] = toupper(key1[0]);

            if (j.count(key1) > 0) {
                return key1;
            }
//            logging::log("get_key: not contain " + key1);
            return key1;
        }

        static bool contains_key(const nlohmann::json j, std::string key1) {

            if (j.count(key1) > 0) {
                return true;
            }

            key1[0] = toupper(key1[0]);

            if (j.count(key1) > 0) {
                return true;
            }
//            logging::log("contains_key : not contain " + key1);
            return false;
        }

        static std::string to_string(std::vector<int> v) {
            std::stringstream ss;
            for(size_t i = 0; i < v.size(); ++i)
            {
                if(i != 0)
                    ss << ",";
                ss << v[i];
            }
            std::string s = ss.str();
            return s;
        }

        static std::string to_string(std::vector<DotPointInternal> v) {
            std::stringstream ss;
            for(size_t i = 0; i < v.size(); ++i) {
                if(i != 0)
                    ss << ",";
                ss << v[i].to_json().dump();
            }
            std::string s = ss.str();
            return s;
        }

        static std::string to_string(std::vector<PathPointInternal> v) {
            std::stringstream ss;
            for(size_t i = 0; i < v.size(); ++i) {
                if(i != 0)
                    ss << ",";
                ss << v[i].to_json().dump();
            }
            std::string s = ss.str();
            return s;
        }
    };
}

#endif //SAMPLEAPP_PROJECTMODEL_H
