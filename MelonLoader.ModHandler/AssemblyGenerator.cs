using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security.Cryptography;
using Newtonsoft.Json;

namespace MelonLoader
{
    internal static class AssemblyGenerator
    {
        internal static bool Initialize()
        {
            //ServicePointManager.ServerCertificateValidationCallback = (sender, certificate, chain, errors) => true;

            string game_folder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "MelonLoader");
            string base_folder = Path.Combine(game_folder, "AssemblyGenerator");
            if (!Directory.Exists(base_folder))
                Directory.CreateDirectory(base_folder);

            return true;
        }
    }
}
