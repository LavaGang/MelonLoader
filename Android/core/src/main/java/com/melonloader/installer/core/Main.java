package com.melonloader.installer.core;

import com.melonloader.installer.core.steps.*;

import java.io.IOException;
import java.net.URI;
import java.nio.file.*;
import java.util.zip.ZipEntry;
import java.util.zip.ZipFile;

public class Main {
    public static Properties _properties = new Properties() {{
        logger = new SimpleLogger();
    }};

    public static boolean Run(Properties properties) {
        Properties oldProperties = _properties;
        _properties = properties;
        boolean result = true;

        try {
            PathDefinitions paths = DependencyHelper.PrepareTempDir(properties);

            properties.logger.Log("Copying [" + properties.targetApk + "] to [" + paths.outputAPK + "]");
            Files.copy(Paths.get(properties.targetApk), paths.outputAPK, StandardCopyOption.REPLACE_EXISTING);

            InstallerStep[] steps = new InstallerStep[] {
                new Step__00__DetectUnityVersion(),
                new Step__00__ExtractDex(),
                new Step__05__ExtractDependencies(),
                new Step__06__ExtractUnityLibs(),
                new Step__10__PatchDex(),
                new Step__40__RemoveStaleFiles(),
                new Step__50__RepackApk(),
                new Step__55__GeneratingSigner(),
                new Step__60__Sign(),
            };

            for (InstallerStep step : steps) {
                step.SetProps(properties, paths);
                boolean status = step.Run();
                if (!status) throw new Exception("Failed to complete patching.");
            }
        } catch (Exception e) {
            e.printStackTrace();
            properties.logger.Log(e.toString());
            result = false;
        }

        _properties = oldProperties;

        return result;
    }

    public static boolean IsPatched(String targetApk) {
        try {
            ZipFile zip = new ZipFile(targetApk);
            ZipEntry entry = zip.getEntry("assets/melonloader/etc/MelonLoader.dll");

            return entry != null;
        } catch (IOException e) {
            e.printStackTrace();
        }

        return false;
    }

    public static String DetectUnityVersion(String targetApk, String tempDir)
    {
        return (new UnityVersionDetector(targetApk, tempDir)).TryGetVersion();
    }

    public static Properties GetProperties()
    {
        return _properties;
    }
}
