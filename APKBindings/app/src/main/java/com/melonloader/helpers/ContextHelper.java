package com.melonloader.helpers;

import android.content.Context;

import com.melonloader.ApplicationState;
import com.melonloader.LogBridge;
import com.melonloader.exceptions.MissingContext;

public class ContextHelper {
    public static void DefineContext(Context context)
    {
        ApplicationState.ContextDefined = true;
        ApplicationState.Context = context;

        LogBridge.msg("Application Context Defined");
    }

    public static boolean CheckContext(String error) {
        if (!ApplicationState.ContextDefined) {
            new MissingContext(error).printStackTrace();
            return false;
        }

        return true;
    }
}

