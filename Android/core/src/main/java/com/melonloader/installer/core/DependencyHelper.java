package com.melonloader.installer.core;

import java.io.IOException;
import java.nio.file.Files;
import java.nio.file.Paths;

public class DependencyHelper {
    public static PathDefinitions PrepareTempDir(Properties properties) throws IOException {
        PathDefinitions paths = new PathDefinitions(properties);

        Files.createDirectories(paths.base);
        Files.createDirectories(paths.dexBase);
        Files.createDirectories(paths.dexOriginal);
//        Files.createDirectories(paths.dexPatch);
        Files.createDirectories(paths.dexOutput);
        Files.createDirectories(paths.dependenciesDir);

        return paths;
    }
}
