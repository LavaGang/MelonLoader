#pragma once
#include <android/asset_manager.h>
#include <android/asset_manager_jni.h>

class AssetManagerHelper
{
public:
    static AAssetManager* Instance;
    static bool Initialize(); 
};
