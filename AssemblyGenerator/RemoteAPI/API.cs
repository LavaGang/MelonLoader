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
        internal static bool ShouldMakeContact = true;
        private static event Action HostContacts;

        static RemoteAPI()
        {
            HostContacts += RemoteAPIHosts.Melon.Contact;
            HostContacts += RemoteAPIHosts.Melon1.Contact;
            HostContacts += RemoteAPIHosts.Melon2.Contact;
            HostContacts += RemoteAPIHosts.Ruby.Contact;
            HostContacts += RemoteAPIHosts.Samboy.Contact;
        }

        internal static void Contact()
        {
            Logger.Msg("Contacting RemoteAPI...");
            HostContacts();
            DebugPrintResponse();
        }

        private static void DebugPrintResponse()
        {
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
