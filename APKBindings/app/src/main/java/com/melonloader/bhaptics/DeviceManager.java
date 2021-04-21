package com.melonloader.bhaptics;

import com.bhaptics.bhapticsmanger.BhapticsManager;
import com.bhaptics.bhapticsmanger.BhapticsManagerCallback;
import com.bhaptics.bhapticsmanger.BhapticsModule;
import com.bhaptics.bhapticsmanger.HapticPlayer;
import com.bhaptics.commons.model.BhapticsDevice;
import com.melonloader.ApplicationState;
import com.melonloader.LogBridge;

import java.util.List;

public class DeviceManager {
    private static boolean isStarted = false;
    public static BhapticsManager manager;
    public static HapticPlayer player;

    public static void start() throws Exception {
        if (!ApplicationState.IsReady)
            throw new Exception("Cannot start device manager! MelonLoader is not initialized.");

        if (!ApplicationState.ContextDefined)
            throw new Exception("Cannot start device manager! Missing Context.");

        if (isStarted)
        {
            LogBridge.warning("DeviceManager.start() was blocked because already running.");
            return;
        }

        BhapticsModule.initialize(ApplicationState.Context);

        manager = BhapticsModule.getBhapticsManager();
        player = BhapticsModule.getHapticPlayer();

        applyCallbacks();

        isStarted = true;
    }

    private static void applyCallbacks()
    {
        manager.addBhapticsManageCallback(new BhapticsManagerCallback() {
            @Override
            public void onDeviceUpdate(List<BhapticsDevice> list) {
                onDeviceUpdate_native((BhapticsDevice[]) list.toArray());
            }

            @Override
            public void onScanStatusChange(boolean b) {
                onScanStatusChange_native(b);
            }

            @Override
            public void onChangeResponse() {
                onChangeResponse_native();
            }

            @Override
            public void onConnect(String s) {
                onConnect_native(s);
            }

            @Override
            public void onDisconnect(String s) {
                onDisconnect_native(s);
            }
        });

        PlayerWrapper.AddPlayerCallback();
    }

    public static native void onDeviceUpdate_native(BhapticsDevice[] devices);

    public static native void onScanStatusChange_native(boolean b);

    public static native void onChangeResponse_native();

    public static native void onConnect_native(String s);

    public static native void onDisconnect_native(String s);
}
