using System.Runtime.InteropServices;

namespace MelonLoader.Bootstrap.Utils;

public unsafe static partial class Dobby
{
    [LibraryImport("*", EntryPoint = "DobbyPrepare")]
    private static partial int Prepare(nint target, nint detour, nint* original);

    [LibraryImport("*", EntryPoint = "DobbyCommit")]
    private static partial int Commit(nint target);

    [LibraryImport("*", EntryPoint = "DobbyDestroy")]
    private static partial int Destroy(nint target);

    public static nint HookAttach(nint target, nint detour)
    {
        nint original = 0;
        if (Prepare(target, detour, &original) != 0)
        {
            throw new AccessViolationException($"Could not prepare patch to target {target:X}");
        }
        if (Commit(target) != 0)
        {
            throw new AccessViolationException($"Could not commit patch to target {target:X}");
        }
        return original;
    }

    public static void HookDetach(nint target)
    {
        var result = Destroy(target);
        if (result is not 0 and not -1)
        {
            throw new AccessViolationException($"Could not destroy patch for target {target:X}");
        }
    }

    public static Patch<TDelegate> CreatePatch<TDelegate>(nint target, TDelegate detour) where TDelegate : Delegate
    {
        var original = HookAttach(target, Marshal.GetFunctionPointerForDelegate(detour));

        var originalDel = Marshal.GetDelegateForFunctionPointer<TDelegate>(original);

        return new Patch<TDelegate>(target, detour, originalDel);
    }

    public static Patch<TDelegate>? CreatePatch<TDelegate>(nint hModule, string functionName, TDelegate detour) where TDelegate : Delegate
    {
        if (!NativeLibrary.TryGetExport(hModule, functionName, out var func))
            return null;

        return CreatePatch(func, detour);
    }

    public static Patch<TDelegate>? CreatePatch<TDelegate>(string moduleName, string functionName, TDelegate detour) where TDelegate : Delegate
    {
        if (!NativeLibrary.TryLoad(moduleName, out var hModule))
            return null;

        return CreatePatch(hModule, functionName, detour);
    }

    public class Patch<T> where T : Delegate
    {
        public nint Target { get; private set; }
        public T Detour { get; private set; }
        public T Original { get; private set; }

        internal Patch(nint target, T detour, T original)
        {
            Target = target;
            Detour = detour;
            Original = original;
        }

        public void Destroy()
        {
            if (Target == 0)
                return;

            HookDetach(Target);
            Original = Marshal.GetDelegateForFunctionPointer<T>(Target);
            Target = 0;
        }
    }
}