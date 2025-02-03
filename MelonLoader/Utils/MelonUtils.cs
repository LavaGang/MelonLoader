using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using MonoMod.Cil;
using MonoMod.Utils;
using HarmonyLib;
using MelonLoader.InternalUtils;
using MelonLoader.Lemons.Cryptography;
using MelonLoader.Utils;
using MelonLoader.NativeUtils;

namespace MelonLoader
{
    public static class MelonUtils
    {
        private static readonly MethodInfo StackFrameGetMethod = typeof(StackFrame).GetMethod("GetMethod", BindingFlags.Instance | BindingFlags.Public);
        private static readonly LemonSHA256 sha256 = new();
        private static readonly LemonSHA512 sha512 = new();

        public static T Clamp<T>(T value, T min, T max) where T : IComparable<T> { if (value.CompareTo(min) < 0) return min; if (value.CompareTo(max) > 0) return max; return value; }

        public static void SetCurrentDomainBaseDirectory(string dirpath, AppDomain domain = null)
        {
            if(MelonEnvironment.IsDotnetRuntime)
                return;
            
            if (domain == null)
                domain = AppDomain.CurrentDomain;
            try
            {
                ((AppDomainSetup)typeof(AppDomain).GetProperty("SetupInformationNoCopy", BindingFlags.NonPublic | BindingFlags.Instance)
                    .GetValue(domain, new object[0]))
                    .SetApplicationBase(dirpath);
            }
            catch (Exception ex) { MelonLogger.Warning($"AppDomainSetup.ApplicationBase Exception: {ex}"); }
            Directory.SetCurrentDirectory(dirpath);
        }

        public static MelonBase GetMelonFromStackTrace()
        {
            StackTrace st = new(3, true);
            return GetMelonFromStackTrace(st);
        }

        public static MelonBase GetMelonFromStackTrace(StackTrace st, bool allFrames = false)
        {
            if (st.FrameCount <= 0)
                return null;

            if (allFrames)
            {
                foreach (StackFrame frame in st.GetFrames())
                {
                    MelonBase ret = CheckForMelonInFrame(frame);
                    if (ret != null)
                        return ret;
                }
                return null;

            }

            MelonBase output = CheckForMelonInFrame(st);
            if (output == null)
                output = CheckForMelonInFrame(st, 1);
            if (output == null)
                output = CheckForMelonInFrame(st, 2);
            return output;
        }

        private static MelonBase CheckForMelonInFrame(StackTrace st, int frame = 0)
        {
            StackFrame sf = st.GetFrame(frame);
            if (sf == null)
                return null;

            return CheckForMelonInFrame(sf);
        }

        private static MelonBase CheckForMelonInFrame(StackFrame sf)
            //The JIT compiler on .NET 6 on Windows 10 (win11 is fine, somehow) really doesn't like us calling StackFrame.GetMethod here
            //Rather than trying to work out why, I'm just going to call it via reflection.
            => GetMelonFromAssembly(((MethodBase)StackFrameGetMethod.Invoke(sf, new object[0]))?.DeclaringType?.Assembly);

        private static MelonBase GetMelonFromAssembly(Assembly asm)
            => asm == null ? null : MelonPlugin.RegisteredMelons.Cast<MelonBase>().FirstOrDefault(x => x.MelonAssembly.Assembly == asm) ?? MelonMod.RegisteredMelons.FirstOrDefault(x => x.MelonAssembly.Assembly == asm);

        public static string ComputeSimpleSHA256Hash(string filePath)
        {
            if (!File.Exists(filePath))
                return null;

            byte[] byteHash = File.ReadAllBytes(filePath);
            if (byteHash == null)
                return null;

            return sha256.ComputeHash(byteHash).ToString("X2");
        }

        public static string ComputeSimpleSHA512Hash(string filePath)
        {
            if (!File.Exists(filePath))
                return null;

            byte[] byteHash = File.ReadAllBytes(filePath);
            if (byteHash == null)
                return null;

            return sha512.ComputeHash(byteHash).ToString("X2");
        }

        public static string ToString(this byte[] data)
        {
            StringBuilder result = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
                result.Append(data[i].ToString());
            return result.ToString();
        }

        public static string ToString(this byte[] data, string format)
        {
            StringBuilder result = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
                result.Append(data[i].ToString(format));
            return result.ToString();
        }

        public static string ToString(this byte[] data, IFormatProvider provider)
        {
            StringBuilder result = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
                result.Append(data[i].ToString(provider));
            return result.ToString();
        }

        public static string ToString(this byte[] data, string format, IFormatProvider provider)
        {
            StringBuilder result = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
                result.Append(data[i].ToString(format, provider));
            return result.ToString();
        }

        public static IntPtr ToAnsiPointer(this string data)
            => Marshal.StringToHGlobalAnsi(data);
        public static string ToAnsiString(this IntPtr ptr)
            => Marshal.PtrToStringAnsi(ptr);

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

        public static string MakePlural(this string str, int amount)
            => amount == 1 ? str : $"{str}s";

        public static IEnumerable<Type> GetValidTypes(this Assembly asm)
            => GetValidTypes(asm, null);

        public static IEnumerable<Type> GetValidTypes(this Assembly asm, LemonFunc<Type, bool> predicate)
        {
            IEnumerable<Type> returnval = Enumerable.Empty<Type>();
            try { returnval = asm.GetTypes().AsEnumerable(); }
            catch (ReflectionTypeLoadException ex) 
            {
                //MelonLogger.Error($"Failed to get all types in assembly {asm.FullName} due to: {ex.Message}", ex);
                returnval = ex.Types; 
            }
            //catch (Exception ex)
            //{
                //MelonLogger.Error($"Failed to get all types in assembly {asm.FullName} due to: {ex.Message}", ex);
            //    returnval = null;
            //}
            return returnval.Where(x => (x != null) && (predicate == null || predicate(x)));
        }

        public static Type GetValidType(this Assembly asm, string typeName)
            => GetValidType(asm, typeName, null);

        public static Type GetValidType(this Assembly asm, string typeName, LemonFunc<Type, bool> predicate)
        {
            Type x = null;
            try { x = asm.GetType(typeName); }
            catch //(Exception ex)
            {
                //MelonLogger.Error($"Failed to get type {typeName} from assembly {asm.FullName} due to: {ex.Message}", ex);
                x = null;
            }
            if ((x != null) && (predicate == null || predicate(x)))
                return x;
            return null;
        }

        public static bool IsNotImplemented(this MethodBase methodBase)
        {
            if (methodBase == null)
                throw new ArgumentNullException(nameof(methodBase));

            DynamicMethodDefinition method = methodBase.ToNewDynamicMethodDefinition();
            ILContext ilcontext = new(method.Definition);
            ILCursor ilcursor = new(ilcontext);

            bool returnval = (ilcursor.Instrs.Count == 2)
                && (ilcursor.Instrs[1].OpCode.Code == Mono.Cecil.Cil.Code.Throw);

            ilcontext.Dispose();
            method.Dispose();
            return returnval;
        }

        public static bool IsManagedDLL(string path)
        {
            if (Path.GetExtension(path).ToLower() != ".dll")
                return false;

            try
            {
                AssemblyName.GetAssemblyName(path);
                return true;
            }
            catch (FileLoadException)
            {
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static HarmonyMethod ToNewHarmonyMethod(this MethodInfo methodInfo)
        {
            if (methodInfo == null)
                throw new ArgumentNullException(nameof(methodInfo));
            return new HarmonyMethod(methodInfo);
        }

        public static DynamicMethodDefinition ToNewDynamicMethodDefinition(this MethodBase methodBase)
        {
            if (methodBase == null)
                throw new ArgumentNullException(nameof(methodBase));
            return new DynamicMethodDefinition(methodBase);
        }

        private static FieldInfo AppDomainSetup_application_base;
        public static void SetApplicationBase(this AppDomainSetup _this, string value)
        {
            if (AppDomainSetup_application_base == null)
                AppDomainSetup_application_base = typeof(AppDomainSetup).GetField("application_base", BindingFlags.NonPublic | BindingFlags.Instance);
            if (AppDomainSetup_application_base != null)
                AppDomainSetup_application_base.SetValue(_this, value);
        }

        private static FieldInfo HashAlgorithm_HashSizeValue;
        public static void SetHashSizeValue(this HashAlgorithm _this, int value)
        {
            if (HashAlgorithm_HashSizeValue == null)
                HashAlgorithm_HashSizeValue = typeof(HashAlgorithm).GetField("HashSizeValue", BindingFlags.Public | BindingFlags.Instance);
            if (HashAlgorithm_HashSizeValue != null)
                HashAlgorithm_HashSizeValue.SetValue(_this, value);
        }

        // Modified Version of System.IO.Path.HasExtension from .NET Framework's mscorlib.dll
        public static bool ContainsExtension(this string path)
        {
            if (path != null)
            {
                path.CheckInvalidPathChars();
                int num = path.Length;
                while (--num >= 0)
                {
                    char c = path[num];
                    if (c == '.')
                        return num != path.Length - 1;
                    if (c == Path.DirectorySeparatorChar || c == Path.AltDirectorySeparatorChar || c == Path.VolumeSeparatorChar)
                        break;
                }
            }
            return false;
        }

        // Modified Version of System.IO.Path.CheckInvalidPathChars from .NET Framework's mscorlib.dll
        private static void CheckInvalidPathChars(this string path)
        {
            foreach (int num in path)
                if (num == 34 || num == 60 || num == 62 || num == 124 || num < 32)
                    throw new ArgumentException("Argument_InvalidPathChars", nameof(path));
        }

        public static void GetDelegate<T>(this IntPtr ptr, out T output) where T : Delegate
            => output = GetDelegate<T>(ptr);
        public static T GetDelegate<T>(this IntPtr ptr) where T : Delegate
            => GetDelegate(ptr, typeof(T)) as T;
        public static Delegate GetDelegate(this IntPtr ptr, Type type)
        {
            if (ptr == IntPtr.Zero)
                throw new ArgumentNullException(nameof(ptr));
            Delegate del = Marshal.GetDelegateForFunctionPointer(ptr, type);
            if (del == null)
                throw new Exception($"Unable to Get Delegate of Type {type.FullName} for Function Pointer!");
            return del;
        }
        public static IntPtr GetFunctionPointer(this Delegate del)
            => Marshal.GetFunctionPointerForDelegate(del);

        public static void SetConsoleTitle(string title)
        {
            if (LoaderConfig.Current.Console.DontSetTitle || !BootstrapInterop.Library.IsConsoleOpen())
                return;

            // Using reflection to avoid resolver errors
            AccessTools.Property(typeof(Console), "Title")?.SetValue(null, title, null);
        }

        public static string GetFileProductName(string filepath)
        {
            var fileInfo = FileVersionInfo.GetVersionInfo(filepath);
            if (fileInfo != null)
                return fileInfo.ProductName;
            return null;
        }
    }
}
