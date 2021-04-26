package com.melonloader.helpers;

import android.app.Activity;
import android.content.Intent;
import android.content.res.Configuration;
import android.os.Bundle;

import com.melonloader.ApplicationState;
import com.melonloader.bhaptics.DeviceManager;

public class ActivityHelper {
    public static void onConfigurationChanged(Activity activity, Configuration configuration)
    {
        ApplicationState.UpdateActivity(activity);
    }

    public static void onCreate(Activity activity, Bundle bundle)
    {
        ApplicationState.UpdateActivity(activity);
    }

    public static void onDestroy(Activity activity)
    {
        ApplicationState.UpdateActivity(activity);
        DeviceManager.onDestroy();
    }

    public static void onNewIntent(Activity activity, Intent intent)
    {
        ApplicationState.UpdateActivity(activity);
    }

    public static void onPause(Activity activity)
    {
        ApplicationState.UpdateActivity(activity);
    }

    public static void onResume(Activity activity)
    {
        ApplicationState.UpdateActivity(activity);
        DeviceManager.onResume();
    }

    public static void onWindowFocusChanged(Activity activity, boolean hasFocus)
    {
        ApplicationState.UpdateActivity(activity);
    }
}
