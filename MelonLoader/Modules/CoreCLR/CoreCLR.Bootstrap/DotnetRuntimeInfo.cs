using System.Diagnostics;
using System.IO;
using MelonLoader.Interfaces;

namespace MelonLoader.CoreCLR.Bootstrap;

public class DotnetRuntimeInfo : IRuntimeInfo
{
    private string _runtimeVersion;
    public string LibPath { get; private set; }
    public string EngineModulePath { get; private set; }

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
    
    public DotnetRuntimeInfo(string libPath, string engineModulePath)
    {
        LibPath = libPath;
        EngineModulePath = engineModulePath;
    }
}