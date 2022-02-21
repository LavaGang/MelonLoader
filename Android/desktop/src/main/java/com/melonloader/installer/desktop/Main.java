package com.melonloader.installer.desktop;

import com.melonloader.installer.core.Properties;
import com.melonloader.installer.core.SimpleLogger;

import java.nio.file.FileSystems;
import java.nio.file.Path;
import java.nio.file.Paths;

public class Main {
    public static void main(String[] args) throws Exception {
        Path unityBase = Paths.get("/media/akiva/544ACC324ACC12A2/Program Files/Unity/Hub/Editor/2020.1.8f1");

        Properties properties = new Properties() {{
            targetApk = "/home/akiva/CLionProjects/MelonLoader/Android/example/SampleGame.apk";
            tempDir = "/home/akiva/CLionProjects/MelonLoader/Android/example/temp";
            dependencies = "/home/akiva/CLionProjects/MelonLoader/Android/example/installer_deps.zip";
            unityNativeBase = Paths.get(unityBase.toString(), "Editor","Data","PlaybackEngines","AndroidPlayer","Variations","il2cpp","Development","Libs");
            unityManagedBase = Paths.get(unityBase.toString(), "Editor","Data","PlaybackEngines","AndroidPlayer","Variations","il2cpp","Managed");
            logger = new SimpleLogger();
        }};

        properties.logger.Log("I am the launcher!");
        properties.logger.Log(properties.unityNativeBase.toString());

        com.melonloader.installer.core.Main.Run(properties);
    }
}
