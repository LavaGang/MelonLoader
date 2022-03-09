#include <jni.h>
#include "./Logger.h"

extern "C" {
	void Java_com_melonloader_LogBridge_error_1internal__Ljava_lang_String_2(JNIEnv* env, jclass type, jstring msg)
	{
		const char* cMsg = env->GetStringUTFChars(msg, nullptr);

		Logger::Error(cMsg);

		env->ReleaseStringUTFChars(msg, cMsg);
	}

	void Java_com_melonloader_LogBridge_warning_1internal__Ljava_lang_String_2(JNIEnv* env, jclass type, jstring msg)
	{
		const char* cMsg = env->GetStringUTFChars(msg, nullptr);

		Logger::Warning(cMsg);

		env->ReleaseStringUTFChars(msg, cMsg);
	}

	void Java_com_melonloader_LogBridge_msg_1internal__Ljava_lang_String_2(JNIEnv* env, jclass type, jstring msg)
	{
		const char* cMsg = env->GetStringUTFChars(msg, nullptr);

		Logger::Msg(Console::Reset, cMsg);

		env->ReleaseStringUTFChars(msg, cMsg);
	}
}