//
// Created by akiva on 2/10/22.
//

#ifndef BOOTSTRAP_BHAPTICSBRIDGE_HPP
#define BOOTSTRAP_BHAPTICSBRIDGE_HPP

#include <unordered_map>
#include <android/looper.h>
#include "./Mono.h"

class BHapticsBridge {
public:
    static bool Initialize();

    class JavaCalls {
    public:

    static bool Prepare();
    static jobject getManager();

    private:
    static jobject bHapticsManagerInstance;
    };

    class InternalCalls {
    public:
        static void start();
        static void stop();
        static void turnOff(Mono::String* key);
        static void turnOffAll();
        static void registerProject(Mono::String* key, Mono::String* tactFileString);
        static void registerProjectReflected(Mono::String* key, Mono::String* tactFileString);
        static void submitRegisteredWithOption(Mono::String* key, Mono::String* altKey, float intensity, float duration, float offsetAngleX, float offsetY);
        static void submitRegisteredWithTime(Mono::String* key, float startTime);
        static void submitDotArray(Mono::String *key, Mono::String *position, int *indexes, int indexes_size, int *intensities, int intensities_size, int duration);
        static void submitPathArray(Mono::String* key, Mono::String* position, float* x, float* y, int* intensities, int* sizes, int duration);
        static Mono::String* getPositionStatus(Mono::String* position);
        static bool isRegistered(Mono::String* key);
        static bool isPlaying(Mono::String* key);
        static bool isAnythingPlaying();
    };

    class MethodMap
    {
    public:
        enum CachedClassKeys
        {
            Klass_BhapticsManagerWrapper,
            Klass_UnityPlayer,
            Klass_ApplicationState
        };
        static std::unordered_map<CachedClassKeys, jclass> CachedClasses;
        static jclass GetKlass(CachedClassKeys key);

        enum CachedMethodKeys
        {
            Player_Constructor,
            Player_Start,
            Player_Stop,
            Player_TurnOff,
            Player_TurnOffAll,
            Player_RegisterProject,
            Player_RegisterProjectReflected,
            Player_SubmitRegistered,
            Player_SubmitRegisteredWithTime,
            Player_SubmitDot,
            Player_SubmitPath,
            Player_GetPositionStatus,
            Player_IsRegistered,
            Player_IsPlaying,
            Player_IsAnythingPlaying,
            Player_PingAll,
        };
        static std::unordered_map<CachedMethodKeys, jmethodID> CachedMethods;
        static jmethodID GetMethod(CachedMethodKeys key);

        enum CachedStaticFieldKeys
        {
            Field_AppContext,
            Unity_AppContext,
        };
        static std::unordered_map<CachedStaticFieldKeys, jfieldID> CachedStaticFields;
        static jfieldID GetStaticField(CachedStaticFieldKeys key);

        static bool SeedMap();
    };

    class MonoJNIInterop {
    public:
        class Mono {
        public:
            static jstring StringConvert(::Mono::String* value);
        };
    };
};


#endif //BOOTSTRAP_BHAPTICSBRIDGE_HPP
