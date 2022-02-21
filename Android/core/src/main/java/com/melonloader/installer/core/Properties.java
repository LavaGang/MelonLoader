package com.melonloader.installer.core;

import java.nio.file.Path;

/**
 * keystore generation example
 * keytool -genkey -keystore melonloader.keystore -alias melonloader -storepass 123456 -keypass 123456 -keyalg RSA -keysize 2048 -validity 10000 -dname "cn=, ou=, c=, c="
 */
public class Properties {
    public String targetApk;
    public String tempDir;
    public String dependencies;
    public String unityArchive;

    public Path unityNativeBase;
    public Path unityManagedBase;

    public String keystore;
    public String keystorePass;

    public String zipAlign;

    public ILogger logger;
}
