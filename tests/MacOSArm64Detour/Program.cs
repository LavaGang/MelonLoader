using System.Reflection;
using System.Runtime.InteropServices;
using MelonLoader.Fixes.MonoMod;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

if (args.Length != 1) throw new ArgumentException("Expected the production Core.cs path.");

uint[] expectedSizes = [8, 12, 8, 12, 16];
MethodInfo wrapperSize = typeof(MacOSArm64NativeDetourPlatform).GetMethod(
    "GetDetourSize", BindingFlags.NonPublic | BindingFlags.Static)!;

Assembly runtimeDetour = typeof(MonoMod.RuntimeDetour.NativeDetourData).Assembly;
Type armPlatformType = runtimeDetour.GetType(
    "MonoMod.RuntimeDetour.Platforms.DetourNativeARMPlatform", throwOnError: true)!;
object armPlatform = Activator.CreateInstance(armPlatformType, nonPublic: true)!;
MethodInfo create = armPlatformType.GetMethod("Create")!;
MethodInfo apply = armPlatformType.GetMethod("Apply")!;
MethodInfo free = armPlatformType.GetMethod("Free")!;
FieldInfo sizeField = typeof(MonoMod.RuntimeDetour.NativeDetourData).GetField("Size")!;

IntPtr source = Marshal.AllocHGlobal(64);
try
{
    for (byte type = 0; type < expectedSizes.Length; ++type)
    {
        uint wrapper = (uint)wrapperSize.Invoke(null, [type])!;
        object detour = create.Invoke(armPlatform, [source, new IntPtr(0x12345678), (byte?)type])!;
        uint writer = (uint)sizeField.GetValue(detour)!;
        Check(wrapper == expectedSizes[type], $"wrapper type {type} size");
        Check(writer == expectedSizes[type], $"MonoMod writer type {type} size");
        free.Invoke(armPlatform, [detour]);
    }

    const long targetValue = 0x123456789ABCDF0;
    object arm64 = create.Invoke(armPlatform, [source, new IntPtr(targetValue), (byte?)4])!;
    apply.Invoke(armPlatform, [arm64]);
    byte[] bytes = new byte[16];
    Marshal.Copy(source, bytes, 0, bytes.Length);
    Check(bytes[..8].SequenceEqual(new byte[] { 0x4f, 0x00, 0x00, 0x58, 0xe0, 0x01, 0x1f, 0xd6 }),
        "ARM64 writer emits LDR X15 / BR X15");
    Check(BitConverter.ToInt64(bytes, 8) == targetValue, "ARM64 writer emits complete 64-bit target");
    free.Invoke(armPlatform, [arm64]);
}
finally
{
    Marshal.FreeHGlobal(source);
}

Console.WriteLine("ALL MANAGED DETOUR SIZE CHECKS PASSED");

string coreSource = File.ReadAllText(args[0]);
var parseOptions = new CSharpParseOptions(
    LanguageVersion.Latest, preprocessorSymbols: ["OSX", "ARM64", "NET6_0_OR_GREATER"]);
var coreRoot = CSharpSyntaxTree.ParseText(coreSource, parseOptions).GetRoot();
var assignments = coreRoot.DescendantNodes().OfType<AssignmentExpressionSyntax>().ToArray();
var nativeAssignments = assignments.Where(assignment =>
    assignment.Left.ToString() == "DetourHelper.Native").ToArray();
Check(nativeAssignments.Length == 1, "net6 macOS ARM64 selects one explicit native platform");
Check(nativeAssignments[0].Right.ToString().Contains("MacOSArm64NativeDetourPlatform"),
    "net6 macOS ARM64 selects protected detour wrapper");
var harmonyAssignment = assignments.Single(assignment => assignment.Left.ToString() == "HarmonyInstance");
Check(nativeAssignments[0].SpanStart < harmonyAssignment.SpanStart,
    "net6 macOS ARM64 selects wrapper before Harmony initialization");
Console.WriteLine("ALL CORE PLATFORM SELECTION CHECKS PASSED");

static void Check(bool condition, string label)
{
    if (!condition) throw new Exception("FAIL: " + label);
    Console.WriteLine("PASS: " + label);
}
