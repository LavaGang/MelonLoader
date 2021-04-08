//Copyright bHaptics Inc. 2017
#ifndef BHAPTICS_TIMER
#define BHAPTICS_TIMER

#include <chrono>
#include <thread>
#include <deque>
#include <mutex>
#include <future>
#include <atomic>

namespace bhaptics {
    class HapticTimer {
    public:
        HapticTimer();
        ~HapticTimer();
        void start();
        void add_timer_handler(std::function<void()> &callback);
        void stop();

    private:
        std::atomic<bool> started;
        std::atomic<bool> isRunning;
        std::function<void()> callback_function;
        int interval = 20;
        int sleepTime = 4;

        std::chrono::steady_clock::time_point prev;

        void workerFunc();

        std::thread *runner;

    };
}

#endif