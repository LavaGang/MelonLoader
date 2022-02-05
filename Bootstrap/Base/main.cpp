#include "../Core.h"
#include <jni.h>

extern "C"
{
JNIEXPORT jint JNI_OnLoad(JavaVM* vm, void* reserved)
{
    Core::Bootstrap = vm;
    return (Core::Inject() ? JNI_VERSION_1_6 : 0x0);
}

jboolean Java_com_melonloader_Bootstrap_Initialize(JNIEnv* env)
{
    Core::Env = env;
    return (jboolean)Core::Initialize();
}
}
