#if WINDOWS
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace MelonLoader.Bootstrap.Proxy;

// Credits for the idea: https://github.com/NotNite/NativeProxy
public static partial class ProxyResolver
{
    private const uint PageExecuteReadWrite = 0x40;
    private const string ModuleExtension = ".dll";

    public static unsafe void Init(nint ourHandle)
    {
        var ourPathBdr = new StringBuilder(1024);
        if (GetModuleFileName(ourHandle, ourPathBdr, (uint)ourPathBdr.Capacity) == 0)
            return;

        var ourFilePath = ourPathBdr.ToString();
        var ourName = Path.GetFileNameWithoutExtension(ourFilePath);

        var proxy = ProxyMap.proxies.FirstOrDefault(x => ourName.Equals(x.FileName, StringComparison.OrdinalIgnoreCase));
        if (proxy == null)
            return;

        var basePath = Path.GetDirectoryName(ourFilePath);
        nint ogHandle = 0;
        if (!LoadModuleFromLocalCopy(basePath, ourName, "_original", ref ogHandle)
            && !LoadModuleFromSystemCopy(ourName, ref ogHandle))
            return;

        foreach (var exportMethod in ProxyMap.sharedProxy.GetMethods())
            CreateExport(ourHandle, ogHandle, exportMethod);
        foreach (var exportMethod in proxy.ProxyFuncs.GetMethods())
            CreateExport(ourHandle, ogHandle, exportMethod);
    }

    private static bool LoadModuleFromLocalCopy(string? basePath, 
        string fileName, 
        string tag,
        ref nint handle)
    {
        if (string.IsNullOrEmpty(basePath))
            return false;

        string filePath = Path.Combine(basePath, $"{fileName}{tag}{ModuleExtension}");
        return LoadModule(filePath, ref handle);
    }

    private static bool LoadModuleFromSystemCopy(string fileName, ref nint handle)
    {
        string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), $"{fileName}{ModuleExtension}");
        return LoadModule(filePath, ref handle);
    }

    internal static bool LoadModule(string filePath, ref nint handle)
    {
        if (!File.Exists(filePath))
            return false;

        nint moduleHandle = 0;
        if (!NativeLibrary.TryLoad(filePath, out moduleHandle))
            return false;

        handle = moduleHandle;
        return true;
    }

    private static unsafe void CreateExport(nint ourHandle,
        nint ogHandle, 
        MethodInfo method)
    {
        var export = method.Name;
        var preTag = "Impl";
        if (!export.StartsWith(preTag))
            return;

        export = export[preTag.Length..];

        if (!NativeLibrary.TryGetExport(ogHandle, export, out var theirExport)
            || !NativeLibrary.TryGetExport(ourHandle, export, out var ourExport))
        {
            //Console.WriteLine($"Proxy export not found: '{export}'");
            return;
        }

        var jump = AssembleJump(theirExport);
        if (VirtualProtect(ourExport, jump.Length, PageExecuteReadWrite, out var oldProtect) != 1)
            return;

        var span = new Span<byte>((byte*)ourExport, jump.Length);
        jump.CopyTo(span);
        VirtualProtect(ourExport, jump.Length, oldProtect, out _);
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
}
#endif