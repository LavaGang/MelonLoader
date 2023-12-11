using MelonLoader.NativeUtils;
using MelonLoader.Utils;

namespace MelonLoader.CoreCLR.Bootstrap;

public static class DotnetLoader
{
    #region Public Members

    public static DotnetRuntimeInfo RuntimeInfo { get; private set; }

    #endregion
    
    #region Bootstrap
    
    public static void Startup(DotnetRuntimeInfo runtimeInfo)
    {
        // Apply the information
        RuntimeInfo = runtimeInfo;

        // Check if it found any Mono variant library
        if (RuntimeInfo == null
            || string.IsNullOrEmpty(RuntimeInfo.LibPath))
        {
            MelonAssertion.ThrowInternalFailure($"Failed to find HostFxr Library!");
            return;
        }

        HostFxrLibrary.Instance = MelonNativeLibrary.ReflectiveLoad<HostFxrLibrary>(runtimeInfo.LibPath);
    }
    
    #endregion
}