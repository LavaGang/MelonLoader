package com.melonloader;

import android.Manifest;
import android.annotation.SuppressLint;
import android.app.Activity;
import android.content.Context;
import android.content.pm.PackageManager;
import android.os.Build;
import android.os.Environment;
import android.util.Log;

import java.nio.file.Files;
import java.nio.file.Paths;
import java.util.ArrayList;
import java.util.List;

public class ApplicationState {
    public static String LogTag = "MelonLoader";

    public static boolean IsReady = false;
    public static boolean Debug = false;

    public static boolean ContextDefined = false;

    public static String BaseDirectory;

    // this is updated every time a new context is started
    @SuppressLint("StaticFieldLeak")
    public static android.content.Context Context;

    // this is updated every time the activity updates
    @SuppressLint("StaticFieldLeak")
    public static Activity Activity;

    public static void UpdateActivity(Activity activity) {
        if (Activity == activity)
            return;

        if (BaseDirectory == null) {
            BaseDirectory = Paths.get(activity.getExternalFilesDir(null).toString()).toString();
        }

        LogBridge.msg("Activity Updated");
        Activity = activity;
        Context = activity.getApplicationContext();
    }

    private static List<String> GetMissingPermissions(Activity activity)
    {
        List<String> missingPerms = new ArrayList<String>();

        if (Build.VERSION.SDK_INT <= 18)
            if(activity.checkSelfPermission(Manifest.permission.WRITE_EXTERNAL_STORAGE) != PackageManager.PERMISSION_GRANTED)
                missingPerms.add(Manifest.permission.WRITE_EXTERNAL_STORAGE);

        if (activity.checkSelfPermission(Manifest.permission.READ_EXTERNAL_STORAGE) != PackageManager.PERMISSION_GRANTED)
            missingPerms.add(Manifest.permission.READ_EXTERNAL_STORAGE);

        if (activity.checkSelfPermission(Manifest.permission.WRITE_EXTERNAL_STORAGE) != PackageManager.PERMISSION_GRANTED)
            missingPerms.add(Manifest.permission.WRITE_EXTERNAL_STORAGE);

        if (activity.checkSelfPermission(Manifest.permission.ACCESS_MEDIA_LOCATION) != PackageManager.PERMISSION_GRANTED)
            missingPerms.add(Manifest.permission.ACCESS_MEDIA_LOCATION);

        if (Build.VERSION.SDK_INT >= 30)
            if(activity.checkSelfPermission(Manifest.permission.MANAGE_EXTERNAL_STORAGE) != PackageManager.PERMISSION_GRANTED)
                missingPerms.add(Manifest.permission.WRITE_EXTERNAL_STORAGE);

        return missingPerms;
    }
}
