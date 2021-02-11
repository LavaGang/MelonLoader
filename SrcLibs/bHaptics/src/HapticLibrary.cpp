
#include<string.h>
#if defined _WIN32 || defined _WIN64
    #include <Windows.h>
#else
    #include <stdio.h>
#endif

#include "shared/HapticLibrary.h"
#include "hapticsManager.h"
#include "shared/model.h"
#include "util.h"
#include "commonutils.h"
#include <iostream>
#include "ProjectModel.h"

using namespace bhaptics;
using namespace std;

static bool isInit = false;
static std::string exeFilePath = "";

const char* GetExePath() {
    if (isInit)
    {
        return exeFilePath.c_str();
    }
    isInit = true;
#if defined _WIN32 || defined _WIN64 ||  defined(__WIN32)
    HKEY hKey;
    WCHAR szBuffer[512];
    DWORD dwBufferSize = sizeof(szBuffer);
    LONG resultFirstKey = RegOpenKeyExW(HKEY_CLASSES_ROOT, L"bhaptics-app\\shell\\open\\command", 0, KEY_READ, &hKey);

	LONG result = RegQueryValueEx(hKey, L"", 0, NULL, (LPBYTE)szBuffer, &dwBufferSize);
    RegCloseKey(hKey);

    if (result == ERROR_SUCCESS)
    {
        std::wstring path = szBuffer;
        int size = WideCharToMultiByte(CP_UTF8, 0, path.c_str(), -1, NULL, 0, NULL, NULL);

        char *buffer = new char[size + 1];
        WideCharToMultiByte(CP_UTF8, 0, path.c_str(), -1, buffer, size, NULL, NULL);
        std::string str(buffer);

        delete[]buffer;
        exeFilePath = str;
    }
#else
#endif

    return exeFilePath.c_str();;
}

//char* getExePath
bool TryGetExePath(char* buf, int& buf_size)
{
    if (isInit)
    {
        buf_size = (int)exeFilePath.size();
        strcpy(buf, exeFilePath.c_str());
        return true;
    }
    isInit = true;
#if defined _WIN32 || defined _WIN64 ||  defined(__WIN32)
    HKEY hKey;
    WCHAR szBuffer[512];
    DWORD dwBufferSize = sizeof(szBuffer);
    LONG resultFirstKey = RegOpenKeyExW(HKEY_CLASSES_ROOT, L"bhaptics-app\\shell\\open\\command", 0, KEY_READ, &hKey);

	LONG result = RegQueryValueEx(hKey, L"", 0, NULL, (LPBYTE)szBuffer, &dwBufferSize);
    RegCloseKey(hKey);

    if (result == ERROR_SUCCESS)
    {
        std::wstring path = szBuffer;
        int size = WideCharToMultiByte(CP_UTF8, 0, path.c_str(), -1, NULL, 0, NULL, NULL);

        char *buffer = new char[size + 1];
        WideCharToMultiByte(CP_UTF8, 0, path.c_str(), -1, buffer, size, NULL, NULL);
        std::string str(buffer);

        delete[]buffer;
        exeFilePath = str;
        buf_size = (int)exeFilePath.size();
        std::cout << buf_size << std::endl;
        strcpy(buf, exeFilePath.c_str());
        return true;
    }
#else
#endif
    buf_size = (int)exeFilePath.size();
    strcpy(buf, exeFilePath.c_str());
    return false;
}

void SubmitPathArray(const char *key, bhaptics::PositionType Pos, point *points, size_t length,
                     int durationMillis)
{
    std::vector<bhaptics::PathPoint> motorbyts;

    for(size_t i = 0 ; i< length ; i++)
    {
        bhaptics::PathPoint point(points[i].x, points[i].y, points[i].intensity, points[i].motorCount);
        motorbyts.push_back(point);
    }
    SubmitPath(key, Pos, motorbyts, durationMillis);
}

void SubmitByteArray(const char* key, bhaptics::PositionType Pos, unsigned char* buf, size_t length, int durationMillis)
{
    std::vector<uint8_t> motorbyts(20);

    size_t l = length > 20 ? 20 : length;

    for(size_t i = 0 ; i< l ; i++)
    {
        motorbyts[i] = buf[i];
    }
    Submit(key, Pos, motorbyts, durationMillis);
}

void ChangeUrl(const char* url)
{
    bhaptics::HapticPlayer::instance()->changeUrl(url);
}

void InitialiseSync(const char* appId, const char* appName)
{
    bhaptics::HapticPlayer::instance()->registerConnection(appId, appName);
}

void Initialise(const char* appId, const char* appName) {
    std::thread thread (InitialiseSync, appId, appName);
    thread.detach();
}

void Destroy()
{
    bhaptics::HapticPlayer::instance()->destroy();
}

void RegisterFeedback(const char* key, const char* projectJson)
{
    bhaptics::HapticPlayer::instance()->registerFeedbackFromString(key, projectJson);
}

void RegisterFeedbackFromTactFile(const char* key, const char* tactFileStr)
{
    auto hapticFile = Util::parseFromTactFileString(tactFileStr);
    bhaptics::HapticPlayer::instance()->registerFeedbackFromString(key, hapticFile.ProjectJson);
}
void RegisterFeedbackFromTactFileReflected(const char* key, const char* tactFileStr)
{
    auto hapticFile = Util::parseFromTactFileString(tactFileStr);

    auto parsed = nlohmann::json::parse(hapticFile.ProjectJson);
    Project project(parsed);

    vector<std::string> v;

    bool finished = false;

    for (auto& projectTrack : project.tracks)
    {
        for (auto& projectTrackEffect : projectTrack.effects)
        {
            if (finished) {
                continue;
            }
            for(auto it = projectTrackEffect.modes.begin(); it != projectTrackEffect.modes.end(); ++it) {
                v.push_back(it->first);
            }
            finished = true;
        }
    }

    if (v.size() != 2) {
        bhaptics::HapticPlayer::instance()->registerFeedbackFromString(key, hapticFile.ProjectJson);
        return;
    }

    std::string TypeRight = v[0];
    std::string TypeLeft = v[1];

    for (size_t track_index = 0 ; track_index < project.tracks.size() ; track_index++)
    {
        auto projectTrack = project.tracks[track_index];
        for (size_t effect_index = 0 ; effect_index < projectTrack.effects.size() ; effect_index++) {
            auto projectTrackEffect = projectTrack.effects[effect_index];

            if (projectTrackEffect.modes.count(TypeRight) > 0) {
                project.tracks[track_index].effects[effect_index].modes[TypeLeft] = projectTrackEffect.modes[TypeRight];
            }

            if (projectTrackEffect.modes.count(TypeLeft) > 0) {
                project.tracks[track_index].effects[effect_index].modes[TypeRight] = projectTrackEffect.modes[TypeLeft];
            }
        }
    }

    bhaptics::HapticPlayer::instance()->registerFeedbackFromString(key, project.to_json().dump());
}

void LoadAndRegisterFeedback(const char* Key, const char* FilePath)
{
    bhaptics::HapticPlayer::instance()->registerFeedbackFromFile(Key,FilePath);
}

void SubmitRegistered(const char* key)
{
    bhaptics::HapticPlayer::instance()->submitRegistered(key);
}

void SubmitRegisteredAlt(const char* Key, const char* AltKey, bhaptics::ScaleOption ScaleOpt, bhaptics::RotationOption RotOption)
{
    bhaptics::HapticPlayer::instance()->submitRegistered(Key, AltKey, ScaleOpt, RotOption);
}

void SubmitRegisteredWithOption(const char* Key, const char* AltKey, float intensity, float duration, float offsetAngleX, float offsetAngleY)
{
    bhaptics::ScaleOption  ScaleOpt;
    ScaleOpt.Intensity = intensity;
    ScaleOpt.Duration = duration;

    bhaptics::RotationOption RotOption;
    RotOption.OffsetAngleX = offsetAngleX;
    RotOption.OffsetY = offsetAngleY;
    bhaptics::HapticPlayer::instance()->submitRegistered(Key, AltKey, ScaleOpt, RotOption);
}

void Submit(const char* Key, bhaptics::PositionType Pos, std::vector<uint8_t>& MotorBytes, int DurationMillis)
{
    bhaptics::HapticPlayer::instance()->submit(Key, Pos, MotorBytes, DurationMillis);
}
void SubmitRegisteredStartMillis(const char* key, int startMillis)
{
    bhaptics::HapticPlayer::instance()->submitRegistered(key, startMillis);
}

void SubmitDot(const char* Key, bhaptics::PositionType Pos, std::vector<bhaptics::DotPoint>& Points, int DurationMillis)
{
    bhaptics::HapticPlayer::instance()->submit(Key, Pos, Points, DurationMillis);
}

void SubmitPath(const char* Key, bhaptics::PositionType Pos, std::vector<bhaptics::PathPoint>& Points, int DurationMillis)
{
    bhaptics::HapticPlayer::instance()->submit(Key, Pos, Points, DurationMillis);
}

bool IsFeedbackRegistered(const char* key)
{
    return bhaptics::HapticPlayer::instance()->isFeedbackRegistered(key);
}

bool IsPlaying()
{
    return bhaptics::HapticPlayer::instance()->isPlaying();
}

bool IsPlayingKey(const char* Key)
{
    return bhaptics::HapticPlayer::instance()->isPlaying(Key);
}

void TurnOff()
{
    bhaptics::HapticPlayer::instance()->turnOff();
}

void TurnOffKey(const char* Key)
{
    bhaptics::HapticPlayer::instance()->turnOff(Key);
}

void EnableFeedback()
{
    bhaptics::HapticPlayer::instance()->enableFeedback();
}

void DisableFeedback()
{
    bhaptics::HapticPlayer::instance()->disableFeedback();
}

void ToggleFeedback()
{
    bhaptics::HapticPlayer::instance()->toggleFeedback();
}

bool IsDevicePlaying(bhaptics::PositionType Pos)
{
    return bhaptics::HapticPlayer::instance()->isDevicePlaying(Pos);
}

bool TryGetResponseForPosition(bhaptics::PositionType Pos, status& s)
{
    if (!bhaptics::HapticPlayer::instance()->getResponseStatus().empty())
    {
        std::string pos = bhaptics::commonutils::posToString(Pos);
        std::map<std::string, std::vector<int>> responseMap = bhaptics::HapticPlayer::instance()->getResponseStatus();
        if (responseMap.find(pos) != responseMap.end())
        {
            std::vector<int> response = responseMap.at(pos);

            // TODO validation
            if (response.size() < 20) {
                for (size_t i = 0; i < response.size(); i++)
                {
                    s.values[i] = response.at(i);
                }
                return true;
            }
            for (size_t i = 0; i < 20; i++)
            {
                s.values[i] = response.at(i);
            }

            return true;
        }
    }

    return false;
}

void GetResponseStatus(std::vector<bhaptics::HapticFeedback>& retValues)
{
    std::map<std::string, std::vector<int>> response = bhaptics::HapticPlayer::instance()->getResponseStatus();
    for (auto& Device : response)
    {
        bhaptics::PositionType Position;

        bool result = bhaptics::commonutils::TryParseToPosition(Device.first, Position);

        if (result) {
            bhaptics::HapticFeedback Feedback(Position, Device.second);
            retValues.push_back(Feedback);
        }

    }
}