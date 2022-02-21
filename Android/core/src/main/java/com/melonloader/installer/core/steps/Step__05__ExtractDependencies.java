package com.melonloader.installer.core.steps;

import com.melonloader.installer.core.InstallerStep;
import com.melonloader.installer.core.ZipHelper;

import java.nio.file.Paths;
import java.util.List;

public class Step__05__ExtractDependencies extends InstallerStep {
    @Override
    public boolean Run() throws Exception {
        properties.logger.Log("Extracting Dependencies");

        ZipHelper zipHelper = new ZipHelper(properties.dependencies);
        List<String> files = zipHelper.GetFiles();

        for (String file : files) {
            zipHelper.QueueExtract(file, Paths.get(paths.dependenciesDir.toString(), file).toString());
        }

        zipHelper.Extract(true);

        paths.dexPatch = Paths.get(paths.dependenciesDir.toString(), "dex");

        return true;
    }
}
