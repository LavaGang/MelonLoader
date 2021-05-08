package com.melonloader;

import android.annotation.SuppressLint;
import android.app.Activity;
import android.content.Context;

public class ApplicationState {
    public static String LogTag = "MelonLoader";

    public static boolean IsReady = false;
    public static boolean Debug = true;

    public static boolean ContextDefined = false;

    // this is updated every time a new context is started
    @SuppressLint("StaticFieldLeak")
    public static android.content.Context Context;

    // this is updated every time the activity updates
    @SuppressLint("StaticFieldLeak")
    public static Activity Activity;

    public static void UpdateActivity(Activity activity)
    {
        if (Activity == activity)
            return;

        LogBridge.msg("Activity Updated");
        Activity = activity;
        Context = activity.getApplicationContext();
    }
}
