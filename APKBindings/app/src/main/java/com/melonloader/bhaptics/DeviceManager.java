package com.melonloader.bhaptics;

import android.Manifest;
import android.app.Activity;
import android.content.Context;
import android.content.pm.PackageManager;
import android.util.Log;

import androidx.core.app.ActivityCompat;

import com.bhaptics.bhapticsmanger.BhapticsManager;
import com.bhaptics.bhapticsmanger.BhapticsManagerCallback;
import com.bhaptics.bhapticsmanger.BhapticsModule;
import com.bhaptics.bhapticsmanger.HapticPlayer;
import com.bhaptics.commons.model.BhapticsDevice;
import com.melonloader.ApplicationState;
import com.melonloader.LogBridge;

import java.util.List;

public class DeviceManager {
    public static boolean isStarted = false;
    public static BhapticsManager manager;
    public static HapticPlayer player;

    public static void start() throws Exception {
        if (!ApplicationState.IsReady)
            throw new Exception("Cannot start device manager! MelonLoader is not initialized.");

        if (!ApplicationState.ContextDefined)
            throw new Exception("Cannot start device manager! Missing Context.");

        if (isStarted) {
            LogBridge.warning("DeviceManager.start() was blocked because already running.");
            return;
        }

        BhapticsModule.initialize(ApplicationState.Context);

        manager = BhapticsModule.getBhapticsManager();
        player = BhapticsModule.getHapticPlayer();

        applyCallbacks();

        isStarted = true;
    }

    private static void applyCallbacks() {
        manager.addBhapticsManageCallback(new BhapticsManagerCallback() {
            @Override
            public void onDeviceUpdate(List<BhapticsDevice> list) {
                onDeviceUpdate_native(list.toArray());
                PairDevices(list);
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

    public static boolean hasPermissions(Context context, String... permissions) {
        if (context != null && permissions != null) {
            for (String permission : permissions) {
                if (ActivityCompat.checkSelfPermission(context, permission) != PackageManager.PERMISSION_GRANTED) {
                    return false;
                }
            }
        }
        return true;
    }

    public static void onResume() {
        if (ApplicationState.Context.checkSelfPermission(Manifest.permission.ACCESS_FINE_LOCATION) != PackageManager.PERMISSION_GRANTED) {
            // Permission is not granted
            LogBridge.error("onResume: permission ACCESS_FINE_LOCATION");
            ActivityCompat.requestPermissions(ApplicationState.Activity,
                    new String[]{Manifest.permission.ACCESS_FINE_LOCATION},
                    1);
        } else {
            manager.scan();
        }

//        if (!hasPermissions(ApplicationState.Activity.getApplicationContext(),
//                Manifest.permission.READ_EXTERNAL_STORAGE,
//                Manifest.permission.WRITE_EXTERNAL_STORAGE)) {
//            ActivityCompat.requestPermissions(ApplicationState.Activity,
//                    new String[]{Manifest.permission.READ_EXTERNAL_STORAGE, Manifest.permission.WRITE_EXTERNAL_STORAGE,},
//                    1);
//        }
//
//        LogBridge.msg("Complete");
    }

    public static void PairDevices(List<BhapticsDevice> devices)
    {
        for (BhapticsDevice device: devices) {
            if (device.isPaired())
                continue;

            LogBridge.msg("Pairing " + device.getAddress());
            manager.pair(device.getAddress());
        }
    }

    public static void onDestroy() {
        BhapticsModule.destroy();
    }

    public static native void onDeviceUpdate_native(Object[] devices);

    public static native void onScanStatusChange_native(boolean b);

    public static native void onChangeResponse_native();

    public static native void onConnect_native(String s);

    public static native void onDisconnect_native(String s);
}
