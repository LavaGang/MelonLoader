import os
import helper.common


class Paths:
    # bootstrap
    bootstrap_build_dir = os.path.join(helper.common.Settings.bin_path, "precompiled", "lib", "arm64-v8a")
    bootstrap_file = "libBootstrap.so"

    # il2cpp assembly generator
    il2cpp_gen_asm_paths = [
        os.path.join(helper.common.Settings.bin_path, "precompiled", "runtime", "assembly_generation")
    ]
    il2cpp_gen_assemblies_path = os.path.join("assets", "melonloader", "etc", "assembly_generation", "managed")

    # injeciton
    injection_target_path = os.path.join("com", "unity3d", "player", "UnityPlayer.smali")

    # melonloader
    melonloader_file = "MelonLoader.dll"
    melonloader_build_dir = os.path.join(helper.common.Settings.bin_path, "precompiled", "runtime")
    melonloader_dest = os.path.join("assets", "melonloader", "etc")

    # mono
    mono_assemblies_path = os.path.join(helper.common.Settings.bin_path, "precompiled", "runtime", "managed", "mono")
    mono_monodroid_assemblies_path = os.path.join(helper.common.Settings.visual_studio_path(), "Common7", "IDE", "ReferenceAssemblies", "Microsoft", "Framework", "MonoAndroid", "v8.1", "Mono.Android.dll")
    mono_assemblies_target = os.path.join("assets", "melonloader", "etc", "managed")
    mono_native_assemblies_path = os.path.join(helper.common.Settings.bin_path, "precompiled", "lib")
    # key value relationship so file can be renamed when copied
    # key is orignal name
    # value is copied name
    mono_native_assemblies = {
        "libmono-native.so": "libmono-native.so",
        "libMonoPosixHelper.so": "libMonoPosixHelper.so",
        "libmonosgen-2.0.so": "libmonosgen-2.0.so",
        "libmono-android.debug.so": "libmonodroid.so",
        "libxa-internal-api.so": "libxa-internal-api.so",
        "libxamarin-debug-app-helper.so": "libxamarin-debug-app-helper.so"
    }

    # support apk
    support_dirname = 'support'
    support_apk_path = os.path.join(helper.common.Settings.bin_path, "precompiled", 'apk_extensions.apk')
    support_apk_dest = os.path.join(helper.common.Settings.file_path, support_dirname)

    # xamarin apk
    xamarin_dirname = 'xamarin'
    xamarin_apk_path = os.path.join(helper.common.Settings.base_dir, "precompiled", "xamarin_helper.apk")
    xamarin_apk_dest = os.path.join(helper.common.Settings.file_path, xamarin_dirname)

    # support module
    support_module_dir = os.path.join(helper.common.Settings.bin_path, "precompiled", "runtime", "support")
    support_module_dest = os.path.join("assets", "melonloader", "etc", "support")

    # unity
    unity_java_file_dir = os.path.join("com", "unity3d", "player")
    unity_il2cpp_base_dir = os.path.join("Editor", "Data", "PlaybackEngines", "AndroidPlayer", "Variations", "il2cpp")
    unity_native_assemblies_dir = os.path.join("Release", "Libs")
    unity_globalgamemanager = os.path.join("assets", "bin", "Data", "globalgamemanagers")
    unity_maindata = os.path.join("assets", "bin", "Data", "data.unity3d")
    unity_native_assemblies = ["libunity.so", "libmain.so"]
    unity_assemblies_dest = os.path.join("assets", "melonloader", "etc", "assembly_generation", "unity")

    # static settings
    static_settings_output_dir = os.path.join("assets")
    static_settings_filename = "static_settings"
