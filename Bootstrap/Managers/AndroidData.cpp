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
    std::string DataDirStr = (std::string(BaseDataDir) + "/" + std::string(AppName) + "/files");
    DataDir = new char[DataDirStr.size() + 1];
    std::copy(DataDirStr.begin(), DataDirStr.end(), DataDir);
    DataDir[DataDirStr.size()] = '\0';
}
