#include "bHapticsBridge.h"
#include <jni.h>
#include "../Utils/Console/Debug.h"

extern "C" {
	void Java_com_melonloader_bhaptics_DeviceManager_onDeviceUpdate_1native(JNIEnv* env, jclass type, jobjectArray jDevices)
	{
		jsize deviceCount = env->GetArrayLength(jDevices);

		Debug::Msgf("bHaptics: device count %d", deviceCount);
	}

	void Java_com_melonloader_bhaptics_DeviceManager_onScanStatusChange_1native(JNIEnv* env, jclass type, jboolean b)
	{
		bool scanStatus = (bool)b;

		Debug::Msgf("bHaptics: scan status %s", b ? "true" : "false");
	}

	void Java_com_melonloader_bhaptics_DeviceManager_onChangeResponse_1native(JNIEnv* env, jclass type)
	{
		Debug::Msgf("bHaptics: onChangeResponse");
	}
	void Java_com_melonloader_bhaptics_DeviceManager_onConnect_1native(JNIEnv* env, jclass type, jstring s)
	{
		const char* cMsg = env->GetStringUTFChars(s, nullptr);

		Debug::Msgf("bHaptics: onConnect %s", cMsg);

		env->ReleaseStringUTFChars(s, cMsg);
	}

	void Java_com_melonloader_bhaptics_DeviceManager_onDisconnect_1native(JNIEnv* env, jclass type, jstring s)
	{
		const char* cMsg = env->GetStringUTFChars(s, nullptr);

		Debug::Msgf("bHaptics: onDisconnect %s", cMsg);

		env->ReleaseStringUTFChars(s, cMsg);
	}

	void Java_com_melonloader_bhaptics_PlayerWrapper_onStatusChange_1native(JNIEnv* env, jclass type)
	{
		Debug::Msgf("bHaptics: onStatusChange");
	}
}