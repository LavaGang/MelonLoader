package com.melonloader;

import android.annotation.SuppressLint;
import android.content.Context;
import android.util.Log;

import com.melonloader.bridge.Assertion;
import com.melonloader.exceptions.MissingContext;

public class ContextHelper {
    public static void DefineContext(Context context)
    {
        ApplicationState.ContextDefined = true;
        ApplicationState.Context = context;

        LogBridge.msg("Application Context Defined");

        AssemblyHelper.InstallAssemblies();
    }

    public static boolean CheckContext(String error) {
        if (!ApplicationState.ContextDefined) {
            new MissingContext(error).printStackTrace();
            return false;
        }

        return true;
    }
}

