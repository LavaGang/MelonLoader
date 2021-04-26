package com.melonloader.bhaptics;

import com.melonloader.LogBridge;

public class PlayerWrapper {
    public static void AddPlayerCallback()
    {
        DeviceManager.player.setStatusChangeCallback(PlayerWrapper::onStatusChange_native);
    }


    public static void submitDot(String key, String position, int[] indexes, int[] points, int duration)
    {
        LogBridge.msg("SubmitDot");

        if (!DeviceManager.isStarted)
            return;

        DeviceManager.player.submitDot(key, position, indexes, points, duration);
    }

    private static native void onStatusChange_native();
}
