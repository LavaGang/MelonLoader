#include "bHapticsPlayer.h"
#include "../Base/Core.h"
#include "../Utils/Console/Debug.h"

void bHapticsPlayer::HapticPlayer::TurnOff(const char* key)
{
    auto [ playerKlass, player ] = GetPlayer();

    jmethodID jMID;
    if ((jMID = GetMethod(playerKlass, "turnOff", "(Ljava/lang/String;)V", Player_TurnOff)) == NULL)
        return;

    jstring jkey = Core::Env->NewStringUTF(key);

    Core::Env->CallVoidMethod(player, jMID, jkey);

    Core::Env->DeleteLocalRef(jkey);
}

void bHapticsPlayer::HapticPlayer::TurnOffAll()
{
    auto [ playerKlass, player ] = GetPlayer();

    jmethodID jMID;
    if ((jMID = GetMethod(playerKlass, "turnOffAll", "()V", Player_TurnOffAll)) == NULL)
        return;
    
    Core::Env->CallVoidMethod(player, jMID);
}

void bHapticsPlayer::HapticPlayer::RegisterProject(const char* key, const char* contents)
{
    auto [ playerKlass, player ] = GetPlayer();

    jmethodID jMID;
    if ((jMID = GetMethod(playerKlass, "registerProject", "(Ljava/lang/String;Ljava/lang/String;)V", Player_RegisterProject)) == NULL)
        return;

    jstring jkey = Core::Env->NewStringUTF(key);
    jstring jcontents = Core::Env->NewStringUTF(contents);

    Core::Env->CallVoidMethod(player, jMID, jkey, jcontents);

    Core::Env->DeleteLocalRef(jkey);
    Core::Env->DeleteLocalRef(jcontents);
}

void bHapticsPlayer::HapticPlayer::RegisterProjectReflected(const char* key, const char* contents)
{
    auto [ playerKlass, player ] = GetPlayer();

    jmethodID jMID;
    if ((jMID = GetMethod(playerKlass, "registerProjectReflected", "(Ljava/lang/String;Ljava/lang/String;)V", Player_RegisterProjectReflected)) == NULL)
        return;

    jstring jkey = Core::Env->NewStringUTF(key);
    jstring jcontents = Core::Env->NewStringUTF(contents);
    
    Core::Env->CallVoidMethod(player, jMID, jkey, jcontents);

    Core::Env->DeleteLocalRef(jkey);
    Core::Env->DeleteLocalRef(jcontents);
}

void bHapticsPlayer::HapticPlayer::SubmitRegistered(const char* key, const char* alt, t_options& options)
{
    auto [ playerKlass, player ] = GetPlayer();

    jmethodID jMID;
    if ((jMID = GetMethod(playerKlass, "submitRegistered", "(Ljava/lang/String;Ljava/lang/String;FFFF)V", Player_SubmitRegistered)) == NULL)
        return;

    jstring jkey = Core::Env->NewStringUTF(key);
    jstring jalt = Core::Env->NewStringUTF(alt);

    Core::Env->CallVoidMethod(player, jMID, jkey, jalt, options.intensity, options.duration, options.offsetAngleX, options.offsetY);
    
    Core::Env->DeleteLocalRef(jkey);
    Core::Env->DeleteLocalRef(jalt);
}

void bHapticsPlayer::HapticPlayer::SubmitRegisteredWithTime(const char* key, int startTime)
{
    auto [ playerKlass, player ] = GetPlayer();

    jmethodID jMID;
    if ((jMID = GetMethod(playerKlass, "submitRegisteredWithTime", "(Ljava/lang/String;I)V", Player_SubmitRegisteredWithTime)) == NULL)
        return;

    jstring jkey = Core::Env->NewStringUTF(key);

    Core::Env->CallVoidMethod(player, jMID, jkey, startTime);

    Core::Env->DeleteLocalRef(jkey);
}

void bHapticsPlayer::HapticPlayer::SubmitDot(const char* key, const char* position, int* index, size_t index_len, int* intensity, size_t intensity_len, int duration)
{
    auto [ playerKlass, player ] = GetPlayer();

    jmethodID jMID;
    if ((jMID = GetMethod(playerKlass, "submitDot", "(Ljava/lang/String;Ljava/lang/String;[I[II)V", Player_SubmitDot)) == NULL)
        return;
	
    jstring jkey = ::Core::Env->NewStringUTF(key);
    jstring jposition = ::Core::Env->NewStringUTF(position);
    jintArray jindex = ::Core::Env->NewIntArray(index_len);
    jintArray jintensity = ::Core::Env->NewIntArray(intensity_len);

    ::Core::Env->SetIntArrayRegion(jindex, 0, index_len, index);
    ::Core::Env->SetIntArrayRegion(jintensity, 0, intensity_len, intensity);

    ::Core::Env->CallVoidMethod(player, jMID, jkey, jposition, jindex, jintensity, duration);

    Core::Env->DeleteLocalRef(jkey);
    Core::Env->DeleteLocalRef(jposition);
    Core::Env->DeleteLocalRef(jindex);
    Core::Env->DeleteLocalRef(jintensity);
}

void bHapticsPlayer::HapticPlayer::SubmitPath(const char* key, const char* position, float* xPos, size_t xPos_len, float* yPos, size_t yPos_len, int* intensity, size_t intensity_len, int duration)
{
    auto [ playerKlass, player ] = GetPlayer();

    jmethodID jMID;
    if ((jMID = GetMethod(playerKlass, "submitPath", "(Ljava/lang/String;Ljava/lang/String;[F[F[II)V", Player_SubmitPath)) == NULL)
        return;
	
    jstring jkey = ::Core::Env->NewStringUTF(key);
    jstring jposition = ::Core::Env->NewStringUTF(position);
    jfloatArray jXPos = ::Core::Env->NewFloatArray(xPos_len);
    jfloatArray jYPos = ::Core::Env->NewFloatArray(yPos_len);
    jintArray jintensity = ::Core::Env->NewIntArray(intensity_len);

    ::Core::Env->SetFloatArrayRegion(jXPos, 0, xPos_len, xPos);
    ::Core::Env->SetFloatArrayRegion(jYPos, 0, yPos_len, yPos);
    ::Core::Env->SetIntArrayRegion(jintensity, 0, intensity_len, intensity);

    ::Core::Env->CallVoidMethod(player, jMID, jkey, jposition, jXPos, jYPos, jintensity, duration);

    Core::Env->DeleteLocalRef(jkey);
    Core::Env->DeleteLocalRef(jposition);
    Core::Env->DeleteLocalRef(jXPos);
    Core::Env->DeleteLocalRef(jYPos);
    Core::Env->DeleteLocalRef(jintensity);
}

std::vector<char> bHapticsPlayer::HapticPlayer::GetPositionStatus(const char* position)
{
    auto [ playerKlass, player ] = GetPlayer();

    jmethodID jMID;
    if ((jMID = GetMethod(playerKlass, "getPositionStatus", "(Lcom/bhaptics/commons/model/PositionType;)[B", Player_GetPositionStatus)) == NULL)
        return;
    
    jclass jPositionEnumKlass  = Core::Env->FindClass("com/bhaptics/commons/model/PositionType");
    
    jmethodID jPositionMID;
    if ((jPositionMID = GetMethod(jPositionEnumKlass, "valueOf", "(Ljava/lang/String;)Lcom/bhaptics/commons/model/PositionType;", PositionEnum_ValueOf)) == NULL)
        return;

    jstring jPostionStr = ::Core::Env->NewStringUTF(position);

    jobject jPostion = Core::Env->CallStaticObjectMethod(jPositionEnumKlass, jPositionMID, jPostionStr);
    
    jbyteArray jStatuses = (jbyteArray)Core::Env->CallObjectMethod(player, jMID, jPostion);

    size_t jStatusesSize = Core::Env->GetArrayLength(jStatuses);
    std::vector<char> statuses(jStatusesSize);
    jbyte* dataBytes = reinterpret_cast<jbyte*>(statuses.data());

    Core::Env->GetByteArrayRegion(jStatuses, 0, jStatusesSize, dataBytes);

    Core::Env->DeleteLocalRef(jPostionStr);
    Core::Env->DeleteLocalRef(jStatuses);

    return statuses;
}

bool bHapticsPlayer::HapticPlayer::IsRegistered(const char* key)
{
    auto [ playerKlass, player ] = GetPlayer();

    jmethodID jMID;
    if ((jMID = GetMethod(playerKlass, "isRegistered", "(Ljava/lang/String;)Z", Player_IsRegistered)) == NULL)
        return;

    jstring jkey = Core::Env->NewStringUTF(key);

    jboolean res = Core::Env->CallBooleanMethod(player, jMID, jkey);

    Core::Env->DeleteLocalRef(jkey);

    return res;
}

bool bHapticsPlayer::HapticPlayer::IsPlaying(const char* key)
{
    auto [ playerKlass, player ] = GetPlayer();

    jmethodID jMID;
    if ((jMID = GetMethod(playerKlass, "isPlaying", "(Ljava/lang/String;)Z", Player_IsPlaying)) == NULL)
        return;

    jstring jkey = Core::Env->NewStringUTF(key);

    jboolean res = Core::Env->CallBooleanMethod(player, jMID, jkey);

    Core::Env->DeleteLocalRef(jkey);

    return res;
}

bool bHapticsPlayer::HapticPlayer::IsAnythingPlaying()
{
    auto [ playerKlass, player ] = GetPlayer();

    jmethodID jMID;
    if ((jMID = GetMethod(playerKlass, "isAnythingPlaying", "()Z", Player_IsAnythingPlaying)) == NULL)
        return;

    jboolean res = Core::Env->CallBooleanMethod(player, jMID);
    
    return res;
}

std::tuple<jclass, jobject> bHapticsPlayer::HapticPlayer::GetPlayer()
{
    if (PlayerClass != NULL)
        return { PlayerClass, PlayerClassInstance };
    
    PlayerClass = ::Core::Env->FindClass("com/melonloader/bhaptics/DeviceManager");

    jfieldID fieldId = ::Core::Env->GetStaticFieldID(PlayerClass, "player", "Lcom/bhaptics/bhapticsmanger/HapticPlayer");
    if (fieldId == NULL) {
        return { NULL, NULL };
    }

    PlayerClassInstance = Core::Env->GetStaticObjectField(PlayerClass, fieldId);
    
    return { PlayerClass, PlayerClassInstance };
}

jmethodID bHapticsPlayer::GetMethod(jclass klass, const char* name, const char* sig, CachedMethodKeys key)
{
    if (CachedMethods.find(key) != CachedMethods.end())
        return CachedMethods[key];

    jmethodID jMethodID = ::Core::Env->GetMethodID(klass, name, sig);
    if (jMethodID == NULL)
    {
        Debug::Msgf("Failed to get %s%s", name, sig);
    }

    CachedMethods[key] = jMethodID;
    return jMethodID;
}
