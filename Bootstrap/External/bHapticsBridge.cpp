#include <jni.h>
#include "../Utils/Console/Debug.h"
#include "../Managers/bHapticsPlayer.h"

extern "C" {
	void Java_com_melonloader_bhaptics_DeviceManager_onDeviceUpdate_1native(JNIEnv* env, jclass type, jobject jDevices)
	{
		Debug::Msg("bHaptics: onDeviceUpdate");
		bHapticsPlayer::Callbacks::OnDeviceUpdate(jDevices);
	}

	void Java_com_melonloader_bhaptics_DeviceManager_onScanStatusChange_1native(JNIEnv* env, jclass type, jboolean b)
	{
		Debug::Msgf("bHaptics: scan status %s", b ? "true" : "false");
		bHapticsPlayer::Callbacks::OnScanStatusChange(b);
	}

	void Java_com_melonloader_bhaptics_DeviceManager_onChangeResponse_1native(JNIEnv* env, jclass type)
	{
		Debug::Msgf("bHaptics: onChangeResponse");
		bHapticsPlayer::Callbacks::OnChangeResponse();
	}
	
	void Java_com_melonloader_bhaptics_DeviceManager_onConnect_1native(JNIEnv* env, jclass type, jstring s)
	{
		const char* cMsg = env->GetStringUTFChars(s, nullptr);
		Debug::Msgf("bHaptics: onConnect %s", cMsg);
		env->ReleaseStringUTFChars(s, cMsg);

		bHapticsPlayer::Callbacks::OnConnect(s);
	}

	void Java_com_melonloader_bhaptics_DeviceManager_onDisconnect_1native(JNIEnv* env, jclass type, jstring s)
	{
		const char* cMsg = env->GetStringUTFChars(s, nullptr);
		Debug::Msgf("bHaptics: onDisconnect %s", cMsg);
		env->ReleaseStringUTFChars(s, cMsg);

		bHapticsPlayer::Callbacks::OnDisconnect(s);
	}

	void Java_com_melonloader_bhaptics_PlayerWrapper_onStatusChange_1native(JNIEnv* env, jclass type)
	{
		Debug::Msgf("bHaptics: onStatusChange");
		bHapticsPlayer::Callbacks::OnChange();
	}
}