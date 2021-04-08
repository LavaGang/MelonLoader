#ifndef HAPTIC_LIBRARY_H
#define HAPTIC_LIBRARY_H


#if defined _WIN32 || defined __CYGWIN__
#ifdef BHAPTICS_BUILDING_DLL
#ifdef __GNUC__
      #define DLL_PUBLIC __attribute__ ((dllexport))
    #else
      #define DLL_PUBLIC __declspec(dllexport) // Note: actually gcc seems to also supports this syntax.
    #endif
#else
#ifdef __GNUC__
#define DLL_PUBLIC __attribute__ ((dllimport))
#else
#define DLL_PUBLIC __declspec(dllimport) // Note: actually gcc seems to also supports this syntax.
#endif
#endif
#define DLL_LOCAL
#else
#if __GNUC__ >= 4
#define DLL_PUBLIC __attribute__ ((visibility ("default")))
#define DLL_LOCAL  __attribute__ ((visibility ("hidden")))
#else
#define DLL_PUBLIC
    #define DLL_LOCAL
#endif
#endif

#include "model.h"
#include <vector>


#ifdef __cplusplus
extern "C" {  // only need to export C interface if
// used by C++ source code
#endif

struct point {
    float x, y;
    int intensity;
    int motorCount;
};

struct status {
    int values[20];
};

DLL_PUBLIC void ChangeUrl(const char* url);

DLL_PUBLIC bool TryGetExePath(char* buf, int& size);

DLL_PUBLIC const char* GetExePath();

// Initialises a connection to the bHaptics Player. Should only be called once: when the game starts.
DLL_PUBLIC void Initialise(const char* appId, const char* appName);
DLL_PUBLIC void InitialiseSync(const char* appId, const char* appName);

// End the connecection to the bHaptics Player. Should only be called once: when the game ends.
DLL_PUBLIC void Destroy();

// Register a preset .tact feedback file, created using the bHaptics Designer.
// Registered files can then be called and played back using the given key.
// File is submitted as an already processed JSON string of the Project attribute.
DLL_PUBLIC void RegisterFeedback(const char* key, const char* projectJson);

DLL_PUBLIC void RegisterFeedbackFromTactFile(const char* key, const char* tactFileStr);

DLL_PUBLIC void RegisterFeedbackFromTactFileReflected(const char* key, const char* tactFileStr);

// Register a preset .tact feedback file, created using the bHaptics Designer.
// Registered files can then be called and played back using the given key.
// File Path is given, and feedback file is parsed and processed by the SDK.
DLL_PUBLIC void LoadAndRegisterFeedback(const char* Key, const char* FilePath);

// Submit a request to play a registered feedback file using its Key.
DLL_PUBLIC void SubmitRegistered(const char* key);

DLL_PUBLIC void SubmitRegisteredStartMillis(const char* key, int startMillis);

// Submit a request to play a registered feedback file, with additional options.
// ScaleOption scales the intensity and duration of the feedback by some factor.
// RotationOption uses cylindrical projection to rotate a Vest feedback file, as well as the vertical position.
// AltKey provides a unique key to play this custom feedback under, as opposed to the original feedback Key.
DLL_PUBLIC void SubmitRegisteredAlt(const char* Key, const char* AltKey, bhaptics::ScaleOption ScaleOpt, bhaptics::RotationOption RotOption);

DLL_PUBLIC void SubmitRegisteredWithOption(const char* Key, const char* AltKey, float intensity, float duration, float offsetAngleX, float offsetAngleY);

DLL_PUBLIC void SubmitByteArray(const char *key, bhaptics::PositionType Pos, unsigned char *buf, size_t length,
                                int durationMillis);

DLL_PUBLIC void SubmitPathArray(const char *key, bhaptics::PositionType Pos, point *points, size_t length,
                                int durationMillis);

// Submit an array of 20 integers, representing the strength of each motor vibration, ranging from 0 to 100.
// Specify the Position (playback device) as well as the duration of the feedback effect in milliseconds.
DLL_PUBLIC void Submit(const char* Key, bhaptics::PositionType Pos, std::vector<uint8_t>& MotorBytes, int DurationMillis);

// Submit an array of DotPoints, representing the motor's Index and Intensity.
// Specify the Position (playback device) as well as the duration of the feedback effect in milliseconds.
DLL_PUBLIC void SubmitDot(const char* Key, bhaptics::PositionType Pos, std::vector<bhaptics::DotPoint>& Points, int DurationMillis);

// Submit an array of PathPoints, representing the xy-position of the feedback on a unit square.
// Based off the intensity and position, specific motors are chosen by the bHaptics Player and vibrated.
// Specify the Position (playback device) as well as the duration of the feedback effect in milliseconds.
DLL_PUBLIC void SubmitPath(const char* Key, bhaptics::PositionType Pos, std::vector<bhaptics::PathPoint>& Points, int DurationMillis);

// Boolean to check if a Feedback has been registered or not under the given Key.
DLL_PUBLIC bool IsFeedbackRegistered(const char* key);

// Boolean to check if there are any Feedback effects currently playing.
DLL_PUBLIC bool IsPlaying();

// Boolean to check if a feedback effect under the given Key is currently playing.
DLL_PUBLIC bool IsPlayingKey(const char* Key);

// Turn off all currently playing feedback effects.
DLL_PUBLIC void TurnOff();

// Turn off the feedback effect specified by the Key.
DLL_PUBLIC void TurnOffKey(const char* Key);

// Enable Haptic Feedback calls to the bHaptics Player (on by default)
DLL_PUBLIC void EnableFeedback();

// Disable Haptic Feedback calls to the bHaptics Player
DLL_PUBLIC void DisableFeedback();

// Toggle between Enabling/Disabling haptic feedback.
DLL_PUBLIC void ToggleFeedback();

// Boolean to check if a specific device is connected to the bHaptics Player.
DLL_PUBLIC bool IsDevicePlaying(bhaptics::PositionType Pos);

// Returns an array of the current status of each device.
// Used for UI to ensure that haptic feedback is playing.
//DLL_PUBLIC void GetResponseStatus(std::vector<bhaptics::HapticFeedback>& retValues);

// Returns the current motor values for a given device.
// Used for UI to ensure that haptic feedback is playing.
// return value is 20
DLL_PUBLIC bool TryGetResponseForPosition(bhaptics::PositionType Pos, status& s);

#ifdef __cplusplus
}
#endif

#endif