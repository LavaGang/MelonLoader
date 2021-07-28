package com.melonloader;

import android.app.Activity;
import android.content.res.AssetManager;

import com.melonloader.helpers.ContextHelper;

public class Core {
    public static native void KillCurrentProcess();

    public static AssetManager GetAssetManager()
    {
        if (!ContextHelper.CheckContext("Cannot get Asset Manager"))
            return null;

        return ApplicationState.Context.getAssets();
    }
}
