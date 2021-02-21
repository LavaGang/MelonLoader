//Copyright bHaptics Inc. 2017
#include "timer.h"

bhaptics::HapticTimer::HapticTimer() {
    started = false;
    isRunning = false;
}

bhaptics::HapticTimer::~HapticTimer() {
    stop();
    started = false;
    if (runner->joinable()) {
        runner->join();
    }
}

void bhaptics::HapticTimer::start() {
    if (!started) {
        started = true;
        runner = new std::thread(&HapticTimer::workerFunc, this);
    }

    isRunning = true;
}

void bhaptics::HapticTimer::add_timer_handler(std::function<void()> &callback) {
    callback_function = callback;
}

void bhaptics::HapticTimer::stop() {
    isRunning = false;
}

void bhaptics::HapticTimer::workerFunc() {
    started = true;

    while (started) {
        if (!isRunning) {
            std::this_thread::sleep_for(std::chrono::milliseconds(sleepTime));
            continue;
        }

        std::chrono::steady_clock::time_point current = std::chrono::steady_clock::now();

        bool isIntervalOver = (current >= (prev + std::chrono::milliseconds(interval - 1)));

        if(callback_function && isIntervalOver) {
            prev = current;
            try {
                callback_function();
            } catch (const std::exception& e) {
            }
        }

        std::this_thread::sleep_for(std::chrono::milliseconds(sleepTime));
    }
}
