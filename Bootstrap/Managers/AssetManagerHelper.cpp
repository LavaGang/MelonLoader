#include "AssetManagerHelper.h"
#include "../Base/Core.h"
#include "../Utils/Assertion.h"

AAssetManager* AssetManagerHelper::Instance = nullptr;

bool AssetManagerHelper::Initialize()
{
    jclass jCore = Core::Env->FindClass("com/melonloader/Core");
    if (jCore == NULL)
    {
        Assertion::ThrowInternalFailure("Failed to find class com.melonloader.Core");
        return false;
    }

    jmethodID mid = Core::Env->GetStaticMethodID(jCore, "GetAssetManager", "()Landroid/content/res/AssetManager;");
    if (mid == NULL)
    {
        Assertion::ThrowInternalFailure("Failed to find method com.melonloader.Core.GetAssetManager()");
        return false;
    }

    jobject jAM = Core::Env->CallStaticObjectMethod(jCore, mid);
    if (jAM == NULL)
    {
        Assertion::ThrowInternalFailure("Failed to invoke com.melonloader.Core.GetAssetManager()");
        return false;
    }
    
    Instance = AAssetManager_fromJava(Core::Env, jAM);
    if (Instance == NULL)
    {
        Assertion::ThrowInternalFailure("Failed to create AssetManager instance");
        return false;
    }

    return true;
}
