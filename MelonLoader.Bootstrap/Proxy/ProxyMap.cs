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
            FileName = "D3D10",
            ProxyFuncs = typeof(D3D10Exports),
        },
        new()
        {
            FileName = "D3D11",
            ProxyFuncs = typeof(D3D11Exports),
        },
        new()
        {
            FileName = "D3D12",
            ProxyFuncs = typeof(D3D12Exports),
        },
        new()
        {
            FileName = "D3D8",
            ProxyFuncs = typeof(D3D8Exports),
        },
        new()
        {
            FileName = "D3D9",
            ProxyFuncs = typeof(D3D9Exports),
        },
        new()
        {
            FileName = "DDraw",
            ProxyFuncs = typeof(DDrawExports),
        },
        new()
        {
            FileName = "DInput",
            ProxyFuncs = typeof(DInputExports),
        },
        new()
        {
            FileName = "DInput8",
            ProxyFuncs = typeof(DInput8Exports),
        },
        new()
        {
            FileName = "DSound",
            ProxyFuncs = typeof(DSoundExports),
        },
        new()
        {
            FileName = "MSACM32",
            ProxyFuncs = typeof(MSACM32Exports),
        },
        new()
        {
            FileName = "Version",
            ProxyFuncs = typeof(VersionExports),
        },
        new()
        {
            FileName = "WinHTTP",
            ProxyFuncs = typeof(WinHTTPExports),
        },
        new()
        {
            FileName = "WinMM",
            ProxyFuncs = typeof(WinMMExports),
        },
    ];
}
#endif
