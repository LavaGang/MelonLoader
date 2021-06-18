package com.melonloader.helpers;

import android.app.Application;
import android.content.BroadcastReceiver;
import android.content.Context;
import android.content.IntentFilter;
import android.content.pm.ApplicationInfo;
import android.os.Build;
import android.util.Log;

import java.lang.reflect.*;

import com.melonloader.ApplicationState;
import com.melonloader.Core;
import com.melonloader.LogBridge;

import java.util.Locale;

public class MonoDroidHelper {
    static final Object lock = new Object();

    static boolean initialized = false;

    public static void LoadApplication() {
        synchronized (MonoDroidHelper.class) {
            LogBridge.msg("Starting Mono");
            if (!ApplicationState.ContextDefined)
                LogBridge.error("Cannot find context");

            if (initialized)
            {
                LogBridge.error("Already init");
                return;
            }

            Context paramContext = ApplicationState.Context;
            ApplicationInfo paramApplicationInfo = ApplicationState.Context.getApplicationInfo();

            Locale locale = Locale.getDefault();

            String lang = locale.getLanguage() + "-" + locale.getCountry();
            ClassLoader classLoader = paramContext.getClassLoader();
            String runtimeNativeLibDir = getNativeLibraryPath(paramApplicationInfo);

            String[] appDirs = {
                    "/storage/emulated/0/Android/data/com.SirCoolness.PlaygroundQuestGame/files/melonloader/etc",
                    "/storage/emulated/0/Android/data/com.SirCoolness.PlaygroundQuestGame/files/melonloader/etc/tmp",
                    getNativeLibraryPath(paramContext)
            };

            try {
                if (ApplicationState.Debug) {
                    System.loadLibrary("xamarin-debug-app-helper");
                    DebugInit(new String[0], runtimeNativeLibDir, appDirs);
                } else {
                    System.loadLibrary("monosgen-2.0");
                }

                System.loadLibrary("xamarin-app");
                System.loadLibrary("mono-native");
                System.loadLibrary("monodroid");
            } catch (Exception e) {
                LogBridge.error(e.getMessage());
                java.lang.Runtime.getRuntime().exit(-1);
                Core.KillCurrentProcess(); // death
                return;
            }

            mono.android.Runtime.initInternal(
                    lang,
                    new String[0],
                    runtimeNativeLibDir,
                    appDirs,
                    classLoader,
                    new String[0],
                    Build.VERSION.SDK_INT,
                    isEmulator());
            initialized = true;
        }
    }

    private static void DebugInit(String[] paramArrayOfString1, String paramString, String[] paramArrayOfString2)
    {

    }

    static boolean isEmulator() {
        String str = Build.HARDWARE;
        return str.contains("ranchu") || str.contains("goldfish");
    }

    static String getNativeLibraryPath(Context paramContext) {
        return getNativeLibraryPath(paramContext.getApplicationInfo());
    }

    static String getNativeLibraryPath(ApplicationInfo paramApplicationInfo) {
        if (Build.VERSION.SDK_INT >= 9)
            return paramApplicationInfo.nativeLibraryDir;
        return paramApplicationInfo.dataDir + "/lib";
    }
}
