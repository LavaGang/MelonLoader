using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Text;

namespace MelonLoader.Bootstrap.Proxy;

// Credits for the idea: https://github.com/NotNite/NativeProxy
public static partial class ProxyResolver
{
    private const uint PageExecuteReadWrite = 0x40;

    private static readonly Proxy[] proxies =
    [
        new()
        {
            OriginalPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "version.dll"),
            ProxyFuncs = typeof(VersionExports)
        },
        new()
        {
            OriginalPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "winhttp.dll"),
            ProxyFuncs = typeof(WinHttpExports)
        },
        new()
        {
            OriginalPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "winmm.dll"),
            ProxyFuncs = typeof(WinMMExports)
        }
    ];

    public static unsafe void Init(nint ourHandle)
    {
        var ourPathBdr = new StringBuilder(1024);
        if (GetModuleFileName(ourHandle, ourPathBdr, (uint)ourPathBdr.Capacity) == 0)
            return;

        var ourName = Path.GetFileName(ourPathBdr.ToString());

        var proxy = proxies.FirstOrDefault(x => ourName.Equals(Path.GetFileName(x.OriginalPath), StringComparison.OrdinalIgnoreCase));
        if (proxy == null)
            return;

        if (!NativeLibrary.TryLoad(proxy.OriginalPath, out var ogHandle))
            return;

        foreach (var exportMethod in proxy.ProxyFuncs.GetMethods())
        {
            var export = exportMethod.Name;
            var preTag = "Impl";
            if (!export.StartsWith(preTag))
                continue;

            export = export[preTag.Length..];

            if (!NativeLibrary.TryGetExport(ogHandle, export, out var theirExport)
                || !NativeLibrary.TryGetExport(ourHandle, export, out var ourExport))
            {
                Console.WriteLine($"Proxy export not found: '{export}'");
                continue;
            }

            var jump = AssembleJump(theirExport);
            if (VirtualProtect(ourExport, jump.Length, PageExecuteReadWrite, out var oldProtect) != 1)
                continue;

            var span = new Span<byte>((byte*)ourExport, jump.Length);
            jump.CopyTo(span);
            VirtualProtect(ourExport, jump.Length, oldProtect, out _);
        }
    }

    private static unsafe byte[] AssembleJump(nint addr)
    {
#if X64
        byte[] shellcode =
        [
            // mov r11
            0x49, 0xBB,
            // addr
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            // jmp r11
            0x41, 0xFF, 0xE3
        ];
        var offset = 2;
#else
        byte[] shellcode =
        [
            // mov eax
            0xB8,
            // addr
            0x00, 0x00, 0x00, 0x00,
            // jmp eax
            0xFF, 0xE0
        ];
        var offset = 1;
#endif

        fixed (byte* p = shellcode)
        {
            *(nint*)(p + offset) = addr;
        }

        return shellcode;
    }

    [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    private static extern uint GetModuleFileName(nint module, StringBuilder filename, uint size);

    [LibraryImport("kernel32.dll", SetLastError = true)]
    private static partial byte VirtualProtect(nint address, nint size, uint newProtect, out uint oldProtect);

    private class Proxy
    {
        public required string OriginalPath { get; init; }

        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicMethods)]
        public required Type ProxyFuncs { get; init; }
    }
}