#include "StaticSettings.h"

#include <cstring>
#include <bits/seek_constants.h>

#include "AssetManagerHelper.h"
#include "../Utils/Assertion.h"
#include "../Utils/Console/Debug.h"

const char* settings_path = "static_settings";

StaticSettings::Settings_t StaticSettings::Settings;

bool StaticSettings::Initialize()
{
    char* buffer;
    size_t len;
    ReadSettings(buffer, len);
    if (!Assertion::ShouldContinue)
        return false;

    memcpy(&Settings, buffer, len);

    Debug::Msgf("Safe Mode - %s", Settings.safeMode ? "on" : "off");
    
    return true;
}

void StaticSettings::ReadSettings(char* &buffer, size_t &len)
{
    AAsset* asset = AAssetManager_open(AssetManagerHelper::Instance, settings_path, AASSET_MODE_STREAMING);
    if (asset == nullptr)
    {
        Assertion::ThrowInternalFailuref("Failed to read settings file [%s]", settings_path);
        return;
    }

    if ((len = AAsset_getLength(asset)) != sizeof(StaticSettings::Settings_t))
    {
        Assertion::ThrowInternalFailure("Settings dont match expected size.");
        return;
    }

    if (AAsset_seek(asset, 0, SEEK_SET) == -1)
    {
        Assertion::ThrowInternalFailure("Failed to seek to start of settings file.");
        return;
    }

    int error_code;
    buffer = new char[len];
    if ((error_code = AAsset_read(asset, buffer, len)) != len)
    {
        Assertion::ThrowInternalFailuref("Failed read settings. Code %d", error_code);
        return;
    }

    AAsset_close(asset);
}


