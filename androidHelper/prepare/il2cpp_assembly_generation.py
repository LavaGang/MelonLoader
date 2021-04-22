import os
import helpers
import shutil

asm_gen_paths = [
    os.path.join(helpers.Settings.base_dir, "..", "Output", "Debug", "AnyCPU", "MelonLoader", "Dependencies", "Il2CppAssemblyGenerator", "net4.7.2"),
    os.path.join(helpers.Settings.base_dir, "..", "external", "Il2CppAssemblyUnhollower", "AssemblyUnhollower", "bin", "Debug", "net4.7.2", "AssemblyUnhollower.dll"),
    os.path.join(helpers.Settings.base_dir, "..", "external", "Il2CppAssemblyUnhollower", "AssemblyUnhollower", "bin", "Debug", "net4.7.2", "Iced.dll"), # TODO: Remove
    os.path.join(helpers.Settings.base_dir, "..", "external", "Il2CppAssemblyUnhollower", "UnhollowerBaseLib", "bin", "Debug", "net4.7.2", "UnhollowerBaseLib.dll"),
    os.path.join(helpers.Settings.base_dir, "..", "external", "Il2CppAssemblyUnhollower", "UnhollowerRuntimeLib", "bin", "Debug", "net4.7.2", "UnhollowerRuntimeLib.dll")
]


def install_il2cpp_gen(path):
    assemblies_path = os.path.join(path, "assets", "melonloader", "etc", "assembly_generation", "managed")
    helpers.prepare_dir(assemblies_path)

    for path in asm_gen_paths:
        if os.path.isdir(path):
            install_asm_dir(assemblies_path, path)
        elif os.path.isfile(path):
            shutil.copyfile(path, os.path.join(assemblies_path, os.path.basename(path)))
        else:
            print("Cannot find file %s" % path)
            return False
    return True


def install_asm_dir(base_path, dir):
    for path in os.listdir(dir):
        if not os.path.isfile(os.path.join(dir, path)):
            continue

        if not path.endswith(".dll"):
            continue

        shutil.copyfile(os.path.join(dir, path), os.path.join(base_path, path))
