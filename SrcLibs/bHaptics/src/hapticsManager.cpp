//Copyright bHaptics Inc. 2018
#ifndef BHAPTICS_HPP
#define BHAPTICS_HPP

#include "hapticsManager.h"
#include "util.h"
#include "commonutils.h"

#ifdef _WIN32
#pragma comment( lib, "ws2_32" )
#include <WinSock2.h>
#endif

#include <assert.h>
#include <stdio.h>
#include <string>
#include <memory>
#include <iostream>

namespace bhaptics
{
    using easywsclient::WebSocket;

    void HapticPlayer::reconnect()
    {

        if (!retryConnection)
        {
            return;
        }

        if (connectionCheck())
        {
            return;
        }

        std::chrono::steady_clock::time_point current = std::chrono::steady_clock::now();

        int values = (int) std::chrono::duration_cast<std::chrono::seconds>(current - prevReconnect).count();
        bool IsPassedInterval = (current > (prevReconnect + std::chrono::seconds(reconnectSec)));
        if (IsPassedInterval &&_enable)
        {

            ws.reset(WebSocket::from_url(url));

            isRegisterSent = false;

            prevReconnect = current;
        }
    }

    void HapticPlayer::resendRegistered()
    {
        if (connectionCheck() && _registered.size()>0)
        {
            PlayerRequest req;
            registerMtx.lock();
            std::vector<RegisterRequest> tempRegister = _registered;
            registerMtx.unlock();

            for (size_t i = 0; i < tempRegister.size(); i++)
            {
                req.Register.push_back(tempRegister[i]);
            }
            send(req);

            isRegisterSent = true;
        }
    }

    bool HapticPlayer::connectionCheck()
    {
        if (!ws)
        {
            return false;
        }

        pollingMtx.lock();
        WebSocket::readyStateValues isClosed = ws->getReadyState();
        pollingMtx.unlock();
        if (isClosed == WebSocket::CLOSED)
        {
            ws.reset(nullptr);
            return false;
        }

        return true;
    }

    void HapticPlayer::send(PlayerRequest request)
    {
        if (!connectionCheck())
        {
            return;
        }

        std::string jStr = request.to_string();
        pollingMtx.lock();
        ws->send(jStr);
        ws->poll();
        pollingMtx.unlock();
    }

    void HapticPlayer::updateActive(const std::string &key, const HapticFrame& signal)
    {
        if (!_enable || !connectionCheck())
        {
            return;
        }

        SubmitRequest req;
        PlayerRequest playerReq;
        req.Frame = signal;
        req.Key = key;
        req.Type = "frame";

        playerReq.Submit.push_back(req);

        send(playerReq);
    }

    void HapticPlayer::remove(const std::string &key)
    {
        if (!_enable || !connectionCheck())
        {
            return;
        }

        SubmitRequest req;
        PlayerRequest playerReq;
        req.Key = key;
        req.Type = "turnOff";

        playerReq.Submit.push_back(req);

        send(playerReq);
    }

    void HapticPlayer::removeAll()
    {
        if (!_enable)
        {
            return;
        }

        SubmitRequest req;
        PlayerRequest playerReq;
        req.Type = "turnOffAll";

        playerReq.Submit.push_back(req);

        send(playerReq);
    }

    void HapticPlayer::callbackFunc()
    {
        reconnect();
        doRepeat();
        _currentTime += _interval;
    }

    void HapticPlayer::doRepeat()
    {
        if (isRunning)
        {
            return;
        }
        isRunning = true;

        if (!isRegisterSent)
        {
            resendRegistered();
        }

        checkMessage();

        isRunning = false;
    }

    int HapticPlayer::registerFeedbackFromFile(const std::string &key, const std::string &filePath)
    {
        HapticFile file = Util::parse(filePath);
        if (file.ProjectJson == "")
        {
            return 0;
        }

        RegisterRequest req;
        req.Key = key;
        req.ProjectJson = file.ProjectJson;

        registerMtx.lock();
        _registered.push_back(req);

        PlayerRequest playerReq;

        playerReq.Register.push_back(req);

        send(playerReq);
        registerMtx.unlock();
        return 1;
    }

    int HapticPlayer::registerFeedbackFromString(const std::string &key, const std::string &jsonString)
    {
        RegisterRequest req;
        req.Key = key;
        req.ProjectJson = jsonString;
        registerMtx.lock();
        _registered.push_back(req);

        PlayerRequest playerReq;

        playerReq.Register.push_back(req);

        send(playerReq);
        registerMtx.unlock();
        return 0;
    }

    void HapticPlayer::init()
    {

#ifdef _WIN32
        INT rc;
        WSADATA wsaData;

        rc = WSAStartup(MAKEWORD(2, 2), &wsaData);
        if (rc) {
            printf("WSAStartup Failed.\n");
            return;
        }
#endif


        if (_enable || ws)
            return;
        std::function<void()> callback = std::bind(&HapticPlayer::callbackFunc, this);
        timer.add_timer_handler(callback);
        ws = std::unique_ptr<WebSocket>(WebSocket::from_url(url + "?app_id=" + appId + "&app_name=" + appName));

        connectionCheck();
        timer.start();

        _enable = true;
    }

    void HapticPlayer::submit(const std::string &key, PositionType position, const std::vector<uint8_t> &motorBytes, int durationMillis)
    {
        if (!_enable || !connectionCheck())
        {
            return;
        }

        std::vector<DotPoint> points;
        for (size_t i = 0; i < motorBytes.size(); i++)
        {
            if (motorBytes[i] > 0)
            {
                points.push_back(DotPoint((int)i, motorBytes[i]));
            }
        }

        HapticFrame submitFrame = HapticFrame::AsDotPointFrame(points, position, durationMillis);
        updateActive(key, submitFrame);
    }

    void HapticPlayer::submit(const std::string &key, PositionType position, const std::vector<DotPoint> &points, int durationMillis)
    {
        HapticFrame req = HapticFrame::AsDotPointFrame(points, position, durationMillis);
        updateActive(key, req);
    }

    void HapticPlayer::submit(const std::string &key, PositionType position, const std::vector<PathPoint> &points, int durationMillis)
    {
        HapticFrame req = HapticFrame::AsPathPointFrame(points, position, durationMillis);
        updateActive(key, req);
    }

    void HapticPlayer::submitRegistered(const std::string &key, const std::string &altKey, ScaleOption option, RotationOption rotOption)
    {
        if (!_enable || !connectionCheck())
        {
            return;
        }

        SubmitRequest req;
        PlayerRequest playerReq;
        req.Key = key;
        req.Type = "key";
        req.Parameters["scaleOption"] = option.to_string();
        req.Parameters["rotationOption"] = rotOption.to_string();
        if (!altKey.empty())
        {
            req.Parameters["altKey"] = "\""+altKey+"\"";
        }
        playerReq.Submit.push_back(req);

        send(playerReq);
    }

    void HapticPlayer::submitRegistered(const std::string &key, int startTimeMillis)
    {
        if (!_enable || !connectionCheck())
        {
            return;
        }

        if (startTimeMillis <= 0 ) {
            return;
        }

        SubmitRequest req;
        PlayerRequest playerReq;
        req.Key = key;
        req.Type = "key";
        req.Parameters["startTimeMillis"] = "\""+std::to_string(startTimeMillis) + "\"";

        playerReq.Submit.push_back(req);

        send(playerReq);
    }

    void HapticPlayer::submitRegistered(const std::string &key)
    {
        if (!_enable || !connectionCheck())
        {
            return;
        }

        SubmitRequest req;
        PlayerRequest playerReq;
        req.Key = key;
        req.Type = "key";

        playerReq.Submit.push_back(req);

        send(playerReq);
    }

    bool HapticPlayer::isPlaying()
    {
        return _activeKeys.size() > 0;
    }

    bool HapticPlayer::isPlaying(const std::string &key)
    {
        std::vector<std::string> temp = _activeKeys;

        bool ret = std::find(_activeKeys.begin(), _activeKeys.end(), key) != _activeKeys.end();
        return ret;
    }

    void HapticPlayer::turnOff()
    {
        removeAll();
    }

    void HapticPlayer::turnOff(const std::string &key)
    {
        remove(key);
    }

    void HapticPlayer::parseReceivedMessage(const char * message)
    {

        nlohmann::json JsonObject = nlohmann::json::parse(message);
        PlayerResponse Response;

        commonutils::from_json(JsonObject, Response);
        CurrentResponse = Response;
        parseResponse(Response);

    }

    void HapticPlayer::checkMessage() {
        if (!ws)
        {
            return;
        }

        if (pollingMtx.try_lock())
        {
            WebSocket::pointer wsp = &*ws;
            ws->poll();
            ws->dispatch([this](const std::string & s) {
                this->parseReceivedMessage(s.c_str());
            });
            pollingMtx.unlock();
        }
    }

    void HapticPlayer::destroy()
    {
        if (!ws)
        {
            return;
        }
        _enable = false; //ensures no more sends when destroying
        timer.stop();
        pollingMtx.lock();
        ws->close();
        ws->poll();
        ws.reset();
        pollingMtx.unlock();

        _activeDevices.erase(_activeDevices.begin(), _activeDevices.end());
        _activeKeys.erase(_activeKeys.begin(), _activeKeys.end());

#ifdef _WIN32
        WSACleanup();
#endif
    }

    void HapticPlayer::enableFeedback()
    {
        _enable = true;
    }

    void HapticPlayer::disableFeedback()
    {
        _enable = false;
    }

    void HapticPlayer::toggleFeedback()
    {
        if (!ws)
        {
            return;
        }

        _enable = !_enable;
    }

    void HapticPlayer::parseResponse(PlayerResponse response)
    {
        mtx.lock();
        _activeKeys = response.ActiveKeys;
        _activeDevices = response.ConnectedPositions;
        mtx.unlock();

        responseMtx.lock();
        _activeFeedback = response.Status;
        responseMtx.unlock();
    }

    bool HapticPlayer::isDevicePlaying(PositionType device)
    {
        mtx.lock();
        std::vector<bhaptics::PositionType> temp = _activeDevices;
        mtx.unlock();
        bool ret = std::find(temp.begin(), temp.end(), device) != temp.end();
        return ret;
    }

    bool HapticPlayer::isFeedbackRegistered(std::string key)
    {
        std::vector<std::string> tempReg = CurrentResponse.RegisteredKeys;
        bool ret = std::find(tempReg.begin(), tempReg.end(), key) != tempReg.end();
        return ret;
    }

    bool HapticPlayer::anyFilesLoaded()
    {
        return  _registered.size() > 0;
    }

    std::vector<std::string> HapticPlayer::fileNames()
    {
        std::vector<std::string> keys;
        registerMtx.lock();
        std::vector<RegisterRequest> tempRegister = _registered;
        registerMtx.unlock();

        for (size_t i = 0; i < tempRegister.size(); i++)
        {
            keys.push_back(tempRegister[i].Key);
        }
        return keys;
    }

    void HapticPlayer::registerConnection(std::string appId, std::string appName)
    {
        this->appId = appId;
        this->appName = appName;
        if (!ws)
        {
            init();
        }
    }

    std::map<std::string, std::vector<int>> HapticPlayer::getResponseStatus()
    {
        responseMtx.lock();
        std::map<std::string, std::vector<int>> ret = _activeFeedback;
        responseMtx.unlock();
        return ret;
    }
}

bhaptics::HapticPlayer *bhaptics::HapticPlayer::hapticManager = 0;

void bhaptics::HapticPlayer::changeUrl(const std::string _url) {
    url = _url;
}

#endif