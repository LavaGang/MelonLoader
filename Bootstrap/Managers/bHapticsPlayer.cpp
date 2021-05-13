#include "bHapticsPlayer.h"
#include "../Base/Core.h"
#include "../Utils/AnalyticsBlocker.h"
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

void bHapticsPlayer::HapticPlayer::SubmitRegistered(const char* key, const char* alt, float intensity, float duration, float offsetAngleX, float offsetY)
{
    auto [ playerKlass, player ] = GetPlayer();

    jmethodID jMID;
    if ((jMID = GetMethod(playerKlass, "submitRegistered", "(Ljava/lang/String;Ljava/lang/String;FFFF)V", Player_SubmitRegistered)) == NULL)
        return;

    jstring jkey = Core::Env->NewStringUTF(key);
    jstring jalt = Core::Env->NewStringUTF(alt);

    Core::Env->CallVoidMethod(player, jMID, jkey, jalt, intensity, duration, offsetAngleX, offsetY);
    
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

    Core::Env->SetIntArrayRegion(jindex, 0, index_len, index);
    Core::Env->SetIntArrayRegion(jintensity, 0, intensity_len, intensity);

    Core::Env->CallVoidMethod(player, jMID, jkey, jposition, jindex, jintensity, duration);

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
        return std::vector<char>();
    
    jclass jPositionEnumKlass  = Core::Env->FindClass("com/bhaptics/commons/model/PositionType");
    
    jmethodID jPositionMID;
    if ((jPositionMID = GetMethod(jPositionEnumKlass, "valueOf", "(Ljava/lang/String;)Lcom/bhaptics/commons/model/PositionType;", PositionEnum_ValueOf)) == NULL)
        return std::vector<char>();

    jstring jPostionStr = ::Core::Env->NewStringUTF(position);

    jobject jPosition = Core::Env->CallStaticObjectMethod(jPositionEnumKlass, jPositionMID, jPostionStr);
    
    jbyteArray jStatuses = (jbyteArray)Core::Env->CallObjectMethod(player, jMID, jPosition);

    size_t jStatusesSize = Core::Env->GetArrayLength(jStatuses);
    std::vector<char> statuses(jStatusesSize);
    jbyte* dataBytes = reinterpret_cast<jbyte*>(statuses.data());

    Core::Env->GetByteArrayRegion(jStatuses, 0, jStatusesSize, dataBytes);

    Core::Env->DeleteLocalRef(jPostionStr);

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
    
    if ((PlayerClass = GetKlass("com/melonloader/bhaptics/DeviceManager", CachedClassKeys::DeviceManager)) == NULL)
        return { NULL, NULL };

    jfieldID fieldId = ::Core::Env->GetStaticFieldID(PlayerClass, "player", "Lcom/bhaptics/bhapticsmanger/HapticPlayer");
    if (fieldId == NULL) {
        return { NULL, NULL };
    }

    PlayerClassInstance = Core::Env->GetStaticObjectField(PlayerClass, fieldId);
    
    return { PlayerClass, PlayerClassInstance };
}

void bHapticsPlayer::BhapticsManager::Scan()
{
    auto [ managerKlass, manager ] = GetManager();

    jmethodID jMID;
    if ((jMID = GetMethod(managerKlass, "scan", "()V", Manager_Scan)) == NULL)
        return;

    Core::Env->CallVoidMethod(manager, jMID);
}

void bHapticsPlayer::BhapticsManager::StopScan()
{
    auto [ managerKlass, manager ] = GetManager();

    jmethodID jMID;
    if ((jMID = GetMethod(managerKlass, "stopScan", "()V", Manager_StopScan)) == NULL)
        return;

    Core::Env->CallVoidMethod(manager, jMID);
}

bool bHapticsPlayer::BhapticsManager::IsScanning()
{
    auto [ managerKlass, manager ] = GetManager();

    jmethodID jMID;
    if ((jMID = GetMethod(managerKlass, "isScanning", "()Z", Manager_IsScanning)) == NULL)
        return;

    return Core::Env->CallBooleanMethod(manager, jMID);
}

void bHapticsPlayer::BhapticsManager::RefreshPairingInfo()
{
    auto [ managerKlass, manager ] = GetManager();

    jmethodID jMID;
    if ((jMID = GetMethod(managerKlass, "refreshPairingInfo", "()V", Manager_RefreshPairingInfo)) == NULL)
        return;

    Core::Env->CallVoidMethod(manager, jMID);
}

void bHapticsPlayer::BhapticsManager::Pair(const char* address)
{
    auto [ managerKlass, manager ] = GetManager();

    jmethodID jMID;
    if ((jMID = GetMethod(managerKlass, "pair", "(Ljava/lang/String;)V", Manager_Pair)) == NULL)
        return;

    jstring jaddress = Core::Env->NewStringUTF(address);

    Core::Env->CallVoidMethod(manager, jMID, jaddress);

    Core::Env->DeleteLocalRef(jaddress);
}

void bHapticsPlayer::BhapticsManager::PairPositioned(const char* address, const char* position)
{
    auto [ managerKlass, manager ] = GetManager();

    jmethodID jMID;
    if ((jMID = GetMethod(managerKlass, "pair", "(Ljava/lang/String;Lcom/bhaptics/commons/model/PositionType;)V", Manager_PairPositioned)) == NULL)
        return;

    jclass jPositionEnumKlass = GetKlass("com/bhaptics/commons/model/PositionType", CachedClassKeys::PositionType);
    
    jmethodID jPositionMID;
    if ((jPositionMID = GetMethod(jPositionEnumKlass, "valueOf", "(Ljava/lang/String;)Lcom/bhaptics/commons/model/PositionType;", PositionEnum_ValueOf)) == NULL)
        return;

    jstring jPostionStr = Core::Env->NewStringUTF(position);
    jobject jPosition = Core::Env->CallStaticObjectMethod(jPositionEnumKlass, jPositionMID, jPostionStr);

    jstring jaddress = Core::Env->NewStringUTF(address);
    Core::Env->CallVoidMethod(manager, jMID, jaddress, jPosition);

    Core::Env->DeleteLocalRef(jaddress);
    Core::Env->DeleteLocalRef(jPostionStr);
}

void bHapticsPlayer::BhapticsManager::Unpair(const char* address)
{
    auto [ managerKlass, manager ] = GetManager();

    jmethodID jMID;
    if ((jMID = GetMethod(managerKlass, "unpair", "(Ljava/lang/String;)V", Manager_Unpair)) == NULL)
        return;

    jstring jaddress = Core::Env->NewStringUTF(address);

    Core::Env->CallVoidMethod(manager, jMID, jaddress);

    Core::Env->DeleteLocalRef(jaddress);
}

void bHapticsPlayer::BhapticsManager::UnpairAll()
{
    auto [ managerKlass, manager ] = GetManager();

    jmethodID jMID;
    if ((jMID = GetMethod(managerKlass, "unpairAll", "()V", Manager_UnpairAll)) == NULL)
        return;

    Core::Env->CallVoidMethod(manager, jMID);
}

void bHapticsPlayer::BhapticsManager::ChangePosition(const char* address, const char* position)
{
    auto [ managerKlass, manager ] = GetManager();

    jmethodID jMID;
    if ((jMID = GetMethod(managerKlass, "changePosition", "(Ljava/lang/String;Lcom/bhaptics/commons/model/PositionType;)V", Manager_ChangePosition)) == NULL)
        return;

    jclass jPositionEnumKlass = GetKlass("com/bhaptics/commons/model/PositionType", CachedClassKeys::PositionType);
    
    jmethodID jPositionMID;
    if ((jPositionMID = GetMethod(jPositionEnumKlass, "valueOf", "(Ljava/lang/String;)Lcom/bhaptics/commons/model/PositionType;", PositionEnum_ValueOf)) == NULL)
        return;

    jstring jPostionStr = Core::Env->NewStringUTF(position);
    jobject jPosition = Core::Env->CallStaticObjectMethod(jPositionEnumKlass, jPositionMID, jPostionStr);

    jstring jaddress = Core::Env->NewStringUTF(address);
    Core::Env->CallVoidMethod(manager, jMID, jaddress, jPosition);

    Core::Env->DeleteLocalRef(jaddress);
    Core::Env->DeleteLocalRef(jPostionStr);
}

void bHapticsPlayer::BhapticsManager::TogglePosition(const char* address)
{
    auto [ managerKlass, manager ] = GetManager();

    jmethodID jMID;
    if ((jMID = GetMethod(managerKlass, "togglePosition", "(Ljava/lang/String;)V", Manager_TogglePosition)) == NULL)
        return;

    jstring jaddress = Core::Env->NewStringUTF(address);

    Core::Env->CallVoidMethod(manager, jMID, jaddress);

    Core::Env->DeleteLocalRef(jaddress);
}

void bHapticsPlayer::BhapticsManager::SetMotor(const char* address, char* bytes, size_t bytes_len)
{
    auto [ managerKlass, manager ] = GetManager();

    jmethodID jMID;
    if ((jMID = GetMethod(managerKlass, "setMotor", "(Ljava/lang/String;[B)V", Manager_SetMotor)) == NULL)
        return;

    jstring jaddress = Core::Env->NewStringUTF(address);
    jbyteArray jBytes = Core::Env->NewByteArray(bytes_len);

    Core::Env->SetByteArrayRegion(jBytes, 0, bytes_len, (jbyte*)bytes);

    Core::Env->CallVoidMethod(manager, jMID, jaddress, jBytes);

    Core::Env->DeleteLocalRef(jaddress);
    Core::Env->DeleteLocalRef(jBytes);
}

void bHapticsPlayer::BhapticsManager::Ping(const char* address)
{
    auto [ managerKlass, manager ] = GetManager();

    jmethodID jMID;
    if ((jMID = GetMethod(managerKlass, "ping", "(Ljava/lang/String;)V", Manager_Ping)) == NULL)
        return;

    jstring jaddress = Core::Env->NewStringUTF(address);

    Core::Env->CallVoidMethod(manager, jMID, jaddress);

    Core::Env->DeleteLocalRef(jaddress);
}

void bHapticsPlayer::BhapticsManager::PingAll()
{
    auto [ managerKlass, manager ] = GetManager();

    jmethodID jMID;
    if ((jMID = GetMethod(managerKlass, "pingAll", "()V", Manager_PingAll)) == NULL)
        return;

    Core::Env->CallVoidMethod(manager, jMID);
}

bool bHapticsPlayer::BhapticsManager::IsDeviceConnected(const char* type)
{
    auto [ managerKlass, manager ] = GetManager();

    jmethodID jMID;
    if ((jMID = GetMethod(managerKlass, "isDeviceConnected", "(Lcom/bhaptics/bhapticsmanger/BhapticsManager$DeviceType;)Z", Manager_IsDeviceConnected)) == NULL)
        return;
    
    jclass jDeviceTypeEnumKlass = GetKlass("com/bhaptics/bhapticsmanger/BhapticsManager$DeviceType", CachedClassKeys::InlineDeviceType);
    
    jmethodID jDeviceMID;
    if ((jDeviceMID = GetMethod(jDeviceTypeEnumKlass, "valueOf", "(Ljava/lang/String;)Lcom/bhaptics/bhapticsmanger/BhapticsManager$DeviceType;", InlineDeviceTypeEnum_ValueOf)) == NULL)
        return;

    jstring jDevTypeStr = ::Core::Env->NewStringUTF(type);
    jobject jDevType = Core::Env->CallStaticObjectMethod(jDeviceTypeEnumKlass, jDeviceMID, jDevTypeStr);
    
    jboolean jIsConnected = Core::Env->CallBooleanMethod(manager, jMID, jDevType);

    Core::Env->DeleteLocalRef(jDevTypeStr);

    return jIsConnected;
}

std::vector<jobject> bHapticsPlayer::BhapticsManager::GetDeviceList()
{
    auto [ managerKlass, manager ] = GetManager();

    jmethodID jMID;
    if ((jMID = GetMethod(managerKlass, "getDeviceList", "()Ljava/util/List;", Manager_GetDeviceList)) == NULL)
        return std::vector<jobject>();

    jobject jDeviceListObj = Core::Env->CallObjectMethod(manager, jMID);

    jclass listKlass = GetKlass("java/util/List", JavaList);
    jmethodID jListToArrayMID;
    if ((jListToArrayMID = GetMethod(listKlass, "toArray", "()[Ljava/lang/Object;", List_ToArray)) == NULL)
        return std::vector<jobject>();
    jobjectArray jDeviceList = (jobjectArray)Core::Env->CallObjectMethod(jDeviceListObj, jListToArrayMID);
    
    size_t jDeviceListLen = Core::Env->GetArrayLength(jDeviceList);
    std::vector<jobject> deviceList(jDeviceListLen);

    for (size_t i = 0; i < jDeviceListLen; i++)
        deviceList[i] = Core::Env->GetObjectArrayElement(jDeviceList, i);

    return deviceList;
}

std::tuple<jclass, jobject> bHapticsPlayer::BhapticsManager::GetManager()
{
    if (ManagerClass != NULL)
        return { ManagerClass, ManagerClassInstance };
    
    if ((ManagerClass = GetKlass("com/melonloader/bhaptics/DeviceManager", CachedClassKeys::DeviceManager)) == NULL)
        return { NULL, NULL };

    jfieldID fieldId = Core::Env->GetStaticFieldID(ManagerClass, "manager", "Lcom/bhaptics/bhapticsmanger/BhapticsManager");
    if (fieldId == NULL) {
        return { NULL, NULL };
    }

    ManagerClassInstance = Core::Env->GetStaticObjectField(ManagerClass, fieldId);
    
    return { ManagerClass, ManagerClassInstance };
}

bool bHapticsPlayer::BhapticsDevice::IsPing(jobject jDevice)
{
    jclass deviceKlass = GetKlass("com/bhaptics/commons/model/BhapticsDevice", CachedClassKeys::BhapticsDevice);

    jmethodID jMID;
    if ((jMID = GetMethod(deviceKlass, "isPing", "()Z", Device_IsPing)) == NULL)
        return false;

    return Core::Env->CallBooleanMethod(jDevice, jMID);
}

bool bHapticsPlayer::BhapticsDevice::IsPaired(jobject jDevice)
{
    jclass deviceKlass = GetKlass("com/bhaptics/commons/model/BhapticsDevice", CachedClassKeys::BhapticsDevice);

    jmethodID jMID;
    if ((jMID = GetMethod(deviceKlass, "isPaired", "()Z", Device_IsPaired)) == NULL)
        return false;

    return Core::Env->CallBooleanMethod(jDevice, jMID);
}

int bHapticsPlayer::BhapticsDevice::GetConnectFailedCount(jobject jDevice)
{
    jclass deviceKlass = GetKlass("com/bhaptics/commons/model/BhapticsDevice", CachedClassKeys::BhapticsDevice);

    jmethodID jMID;
    if ((jMID = GetMethod(deviceKlass, "getConnectFailCount", "()I", Device_GetConnectFailCount)) == NULL)
        return 0;

    return Core::Env->CallIntMethod(jDevice, jMID);
}

int bHapticsPlayer::BhapticsDevice::GetRSSI(jobject jDevice)
{
    jclass deviceKlass = GetKlass("com/bhaptics/commons/model/BhapticsDevice", CachedClassKeys::BhapticsDevice);

    jmethodID jMID;
    if ((jMID = GetMethod(deviceKlass, "getRssi", "()I", Device_GetRSSI)) == NULL)
        return 0;

    return Core::Env->CallIntMethod(jDevice, jMID);
}

const char* bHapticsPlayer::BhapticsDevice::GetConnectionStatus(jobject jDevice)
{
    jclass deviceKlass = GetKlass("com/bhaptics/commons/model/BhapticsDevice", CachedClassKeys::BhapticsDevice);

    jmethodID jMID;
    if ((jMID = GetMethod(deviceKlass, "getConnectionStatus", "()Lcom/bhaptics/commons/model/ConnectionStatus;", Device_GetConnectionStatus)) == NULL)
        return new char[] { '\0' };

    jobject jEnum = Core::Env->CallObjectMethod(jDevice, jMID);
    
    jclass jEnumKlass = GetKlass("com/bhaptics/commons/model/ConnectionStatus", ConnectionStatus);
    jmethodID jEnumNameMID;
    if ((jEnumNameMID = GetMethod(jEnumKlass, "name", "()Ljava/lang/String;", ConnectionStatusEnum_Name)) == NULL)
        return new char[] { '\0' };

    jstring jName = (jstring)Core::Env->CallObjectMethod(jEnum, jEnumNameMID);
    
    return Core::Env->GetStringUTFChars(jName, NULL);
}

const char* bHapticsPlayer::BhapticsDevice::GetPosition(jobject jDevice)
{
    jclass deviceKlass = GetKlass("com/bhaptics/commons/model/BhapticsDevice", CachedClassKeys::BhapticsDevice);

    jmethodID jMID;
    if ((jMID = GetMethod(deviceKlass, "getPosition", "()Lcom/bhaptics/commons/model/PositionType;", Device_GetPosition)) == NULL)
        return new char[] { '\0' };

    jobject jEnum = Core::Env->CallObjectMethod(jDevice, jMID);
    
    jclass jEnumKlass = GetKlass("com/bhaptics/commons/model/PositionType", PositionType);
    jmethodID jEnumNameMID;
    if ((jEnumNameMID = GetMethod(jEnumKlass, "name", "()Ljava/lang/String;", PositionEnum_Name)) == NULL)
        return new char[] { '\0' };

    jstring jName = (jstring)Core::Env->CallObjectMethod(jEnum, jEnumNameMID);
    
    return Core::Env->GetStringUTFChars(jName, NULL);
}

const char* bHapticsPlayer::BhapticsDevice::GetAddress(jobject jDevice)
{
    jclass deviceKlass = GetKlass("com/bhaptics/commons/model/BhapticsDevice", CachedClassKeys::BhapticsDevice);

    jmethodID jMID;
    if ((jMID = GetMethod(deviceKlass, "getAddress", "()Ljava/lang/String;", Device_GetAddress)) == NULL)
        return new char[] { '\0' };

    jstring jAddress = (jstring)Core::Env->CallObjectMethod(jDevice, jMID);
    return Core::Env->GetStringUTFChars(jAddress, NULL);
}

const char* bHapticsPlayer::BhapticsDevice::GetDeviceName(jobject jDevice)
{
    jclass deviceKlass = GetKlass("com/bhaptics/commons/model/BhapticsDevice", CachedClassKeys::BhapticsDevice);

    jmethodID jMID;
    if ((jMID = GetMethod(deviceKlass, "getDeviceName", "()Ljava/lang/String;", Device_GetDeviceName)) == NULL)
        return new char[] { '\0' };

    jstring jAddress = (jstring)Core::Env->CallObjectMethod(jDevice, jMID);
    return Core::Env->GetStringUTFChars(jAddress, NULL);
}

int bHapticsPlayer::BhapticsDevice::GetBattery(jobject jDevice)
{
    jclass deviceKlass = GetKlass("com/bhaptics/commons/model/BhapticsDevice", CachedClassKeys::BhapticsDevice);

    jmethodID jMID;
    if ((jMID = GetMethod(deviceKlass, "getBattery", "()I", Device_GetBattery)) == NULL)
        return 0;

    return Core::Env->CallIntMethod(jDevice, jMID);
}

const char* bHapticsPlayer::BhapticsDevice::GetType(jobject jDevice)
{
    jclass deviceKlass = GetKlass("com/bhaptics/commons/model/BhapticsDevice", CachedClassKeys::BhapticsDevice);

    jmethodID jMID;
    if ((jMID = GetMethod(deviceKlass, "getPosition", "()Lcom/bhaptics/commons/model/BhapticsDeviceType;", Device_GetType)) == NULL)
        return new char[] { '\0' };

    jobject jEnum = Core::Env->CallObjectMethod(jDevice, jMID);
    
    jclass jEnumKlass = GetKlass("com/bhaptics/commons/model/BhapticsDeviceType", DeviceType);
    jmethodID jEnumNameMID;
    if ((jEnumNameMID = GetMethod(jEnumKlass, "name", "()Ljava/lang/String;", DeviceTypeEnum_Name)) == NULL)
        return new char[] { '\0' };

    jstring jName = (jstring)Core::Env->CallObjectMethod(jEnum, jEnumNameMID);
    
    return Core::Env->GetStringUTFChars(jName, NULL);
}

std::vector<char> bHapticsPlayer::BhapticsDevice::GetLastBytes(jobject jDevice)
{
    jclass deviceKlass = GetKlass("com/bhaptics/commons/model/BhapticsDevice", CachedClassKeys::BhapticsDevice);

    jmethodID jMID;
    if ((jMID = GetMethod(deviceKlass, "getLastBytes", "()[B", Device_GetLastBytes)) == NULL)
        return std::vector<char>();
    
    jbyteArray jBytes = (jbyteArray)Core::Env->CallObjectMethod(jDevice, jMID);

    size_t jBytesSize = Core::Env->GetArrayLength(jBytes);
    std::vector<char> bytes(jBytesSize);
    jbyte* dataBytes = reinterpret_cast<jbyte*>(bytes.data());

    Core::Env->GetByteArrayRegion(jBytes, 0, jBytesSize, dataBytes);

    return bytes;
}

uint64_t bHapticsPlayer::BhapticsDevice::GetLastScannedTime(jobject jDevice)
{
    jclass deviceKlass = GetKlass("com/bhaptics/commons/model/BhapticsDevice", CachedClassKeys::BhapticsDevice);

    jmethodID jMID;
    if ((jMID = GetMethod(deviceKlass, "getLastScannedTime", "()J", Device_GetLastScannedTime)) == NULL)
        return 0;

    return Core::Env->CallLongMethod(jDevice, jMID);
}

const char* bHapticsPlayer::BhapticsDevice::ToString(jobject jDevice)
{
    jclass deviceKlass = GetKlass("com/bhaptics/commons/model/BhapticsDevice", CachedClassKeys::BhapticsDevice);

    jmethodID jMID;
    if ((jMID = GetMethod(deviceKlass, "toString", "()Ljava/lang/String;", Device_ToString)) == NULL)
        return new char[] { '\0' };

    jstring jName = (jstring)Core::Env->CallObjectMethod(jDevice, jMID);
    
    return Core::Env->GetStringUTFChars(jName, NULL);
}

jobject bHapticsPlayer::BhapticsDevice::GetDevice(const char* address)
{
    auto hash = HashAddress(address);

    if (DeviceMap.find(hash) == DeviceMap.end())
        return NULL;

    return DeviceMap[hash];
}

void bHapticsPlayer::BhapticsDevice::OnDeviceUpdate(jobject jDeviceListObj)
{
    jclass listKlass = GetKlass("java/util/List", JavaList);
    jmethodID jListToArrayMID;
    if ((jListToArrayMID = GetMethod(listKlass, "toArray", "()[Ljava/lang/Object;", List_ToArray)) == NULL)
        return;
    jobjectArray jDeviceList = (jobjectArray)Core::Env->CallObjectMethod(jDeviceListObj, jListToArrayMID);
    
    size_t jDeviceListLen = Core::Env->GetArrayLength(jDeviceList);

    DeviceMap.clear();
    
    for (size_t i = 0; i < jDeviceListLen; i++)
    {
        jobject jDevice = Core::Env->GetObjectArrayElement(jDeviceList, i);
        const char* address = GetAddress(jDevice);
        auto deviceHash = HashAddress(address);

        DeviceMap[deviceHash] = jDevice;
        
        delete address;
    }
}

void bHapticsPlayer::Callbacks::OnDeviceUpdate(jobject jDeviceList)
{
    BhapticsDevice::OnDeviceUpdate(jDeviceList);
}

void bHapticsPlayer::Callbacks::OnScanStatusChange(jboolean scanning)
{
    // Implement me
}

void bHapticsPlayer::Callbacks::OnChangeResponse()
{
    // Implement me
}

void bHapticsPlayer::Callbacks::OnConnect(jstring address)
{
    // Implement me
}

void bHapticsPlayer::Callbacks::OnDisconnect(jstring address)
{
    // Implement me
}

void bHapticsPlayer::Callbacks::OnChange()
{
    // Implement me
}

std::hash<std::string> bHapticsPlayer::BhapticsDevice::HashAddress(const char* address)
{
    return std::hash(std::string(address));
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

jclass bHapticsPlayer::GetKlass(const char* name, CachedClassKeys key)
{
    if (CachedClasses.find(key) != CachedClasses.end())
        return CachedClasses[key];

    jclass jKlass = Core::Env->FindClass(name);
    if (jKlass == NULL)
    {
        Debug::Msgf("Failed to get %s", name);
    }

    CachedClasses[key] = jKlass;
    return jKlass;
}
