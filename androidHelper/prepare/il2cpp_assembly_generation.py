import os
from helper import common
import shutil
from variants import paths


def install_il2cpp_gen(path):
    assemblies_path = os.path.join(path, paths.Paths.il2cpp_gen_assemblies_path)
    common.prepare_dir(assemblies_path)

    for path in paths.Paths.il2cpp_gen_asm_paths:
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
