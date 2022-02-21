package com.melonloader.installer.core.steps;

import com.melonloader.installer.core.InstallerStep;
import com.melonloader.installer.core.ZipHelper;

import java.nio.file.Paths;
import java.util.List;
import java.util.regex.Pattern;

public class Step__40__RemoveStaleFiles extends InstallerStep {
    @Override
    public boolean Run() throws Exception {
        properties.logger.Log("Removing old files");

        Pattern[] patterns = new Pattern[] {
            Pattern.compile("^classes\\d*\\.dex$", Pattern.MULTILINE),
            Pattern.compile("^META-INF\\/.*", Pattern.MULTILINE),
            Pattern.compile("^lib\\/.*\\/libunity\\.so", Pattern.MULTILINE),
        };

        ZipHelper zipHelper = new ZipHelper(paths.outputAPK.toString());
        List<String> files = zipHelper.GetFiles();

        for (String fileName : files) {
            boolean matches = false;
            for (Pattern pattern : patterns) {
                if (pattern.matcher(fileName).matches()) {
                    matches = true;
                    break;
                }
            }

            if (!matches)
                continue;

            zipHelper.QueueDelete(fileName);
        }

        zipHelper.Delete();

        return true;
    }
}
