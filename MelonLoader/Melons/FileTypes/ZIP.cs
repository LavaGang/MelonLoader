using System;
using System.Collections.Generic;
using System.IO;
using MelonLoader.ICSharpCode.SharpZipLib.Zip;

namespace MelonLoader.MelonFileTypes
{
    internal static class ZIP
    {
        internal static void LoadAll(string folderpath)
        {
            string[] filearr = Directory.GetFiles(folderpath, "*.zip");
            if (filearr.Length <= 0)
                return;
            for (int i = 0; i < filearr.Length; i++)
            {
                string filepath = filearr[i];
                if (string.IsNullOrEmpty(filepath))
                    continue;

                LoadFromFile(filepath);
            }
        }

        private class ByteArrayPair
        {
            internal byte[] filedata;
            internal byte[] symbolsdata;
        }
        internal static void LoadFromFile(string filepath)
        {
            if (string.IsNullOrEmpty(filepath))
                return;

            Dictionary<string, ByteArrayPair> pairs = new Dictionary<string, ByteArrayPair>();
            bool source_code_detected = false;

            try
            {
                using (var filestream = File.OpenRead(filepath))
                using (var zipstream = new ZipInputStream(filestream))
                {
                    ZipEntry entry;
                    while ((entry = zipstream.GetNextEntry()) != null)
                    {
                        if (string.IsNullOrEmpty(entry.Name))
                            continue;

                        string entry_path = entry.Name;
                        string entry_filename = Path.GetFileName(entry.Name);
                        if (string.IsNullOrEmpty(entry_filename))
                            continue;

                        string extension = Path.GetExtension(entry_filename).ToLowerInvariant();
                        if (extension.Equals(".cs"))
                        {
                            source_code_detected = true;
                            break;
                        }

                        bool is_dll = extension.Equals(".dll");
                        bool is_mdb = extension.Equals(".mdb");
                        if (!is_dll && !is_mdb)
                            continue;

                        string entrypairid = entry_path;
                        if (is_mdb)
                        {
                            string file_name_no_ext = Path.GetFileNameWithoutExtension(entrypairid).ToLowerInvariant();
                            if (file_name_no_ext.EndsWith(".dll"))
                                entrypairid = Path.Combine(Path.GetDirectoryName(entry_path), Path.GetFileNameWithoutExtension(entrypairid));
                        }

                        ByteArrayPair byteArrayPair = null;
                        if (!pairs.TryGetValue(entrypairid, out byteArrayPair))
                        {
                            byteArrayPair = new ByteArrayPair();
                            pairs[entry.Name] = byteArrayPair;
                        }

                        try
                        {
                            using (MemoryStream memorystream = new MemoryStream())
                            {
                                int size = 0;
                                byte[] buffer = new byte[4096];
                                while (true)
                                {
                                    size = zipstream.Read(buffer, 0, buffer.Length);
                                    if (size > 0)
                                        memorystream.Write(buffer, 0, size);
                                    else
                                        break;
                                }
                                if (is_dll)
                                    byteArrayPair.filedata = memorystream.ToArray();
                                else if (is_mdb)
                                    byteArrayPair.symbolsdata = memorystream.ToArray();
                            }
                        }
                        catch (Exception ex)
                        {
                            MelonLogger.Error($"Failed to Read Entry {entry.Name} in ZIP Archive {filepath}: {ex}");
                            continue;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MelonLogger.Error($"Failed to Read ZIP Archive {filepath}: {ex}");
                return;
            }

            if (source_code_detected)
            {
                MelonLogger.Error($"Source Code ZIP Detected {filepath}");
                return;
            }

            if (pairs.Count <= 0)
                return;

            foreach (KeyValuePair<string, ByteArrayPair> keyValuePair in pairs)
            {
                if ((keyValuePair.Value == null)
                    || (keyValuePair.Value.filedata == null))
                    continue;
                DLL.LoadFromByteArray(keyValuePair.Value.filedata, keyValuePair.Value.symbolsdata, filepath);
            }
        }
    }
}