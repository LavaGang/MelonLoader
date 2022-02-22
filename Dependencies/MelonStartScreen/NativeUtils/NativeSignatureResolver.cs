using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using MelonLoader.NativeUtils;

namespace MelonLoader.MelonStartScreen.NativeUtils
{
    internal static class NativeSignatureResolver
    {
        internal static bool Apply()
        {
            NativeSignatureFlags currentFlags = default;
            currentFlags |= MelonUtils.IsGame32Bit() ? NativeSignatureFlags.X86 : NativeSignatureFlags.X64;
            currentFlags |= MelonUtils.IsGameIl2Cpp() ? NativeSignatureFlags.Il2Cpp : NativeSignatureFlags.Mono;
            MelonDebug.Msg("Current Unity flags: " + (uint)currentFlags);

            string currentUnityVersion = InternalUtils.UnityInformationHandler.EngineVersion.ToStringWithoutType();
            string moduleName = "UnityPlayer.dll";
#if LINUX
            // TODO
#elif OSX
            // TODO
#elif ANDROID
            // TODO
#else
            if (!currentUnityVersion.StartsWith("20") || currentUnityVersion.StartsWith("2017.1"))
                moduleName = "player_win.exe";
#endif


            IntPtr moduleAddress = IntPtr.Zero;
            int moduleSize = 0;

            foreach (ProcessModule module in Process.GetCurrentProcess().Modules)
            {
                if (module.ModuleName == moduleName)
                {
                    moduleAddress = module.BaseAddress;
                    moduleSize = module.ModuleMemorySize;
                    break;
                }
            }

            if (moduleAddress == IntPtr.Zero)
            {
                Core.Logger.Error($"Failed to find module \"{moduleName}\"");
                return false;
            }

            bool success = true;
            foreach (Type type in typeof(NativeSignatureResolver).Assembly.GetTypes())
            {
                foreach (FieldInfo fi in type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static))
                {
                    bool hasAttribute = false;
                    bool signaturefound = false;
                    bool optionalOnEarlyVersion = false;
                    IOrderedEnumerable<NativeSignatureAttribute> nativeSignatureAttributes = fi.GetCustomAttributes(false)
                        .Where(attr => attr is NativeSignatureAttribute)
                        .Select(attr => (NativeSignatureAttribute)attr)
                        .OrderByDescending(attr => attr.LookupIndex);
                    foreach (NativeSignatureAttribute attribute in nativeSignatureAttributes)
                    {
                        hasAttribute = true;
                        if ((attribute.Flags & currentFlags) != attribute.Flags)
                            continue;

                        if (!IsUnityVersionOverOrEqual(currentUnityVersion, attribute.MinimalUnityVersions))
                            continue;

                        signaturefound = true;

                        if (attribute.Signature == null)
                        {
                            optionalOnEarlyVersion = true;
                            break;
                        }

                        IntPtr ptr = CppUtils.Sigscan(moduleAddress, moduleSize, attribute.Signature);
                        if (ptr == IntPtr.Zero)
                        {
                            success = false;
                            Core.Logger.Error("Failed to find the signature for field " + fi.Name + " in module. Signature: " + attribute.Signature);
                            break;
                        }

                        if (typeof(Delegate).IsAssignableFrom(fi.FieldType))
                            fi.SetValue(null, Marshal.GetDelegateForFunctionPointer(ptr, fi.FieldType));
                        else if (typeof(IntPtr).IsAssignableFrom(fi.FieldType))
                            fi.SetValue(null, ptr);
                        else
                            Core.Logger.Error($"Invalid target type for field \"{fi.FieldType} {fi.Name}\"");

                        MelonDebug.Msg("Signature for " + fi.Name + ": " + attribute.Signature);
                        break;
                    }

                    if (hasAttribute && !signaturefound && !optionalOnEarlyVersion)
                    {
                        Core.Logger.Error("Failed to find a signature for field " + fi.Name + " for this version of Unity");
                        success = false;
                    }

                    hasAttribute = false;
                    signaturefound = false;
                    IOrderedEnumerable<NativeFieldValueAttribute> nativeFieldValueAttributes = fi.GetCustomAttributes(false)
                        .Where(attr => attr is NativeFieldValueAttribute)
                        .Select(attr => (NativeFieldValueAttribute)attr)
                        .OrderByDescending(attr => attr.LookupIndex);
                    foreach (NativeFieldValueAttribute attribute in nativeFieldValueAttributes)
                    {
                        hasAttribute = true;
                        if ((attribute.Flags & currentFlags) != attribute.Flags)
                            continue;

                        if (!IsUnityVersionOverOrEqual(currentUnityVersion, attribute.MinimalUnityVersions))
                            continue;

                        signaturefound = true;

                        fi.SetValue(null, attribute.Value);

                        MelonDebug.Msg("Value for " + fi.Name + ": " + attribute.Value);
                        break;
                    }

                    if (hasAttribute && !signaturefound)
                    {
                        Core.Logger.Error("Failed to find a value for field " + fi.Name + " for this version of Unity");
                        success = false;
                    }
                }
            }

            return success;
        }

        internal static bool IsUnityVersionOverOrEqual(string currentversion, string[] validversions)
        {
            if (validversions == null || validversions.Length == 0)
                return true;

            string[] versionparts = currentversion.Split('.');

            foreach (string validversion in validversions)
            {
                string[] validversionparts = validversion.Split('.');

                if (
                    int.Parse(versionparts[0]) >= int.Parse(validversionparts[0]) &&
                    int.Parse(versionparts[1]) >= int.Parse(validversionparts[1]) &&
                    int.Parse(versionparts[2]) >= int.Parse(validversionparts[2]))
                    return true;
            }

            return false;
        }
    }
}
