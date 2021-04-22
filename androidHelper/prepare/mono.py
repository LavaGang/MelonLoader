import os
import helpers
import shutil

mono_assemblies_path = os.path.join(helpers.Settings.visual_studio_path(), "Common7", "IDE", "ReferenceAssemblies", "Microsoft", "Framework", "MonoAndroid", "v1.0")
mono_native_assemblies_path = os.path.join(helpers.Settings.visual_studio_path(), "MSBuild", "Xamarin", "Android", "lib")

# key value relationship so file can be renamed when copied
# key is orignal name
# value is copied name
mono_native_assemblies = {
    "libmono-native.so": "libmono-native.so",
    "libMonoPosixHelper.so": "libMonoPosixHelper.so",
    "libmonosgen-2.0.so": "libmonosgen-2.0.so"
}


def install_mono(path):
    if not os.path.isdir(mono_assemblies_path):
        print("Cannot find mono C# assemblies in %s" % mono_assemblies_path)
        return False

    assemblies_path = os.path.join(path, "assets", "melonloader", "etc", "managed")
    helpers.prepare_dir(assemblies_path)

    for path in os.listdir(mono_assemblies_path):
        if not os.path.isfile(os.path.join(mono_assemblies_path, path)):
            continue

        if not path.endswith(".dll"):
            continue

        shutil.copyfile(os.path.join(mono_assemblies_path, path), os.path.join(assemblies_path, path))

    return True


def install_mono_native(path):
    return True