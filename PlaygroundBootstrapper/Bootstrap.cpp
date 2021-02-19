#include "pch.h"

extern "C"
{
	JNIEXPORT void JNICALL Java_com_unity3d_player_NativeLoader_initmods(JNIEnv* pEnv, jobject thiz)
	{
		__android_log_print(ANDROID_LOG_INFO, "MelonLoader", "Initializing Mods");
	}

	JNIEXPORT jint JNI_OnLoad(JavaVM* vm, void* reserved)
	{
		__android_log_print(ANDROID_LOG_INFO, "MelonLoader", "Loaded MelonLoader");
		return JNI_VERSION_1_6;
	}
}