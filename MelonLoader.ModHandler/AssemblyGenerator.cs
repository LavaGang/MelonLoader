using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
//using System.Net;
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

            bool was_successful = true;
            string game_folder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "MelonLoader");
            string base_folder = Path.Combine(game_folder, "AssemblyGenerator");
            if (!Directory.Exists(base_folder))
                Directory.CreateDirectory(base_folder);

            // Get GameAssembly Hash
            string game_assembly_dll = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "GameAssembly.dll");
            string game_assembly_hash = null;
            using (var md5 = MD5.Create())
            {
                using (var stream = File.OpenRead(game_assembly_dll))
                {
                    var hash = md5.ComputeHash(stream);
                    game_assembly_hash = BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
                }
            }

            // Check Engine Version, Game Version, and GameAssembly Hash for Changes
            IniFile version_file = new IniFile(Path.Combine(base_folder, "Config.ini"));
            if (string.IsNullOrEmpty(version_file.IniReadValue("AssemblyGenerator", "GameAssembly"))
                || string.IsNullOrEmpty(game_assembly_hash)
                || !game_assembly_hash.Equals(version_file.IniReadValue("AssemblyGenerator", "GameAssembly")))
            {
                MelonModLogger.Log("Assembly Generation Needed!");
                string managed_folder = Path.Combine(game_folder, "Managed");

                // Delete Old Files
                string file_list_json = Path.Combine(base_folder, "FileList.json");
                if (File.Exists(file_list_json))
                {
                    List<string> filenames = JsonConvert.DeserializeObject<List<string>>(File.ReadAllText(file_list_json));
                    string[] files = Directory.GetFiles(managed_folder, "*.dll");
                    if (files.Length > 0)
                    {
                        foreach (string s in files)
                        {
                            if (!File.Exists(s) || !s.EndsWith(".dll", true, null))
                                break;
                            if (filenames.Contains(Path.GetFileName(s)))
                                File.Delete(s);
                        }
                    }
                    File.Delete(file_list_json);
                }

                // Check Il2CppDumper Folder
                string dumper_folder = Path.Combine(base_folder, "Il2CppDumper");
                if (!Directory.Exists(dumper_folder))
                    Directory.CreateDirectory(dumper_folder);
                else
                {
                    // Delete Old Files
                    if (File.Exists(Path.Combine(dumper_folder, "dump.cs")))
                        File.Delete(Path.Combine(dumper_folder, "dump.cs"));
                    if (File.Exists(Path.Combine(dumper_folder, "il2cpp.h")))
                        File.Delete(Path.Combine(dumper_folder, "il2cpp.h"));
                    if (File.Exists(Path.Combine(dumper_folder, "script.json")))
                        File.Delete(Path.Combine(dumper_folder, "script.json"));
                }

                // Check Il2CppDumper Output Folder
                string dumper_output_folder = Path.Combine(dumper_folder, "DummyDll");
                if (!Directory.Exists(dumper_output_folder))
                    Directory.CreateDirectory(dumper_output_folder);
                else
                {
                    // Delete Old Files
                    string[] files = Directory.GetFiles(dumper_output_folder, "*.dll");
                    if (files.Length > 0)
                    {
                        foreach (string s in files)
                        {
                            if (!File.Exists(s) || !s.EndsWith(".dll", true, null))
                                break;
                            File.Delete(s);
                        }
                    }
                    Directory.Delete(dumper_output_folder);
                    Directory.CreateDirectory(dumper_output_folder);
                }

                // Check Il2CppDumper Folder for Il2CppDumper.exe
                string dumper_exe = Path.Combine(dumper_folder, "Il2CppDumper.exe");
                if (!File.Exists(dumper_exe))
                {
                    // Delete Existing Files in Il2CppDumper

                    // Download Il2CppDumper

                    // Extract Il2CppDumper to the Il2CppDumper Folder

                    was_successful = false;
                }

                if (!was_successful)
                    MelonModLogger.LogError("Unable to Download Il2CppDumper!");
                else
                {
                    // Load and Execute Il2CppDumper
                    try
                    {
                        Assembly a = Assembly.LoadFrom(dumper_exe);
                        if (a != null)
                        {
                            Type b = a.GetType("Il2CppDumper.Program");
                            if (b != null)
                            {
                                MethodInfo c = b.GetMethod("Main", BindingFlags.NonPublic | BindingFlags.Static);
                                if (c != null)
                                {
                                    Directory.SetCurrentDirectory(dumper_folder);
                                    MelonModLogger.Log("Running Il2CppDumper...");
                                    c.Invoke(null, new object[] { new string[] { game_assembly_dll, Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Directory.GetDirectories(AppDomain.CurrentDomain.BaseDirectory).First(f => f.EndsWith("_Data")), "il2cpp_data", "Metadata", "global-metadata.dat") } });
                                }
                                else
                                    was_successful = false;
                            }
                            else
                                was_successful = false;
                        }
                        else
                            was_successful = false;
                    }
                    catch (Exception e) { was_successful = false; MelonModLogger.LogError(e.ToString()); }

                    if (!was_successful)
                        MelonModLogger.LogError("Unable to Load Il2CppDumper!");
                    else
                    {
                        // Check Il2CppAssemblyUnhollower Folder
                        string unhollower_folder = Path.Combine(base_folder, "Il2CppAssemblyUnhollower");
                        if (!Directory.Exists(unhollower_folder))
                            Directory.CreateDirectory(unhollower_folder);

                        // Check Il2CppAssemblyUnhollower Folder for AssemblyUnhollower.exe
                        string unhollower_exe = Path.Combine(unhollower_folder, "AssemblyUnhollower.exe");
                        if (!File.Exists(unhollower_exe))
                        {
                            // Delete Existing Files in Il2CppAssemblyUnhollower

                            // Download Il2CppAssemblyUnhollower

                            // Extract Il2CppAssemblyUnhollower to the Il2CppAssemblyUnhollower Folder

                            was_successful = false;
                        }

                        if (!was_successful)
                            MelonModLogger.LogError("Unable to Download Il2CppAssemblyUnhollower!");
                        else
                        {
                            // Check Il2CppAssemblyUnhollower Unity BaseLibs Folder
                            string unhollower_baselibs_folder = Path.Combine(unhollower_folder, "BaseLibs");
                            if (!Directory.Exists(unhollower_baselibs_folder))
                            {
                                Directory.CreateDirectory(unhollower_baselibs_folder);

                                // Download Il2CppAssemblyUnhollower Unity BaseLibs

                                // Extract Il2CppAssemblyUnhollower Unity BaseLibs to the Il2CppAssemblyUnhollower Unity BaseLibs Folder
                            }

                            if (!was_successful)
                                MelonModLogger.LogError("Unable to Download Il2CppAssemblyUnhollower Unity BaseLibs!");
                            else
                            {
                                // Check Il2CppAssemblyUnhollower Output Folder
                                string unhollower_output_folder = Path.Combine(unhollower_folder, "Output");
                                if (!Directory.Exists(unhollower_output_folder))
                                    Directory.CreateDirectory(unhollower_output_folder);
                                else
                                {
                                    // Delete Old Files
                                    string[] files = Directory.GetFiles(unhollower_output_folder, "*.dll");
                                    if (files.Length > 0)
                                    {
                                        foreach (string s in files)
                                        {
                                            if (!File.Exists(s) || !s.EndsWith(".dll", true, null))
                                                break;
                                            File.Delete(s);
                                        }
                                    }
                                    Directory.Delete(unhollower_output_folder);
                                    Directory.CreateDirectory(unhollower_output_folder);
                                }

                                // Load and Execute Il2CppAssemblyUnhollower
                                try
                                {
                                    Assembly a = Assembly.LoadFrom(unhollower_exe);
                                    if (a != null)
                                    {
                                        Type b = a.GetType("AssemblyUnhollower.Program");
                                        if (b != null)
                                        {
                                            MethodInfo c = b.GetMethods(BindingFlags.Public | BindingFlags.Static).First(x => ((x.GetParameters().Count() == 1) && (x.GetParameters()[0].ParameterType == typeof(string[]))));
                                            if (c != null)
                                            {
                                                Directory.SetCurrentDirectory(unhollower_folder);
                                                MelonModLogger.Log("Running Il2CppAssemblyUnhollower...");
                                                c.Invoke(null, new object[] { new string[] { ("--input=" + dumper_output_folder), ("--output=" + unhollower_output_folder), ("--mscorlib=" + Path.Combine(managed_folder, "mscorlib.dll")), ("--unity=" + unhollower_baselibs_folder), "--blacklist-assembly=SharpYml", "--blacklist-assembly=Mono.Security", "--blacklist-assembly=Newtonsoft.Json", "--blacklist-assembly=Valve.Newtonsoft.Json" } });
                                            }
                                            else
                                                was_successful = false;
                                        }
                                        else
                                            was_successful = false;
                                    }
                                    else
                                        was_successful = false;
                                }
                                catch (Exception e) { MelonModLogger.LogError(e.ToString()); }

                                if (!was_successful)
                                    MelonModLogger.LogError("Unable to Load Il2CppAssemblyUnhollower!");
                                else
                                {
                                    // Delete Unused Files
                                    if (File.Exists(Path.Combine(dumper_folder, "dump.cs")))
                                        File.Delete(Path.Combine(dumper_folder, "dump.cs"));
                                    if (File.Exists(Path.Combine(dumper_folder, "il2cpp.h")))
                                        File.Delete(Path.Combine(dumper_folder, "il2cpp.h"));
                                    if (File.Exists(Path.Combine(dumper_folder, "script.json")))
                                        File.Delete(Path.Combine(dumper_folder, "script.json"));

                                    // Get Files in Il2CppDumper Output Folder
                                    string[] files = Directory.GetFiles(dumper_output_folder);
                                    if (files.Length > 0)
                                    {
                                        // Get Files in Il2CppAssemblyUnhollower Output Folder
                                        string[] files2 = Directory.GetFiles(unhollower_output_folder);
                                        if (files2.Length > 0)
                                        {
                                            // Delete Unneeded Files
                                            if (File.Exists(Path.Combine(unhollower_output_folder, "Mono.Security.dll")))
                                                File.Delete(Path.Combine(unhollower_output_folder, "Mono.Security.dll"));
                                            if (File.Exists(Path.Combine(unhollower_output_folder, "SharpYml.dll")))
                                                File.Delete(Path.Combine(unhollower_output_folder, "SharpYml.dll"));
                                            if (File.Exists(Path.Combine(unhollower_output_folder, "Newtonsoft.Json.dll")))
                                                File.Delete(Path.Combine(unhollower_output_folder, "Newtonsoft.Json.dll"));
                                            if (File.Exists(Path.Combine(unhollower_output_folder, "Valve.Newtonsoft.Json.dll")))
                                                File.Delete(Path.Combine(unhollower_output_folder, "Valve.Newtonsoft.Json.dll"));

                                            // Move Files from Il2CppAssemblyUnhollower Output Folder to Managed Folder
                                            List<string> filenames = new List<string>();
                                            foreach (string s in files2)
                                            {
                                                if (!File.Exists(s) || !s.EndsWith(".dll", true, null))
                                                    break;
                                                string file_name = Path.GetFileName(s);
                                                filenames.Add(file_name);
                                                string new_file_path = Path.Combine(managed_folder, file_name);
                                                if (File.Exists(new_file_path))
                                                    File.Delete(new_file_path);
                                                File.Move(s, new_file_path);
                                            }

                                            // Clean Up and Save Info
                                            Directory.Delete(unhollower_output_folder);
                                            File.WriteAllText(file_list_json, JsonConvert.SerializeObject(filenames, Formatting.Indented));
                                            version_file.IniWriteValue("AssemblyGenerator", "GameAssembly", game_assembly_hash);
                                            MelonModLogger.Log("SUCCESS!");
                                        }
                                        else
                                            was_successful = false;
                                    }
                                    else
                                        was_successful = false;
                                }
                            }
                        }
                    }
                }
            }
            if (!was_successful)
                MelonModLogger.LogError("FAILURE!");
            return true; //ONLY TEMPORARY, WAIT FOR COMPLETE IMPLEMENTATION
        }
    }
}
