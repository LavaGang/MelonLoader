package com.melonloader.installer.core;

import java.io.*;
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

        if (apk == null)
            return;

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
        InputStream stream = getStream("globalgamemanagers");
        if (stream == null) return null;

        BufferedInputStream buf = new BufferedInputStream(stream);

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
                f_found = false;
                break;
            }
        }

        buf.close();
        stream.close();

        if (!f_found) {
            if (f_mode) return null;

            return GameManagerMethod(true);
        }


        return version.toString();
    }

    protected String MainDataMethod() throws IOException {
        InputStream stream = getStream("data.unity3d");
        if (stream == null) return null;

        BufferedInputStream buf = new BufferedInputStream(stream);

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
                buf.close();
                return null;
            }
        }

        buf.close();
        stream.close();

        return version.toString();
    }

    protected InputStream getStream(String local_path)
    {
        String path = Paths.get(tempDir, local_path).toString();

        try {
            return new FileInputStream(path);
        } catch (FileNotFoundException e) {
            return null;
        }
    }
}
