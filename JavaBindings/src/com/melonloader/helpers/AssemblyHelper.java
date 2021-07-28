package com.melonloader.helpers;

import android.content.res.AssetManager;

import com.melonloader.ApplicationState;
import com.melonloader.LogBridge;

import java.io.File;
import java.io.FileOutputStream;
import java.io.IOException;
import java.io.InputStream;
import java.io.OutputStream;

public class AssemblyHelper {
    private static AssemblyHelper instance;

    AssemblyHelper()
    {
        instance = this;
    }

    public static boolean InstallAssemblies()
    {
        if (!ContextHelper.CheckContext("Cannot install assemblies"))
            return false;

        LogBridge.msg("Installing Assemblies");

        new AssemblyHelper();

        AssetManager am = ApplicationState.Context.getAssets();

        File path = ApplicationState.Context.getExternalFilesDir(null);
        return AssemblyHelper.instance.CopyAssetsTo(path);
    }

    private boolean CopyAssetsTo(File path)
    {
        AssetManager am = ApplicationState.Context.getAssets();

        String[] files;

        try {
            files = am.list("");
        } catch (IOException e) {
            LogBridge.error(e.getMessage());
            return false;
        }

        this.copyFileOrDir("melonloader", path);

        return true;
    }

    public void copyFileOrDir(String path, File base) {
        AssetManager assetManager = ApplicationState.Context.getAssets();
        String assets[] = null;
        try {
            assets = assetManager.list(path);
            if (assets.length == 0) {
                copyFile(path, base);
            } else {
                String fullPath = base + "/" + path;
                File dir = new File(fullPath);
                if (!dir.exists())
                    dir.mkdir();
                for (int i = 0; i < assets.length; ++i) {
                    copyFileOrDir(path + "/" + assets[i], base);
                }
            }
        } catch (IOException ex) {
            LogBridge.error(ex.getMessage());
        }
    }

    private void copyFile(String filename, File base) {
        AssetManager assetManager = ApplicationState.Context.getAssets();

        InputStream in = null;
        OutputStream out = null;
        try {
            in = assetManager.open(filename);
            String newFileName = base + "/" + filename;
            out = new FileOutputStream(newFileName);

            byte[] buffer = new byte[1024];
            int read;
            while ((read = in.read(buffer)) != -1) {
                out.write(buffer, 0, read);
            }
            in.close();
            in = null;
            out.flush();
            out.close();
            out = null;
        } catch (Exception e) {
            LogBridge.error(e.getMessage());
        }
    }
}
