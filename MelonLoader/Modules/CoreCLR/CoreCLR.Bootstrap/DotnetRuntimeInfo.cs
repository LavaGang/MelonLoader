namespace MelonLoader.CoreCLR.Bootstrap;

public class DotnetRuntimeInfo
{
    public string LibPath { get; private set; }
    
    public DotnetRuntimeInfo(string libPath)
    {
        LibPath = libPath;
    }
}