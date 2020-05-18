using System;
using System.IO;
using System.Linq;
using ICSharpCode.SharpZipLib.Zip;

namespace MelonLoader.Support
{
    public static class Mono472
    {
        public static void LoadZippedMods(string modDirectory, bool preload, Action<byte[], bool> LoadAssembly)
        {
            string[] zippedFiles = Directory.GetFiles(modDirectory, "*.zip");
            if (zippedFiles.Length > 0)
            {
                for (int i = 0; i < zippedFiles.Count(); i++)
                {
                    string file = zippedFiles[i];
                    if (!string.IsNullOrEmpty(file))
                    {
                        try
                        {
                            using (var fileStream = File.OpenRead(file))
                            {
                                using (var zipInputStream = new ZipInputStream(fileStream))
                                {
                                    ZipEntry entry;
                                    while ((entry = zipInputStream.GetNextEntry()) != null)
                                    {
                                        if (Path.GetFileName(entry.Name).Length <= 0 ||
                                            !Path.GetFileName(entry.Name).EndsWith(".dll"))
                                            continue;

                                        using (var unzippedFileStream = new MemoryStream())
                                        {
                                            int size = 0;
                                            byte[] buffer = new byte[4096];
                                            while (true)
                                            {
                                                size = zipInputStream.Read(buffer, 0, buffer.Length);
                                                if (size > 0)
                                                    unzippedFileStream.Write(buffer, 0, size);
                                                else
                                                    break;
                                            }

                                            LoadAssembly(unzippedFileStream.ToArray(), preload);
                                        }
                                    }
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            MelonModLogger.LogError("Unable to load " + file + ":\n" + e.ToString());
                            MelonModLogger.Log("------------------------------");
                        }
                    }
                }
            }
        }
    }
}