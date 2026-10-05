namespace MelonLoader.Il2CppAssemblyGenerator.Packages.Models
{
    // Only the unrelated package/download and logging dependencies are stubbed.
    internal class PackageBase
    {
        internal string Name;
        internal static void ThrowInternalFailure(string message) => throw new Exception(message);
    }
}

namespace MelonLoader.Il2CppAssemblyGenerator
{
    internal static class Core
    {
        internal static readonly TestLogger Logger = new();
    }

    internal sealed class TestLogger
    {
        internal readonly Dictionary<string, string> Report = new();
        internal void Msg(string text)
        {
            if (!text.StartsWith("REPORT:")) return;
            var separator = text.IndexOf('=', 7);
            Report.Add(text[7..separator], text[(separator + 1)..]);
        }
        internal void Error(string text) => Console.Error.WriteLine(text);
    }
}
