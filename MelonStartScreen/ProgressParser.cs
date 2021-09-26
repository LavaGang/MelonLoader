using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace MelonLoader.MelonStartScreen
{
    internal static class ProgressParser
    {
        private static float generationPercent = MelonUtils.IsGameIl2Cpp() ? 80f : 10f;
        private static ModLoadStep currentStep = ModLoadStep.Generation;
        private static string currentStepName = "___";

        private static readonly Dictionary<ModLoadStep, string> stepsNames = new Dictionary<ModLoadStep, string>()
        {
            { ModLoadStep.OnApplicationStart_Plugins, "OnApplicationStart - Plugin" },
            { ModLoadStep.LoadMods, "Loading Mod" },
            { ModLoadStep.OnApplicationStart_Mods, "OnApplicationStart - Mod" },
            { ModLoadStep.OnApplicationLateStart_Plugins, "OnApplicationLateStart - Plugin" },
            { ModLoadStep.OnApplicationLateStart_Mods, "OnApplicationLateStart - Mod" }
        };

        public static float GetProgressFromLog(string data, ref string progressText, float default_) // TODO flags (il2cpp/mono, cpp2il/il2cppdumper)
        {
            if (currentStep != ModLoadStep.Generation)
                return default_;

            if (Regex.IsMatch(data, "Assembly is up to date"))
            {
                generationPercent = default_ * 100f;
                return default_;
            }

            float totalTime = 0;
            float progressTime = 0;
            foreach (var entry in averageStepDurations)
            {
                if (progressTime <= 0f && Regex.IsMatch(data, entry.message))
                {
                    progressTime = totalTime;
                    progressText = entry.progresstext ?? data;
                }

                totalTime += entry.weight;
            }

            return progressTime > 0 ? (progressTime / totalTime) * (generationPercent * 0.01f) : default_;
        }

        public static float GetProgressFromMod(string modname, ref string progressText)
        {
            progressText = $"{currentStepName}: {modname}";

            float generationPart = generationPercent * 0.01f;
            return generationPart + (((int)currentStep - 1) * ((1 - generationPart) / 5));
        }

        public static float SetModState(ModLoadStep step, ref string progressText)
        {
            currentStep = step;
            if (!stepsNames.TryGetValue(step, out currentStepName))
                currentStepName = $"{step}";
            progressText = currentStepName;

            float generationPart = generationPercent * 0.01f;
            return generationPart + (((int)currentStep - 1) * ((1 - generationPart) / 5));
        }

        internal static readonly (string message, float weight, string progresstext)[] averageStepDurations = new (string, float, string)[]
        {
            (
                @"Contacting RemoteAPI\.\.\.",
                100f,
                @"Initialization - Contacting Remote API"
            ),
            (
                @"Downloading Unity \S+ Dependencies\.\.\.",
                1000f,
                @"Initialization - Downloading Unity Dependencies"
            ),
            (
                @"Extracting .* to .*UnityDpendencies",
                500f,
                @"Initialization - Extracting Unity Dependencies"
            ),
            (
                @"Downloading Il2CppDumper\.\.\.",
                500f,
                @"Initialization - Downloading Il2CppDumper"
            ),
            (
                @"Extracting .* to .*Il2CppDumper",
                500f,
                @"Initialization - Extracting Il2CppDumper"
            ),
            (
                @"Downloading Il2CppAssemblyUnhollower\.\.\.",
                500f,
                @"Initialization - Downloading Il2CppAssemblyUnhollower"
            ),
            (
                @"Extracting .* to .*Il2CppAssemblyUnhollower",
                500f,
                @"Initialization - Extracting Il2CppAssemblyUnhollower"
            ),
            (
                @"Downloading Deobfuscation Map\.\.\.",
                500f,
                @"Initialization - Downloading Deobfuscation Map"
            ),
            (
                @"Checking GameAssembly\.\.\.",
                1000f,
                @"Initialization - Checking GameAssembly"
            ),
            // Cpp2IL
            // Slaynash: I skipped a lot of steps taking less than 100ms, but we may want to add them back for slower platforms
            (
                @"Initializing metadata\.\.\.",
                2500f,
                "Cpp2IL - Initializing metadata"
            ),
            (
                @"Searching Binary for Required Data",
                402f,
                "Cpp2IL - Seaching Binary for Required Data"
            ),
            (
                @"Initializing Binary",
                402f,
                "Cpp2IL - Initializing Binary"
            ),
            (
                @"Initializing Binary",
                2533f,
                "Cpp2IL - Initializing Binary"
            ),
            (
                @"Pre-generating stubs",
                708f,
                "Cpp2IL - Building Assemblies: Pre-generating stubs"
            ),
            (
                @"Populating mscorlib\.dll",
                340f,
                "Cpp2IL - Building Assemblies: Populating assemblies"
            ),
            (
                @"Populating Assembly-CSharp\.dll",
                3000f,
                "Cpp2IL - Building Assemblies: Populating assemblies"
            ),
            (
                @"Fixing up explicit overrides",
                1000f,
                "Cpp2IL - Fixing up explicit overrides"
            ),
            (
                @"Running Scan for Known Functions",
                300f,
                "Cpp2IL - Running Scan for Known Functions"
            ),
            (
                @"Applying type, method, and field attributes",
                5500f,
                "Cpp2IL - Applying type, method, and field attributes"
            ),
            (
                @"Saving .* assemblies to ",
                4000f,
                "Cpp2IL - Saving assemblies"
            ),
            /*
            // Il2CppDumper
            (
                @"Initializing metadata\.\.\.",
                3500f,
                null
            ),
            (
                @"Initializing il2cpp file\.\.\.",
                1800f,
                null
            ),
            (
                @"Dumping\.\.\.",
                1400f,
                null
            ),
            (
                @"Generate struct\.\.\.",
                26000f,
                null
            ),
            (
                @"Generate dummy dll\.\.\.",
                13000f,
                null
            ),
            */
            // Il2CppAssemblyUnhollower
            (
                @"Reading assemblies\.\.\.",
                170f,
                "Il2CppAssemblyUnhollower - Reading assemblies"
            ),
            (
                @"Reading system assemblies\.\.\.",
                14f,
                "Il2CppAssemblyUnhollower - Reading system assemblies"
            ),
            (
                @"Reading unity assemblies\.\.\.",
                29f,
                "Il2CppAssemblyUnhollower - Reading unity assemblies"
            ),
            (
                @"Creating rewrite assemblies\.\.\.",
                20f,
                "Il2CppAssemblyUnhollower - Creating rewrite assemblies"
            ),
            (
                @"Computing renames\.\.\.",
                281f,
                "Il2CppAssemblyUnhollower - Computing renames"
            ),
            (
                @"Creating typedefs\.\.\.",
                109f,
                "Il2CppAssemblyUnhollower - Creating typedefs"
            ),
            (
                @"Computing struct blittability\.\.\.",
                10f,
                "Il2CppAssemblyUnhollower - Computing struct blittability"
            ),
            (
                @"Filling typedefs\.\.\.",
                27f,
                "Il2CppAssemblyUnhollower - Filling typedefs"
            ),
            (
                @"Filling generic constraints\.\.\.",
                6f,
                "Il2CppAssemblyUnhollower - Filling generic constraints"
            ),
            (
                @"Creating members\.\.\.",
                2256f,
                "Il2CppAssemblyUnhollower - Creating members"
            ),
            (
                @"Scanning method cross-references\.\.\.",
                1919f,
                "Il2CppAssemblyUnhollower - Scanning method cross-references"
            ),
            (
                @"Finalizing method declarations\.\.\.",
                2867f,
                "Il2CppAssemblyUnhollower - Finalizing method declarations"
            ),
            (
                @"Filling method parameters\.\.\.",
                510f,
                "Il2CppAssemblyUnhollower - Filling method parameters"
            ),
            (
                @"Creating static constructors\.\.\.",
                1237f,
                "Il2CppAssemblyUnhollower - Creating static constructors"
            ),
            (
                @"Creating value type fields\.\.\.",
                186f,
                "Il2CppAssemblyUnhollower - Creating value type fields"
            ),
            (
                @"Creating enums\.\.\.",
                69f,
                "Il2CppAssemblyUnhollower - Creating enums"
            ),
            (
                @"Creating IntPtr constructors\.\.\.",
                63f,
                "Il2CppAssemblyUnhollower - Creating IntPtr constructors"
            ),
            (
                @"Creating type getters\.\.\.",
                132f,
                "Il2CppAssemblyUnhollower - Creating type getters"
            ),
            /*
            (
                @"Creating non-blittable struct constructors\.\.\.",
                38f,
                "Il2CppAssemblyUnhollower - Creating non-blittable struct constructors"
            ),
            (
                @"Creating generic method static constructors\.\.\.",
                42f,
                "Il2CppAssemblyUnhollower - Creating generic method static constructors"
            ),
            */
            (
                @"Creating field accessors\.\.\.",
                1642f,
                "Il2CppAssemblyUnhollower - Creating field accessors"
            ),
            (
                @"Filling methods\.\.\.",
                2385f,
                "Il2CppAssemblyUnhollower - Filling methods"
            ),
            (
                @"Generating implicit conversions\.\.\.",
                121f,
                "Il2CppAssemblyUnhollower - Generating implicit conversions"
            ),
            (
                @"Creating properties\.\.\.",
                102f,
                "Il2CppAssemblyUnhollower - Creating properties"
            ),
            (
                @"Unstripping types\.\.\.",
                44f,
                "Il2CppAssemblyUnhollower - Unstripping types"
            ),
            (
                @"Unstripping fields\.\.\.",
                5f,
                "Il2CppAssemblyUnhollower - Unstripping fields"
            ),
            (
                @"Unstripping methods\.\.\.",
                241f,
                "Il2CppAssemblyUnhollower - Unstripping methods"
            ),
            (
                @"Unstripping method bodies\.\.\.",
                266f,
                "Il2CppAssemblyUnhollower - Unstripping method bodies"
            ),
            (
                @"Generating forwarded types\.\.\.",
                4f,
                "Il2CppAssemblyUnhollower - Generating forwarded types"
            ),
            (
                @"Writing xref cache\.\.\.",
                1179f,
                "Il2CppAssemblyUnhollower - Writing xref cache"
            ),
            (
                @"Writing assemblies\.\.\.",
                2586f,
                "Il2CppAssemblyUnhollower - Writing assemblies"
            ),
            (
                @"Writing method pointer map\.\.\.",
                89f,
                "Il2CppAssemblyUnhollower - Writing method pointer map"
            ),
            // Move files
            (
                @"Deleting .*\.dll",
                500f,
                "Il2CppAssemblyUnhollower - Moving files"
            ),
        };
    }
}
