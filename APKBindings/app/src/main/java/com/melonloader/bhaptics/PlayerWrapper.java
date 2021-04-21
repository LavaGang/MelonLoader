package com.melonloader.bhaptics;

public class PlayerWrapper {
    public static void AddPlayerCallback()
    {
        DeviceManager.player.setStatusChangeCallback(PlayerWrapper::onStatusChange_native);
    }

    private static native void onStatusChange_native();
}
