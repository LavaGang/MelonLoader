using System.Diagnostics;
using System.IO;

namespace MelonLoader.CoreCLR.Bootstrap;

public class DotnetRuntimeInfo
{
    private string _runtimeVersion;
    public string LibPath { get; private set; }

    public string RuntimeVersion
    {
        get
        {
            if (!string.IsNullOrEmpty(_runtimeVersion))
                return _runtimeVersion;
            
            if (string.IsNullOrEmpty(LibPath))
                return null;
            
            FileVersionInfo versionInfo = FileVersionInfo.GetVersionInfo(LibPath);
            _runtimeVersion = $"{versionInfo.FileMajorPart}.{versionInfo.FileMinorPart}.{versionInfo.FileBuildPart}";

            return _runtimeVersion;
        }
    }
    
    public DotnetRuntimeInfo(string libPath)
    {
        LibPath = libPath;
    }
}