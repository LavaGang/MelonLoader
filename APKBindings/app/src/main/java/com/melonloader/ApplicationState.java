package com.melonloader;

import android.annotation.SuppressLint;
import android.content.Context;

public class ApplicationState {
    public static String LogTag = "MelonLoader";

    public static boolean IsReady = false;

    public static boolean ContextDefined = false;

    // this is updated every time a new context is started
    @SuppressLint("StaticFieldLeak")
    public static android.content.Context Context;
}
