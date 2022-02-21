package com.melonloader.installer.core;

import java.io.BufferedInputStream;
import java.io.FileInputStream;
import java.io.FileNotFoundException;
import java.io.IOException;
import java.nio.file.Path;
import java.nio.file.Paths;
import java.util.ArrayList;
import java.util.List;
import java.util.regex.Pattern;

public class UnityVersionDetector {
    String targetApk;
    String tempDir;
    public int MaxName = 256;

    public UnityVersionDetector(String apk, String tempDir) {
        targetApk = apk;
        this.tempDir = tempDir;

        try {
            ExtractAssets();
        } catch (Exception e) {
            e.printStackTrace();
        }
    }

    public String TryGetVersion()
    {
        try {
            String version;

            version = GameManagerMethod();
            if (version != null) return version;

            version = MainDataMethod();
            return version;
        } catch (Exception e) {
            e.printStackTrace();
            return null;
        }
    }

    protected void ExtractAssets() throws IOException {
        ZipHelper zipHelper = new ZipHelper(targetApk);

        List<String> files = zipHelper.GetFiles();
        for (String fileName : files) {
            if (fileName.equals("assets/bin/Data/globalgamemanagers"))
                zipHelper.QueueExtract(fileName, Paths.get(tempDir, "globalgamemanagers").toString());
            else if (fileName.equals("assets/bin/Data/data.unity3d"))
                zipHelper.QueueExtract(fileName, Paths.get(tempDir, "data.unity3d").toString());
        }

        zipHelper.Extract(true);
    }

    protected String GameManagerMethod() throws IOException {
        String path = Paths.get(tempDir, "globalgamemanagers").toString();
        BufferedInputStream buf;
        try {
            buf = new BufferedInputStream(new FileInputStream(path));
        } catch (FileNotFoundException e) {
            return null;
        }

        buf.skip(0x30);

        int next = 0;

        StringBuilder version = new StringBuilder();

        while ((next = buf.read()) != -1)
        {
            char bit = (char)next;
            if (bit == 0x00 || bit == 0x66)
                break;
            version.append(bit);

            if (version.length() >= MaxName)
            {
                System.out.println("Version Exceeded Size Limit");
                return null;
            }
        }


        return version.toString();
    }

    protected String MainDataMethod() throws IOException {
        String path = Paths.get(tempDir, "data.unity3d").toString();
        BufferedInputStream buf;
        try {
            buf = new BufferedInputStream(new FileInputStream(path));
        } catch (FileNotFoundException e) {
            return null;
        }

        buf.skip(0x12);

        int next = 0;

        StringBuilder version = new StringBuilder();

        while ((next = buf.read()) != -1)
        {
            char bit = (char)next;
            if (bit == 0x00 || bit == 0x66)
                break;
            version.append(bit);

            if (version.length() >= MaxName)
            {
                System.out.println("Version Exceeded Size Limit");
                return null;
            }
        }

        return version.toString();
    }
}
