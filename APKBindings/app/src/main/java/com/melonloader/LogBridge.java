package com.melonloader;

import android.util.Log;

public class LogBridge {
    public static void error(String msg) {
        if (ApplicationState.IsReady)
        {
            error_internal(msg);
            return;
        }

        Log.e(ApplicationState.LogTag, msg);
    }

    public static void warning(String msg) {
        if (ApplicationState.IsReady)
        {
            warning_internal(msg);
            return;
        }

        Log.w(ApplicationState.LogTag, msg);
    }

    public static void msg(String msg) {
        if (ApplicationState.IsReady)
        {
            msg_internal(msg);
            return;
        }

        Log.i(ApplicationState.LogTag, msg);
    }

    private static native void error_internal(String msg);
    private static native void warning_internal(String msg);
    private static native void msg_internal(String msg);
}
