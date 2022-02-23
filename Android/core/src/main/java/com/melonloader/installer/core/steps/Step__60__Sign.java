package com.melonloader.installer.core.steps;

import com.android.apksigner.ApkSignerTool;
import com.melonloader.installer.core.InstallerStep;
import com.melonloader.installer.core.LogOutputStream;

import java.io.*;
import java.nio.file.Files;
import java.nio.file.Paths;
import java.security.KeyStore;
import java.util.logging.SimpleFormatter;
import java.util.logging.StreamHandler;

public class Step__60__Sign extends InstallerStep {
    @Override
    public boolean Run() throws Exception {
        return Align() && Sign();
    }

    private boolean Sign() throws Exception {
        properties.logger.Log("Signing [" + paths.outputAPK + "]");

        PrintStream oldStream = System.out;
        PrintStream oldStreamErr = System.err;

        if (isAndroid()) {
            OutputStream stream = new LogOutputStream(this.properties.logger);
            System.setOut(new PrintStream(stream));
            System.setErr(new PrintStream(stream));
        }

        ApkSignerTool.main(new String[] {
            "sign",
            "--ks",
            paths.keystore.toString(),
            "--ks-key-alias",
            "cert",
            "--ks-pass",
            "pass:" + properties.keystorePass,
            paths.outputAPK.toString()
        });

        if (isAndroid()) {
            System.out.flush();
            System.setOut(oldStream);
            System.setErr(oldStreamErr);
        }

        return true;
    }

    boolean isAndroid() {
        try {
            Class.forName("the class name");
            return true;
        } catch(ClassNotFoundException e) {
            return false;
        }
    }

    private boolean Align() throws IOException, InterruptedException {
        properties.logger.Log("Aligning [" + paths.outputAPK + "]");

        String alignedFilename = paths.outputAPK.toString() + "-aligned";

        Process process = null;
        process = Runtime.getRuntime().exec(new String[] {
                properties.zipAlign,
                "-v",
                "-f",
                "4",
                paths.outputAPK.toString(),
                alignedFilename
        });

        BufferedReader stdInput = new BufferedReader(new
                InputStreamReader(process.getInputStream()));

        BufferedReader stdError = new BufferedReader(new
                InputStreamReader(process.getErrorStream()));

        String s = null;
        while ((s = stdInput.readLine()) != null) {
            properties.logger.Log(s);
        }

        stdInput.close();
        stdError.close();

        process.waitFor();

        Files.delete(paths.outputAPK);
        Files.move(Paths.get(alignedFilename), paths.outputAPK);

        Files.deleteIfExists(Paths.get(alignedFilename));

        return true;
    }
}
