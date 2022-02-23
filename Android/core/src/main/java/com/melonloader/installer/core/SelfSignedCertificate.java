// source - https://stackoverflow.com/a/59331561
package com.melonloader.installer.core;

//import android.util.Base64;
//import android.util.Log;

import org.spongycastle.asn1.x500.X500Name;
import org.spongycastle.cert.X509CertificateHolder;
import org.spongycastle.cert.X509v3CertificateBuilder;
import org.spongycastle.cert.jcajce.JcaX509CertificateConverter;
import org.spongycastle.cert.jcajce.JcaX509v3CertificateBuilder;
import org.spongycastle.jce.provider.BouncyCastleProvider;
import org.spongycastle.operator.ContentSigner;
import org.spongycastle.operator.jcajce.JcaContentSignerBuilder;

import java.io.File;
import java.io.FileInputStream;
import java.io.FileOutputStream;
import java.io.IOException;
import java.io.OutputStream;
import java.math.BigInteger;
import java.nio.charset.Charset;
import java.security.KeyPair;
import java.security.KeyPairGenerator;
import java.security.NoSuchAlgorithmException;
import java.security.PrivateKey;
import java.security.Provider;
import java.security.SecureRandom;
import java.security.cert.CertificateEncodingException;
import java.security.cert.CertificateException;
import java.security.cert.CertificateFactory;
import java.security.cert.X509Certificate;
import java.util.Base64;
import java.util.Date;

public final class SelfSignedCertificate {

    private static final String TAG = SelfSignedCertificate.class.getSimpleName();

    /**
     * Current time minus 1 year, just in case software clock goes back due to time synchronization
     */
    private static final Date DEFAULT_NOT_BEFORE = new Date(System.currentTimeMillis() - 86400000L * 365);

    /**
     * The maximum possible value in X.509 specification: 9999-12-31 23:59:59
     */
    private static final Date DEFAULT_NOT_AFTER = new Date(253402300799000L);

    /**
     * FIPS 140-2 encryption requires the key length to be 2048 bits or greater.
     * Let's use that as a sane default but allow the default to be set dynamically
     * for those that need more stringent security requirements.
     */
    private static final int DEFAULT_KEY_LENGTH_BITS = 2048;

    /**
     * FQDN to use if none is specified.
     */
    private static final String DEFAULT_FQDN = "example.com";

    /**
     * 7-bit ASCII, as known as ISO646-US or the Basic Latin block of the
     * Unicode character set
     */
    private static final Charset US_ASCII = Charset.forName("US-ASCII");

    private static final Provider provider = new BouncyCastleProvider();

    private final File certificate;
    private final File privateKey;
    private final X509Certificate cert;
    private final PrivateKey key;

    /**
     * Creates a new instance.
     */
    public SelfSignedCertificate() throws CertificateException {
        this(DEFAULT_NOT_BEFORE, DEFAULT_NOT_AFTER);
    }

    /**
     * Creates a new instance.
     *
     * @param notBefore Certificate is not valid before this time
     * @param notAfter  Certificate is not valid after this time
     */
    public SelfSignedCertificate(Date notBefore, Date notAfter) throws CertificateException {
        this("example.com", notBefore, notAfter);
    }

    /**
     * Creates a new instance.
     *
     * @param fqdn a fully qualified domain name
     */
    public SelfSignedCertificate(String fqdn) throws CertificateException {
        this(fqdn, DEFAULT_NOT_BEFORE, DEFAULT_NOT_AFTER);
    }

    /**
     * Creates a new instance.
     *
     * @param fqdn      a fully qualified domain name
     * @param notBefore Certificate is not valid before this time
     * @param notAfter  Certificate is not valid after this time
     */
    public SelfSignedCertificate(String fqdn, Date notBefore, Date notAfter) throws CertificateException {
        // Bypass entropy collection by using insecure random generator.
        // We just want to generate it without any delay because it's for testing purposes only.
        this(fqdn, new SecureRandom(), DEFAULT_KEY_LENGTH_BITS, notBefore, notAfter);
    }

    /**
     * Creates a new instance.
     *
     * @param fqdn   a fully qualified domain name
     * @param random the {@link java.security.SecureRandom} to use
     * @param bits   the number of bits of the generated private key
     */
    public SelfSignedCertificate(String fqdn, SecureRandom random, int bits) throws CertificateException {
        this(fqdn, random, bits, DEFAULT_NOT_BEFORE, DEFAULT_NOT_AFTER);
    }

    /**
     * Creates a new instance.
     *
     * @param fqdn      a fully qualified domain name
     * @param random    the {@link java.security.SecureRandom} to use
     * @param bits      the number of bits of the generated private key
     * @param notBefore Certificate is not valid before this time
     * @param notAfter  Certificate is not valid after this time
     */
    public SelfSignedCertificate(String fqdn, SecureRandom random, int bits, Date notBefore, Date notAfter)
            throws CertificateException {
        // Generate an RSA key pair.
        final KeyPair keypair;
        try {
            KeyPairGenerator keyGen = KeyPairGenerator.getInstance("RSA");
            keyGen.initialize(bits, random);
            keypair = keyGen.generateKeyPair();
        } catch (NoSuchAlgorithmException e) {
            // Should not reach here because every Java implementation must have RSA key pair generator.
            throw new Error(e);
        }

        String[] paths;
        try {
            // Try Bouncy Castle if the current JVM didn't have sun.security.x509.
            paths = generateCertificate(fqdn, keypair, random, notBefore, notAfter);
        } catch (Throwable t2) {
            Main.GetProperties().logger.Log(TAG + " Failed to generate a self-signed X.509 certificate using Bouncy Castle: " + t2);
            throw new CertificateException("No provider succeeded to generate a self-signed certificate. See debug log for the root cause.", t2);
        }

        certificate = new File(paths[0]);
        privateKey = new File(paths[1]);
        key = keypair.getPrivate();
        FileInputStream certificateInput = null;
        try {
            certificateInput = new FileInputStream(certificate);
            cert = (X509Certificate) CertificateFactory.getInstance("X509").generateCertificate(certificateInput);
        } catch (Exception e) {
            throw new CertificateEncodingException(e);
        } finally {
            if (certificateInput != null) {
                try {
                    certificateInput.close();
                } catch (IOException e) {
                    Main.GetProperties().logger.Log(TAG + " Failed to close a file: " + certificate + " " + e);
                }
            }
        }
    }

    /**
     * Returns the generated X.509 certificate file in PEM format.
     */
    public File certificate() {
        return certificate;
    }

    /**
     * Returns the generated RSA private key file in PEM format.
     */
    public File privateKey() {
        return privateKey;
    }

    /**
     * Returns the generated X.509 certificate.
     */
    public X509Certificate cert() {
        return cert;
    }

    /**
     * Returns the generated RSA private key.
     */
    public PrivateKey key() {
        return key;
    }

    /**
     * Deletes the generated X.509 certificate file and RSA private key file.
     */
    public void delete() {
        safeDelete(certificate);
        safeDelete(privateKey);
    }

    private static String[] generateCertificate(String fqdn, KeyPair keypair, SecureRandom random, Date notBefore, Date notAfter)
            throws Exception {
        PrivateKey key = keypair.getPrivate();

        // Prepare the information required for generating an X.509 certificate.
        X500Name owner = new X500Name("CN=" + fqdn);
        X509v3CertificateBuilder builder = new JcaX509v3CertificateBuilder(
                owner, new BigInteger(64, random), notBefore, notAfter, owner, keypair.getPublic());

        ContentSigner signer = new JcaContentSignerBuilder("SHA256WithRSAEncryption").build(key);
        X509CertificateHolder certHolder = builder.build(signer);
        X509Certificate cert = new JcaX509CertificateConverter().setProvider(provider).getCertificate(certHolder);
        cert.verify(keypair.getPublic());

        return newSelfSignedCertificate(fqdn, key, cert);
    }

    private static String[] newSelfSignedCertificate(String fqdn, PrivateKey key, X509Certificate cert) throws IOException, CertificateEncodingException {
        String keyText = "-----BEGIN PRIVATE KEY-----\n" + Base64.getEncoder().encodeToString(key.getEncoded()) + "\n-----END PRIVATE KEY-----\n";
        File keyFile = File.createTempFile("keyutil_" + fqdn + '_', ".key");
        keyFile.deleteOnExit();

        OutputStream keyOut = new FileOutputStream(keyFile);
        try {
            keyOut.write(keyText.getBytes(US_ASCII));
            keyOut.close();
            keyOut = null;
        } finally {
            if (keyOut != null) {
                safeClose(keyFile, keyOut);
                safeDelete(keyFile);
            }
        }

        String certText = "-----BEGIN CERTIFICATE-----\n" + Base64.getEncoder().encodeToString(cert.getEncoded()) + "\n-----END CERTIFICATE-----\n";
        File certFile = File.createTempFile("keyutil_" + fqdn + '_', ".crt");
        certFile.deleteOnExit();

        OutputStream certOut = new FileOutputStream(certFile);
        try {
            certOut.write(certText.getBytes(US_ASCII));
            certOut.close();
            certOut = null;
        } finally {
            if (certOut != null) {
                safeClose(certFile, certOut);
                safeDelete(certFile);
                safeDelete(keyFile);
            }
        }

        return new String[]{certFile.getPath(), keyFile.getPath()};
    }

    private static void safeDelete(File certFile) {
        if (!certFile.delete()) {
            Main.GetProperties().logger.Log(TAG + " Failed to delete a file: " + certFile);
        }
    }

    private static void safeClose(File keyFile, OutputStream keyOut) {
        try {
            keyOut.close();
        } catch (IOException e) {
            Main.GetProperties().logger.Log(TAG + " Failed to close a file: " + keyFile + " " +e);
        }
    }
}