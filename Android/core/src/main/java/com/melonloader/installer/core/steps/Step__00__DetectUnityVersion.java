package com.melonloader.installer.core.steps;

import com.melonloader.installer.core.InstallerStep;
import com.melonloader.installer.core.Main;

public class Step__00__DetectUnityVersion extends InstallerStep {
    @Override
    public boolean Run() throws Exception {
        if (properties.unityVersion != null || (properties.unityNativeBase != null && properties.unityManagedBase != null))
            return true;

        properties.logger.Log("Getting unity version (can take up to 2 minutes)");

        properties.unityVersion = Main.DetectUnityVersion(properties.targetApk.toString(), properties.tempDir.toString());

        if (properties.unityVersion == null) {
            properties.logger.Log("Failed to detect version.");
            return false;
        }

        properties.logger.Log("Unity Version: " + properties.unityVersion);

        return true;
    }
}
