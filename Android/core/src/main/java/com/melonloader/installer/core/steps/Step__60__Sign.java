package com.melonloader.installer.core.steps;

import com.android.apksigner.ApkSignerTool;
import com.melonloader.installer.core.InstallerStep;

import java.io.BufferedReader;
import java.io.IOException;
import java.io.InputStreamReader;
import java.nio.file.Files;
import java.nio.file.Paths;

public class Step__60__Sign extends InstallerStep {
    @Override
    public boolean Run() throws Exception {

        return Align() && Sign();
    }

    private boolean Sign() throws Exception {
        ApkSignerTool.main(new String[] {
            "sign",
            "--ks",
            properties.keystore,
            "--ks-key-alias",
            "cert",
            "--ks-pass",
            "pass:" + properties.keystorePass,
            paths.outputAPK.toString()
        });
        return true;
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
