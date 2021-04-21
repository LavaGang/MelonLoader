package com.melonloader;

import android.content.Context;

public class ContextHelper {
    public static void DefineContext(Context context)
    {
        ApplicationState.ContextDefined = true;
        ApplicationState.Context = context;

        LogBridge.msg("Application Context Defined");
    }
}
