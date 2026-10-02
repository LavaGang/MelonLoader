using MelonLoader.Il2CppAssemblyGenerator;
using MelonLoader.Il2CppAssemblyGenerator.Packages.Models;

if (args.Length != 3)
    throw new ArgumentException("Expected fixture executable and two harmless injection libraries.");

const string injection = "DYLD_INSERT_LIBRARIES";
const string unrelated = "ML_REGRESSION_UNRELATED";
const string overridden = "ML_REGRESSION_OVERRIDE";
const string marker = "ML_REGRESSION_INJECTED";
const string bundle = "DOTNET_BUNDLE_EXTRACT_BASE_DIR";
string[] keys = [injection, unrelated, overridden, marker, bundle];
var original = keys.ToDictionary(key => key, Environment.GetEnvironmentVariable);
var fixture = Path.GetFullPath(args[0]);
var inheritedLibraries = Path.GetFullPath(args[1]) + ":" + Path.GetFullPath(args[2]);
var suppliedLibraries = Path.GetFullPath(args[2]) + ":" + Path.GetFullPath(args[1]);
var package = new ExecutablePackage { Name = "environment fixture", ExeFilePath = fixture };

try
{
    Environment.SetEnvironmentVariable(unrelated, "keep parent value with spaces");
    Environment.SetEnvironmentVariable(overridden, "parent value");
    Environment.SetEnvironmentVariable(marker, null);
    Environment.SetEnvironmentVariable(bundle, "parent bundle setting");

    Run("null environment", inheritedLibraries, null);
    Run("empty environment", inheritedLibraries, new());
    Run("supplied overrides", inheritedLibraries, new()
    {
        [injection] = suppliedLibraries,
        [overridden] = "child override with spaces"
    });
    Run("absent inherited injection", null, null);
    Run("override introduces injection", null, new() { [injection] = suppliedLibraries });
}
finally
{
    foreach (var entry in original)
        Environment.SetEnvironmentVariable(entry.Key, entry.Value);
}

void Run(string label, string inherited, Dictionary<string, string> supplied)
{
    Environment.SetEnvironmentVariable(injection, inherited);
    var before = keys.ToDictionary(key => key, Environment.GetEnvironmentVariable);
    var suppliedBefore = supplied == null ? null : new Dictionary<string, string>(supplied);
    Core.Logger.Report.Clear();
    if (!package.Execute([], environment: supplied)) throw new Exception($"{label}: child failed");

    string expectedInjection = supplied != null && supplied.TryGetValue(injection, out var value)
        ? value : inherited;
#if OSX
    expectedInjection = null;
#endif
    Equal(injection, expectedInjection ?? "<absent>");
    Equal(marker, expectedInjection == null ? "<absent>" : "yes");
    Equal(unrelated, before[unrelated]);
    Equal(overridden, supplied != null && supplied.TryGetValue(overridden, out var childValue)
        ? childValue : before[overridden]);
    Equal(bundle, supplied == null ? before[bundle]
        : Path.Combine(Path.GetDirectoryName(fixture), Path.GetFileNameWithoutExtension(fixture) + "_Temp"));

    foreach (var entry in before)
        if (Environment.GetEnvironmentVariable(entry.Key) != entry.Value)
            throw new Exception($"{label}: parent {entry.Key} changed");
    if (supplied != null && !supplied.OrderBy(x => x.Key).SequenceEqual(suppliedBefore.OrderBy(x => x.Key)))
        throw new Exception($"{label}: caller dictionary changed");
    Console.WriteLine($"PASS: {label}");

    void Equal(string key, string expected)
    {
        if (!Core.Logger.Report.TryGetValue(key, out var actual) || actual != expected)
            throw new Exception($"{label}: {key}: expected '{expected}', got '{actual}'");
    }
}
