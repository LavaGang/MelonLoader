#include "AndroidData.h"

#include <jni.h>
#include <unistd.h>
#include <android/log.h>
#include <stdio.h>
#include <string>
#include <sys/stat.h>
#include <fcntl.h>

#include "../Utils/Console/Debug.h"
#include "../Utils/Assertion.h"
#include "../Core.h"

char* AndroidData::BaseDataDir = "/storage/emulated/0/Android/data";
char* AndroidData::AppName = NULL;
char* AndroidData::DataDir = NULL;

bool AndroidData::Initialize()
{
    GetAppName();
    if (!Assertion::ShouldContinue) return Assertion::ShouldContinue;
    GetDataDir();
    return Assertion::ShouldContinue;
}

void AndroidData::GetAppName()
{
    char* buffer = (char*)malloc(sizeof(char) * 0x1000);

    int fd = open("/proc/self/cmdline", O_RDONLY);
    if (read(fd, buffer, 0x1000) == 0)
    {
        Assertion::ThrowInternalFailure("Cannot get App name");
        return;
    }

    int nbytesread = strlen(buffer);

    AppName = new char[nbytesread + 1];
    AppName = (char*)malloc(nbytesread + 1);
    memcpy(AppName, buffer, nbytesread);
    AppName[nbytesread] = '\0';

    free(buffer);
}

void AndroidData::GetDataDir()
{
    auto env = Core::GetEnv();

    auto klass = env->FindClass("com/melonloader/ApplicationState");
    if (klass == nullptr) {
        Assertion::ThrowInternalFailure("Failed to find com/melonloader/ApplicationState");
        return;
    }

    auto fieldId = env->GetStaticFieldID(klass, "BaseDirectory", "Ljava/lang/String;");
    if (fieldId == nullptr) {
        Assertion::ThrowInternalFailure("Failed to find com/melonloader/ApplicationState::BaseDirectory");
        return;
    }

    auto jStr = env->GetStaticObjectField(klass, fieldId);
    if (jStr == nullptr) {
        Assertion::ThrowInternalFailure("com.melonloader.ApplicationState::BaseDirectory is null");
        return;
    }

    const char* cStr = env->GetStringUTFChars((jstring)jStr, nullptr);
    const size_t cStrSize = strlen(cStr);
    DataDir = new char[cStrSize + 1];
    std::strcpy(DataDir, cStr);
    DataDir[cStrSize] = '\0';

    env->DeleteLocalRef(jStr);
}
