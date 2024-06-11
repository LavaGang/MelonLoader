using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;

#if !NET35
using System.Runtime.InteropServices;
#endif

namespace MelonLoader.Utils
{
    public static class MelonUtils
    {
        public static string HashCode { get; private set; }
        public static string OSVersion { get; private set; }

        internal static void Setup()
        {
            using var sha = SHA256.Create();
            HashCode = string.Join("", sha.ComputeHash(File.ReadAllBytes(Assembly.GetExecutingAssembly().Location)).Select(b => b.ToString("X")).ToArray());
            OSVersion = OsUtils.GetOSVersion();
        }
        
        public static string ComputeSimpleSHA256Hash(string filePath)
        {
            using var sha = SHA256.Create();
            return string.Join("", sha.ComputeHash(File.ReadAllBytes(filePath)).Select(b => b.ToString("X")).ToArray());
        }
        
        public static T PullAttributeFromAssembly<T>(Assembly asm, bool inherit = false) where T : Attribute
        {
            T[] attributetbl = PullAttributesFromAssembly<T>(asm, inherit);
            if ((attributetbl == null) || (attributetbl.Length <= 0))
                return null;
            return attributetbl[0];
        }

        public static T[] PullAttributesFromAssembly<T>(Assembly asm, bool inherit = false) where T : Attribute
        {
            Attribute[] att_tbl = Attribute.GetCustomAttributes(asm, inherit);

            if ((att_tbl == null) || (att_tbl.Length <= 0))
                return null;

            Type requestedType = typeof(T);
            string requestedAssemblyName = requestedType.Assembly.GetName().Name;
            List<T> output = new();
            foreach (Attribute att in att_tbl)
            {
                Type attType = att.GetType();
                string attAssemblyName = attType.Assembly.GetName().Name;

                if ((attType == requestedType)
                    || IsTypeEqualToFullName(attType, requestedType.FullName)
                    || ((attAssemblyName.Equals("MelonLoader")
                         || attAssemblyName.Equals("MelonLoader.ModHandler"))
                        && (requestedAssemblyName.Equals("MelonLoader")
                            || requestedAssemblyName.Equals("MelonLoader.ModHandler"))
                        && IsTypeEqualToName(attType, requestedType.Name)))
                    output.Add(att as T);
            }

            return output.ToArray();
        }
        
        public static bool IsTypeEqualToName(Type type1, string type2)
            => type1.Name == type2 || (type1 != typeof(object) && IsTypeEqualToName(type1.BaseType, type2));

        public static bool IsTypeEqualToFullName(Type type1, string type2)
            => type1.FullName == type2 || (type1 != typeof(object) && IsTypeEqualToFullName(type1.BaseType, type2));
        
        public static Color DefaultTextColor 
            => Color.White;
        public static ConsoleColor DefaultTextConsoleColor 
            => ConsoleColor.White;

        public static string TimeStamp
            => $"{DateTime.Now:HH:mm:ss.fff}";

        public static PlatformID Platform 
            => Environment.OSVersion.Platform;

        public static bool IsUnix
            => !IsAndroid 
                && Platform is PlatformID.Unix;

        public static bool IsWindows
            => Platform is PlatformID.Win32NT 
                or PlatformID.Win32S
                or PlatformID.Win32Windows
                or PlatformID.WinCE;

        public static bool IsMac 
            => Platform is PlatformID.MacOSX;

        public static bool IsWineOrProton
            => OsUtils.IsWineOrProton();

        public static T Clamp<T>(T value, T min, T max) where T : IComparable<T> { if (value.CompareTo(min) < 0) return min; if (value.CompareTo(max) > 0) return max; return value; }

#if !NET35

        public static bool IsAndroid => RuntimeInformation.IsOSPlatform(OSPlatform.Create("ANDROID"));
        public static bool Is32Bit => !Environment.Is64BitProcess;
        public static bool Is64Bit => Environment.Is64BitProcess;

#else

        public static bool Is32Bit => IntPtr.Size == 4;
        public static bool Is64Bit => IntPtr.Size != 4;
        public static bool IsAndroid => false;

#endif
    }
}