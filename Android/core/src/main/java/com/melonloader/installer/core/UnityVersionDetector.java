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

    public UnityVersionDetector(String apk, String _tempDir) {
        targetApk = apk;
        tempDir = _tempDir;

        try {
            ExtractAssets();
        } catch (Exception e) {
            e.printStackTrace();
            Main.GetProperties().logger.Log(e.toString());
        }
    }

    public String TryGetVersion()
    {
        try {
            String version;

            version = GameManagerMethod(false);
            if (version != null) return version;

            version = MainDataMethod();
            return version;
        } catch (Exception e) {
            e.printStackTrace();
            Main.GetProperties().logger.Log(e.toString());
            return null;
        }
    }

    protected void ExtractAssets() throws IOException {
        ZipHelper zipHelper = new ZipHelper(targetApk);

        zipHelper.QueueExtract("assets/bin/Data/globalgamemanagers", Paths.get(tempDir, "globalgamemanagers").toString());
        zipHelper.QueueExtract("assets/bin/Data/data.unity3d", Paths.get(tempDir, "data.unity3d").toString());

        zipHelper.Extract(true);
    }

    protected String GameManagerMethod(boolean f_mode) throws IOException {
        String path = Paths.get(tempDir, "globalgamemanagers").toString();
        BufferedInputStream buf;
        try {
            buf = new BufferedInputStream(new FileInputStream(path));
        } catch (FileNotFoundException e) {
            return null;
        }

        buf.skip(f_mode ? 0x14 : 0x30);

        StringBuilder version = new StringBuilder();

        boolean f_found = false;

        int next = 0;
        while ((next = buf.read()) != -1)
        {
            char bit = (char)next;
            if (bit == 'f')
                f_found = true;

            if (bit == '\0' || bit == 'f')
                break;
            version.append(bit);

            if (version.length() >= MaxName)
            {
                System.out.println("Version Exceeded Size Limit");
                return null;
            }
        }

        buf.close();

        if (!f_found) {
            if (f_mode) return null;

            return GameManagerMethod(true);
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
