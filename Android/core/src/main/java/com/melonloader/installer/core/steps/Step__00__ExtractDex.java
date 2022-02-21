package com.melonloader.installer.core.steps;

import com.google.common.collect.Iterables;
import com.melonloader.installer.core.InstallerStep;
import com.melonloader.installer.core.PathDefinitions;
import com.melonloader.installer.core.Properties;
import com.melonloader.installer.core.ZipHelper;
import org.apache.commons.io.FileUtils;

import java.io.*;
import java.nio.file.*;
import java.util.Arrays;
import java.util.Enumeration;
import java.util.Iterator;
import java.util.List;
import java.util.regex.Pattern;
import java.util.zip.ZipEntry;
import java.util.zip.ZipFile;
import java.util.zip.ZipInputStream;

public class Step__00__ExtractDex extends InstallerStep {
    public boolean Run() throws IOException {
        properties.logger.Log("Extracting dex files.");

        Pattern pattern = Pattern.compile("^classes\\d*\\.dex$", Pattern.MULTILINE);

        ZipHelper zipHelper = new ZipHelper(paths.outputAPK.toString());

        List<String> files = zipHelper.GetFiles();
        for (String fileName : files) {
            if (pattern.matcher(fileName).matches()) {
                Path destination = Paths.get(paths.dexOriginal.toString(), ZipHelper.GetBaseName(fileName));
                zipHelper.QueueExtract(fileName, destination.toString());
            }
        }

        zipHelper.Extract();

        return true;
    }
}
