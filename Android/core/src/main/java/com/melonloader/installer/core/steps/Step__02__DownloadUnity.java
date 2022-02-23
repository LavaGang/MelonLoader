package com.melonloader.installer.core.steps;

import com.melonloader.installer.core.InstallerStep;

import java.io.*;
import java.net.URL;
import java.net.URLConnection;

public class Step__02__DownloadUnity extends InstallerStep {
    @Override
    public boolean Run() throws Exception {
        if (properties.unityNativeBase != null && properties.unityManagedBase != null)
            return true;

        properties.logger.Log("Downloading Unity Dependencies");
        downloadFile(properties.unityProvider + "/" + properties.unityVersion + ".zip", paths.unityZip.toString());

        return false;
    }

    protected void downloadFile(String _url, String _output) throws IOException {
        URL url = new URL(_url);
        URLConnection connection = url.openConnection();
        connection.connect();

        int lenghtOfFile = connection.getContentLength();

        // download the file
        InputStream input = new BufferedInputStream(url.openStream(),
                8192);

        // Output stream
        OutputStream output = new FileOutputStream(_output);

        byte data[] = new byte[1024];

        int count;
        while ((count = input.read(data)) != -1) {
            output.write(data, 0, count);
        }

        output.flush();

        // closing streams
        output.close();
        input.close();
    }
}
