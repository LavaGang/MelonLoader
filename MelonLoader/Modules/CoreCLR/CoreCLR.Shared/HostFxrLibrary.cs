using MelonLoader.NativeUtils;

namespace MelonLoader.CoreCLR;

public class HostFxrLibrary
{
    #region Private Members

    private static HostFxrLibrary _instance;

    #endregion

    #region Public Members

    public static HostFxrLibrary Instance
    {
        get => _instance;
        set
        {
            if (_instance != null)
                return;
            _instance = value;
        }
    }
    
    #endregion
}