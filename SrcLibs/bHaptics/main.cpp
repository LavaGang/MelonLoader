#include <iostream>
#include <fstream>
#include "shared/HapticLibrary.h"
#include <thread>
#include <chrono>
#include <iosfwd>
#include "shared/model.h"
#include "src/json.hpp"
using namespace std;
using json = nlohmann::json;
void test() {
    char playerPath[100];
    int size = 2;
    bool res = TryGetExePath(playerPath, size);
    auto path = GetExePath();

    if (res) {
        cout << "1. getExePath bHaptics Player is installed:  " << playerPath << endl  << path << endl;
        cout << "1. player path size:  " << size << endl;

    } else {
        cout << "1. Cannot find exe path.  "  << endl;
    }


#ifdef BHAPTICS_BUILDING_DLL
    cout << "defined" << endl;
#endif
    const char* appId = "com.bhaptics.yourAppId";
    cout << "2. Initialise() with com.bhaptics.yourAppId" << endl;
    auto start = std::chrono::system_clock::now();

    Initialise(appId, "SampleApp");

    auto end = std::chrono::system_clock::now();

    std::chrono::duration<double> elapsed_seconds = end - start;
    std::cout<< "2. Initialise() elapsed time: " << elapsed_seconds.count() << "s\n";

    cout << "3. Read File" << endl;

    // test.tact file should be in the same folder
    std::ifstream inFile("Pistal_L.tact");

    if (inFile.good()) {
        char inputString[100000];
        while(!inFile.eof()){
            inFile.getline(inputString, 100000);
        }
        inFile.close();
        cout << "4. RegisterFeedbackFromTactFile" << endl;
        RegisterFeedbackFromTactFileReflected("test3", inputString);
    } else {
        cout << "4. Cannot find Pistol_L.tact. Failed to register the feedback file.";
    }

    cout << "5. SubmitRegistered " << endl;
    SubmitRegistered("test3");
    SubmitRegisteredStartMillis("test4", 100);
    std::this_thread::sleep_for(std::chrono::seconds(2));

    std::vector<bhaptics::DotPoint> points;
    bhaptics::DotPoint point(0, 100);
    points.push_back(point);
    std::vector<bhaptics::PathPoint> points2;
    bhaptics::PathPoint point2(500, 500, 100);
    points2.push_back(point2);


    cout << "6. SubmitDot" << endl;
    SubmitDot("test", bhaptics::PositionType::ForearmL, points, 1000);
    SubmitDot("test2", bhaptics::PositionType::VestFront, points, 1000);
    SubmitPath("path", bhaptics::PositionType::VestBack, points2, 3000);

    std::this_thread::sleep_for(std::chrono::seconds(4));
    auto isPlaying = IsPlayingKey("path");

    if (isPlaying) {
        cout << "7. path feedback is now playing" << endl;
    }

    cout << "7. TryGetResponseForPosition" << endl;
    try {
        status s;
        bool result = TryGetResponseForPosition(bhaptics::PositionType::VestBack, s);
        if (result) {
            cout << "8. Result true" << endl;
        }
    } catch (int e) {
        cout << "8. Error " << e << endl;
    }

    cout << "9. TurnOffKey" << endl;
    TurnOffKey("test");

    std::this_thread::sleep_for(std::chrono::seconds(2));

    cout << "10. Destroy" << endl;
    Destroy();
}

int main() {
    test();

//    test();

    return 0;
}
