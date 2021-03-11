using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MelonLoader.AssemblyGenerator
{
    internal static class RemoteAPI
    {
        //private static int TIMEOUT_MS = 7000;
        internal static InfoStruct LAST_RESPONSE = new InfoStruct();

        internal static void Contact()
        {
            Logger.Msg("Contacting RemoteAPI...");
            RemoteAPIHosts.Samboy.Contact();
            if (RemoteAPIHosts.Samboy.LAST_RESPONSE != null)
                LAST_RESPONSE = RemoteAPIHosts.Samboy.LAST_RESPONSE;
            else
            {
                RemoteAPIHosts.Ruby.Contact();
                if (RemoteAPIHosts.Ruby.LAST_RESPONSE != null)
                    LAST_RESPONSE = RemoteAPIHosts.Ruby.LAST_RESPONSE;
            }
            Logger.Debug_Msg($"ForceDumperVersion = {(string.IsNullOrEmpty(LAST_RESPONSE.ForceDumperVersion) ? "null" : LAST_RESPONSE.ForceDumperVersion)}");
            Logger.Debug_Msg($"ForceUnhollowerVersion = {(string.IsNullOrEmpty(LAST_RESPONSE.ForceUnhollowerVersion) ? "null" : LAST_RESPONSE.ForceUnhollowerVersion)}");
            Logger.Debug_Msg($"ObfuscationRegex = {(string.IsNullOrEmpty(LAST_RESPONSE.ObfuscationRegex) ? "null" : LAST_RESPONSE.ObfuscationRegex)}");
            Logger.Debug_Msg($"MappingURL = {(string.IsNullOrEmpty(LAST_RESPONSE.MappingURL) ? "null" : LAST_RESPONSE.MappingURL)}");
            Logger.Debug_Msg($"MappingFileSHA512 = {(string.IsNullOrEmpty(LAST_RESPONSE.MappingFileSHA512) ? "null" : LAST_RESPONSE.MappingFileSHA512)}");
        }

        internal class InfoStruct
        {
            public string ForceDumperVersion = null;
            public string ForceUnhollowerVersion = null;
            public string ObfuscationRegex = null;
            public string MappingURL = null;
            public string MappingFileSHA512 = null;
        }
    }
}
