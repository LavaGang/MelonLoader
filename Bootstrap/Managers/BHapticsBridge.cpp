//
// Created by akiva on 2/10/22.
//

#include <vector>
#include <tuple>
#include <string>
#include <unistd.h>

#include "BHapticsBridge.h"
#include "../Utils/Console/Debug.h"
#include "../Core.h"
#include "../Utils/Console/Logger.h"
#include "../Utils/Assertion.h"

bool BHapticsBridge::Initialize() {
//    if (!UIRunner::InitLooper()) return false;
    if (!MethodMap::SeedMap()) return false;
//    if (!JavaCalls::Prepare()) return false;

    return Assertion::ShouldContinue;
}

#pragma region MethodResolver

std::unordered_map<BHapticsBridge::MethodMap::CachedMethodKeys, jmethodID> BHapticsBridge::MethodMap::CachedMethods;
std::unordered_map<BHapticsBridge::MethodMap::CachedClassKeys, jclass> BHapticsBridge::MethodMap::CachedClasses;
std::unordered_map<BHapticsBridge::MethodMap::CachedStaticFieldKeys, jfieldID> BHapticsBridge::MethodMap::CachedStaticFields;

bool BHapticsBridge::MethodMap::SeedMap() {
    auto env = Core::GetEnv();

    std::vector<std::tuple<CachedClassKeys, std::string>> classMap {
            { Klass_UnityPlayer, "com/unity3d/player/UnityPlayer" },
            { Klass_BhapticsManagerWrapper, "com/bhaptics/bhapticsunity/BhapticsManagerWrapper" },
            { Klass_ApplicationState, "com/melonloader/ApplicationState" },
    };

    std::vector<std::tuple<CachedClassKeys, CachedMethodKeys, std::string, std::string>> methodMap {
            { Klass_BhapticsManagerWrapper, Player_Constructor, "<init>", "(Landroid/app/Activity;)V" },
            { Klass_BhapticsManagerWrapper, Player_TurnOff, "turnOff", "(Ljava/lang/String;)V" },
            { Klass_BhapticsManagerWrapper, Player_TurnOffAll, "turnOffAll", "()V" },
            { Klass_BhapticsManagerWrapper, Player_RegisterProject, "register", "(Ljava/lang/String;Ljava/lang/String;)V" },
            { Klass_BhapticsManagerWrapper, Player_RegisterProjectReflected, "registerReflected", "(Ljava/lang/String;Ljava/lang/String;)V" },
            { Klass_BhapticsManagerWrapper, Player_SubmitRegistered, "submitRegistered", "(Ljava/lang/String;Ljava/lang/String;FFFF)V" },
            { Klass_BhapticsManagerWrapper, Player_SubmitRegisteredWithTime, "submitRegisteredWithTime", "(Ljava/lang/String;I)V" },
            { Klass_BhapticsManagerWrapper, Player_SubmitDot, "submitDot", "(Ljava/lang/String;Ljava/lang/String;[I[II)V" },
            { Klass_BhapticsManagerWrapper, Player_SubmitPath, "submitPath", "(Ljava/lang/String;Ljava/lang/String;[F[F[II)V" },
            { Klass_BhapticsManagerWrapper, Player_GetPositionStatus, "getPositionStatus", "(Ljava/lang/String;)[B" },
            { Klass_BhapticsManagerWrapper, Player_IsRegistered, "isRegistered", "(Ljava/lang/String;)Z" },
            { Klass_BhapticsManagerWrapper, Player_IsPlaying, "isPlaying", "(Ljava/lang/String;)Z" },
            { Klass_BhapticsManagerWrapper, Player_IsAnythingPlaying, "isAnythingPlaying", "()Z"},
            { Klass_BhapticsManagerWrapper, Player_PingAll, "pingAll", "()V"},
    };

    std::vector<std::tuple<CachedClassKeys, CachedStaticFieldKeys, std::string, std::string>> staticFields {
            { Klass_ApplicationState, Field_AppContext, "Activity", "Landroid/app/Activity;" },
            { Klass_UnityPlayer, Unity_AppContext, "currentActivity", "Landroid/app/Activity;" },
    };


    for (auto [ key, path ] : classMap) {
        jclass jKlass = env->FindClass(path.c_str());
        if (jKlass == NULL)
        {
            Debug::Msgf("Failed to get class [%s]", path.c_str());
            return false;
        }

        CachedClasses[key] = jKlass;
    }

    for (auto [ klassKey, key, name, signature ] : methodMap) {
        auto jKlass = GetKlass(klassKey);

        jmethodID jMethodID = env->GetMethodID(jKlass, name.c_str(), signature.c_str());

        if (jMethodID == NULL)
        {
            Debug::Msgf("Failed to get %s%s", name.c_str(), signature.c_str());
            return false;
        }

        CachedMethods[key] = jMethodID;
    }

    for (auto [ klassKey, key, name, signature ] : staticFields) {
        auto jKlass = GetKlass(klassKey);

        jfieldID jfieldId = env->GetStaticFieldID(jKlass, name.c_str(), signature.c_str());

        if (jfieldId == NULL)
        {
            Debug::Msgf("Failed to get %s%s", name.c_str(), signature.c_str());
            return false;
        }

        CachedStaticFields[key] = jfieldId;
    }

    // Core::Bootstrap->DetachCurrentThread();

    return true;
}

jclass BHapticsBridge::MethodMap::GetKlass(CachedClassKeys key)     {
    if (CachedClasses.find(key) != CachedClasses.end())
        return CachedClasses[key];

    return NULL;
}

jmethodID BHapticsBridge::MethodMap::GetMethod(CachedMethodKeys key) {
    if (CachedMethods.find(key) != CachedMethods.end())
        return CachedMethods[key];

    return NULL;
}

jfieldID BHapticsBridge::MethodMap::GetStaticField(BHapticsBridge::MethodMap::CachedStaticFieldKeys key) {
    if (CachedStaticFields.find(key) != CachedStaticFields.end())
        return CachedStaticFields[key];

    return NULL;
}

#pragma endregion

#pragma region InternalCalls
void BHapticsBridge::InternalCalls::start() {
    Debug::Msgf("InternalCalls::BHaptics::Start");

//    CallOnUIThread_StaticVoid((void*)env->functions->CallStaticVoidMethodA, GetPlayer(), GetMethod(Player_PingAll), {});
//    env->CallStaticVoidMethod(JavaCalls::getManager(), GetMethod(Player_Start));
    JavaCalls::Prepare();

    Debug::Msgf("InternalCalls::BHaptics::Start complete");
}

void BHapticsBridge::InternalCalls::stop() {
    Debug::Msgf("InternalCalls::BHaptics::Stop");

//    env->CallStaticVoidMethod(MethodMap::GetPlayer(), MethodMap::GetMethod(MethodMap::Player_Stop));
}

void BHapticsBridge::InternalCalls::turnOff(Mono::String *key) {
    char* cKey = Mono::Exports::mono_string_to_utf8(key);
    Debug::Msgf("InternalCalls::BHaptics::TurnOff (%s)", cKey);

    auto env = Core::GetEnv();

    jstring jKey = env->NewStringUTF(cKey);
    env->CallVoidMethod(JavaCalls::getManager(), MethodMap::GetMethod(MethodMap::Player_TurnOff), jKey);
    env->DeleteLocalRef(jKey);


    Mono::Free(cKey);

    // Core::Bootstrap->DetachCurrentThread();
}

void BHapticsBridge::InternalCalls::turnOffAll() {
    Debug::Msgf("InternalCalls::BHaptics::TurnOffAll");

    auto env = Core::GetEnv();
    env->CallVoidMethod(JavaCalls::getManager(), MethodMap::GetMethod(MethodMap::Player_TurnOffAll));
    // Core::Bootstrap->DetachCurrentThread();
}

void BHapticsBridge::InternalCalls::registerProject(Mono::String *key, Mono::String *tactFileString) {
    char* cKey = Mono::Exports::mono_string_to_utf8(key);
    char* cContents = Mono::Exports::mono_string_to_utf8(tactFileString);

    Debug::Msgf("InternalCalls::BHaptics::RegisterProject (%s, TRUNCATED_STRING)", cKey);

    auto env = Core::GetEnv();
    jstring jKey = env->NewStringUTF(cKey);
    jstring jContents = env->NewStringUTF(cContents);

    env->CallVoidMethod(JavaCalls::getManager(), MethodMap::GetMethod(MethodMap::Player_RegisterProject), jKey, jContents);

    env->DeleteLocalRef(jKey);
    env->DeleteLocalRef(jContents);

    // Core::Bootstrap->DetachCurrentThread();

    Mono::Free(cKey);
    Mono::Free(cContents);
}

void BHapticsBridge::InternalCalls::registerProjectReflected(Mono::String *key, Mono::String *tactFileString) {
    char* cKey = Mono::Exports::mono_string_to_utf8(key);
    char* cContents = Mono::Exports::mono_string_to_utf8(tactFileString);

    Debug::Msgf("InternalCalls::BHaptics::RegisterProjectReflected (%s, TRUNCATED_STRING)", cKey);

    auto env = Core::GetEnv();

    jstring jKey = env->NewStringUTF(cKey);
    jstring jContents = env->NewStringUTF(cContents);

    env->CallVoidMethod(JavaCalls::getManager(), MethodMap::GetMethod(MethodMap::Player_RegisterProjectReflected), jKey, jContents);

    env->DeleteLocalRef(jKey);
    env->DeleteLocalRef(jContents);

    // Core::Bootstrap->DetachCurrentThread();

    Mono::Free(cKey);
    Mono::Free(cContents);
}

void BHapticsBridge::InternalCalls::submitRegisteredWithOption(Mono::String *key, Mono::String *altKey, float intensity,
                                                               float duration, float offsetAngleX, float offsetY) {
    char* cKey = Mono::Exports::mono_string_to_utf8(key);
    char* cAltKey = Mono::Exports::mono_string_to_utf8(altKey);

    Debug::Msgf("InternalCalls::BHaptics::SubmitRegistered %s %s (%f, %f, %f, %f)", cKey, cAltKey, intensity, duration, offsetAngleX, offsetY);

    auto env = Core::GetEnv();

    jstring jKey = env->NewStringUTF(cKey);
    jstring jAlt = env->NewStringUTF(cAltKey);

    env->CallVoidMethod(JavaCalls::getManager(), MethodMap::GetMethod(MethodMap::Player_SubmitRegistered), jKey, jAlt, intensity, duration, offsetAngleX, offsetY);

    env->DeleteLocalRef(jKey);
    env->DeleteLocalRef(jAlt);

    // Core::Bootstrap->DetachCurrentThread();

    Mono::Free(cKey);
    Mono::Free(cAltKey);

}

void BHapticsBridge::InternalCalls::submitRegisteredWithTime(Mono::String *key, float startTime) {
    char* cKey = Mono::Exports::mono_string_to_utf8(key);

    Debug::Msgf("InternalCalls::BHaptics::SubmitRegisteredWithTime %s %d", cKey, startTime);

    auto env = Core::GetEnv();
    jstring jKey = env->NewStringUTF(cKey);

    env->CallVoidMethod(JavaCalls::getManager(), MethodMap::GetMethod(MethodMap::Player_SubmitRegisteredWithTime), jKey, startTime);

    env->DeleteLocalRef(jKey);

    // Core::Bootstrap->DetachCurrentThread();

    Mono::Free(cKey);
}

void
BHapticsBridge::InternalCalls::submitDotArray(Mono::String *key, Mono::String *position, int *indexes, int indexes_size, int *intensities,
                                              int intensities_size, int duration) {
    char* cKey = Mono::Exports::mono_string_to_utf8(key);
    char* cPosition = Mono::Exports::mono_string_to_utf8(position);

    Debug::Msgf("InternalCalls::BHaptics::Internal_SubmitDot %s %s (%d, %d) %d", cKey, cPosition, indexes_size, intensities_size, duration);

    for (size_t i = 0; i < intensities_size; i++) {
        Debug::Msgf("[%d, %d] %d %d", indexes_size, intensities_size, indexes[i], intensities[i]);
    }

    auto env = Core::GetEnv();
    jstring jKey = env->NewStringUTF(cKey);
    jstring jPosition = env->NewStringUTF(cPosition);
    jintArray jIndex = env->NewIntArray(indexes_size);
    jintArray jIntensity = env->NewIntArray(intensities_size);

    env->SetIntArrayRegion(jIndex, 0, indexes_size, indexes);
    env->SetIntArrayRegion(jIntensity, 0, intensities_size, intensities);

//    env->CallVoidMethod(JavaCalls::getManager(), MethodMap::GetMethod(MethodMap::Player_PingAll));
    env->CallVoidMethod(JavaCalls::getManager(), MethodMap::GetMethod(MethodMap::Player_SubmitDot), jKey, jPosition, jIndex, jIntensity, duration);

    env->DeleteLocalRef(jKey);
    env->DeleteLocalRef(jPosition);
    env->DeleteLocalRef(jIndex);
    env->DeleteLocalRef(jIntensity);

    // Core::Bootstrap->DetachCurrentThread();

    Mono::Free(cKey);
    Mono::Free(cPosition);
}

void BHapticsBridge::InternalCalls::submitPathArray(Mono::String *key, Mono::String *position, float *x, float *y,
                                                    int *intensities, int* sizes, int duration) {
    char* cKey = Mono::Exports::mono_string_to_utf8(key);
    char* cPosition = Mono::Exports::mono_string_to_utf8(position);

    size_t x_size = sizes[0];
    size_t y_size = sizes[1];
    size_t intensities_size = sizes[2];

    Debug::Msgf("InternalCalls::BHaptics::Internal_SubmitPath %s %s (%d, %d, %d)", cKey, cPosition, x_size, y_size, intensities_size);

    auto env = Core::GetEnv();
    jstring jKey = env->NewStringUTF(cKey);
    jstring jPosition = env->NewStringUTF(cPosition);
    jfloatArray jXPos = env->NewFloatArray(x_size);
    jfloatArray jYPos = env->NewFloatArray(y_size);
    jintArray jIntensity = env->NewIntArray(intensities_size);

    env->SetFloatArrayRegion(jXPos, 0, x_size, x);
    env->SetFloatArrayRegion(jYPos, 0, y_size, y);
    env->SetIntArrayRegion(jIntensity, 0, intensities_size, intensities);

    env->CallVoidMethod(JavaCalls::getManager(), MethodMap::GetMethod(MethodMap::Player_SubmitPath), jKey, jPosition, jXPos, jYPos, jIntensity, duration);

    env->DeleteLocalRef(jKey);
    env->DeleteLocalRef(jPosition);
    env->DeleteLocalRef(jXPos);
    env->DeleteLocalRef(jYPos);
    env->DeleteLocalRef(jIntensity);

    // Core::Bootstrap->DetachCurrentThread();

    Mono::Free(cKey);
    Mono::Free(cPosition);
}

Mono::String *BHapticsBridge::InternalCalls::getPositionStatus(Mono::String *position) {
    Logger::Error("getPositionStatus not implemented yet");
    return nullptr;
}

bool BHapticsBridge::InternalCalls::isRegistered(Mono::String *key) {
    char* cKey = Mono::Exports::mono_string_to_utf8(key);

    Debug::Msgf("InternalCalls::BHaptics::IsRegistered %s", cKey);

    auto env = Core::GetEnv();
    jstring jKey = env->NewStringUTF(cKey);

    jboolean res = env->CallBooleanMethod(JavaCalls::getManager(), MethodMap::GetMethod(MethodMap::Player_IsRegistered), jKey);

    env->DeleteLocalRef(jKey);

    Mono::Free(cKey);

    // Core::Bootstrap->DetachCurrentThread();

    return res;
}

bool BHapticsBridge::InternalCalls::isPlaying(Mono::String *key) {
    char* cKey = Mono::Exports::mono_string_to_utf8(key);

    Debug::Msgf("InternalCalls::BHaptics::IsPlaying %s", cKey);

    auto env = Core::GetEnv();
    jstring jKey = env->NewStringUTF(cKey);

    jboolean res = env->CallBooleanMethod(JavaCalls::getManager(), MethodMap::GetMethod(MethodMap::Player_IsPlaying), jKey);

    env->DeleteLocalRef(jKey);

    // Core::Bootstrap->DetachCurrentThread();

    Mono::Free(cKey);

    return res;
}

bool BHapticsBridge::InternalCalls::isAnythingPlaying() {
    Debug::Msgf("InternalCalls::BHaptics::TurnOffAll");

    auto env = Core::GetEnv();
    jboolean res = env->CallBooleanMethod(JavaCalls::getManager(), MethodMap::GetMethod(MethodMap::Player_TurnOffAll));
    return res;
}

#pragma endregion

#pragma region MonoJNIInterop

jstring BHapticsBridge::MonoJNIInterop::Mono::StringConvert(::Mono::String* value) {
    auto env = Core::GetEnv();

    char* cValue = ::Mono::Exports::mono_string_to_utf8(value);
    jstring jValue = env->NewStringUTF(cValue);
    ::Mono::Free(cValue);

    // Core::Bootstrap->DetachCurrentThread();

    return jValue;
}

#pragma endregion

#pragma region JavaCalls

jobject BHapticsBridge::JavaCalls::bHapticsManagerInstance = NULL;

bool BHapticsBridge::JavaCalls::Prepare() {
    if (bHapticsManagerInstance != NULL)
        return true;

    JNIEnv* env = Core::GetEnv();

    MethodMap::SeedMap();
    if (env == NULL) {
        Assertion::ThrowInternalFailure("Failed to find thread");
        return false;
    }

    Debug::Msg("JavaCalls::Prepare checking");

//    auto unityPlayer = MethodMap::GetKlass(MethodMap::Klass_UnityPlayer);
//    auto activityField = MethodMap::GetStaticField(MethodMap::Unity_AppContext);

    auto activity = env->GetStaticObjectField(
            MethodMap::GetKlass(MethodMap::Klass_UnityPlayer),
            MethodMap::GetStaticField(MethodMap::Unity_AppContext));
    if (activity == NULL) {
        Debug::Msg("Failed to find activity, waiting");
//        Assertion::ThrowInternalFailure("Failed to find activity");
//        return Assertion::ShouldContinue;
        return false;
    }

    Debug::Msg("got field");

    Debug::Msg("JavaCalls::Prepare creating");
    auto localManager = env->functions->NewObject(
            env,
            MethodMap::GetKlass(MethodMap::Klass_BhapticsManagerWrapper),
            MethodMap::GetMethod(MethodMap::Player_Constructor),
            activity
            );

    Debug::Msg("JavaCalls::Prepare created");
    bHapticsManagerInstance = env->NewGlobalRef(localManager);

    env->DeleteLocalRef(localManager);
    // Core::Bootstrap->DetachCurrentThread();

    return true;
}

jobject BHapticsBridge::JavaCalls::getManager() {
    if (!BHapticsBridge::JavaCalls::Prepare()) {
        Assertion::ThrowInternalFailure("Failed to prepare");
        return nullptr;
    }

    return bHapticsManagerInstance;
}

#pragma endregion