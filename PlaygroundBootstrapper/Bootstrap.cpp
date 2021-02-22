#include "pch.h"
#include <cstdio>
#include <cstring>
#include <stdio.h>
#include <string>
#include <sys/types.h>
#include <unistd.h>

extern "C"
{
	JNIEXPORT void TestExternalCall()
	{
		__android_log_print(ANDROID_LOG_INFO, "MelonLoader", "CALL DID NOT GET REDIRECTED");
	}
	
	JNIEXPORT jint JNI_OnLoad(JavaVM* vm, void* reserved)
	{
		__android_log_print(ANDROID_LOG_INFO, "MelonLoader", "Test Result: %p", *TestExternalCall);

		__android_log_print(ANDROID_LOG_INFO, "MelonLoader", ((std::string)"PID: " + std::to_string(getpid())).c_str());


		int value;
		memcpy(&value, (const void*)*TestExternalCall, 2);
		__android_log_print(ANDROID_LOG_INFO, "MelonLoader", "%p", value);
		
		// int xvalue = 0x55;
		// memcpy((void*)*TestExternalCall, &xvalue, 2);
		//
		// memcpy(&value, (const void*)*TestExternalCall, 2);
		// __android_log_print(ANDROID_LOG_INFO, "MelonLoader", "%p", value);
		
		TestExternalCall();
		return JNI_VERSION_1_6;
	}
}