#include "bHapticsBridge.h"
#include <jni.h>

extern "C" {
	void onDeviceUpdate_native(JNIEnv* env, jclass type, jobjectArray jDevices)
	{
		jsize deviceCount = env->GetArrayLength(jDevices);
	}
}