package com.melonloader.helpers;

import android.content.Context;

import com.melonloader.ApplicationState;
import com.melonloader.Bootstrap;
import com.melonloader.LogBridge;

public class InjectionHelper {
    public static void InjectBootstrap()
    {
        LogBridge.msg("Bootstrapping...");

        try {
            System.loadLibrary("Bootstrap");
        } catch (UnsatisfiedLinkError e) {
            LogBridge.error("Failed to load \"libBootstrap.so\" - Perhaps its not in lib?");
            throw e;
        }

        ApplicationState.IsReady = true;

        LogBridge.msg("libBootstrap.so successfully loaded");
    }

    public static void Initialize(Context context)
    {
        ContextHelper.DefineContext(context);
        Bootstrap.Initialize();
    }
}
