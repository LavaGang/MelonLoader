#if WINDOWS
using MelonLoader.Bootstrap.Proxy.Exports;
using System.Diagnostics.CodeAnalysis;

namespace MelonLoader.Bootstrap.Proxy;

internal static class ProxyMap
{
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicMethods)]
    internal static readonly Type sharedProxy = typeof(SharedExports);

    internal class Proxy
    {
        public required string FileName { get; init; }

        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicMethods)]
        public required Type ProxyFuncs { get; init; }
    }

    internal static readonly Proxy[] proxies =
    [
        new()
        {
            FileName = "d3d10",
            ProxyFuncs = typeof(D3D10Exports),
        },
        new()
        {
            FileName = "d3d11",
            ProxyFuncs = typeof(D3D11Exports),
        },
        new()
        {
            FileName = "d3d12",
            ProxyFuncs = typeof(D3D12Exports),
        },
        new()
        {
            FileName = "d3d8",
            ProxyFuncs = typeof(D3D8Exports),
        },
        new()
        {
            FileName = "d3d9",
            ProxyFuncs = typeof(D3D9Exports),
        },
        new()
        {
            FileName = "ddraw",
            ProxyFuncs = typeof(DDrawExports),
        },
        new()
        {
            FileName = "dinput",
            ProxyFuncs = typeof(DInputExports),
        },
        new()
        {
            FileName = "dinput8",
            ProxyFuncs = typeof(DInput8Exports),
        },
        new()
        {
            FileName = "dsound",
            ProxyFuncs = typeof(DSoundExports),
        },
        new()
        {
            FileName = "msacm32",
            ProxyFuncs = typeof(MSACM32Exports),
        },
        new()
        {
            FileName = "version",
            ProxyFuncs = typeof(VersionExports),
        },
        new()
        {
            FileName = "winhttp",
            ProxyFuncs = typeof(WinHTTPExports),
        },
        new()
        {
            FileName = "winmm",
            ProxyFuncs = typeof(WinMMExports),
        },
    ];
}
#endif
