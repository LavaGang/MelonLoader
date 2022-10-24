using System;
using System.Net;
using System.Net.Security;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;

namespace MelonLoader.Fixes
{
    internal static class ServerCertificateValidation
    {
        internal static void Install()
        {
            Type SPMType = typeof(ServicePointManager);

            // ServicePointManager.Expect100Continue
            FieldInfo expectContinue = SPMType.GetField(nameof(expectContinue), BindingFlags.NonPublic | BindingFlags.Static);
            if (expectContinue != null)
                expectContinue.SetValue(null, true);

            //ServicePointManager.SecurityProtocol
            FieldInfo _securityProtocol = SPMType.GetField(nameof(_securityProtocol), BindingFlags.NonPublic | BindingFlags.Static);
            if (_securityProtocol != null)
                _securityProtocol.SetValue(null,
                    SecurityProtocolType.Ssl3
                    | SecurityProtocolType.Tls
                    | (SecurityProtocolType)768 /* SecurityProtocolType.Tls11 */
                    | (SecurityProtocolType)3072 /* SecurityProtocolType.Tls12 */);

            ServicePointManager.ServerCertificateValidationCallback += CertificateValidation;
        }
        
        // Based on: https://stackoverflow.com/questions/43457050/error-getting-response-stream-write-the-authentication-or-decryption-has-faile
        private static bool CertificateValidation(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            if (sslPolicyErrors == SslPolicyErrors.None)
                return true;
            for (int i = 0; i < chain.ChainStatus.Length; i++)
            {
                if (chain.ChainStatus[i].Status == X509ChainStatusFlags.RevocationStatusUnknown)
                    continue;
                chain.ChainPolicy.RevocationFlag = X509RevocationFlag.EntireChain;
                chain.ChainPolicy.RevocationMode = X509RevocationMode.Online;
                chain.ChainPolicy.UrlRetrievalTimeout = new TimeSpan(0, 1, 0);
                chain.ChainPolicy.VerificationFlags = X509VerificationFlags.AllFlags;
                bool chainIsValid = chain.Build((X509Certificate2)certificate);
                if (!chainIsValid)
                    return false;
            }
            return true;
        }
    }
}
