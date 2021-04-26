#include "bHapticsPlayer.h"
#include "../Base/Core.h"
#include "../Utils/Console/Debug.h"

void bHapticsPlayer::Core::TurnOff(const char*)
{
}

void bHapticsPlayer::Core::TurnOffAll()
{
}

void bHapticsPlayer::Core::RegisterProject(const char*, const char*)
{
	
}

void bHapticsPlayer::Core::RegisterProjectReflected(const char*, const char*)
{
}

void bHapticsPlayer::Core::SubmitRegisteredWithOption(const char*, const char*, t_options)
{
}

void bHapticsPlayer::Core::SubmitRegisteredWithTime(const char*, int)
{
}

void bHapticsPlayer::Core::SubmitDotArray(const char* key, const char* position, int* index, size_t index_len, int* intensity, size_t intensity_len, int duration)
{
    Debug::Msg("Testing SubmitPathArray");
    jclass playerWrapper = ::Core::Env->FindClass("com/melonloader/bhaptics/PlayerWrapper");
    if (playerWrapper == NULL)
    {
        Debug::Msg("Failed to find wrapper");
        return;
    }

    jmethodID submitDot = ::Core::Env->GetMethodID(playerWrapper, "submitDot", "(Ljava/lang/String;Ljava/lang/String;[I[II)V");
    if (submitDot == NULL)
    {
        Debug::Msg("Failed to get method id");
        return;
    }
	
    jstring jkey = ::Core::Env->NewStringUTF(key);
    jstring jposition = ::Core::Env->NewStringUTF(position);
    jintArray jindex = ::Core::Env->NewIntArray(index_len);
    jintArray jintensity = ::Core::Env->NewIntArray(intensity_len);
    jint jduration = duration;

    ::Core::Env->SetIntArrayRegion(jindex, 0, index_len, index);
    ::Core::Env->SetIntArrayRegion(jintensity, 0, intensity_len, intensity);

    ::Core::Env->CallStaticVoidMethod(playerWrapper, submitDot, jkey, jposition, jindex, jintensity, jduration);
}

void bHapticsPlayer::Core::SubmitPathArray(const char* key, const char* position, int* index, size_t index_len, int* intensity, size_t intensity_len, int duration)
{

}

std::vector<int> bHapticsPlayer::Core::GetPositionStatus(const char*)
{
}

bool bHapticsPlayer::Core::IsRegistered(const char*)
{
}

bool bHapticsPlayer::Core::IsPlaying(const char*)
{
}

bool bHapticsPlayer::Core::IsAnythingPlaying(const char*)
{
}

jobject bHapticsPlayer::Core::GetPlayer()
{
    jfieldID fieldId;
    jint fieldValue;

    jclass klass = ::Core::Env->FindClass("com/melonloader/bhaptics/DeviceManager");

    fieldId = ::Core::Env->GetStaticFieldID(klass, "player", "Lcom/bhaptics/bhapticsmanger/HapticPlayer");
    if (fieldId == NULL) {
        return NULL;
    }
	
    return ::Core::Env->GetStaticObjectField(klass, fieldId);
}
