package com.melonloader.installer.core.steps;

import com.melonloader.installer.core.InstallerStep;
import com.melonloader.installer.core.SelfSignedCertificate;

import java.io.File;
import java.io.FileOutputStream;
import java.io.IOException;
import java.security.*;
import java.security.cert.CertificateException;

public class Step__55__GeneratingSigner extends InstallerStep {
    @Override
    public boolean Run() throws Exception {
        if ((new File(paths.keystore.toString())).exists())
            return true;

        properties.logger.Log("Generating Keystore");

        generateKey(paths.keystore.toString());

        return true;
    }

    protected void generateKey(String output) throws KeyStoreException, CertificateException, NoSuchAlgorithmException, NoSuchProviderException, InvalidAlgorithmParameterException, IOException {
        String password = properties.keystorePass;
        KeyStore baseKS = KeyStore.getInstance(KeyStore.getDefaultType());

        KeyStore.ProtectionParameter protParam = new KeyStore.PasswordProtection(password.toCharArray());

        KeyStore.Builder builder = KeyStore.Builder.newInstance(KeyStore.getDefaultType(), baseKS.getProvider(), protParam);

        KeyStore keystore = builder.getKeyStore();

        Provider provider = keystore.getProvider();

        SelfSignedCertificate ssc = new SelfSignedCertificate(properties.keystoreName);

        keystore.setCertificateEntry("cert", ssc.cert());
        keystore.setKeyEntry("cert", ssc.key(), password.toCharArray(), new java.security.cert.Certificate[] { ssc.cert() });

        FileOutputStream fos = new FileOutputStream(output);
        keystore.store(fos, password.toCharArray());
    }
}
