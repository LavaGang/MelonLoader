//Copyright bHaptics Inc. 2018
#ifndef BHAPTICS_HAPTICS_MANAGER
#define BHAPTICS_HAPTICS_MANAGER

#include "easywsclient.h"
#include "timer.h"
#include "shared/model.h"

#include <string>
#include <vector>
#include <mutex>
#include <map>

namespace bhaptics
{
    struct PlayerResponse {
        std::vector<std::string> RegisteredKeys;
        std::vector<std::string> ActiveKeys;
        int ConnectedDeviceCount;
        std::vector<PositionType> ConnectedPositions;
        std::map<std::string, std::vector<int>> Status;
    };

    class HapticPlayer
    {
    private:
        static HapticPlayer *hapticManager;

        std::unique_ptr<easywsclient::WebSocket> ws;
        std::vector<RegisterRequest> _registered;

        std::vector<std::string> _activeKeys;
        std::vector<PositionType> _activeDevices;

        std::string appId = "appId";
        std::string appName = "appName";

        std::map<std::string, std::vector<int>> _activeFeedback;

        std::mutex mtx;// mutex for _activeKeys and _activeDevices variable
        std::mutex registerMtx; //mutex for _registered variable
        std::mutex pollingMtx; //mutex for _registered variable
        std::mutex responseMtx;

        int _currentTime = 0;
        int _interval = 20;
        HapticTimer timer;

        bool isRunning = false;

        bool _enable = false;

        std::string url = "ws://127.0.0.1:15881/v2/feedbacks";
        int reconnectSec = 5;
        std::chrono::steady_clock::time_point prevReconnect;

        bool isRegisterSent = true;

        //functions
        void reconnect();

        void resendRegistered();

        bool connectionCheck();

        void send(PlayerRequest request);

        void updateActive(const std::string &key, const HapticFrame& signal);

        void remove(const std::string &key);

        void removeAll();

        void callbackFunc();

    public:

        bool retryConnection = true;
        PlayerResponse CurrentResponse;

        void doRepeat();

        HapticPlayer() {};

        void changeUrl(const std::string url);

        int registerFeedbackFromFile(const std::string &key, const std::string &filePath);

        int registerFeedbackFromString(const std::string &key, const std::string &jsonString);

        void init();

        void submit(const std::string &key, PositionType position, const std::vector<uint8_t> &motorBytes, int durationMillis);

        void submit(const std::string &key, PositionType position, const std::vector<DotPoint> &points, int durationMillis);

        void submit(const std::string &key, PositionType position, const std::vector<PathPoint> &points, int durationMillis);

        void submitRegistered(const std::string &key, const std::string &altKey, ScaleOption option, RotationOption rotOption);

        void submitRegistered(const std::string &key, int startTimeMillis);

        void submitRegistered(const std::string &key);

        bool isPlaying();

        bool isPlaying(const std::string &key);

        void turnOff();

        void turnOff(const std::string &key);

        void parseReceivedMessage(const char * message);

        void checkMessage();

        void destroy();

        void enableFeedback();

        void disableFeedback();

        void toggleFeedback();

        void parseResponse(PlayerResponse response);

        bool isDevicePlaying(PositionType device);

        bool isFeedbackRegistered(std::string key);

        bool anyFilesLoaded();

        std::vector<std::string> fileNames();

        void registerConnection(std::string id, std::string appName);

        void unregisterConnection(std::string Id);

        std::map<std::string, std::vector<int>> getResponseStatus();

        HapticPlayer(HapticPlayer const&) = delete;
        void operator= (HapticPlayer const&) = delete;

        static HapticPlayer *instance()
        {
            if (!hapticManager)
            {
                hapticManager = new HapticPlayer();
            }

            return hapticManager;
        }

    };


}

#endif