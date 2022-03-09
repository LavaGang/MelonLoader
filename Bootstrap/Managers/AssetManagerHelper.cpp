#include "AssetManagerHelper.h"
#include "../Utils/Assertion.h"
#include "../Core.h"

AAssetManager* AssetManagerHelper::Instance = nullptr;

bool AssetManagerHelper::Initialize()
{
    auto env = Core::GetEnv();

    jclass jCore = env->FindClass("com/melonloader/Core");
    if (jCore == NULL)
    {
        Assertion::ThrowInternalFailure("Failed to find class com.melonloader.Core");
        return false;
    }

    jmethodID mid = env->GetStaticMethodID(jCore, "GetAssetManager", "()Landroid/content/res/AssetManager;");
    if (mid == NULL)
    {
        Assertion::ThrowInternalFailure("Failed to find method com.melonloader.Core.GetAssetManager()");
        return false;
    }

    jobject jAM = env->CallStaticObjectMethod(jCore, mid);
    if (jAM == NULL)
    {
        Assertion::ThrowInternalFailure("Failed to invoke com.melonloader.Core.GetAssetManager()");
        return false;
    }
    
    Instance = AAssetManager_fromJava(env, jAM);
    if (Instance == NULL)
    {
        Assertion::ThrowInternalFailure("Failed to create AssetManager instance");
        return false;
    }

    return true;
}
