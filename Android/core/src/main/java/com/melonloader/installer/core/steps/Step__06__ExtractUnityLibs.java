package com.melonloader.installer.core.steps;

import com.melonloader.installer.core.InstallerStep;
import com.melonloader.installer.core.ZipHelper;

import java.io.IOException;
import java.nio.file.Paths;
import java.util.List;

public class Step__06__ExtractUnityLibs  extends InstallerStep {
    @Override
    public boolean Run() throws Exception {
        if (properties.unityNativeBase == null || properties.unityManagedBase == null)
            return RunZip();

        // assume unity is installed
        paths.unityNativeBase = properties.unityNativeBase;
        paths.unityManagedBase = properties.unityManagedBase;

        return true;
    }

    private boolean RunZip() throws IOException {
        properties.logger.Log("Extracting Unity Dependencies");

        ZipHelper zipHelper = new ZipHelper(paths.unityZip.toString());
        List<String> files = zipHelper.GetFiles();

        for (String file : files) {
            zipHelper.QueueExtract(file, Paths.get(paths.unityBase.toString(), file).toString());
        }

        zipHelper.Extract();

        return true;
    }
}
