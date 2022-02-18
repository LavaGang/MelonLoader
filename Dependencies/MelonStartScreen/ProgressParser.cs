using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;

namespace MelonLoader.MelonStartScreen
{
    internal static class ProgressParser
    {
        private static float generationPercent = MelonUtils.IsGameIl2Cpp() ? 80f : 10f;
        private static ModLoadStep currentStep = ModLoadStep.Generation;
        private static string currentStepName = "___";

        internal struct AverageStepDuration
        {
            public string message;
            public float weight;
            public string progresstext;

            public AverageStepDuration(string message, float weight, string progresstext)
            {
                this.message = message;
                this.weight = weight;
                this.progresstext = progresstext;
            }
        }

        private static readonly Dictionary<ModLoadStep, string> stepsNames = new Dictionary<ModLoadStep, string>()
        {
            { ModLoadStep.LoadMelons, "Loading Melons" },
            { ModLoadStep.InitializeMelons, "Initializing" },
            { ModLoadStep.OnApplicationStart, "Loading..." }
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

        public static float GetProgressFromMod(MelonBase melon, ref string progressText)
        {
            progressText = $"{currentStepName} {melon.MelonTypeName}: {melon.Info.Name} {melon.Info.Version}";

            float generationPart = generationPercent * 0.01f;
            return generationPart + (((int)currentStep - 1) * ((1 - generationPart) / 4));
        }

        public static float GetProgressFromModAssembly(Assembly asm, ref string progressText)
        {
            progressText = $"{currentStepName}: {Path.GetFileName(asm.Location)}";

            float generationPart = generationPercent * 0.01f;
            return generationPart + (((int)currentStep - 1) * ((1 - generationPart) / 4));
        }

        public static bool SetModState(ModLoadStep step, ref string progressText, out float generationPart)
        {
            generationPart = generationPercent * 0.01f;
            generationPart += ((int)step - 1) * ((1 - generationPart) / 3);

            if (currentStep == step)
                return false;

            currentStep = step;
            if (!stepsNames.TryGetValue(step, out currentStepName))
                currentStepName = $"{step}";
            progressText = currentStepName;

            return true;
        }

        internal static readonly AverageStepDuration[] averageStepDurations = new AverageStepDuration[]
        {
            new AverageStepDuration(
                @"Contacting RemoteAPI\.\.\.",
                100f,
                @"Initialization - Contacting Remote API"
            ),
            new AverageStepDuration(
                @"Downloading Unity \S+ Dependencies\.\.\.",
                1000f,
                @"Initialization - Downloading Unity Dependencies"
            ),
            new AverageStepDuration(
                @"Extracting .* to .*UnityDpendencies",
                500f,
                @"Initialization - Extracting Unity Dependencies"
            ),
            new AverageStepDuration(
                @"Downloading Il2CppDumper\.\.\.",
                500f,
                @"Initialization - Downloading Il2CppDumper"
            ),
            new AverageStepDuration(
                @"Extracting .* to .*Il2CppDumper",
                500f,
                @"Initialization - Extracting Il2CppDumper"
            ),
            new AverageStepDuration(
                @"Downloading Il2CppAssemblyUnhollower\.\.\.",
                500f,
                @"Initialization - Downloading Il2CppAssemblyUnhollower"
            ),
            new AverageStepDuration(
                @"Extracting .* to .*Il2CppAssemblyUnhollower",
                500f,
                @"Initialization - Extracting Il2CppAssemblyUnhollower"
            ),
            new AverageStepDuration(
                @"Downloading DeobfuscationMap\.\.\.",
                500f,
                @"Initialization - Downloading DeobfuscationMap"
            ),
            new AverageStepDuration(
                @"Checking GameAssembly\.\.\.",
                1000f,
                @"Initialization - Checking GameAssembly"
            ),
            // Cpp2IL
            // Slaynash: I skipped a lot of steps taking less than 100ms, but we may want to add them back for slower platforms
            new AverageStepDuration(
                @"Initializing metadata\.\.\.",
                2500f,
                "Cpp2IL - Initializing metadata"
            ),
            new AverageStepDuration(
                @"Searching Binary for Required Data",
                402f,
                "Cpp2IL - Seaching Binary for Required Data"
            ),
            new AverageStepDuration(
                @"Initializing Binary",
                402f,
                "Cpp2IL - Initializing Binary"
            ),
            new AverageStepDuration(
                @"Initializing Binary",
                2533f,
                "Cpp2IL - Initializing Binary"
            ),
            new AverageStepDuration(
                @"Pre-generating stubs",
                708f,
                "Cpp2IL - Building Assemblies: Pre-generating stubs"
            ),
            new AverageStepDuration(
                @"Populating mscorlib\.dll",
                340f,
                "Cpp2IL - Building Assemblies: Populating assemblies"
            ),
            new AverageStepDuration(
                @"Populating Assembly-CSharp\.dll",
                3000f,
                "Cpp2IL - Building Assemblies: Populating assemblies"
            ),
            new AverageStepDuration(
                @"Fixing up explicit overrides",
                1000f,
                "Cpp2IL - Fixing up explicit overrides"
            ),
            new AverageStepDuration(
                @"Running Scan for Known Functions",
                300f,
                "Cpp2IL - Running Scan for Known Functions"
            ),
            new AverageStepDuration(
                @"Applying type, method, and field attributes",
                5500f,
                "Cpp2IL - Applying type, method, and field attributes"
            ),
            new AverageStepDuration(
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
            new AverageStepDuration(
                @"Reading assemblies\.\.\.",
                170f,
                "Il2CppAssemblyUnhollower - Reading assemblies"
            ),
            new AverageStepDuration(
                @"Reading system assemblies\.\.\.",
                14f,
                "Il2CppAssemblyUnhollower - Reading system assemblies"
            ),
            new AverageStepDuration(
                @"Reading unity assemblies\.\.\.",
                29f,
                "Il2CppAssemblyUnhollower - Reading unity assemblies"
            ),
            new AverageStepDuration(
                @"Creating rewrite assemblies\.\.\.",
                20f,
                "Il2CppAssemblyUnhollower - Creating rewrite assemblies"
            ),
            new AverageStepDuration(
                @"Computing renames\.\.\.",
                281f,
                "Il2CppAssemblyUnhollower - Computing renames"
            ),
            new AverageStepDuration(
                @"Creating typedefs\.\.\.",
                109f,
                "Il2CppAssemblyUnhollower - Creating typedefs"
            ),
            new AverageStepDuration(
                @"Computing struct blittability\.\.\.",
                10f,
                "Il2CppAssemblyUnhollower - Computing struct blittability"
            ),
            new AverageStepDuration(
                @"Filling typedefs\.\.\.",
                27f,
                "Il2CppAssemblyUnhollower - Filling typedefs"
            ),
            new AverageStepDuration(
                @"Filling generic constraints\.\.\.",
                6f,
                "Il2CppAssemblyUnhollower - Filling generic constraints"
            ),
            new AverageStepDuration(
                @"Creating members\.\.\.",
                2256f,
                "Il2CppAssemblyUnhollower - Creating members"
            ),
            new AverageStepDuration(
                @"Scanning method cross-references\.\.\.",
                1919f,
                "Il2CppAssemblyUnhollower - Scanning method cross-references"
            ),
            new AverageStepDuration(
                @"Finalizing method declarations\.\.\.",
                2867f,
                "Il2CppAssemblyUnhollower - Finalizing method declarations"
            ),
            new AverageStepDuration(
                @"Filling method parameters\.\.\.",
                510f,
                "Il2CppAssemblyUnhollower - Filling method parameters"
            ),
            new AverageStepDuration(
                @"Creating static constructors\.\.\.",
                1237f,
                "Il2CppAssemblyUnhollower - Creating static constructors"
            ),
            new AverageStepDuration(
                @"Creating value type fields\.\.\.",
                186f,
                "Il2CppAssemblyUnhollower - Creating value type fields"
            ),
            new AverageStepDuration(
                @"Creating enums\.\.\.",
                69f,
                "Il2CppAssemblyUnhollower - Creating enums"
            ),
            new AverageStepDuration(
                @"Creating IntPtr constructors\.\.\.",
                63f,
                "Il2CppAssemblyUnhollower - Creating IntPtr constructors"
            ),
            new AverageStepDuration(
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
            new AverageStepDuration(
                @"Creating field accessors\.\.\.",
                1642f,
                "Il2CppAssemblyUnhollower - Creating field accessors"
            ),
            new AverageStepDuration(
                @"Filling methods\.\.\.",
                2385f,
                "Il2CppAssemblyUnhollower - Filling methods"
            ),
            new AverageStepDuration(
                @"Generating implicit conversions\.\.\.",
                121f,
                "Il2CppAssemblyUnhollower - Generating implicit conversions"
            ),
            new AverageStepDuration(
                @"Creating properties\.\.\.",
                102f,
                "Il2CppAssemblyUnhollower - Creating properties"
            ),
            new AverageStepDuration(
                @"Unstripping types\.\.\.",
                44f,
                "Il2CppAssemblyUnhollower - Unstripping types"
            ),
            new AverageStepDuration(
                @"Unstripping fields\.\.\.",
                5f,
                "Il2CppAssemblyUnhollower - Unstripping fields"
            ),
            new AverageStepDuration(
                @"Unstripping methods\.\.\.",
                241f,
                "Il2CppAssemblyUnhollower - Unstripping methods"
            ),
            new AverageStepDuration(
                @"Unstripping method bodies\.\.\.",
                266f,
                "Il2CppAssemblyUnhollower - Unstripping method bodies"
            ),
            new AverageStepDuration(
                @"Generating forwarded types\.\.\.",
                4f,
                "Il2CppAssemblyUnhollower - Generating forwarded types"
            ),
            new AverageStepDuration(
                @"Writing xref cache\.\.\.",
                1179f,
                "Il2CppAssemblyUnhollower - Writing xref cache"
            ),
            new AverageStepDuration(
                @"Writing assemblies\.\.\.",
                2586f,
                "Il2CppAssemblyUnhollower - Writing assemblies"
            ),
            new AverageStepDuration(
                @"Writing method pointer map\.\.\.",
                89f,
                "Il2CppAssemblyUnhollower - Writing method pointer map"
            ),
            // Move files
            new AverageStepDuration(
                @"Deleting .*\.dll",
                500f,
                "Il2CppAssemblyUnhollower - Moving files"
            ),
        };
    }
}
