import os
from helper import common
import shutil

support_module_dir = os.path.join(common.Settings.base_dir, "..", "Output", "Debug", "MelonLoader", "Dependencies", "SupportModules")


def install_support_modules(path):
    if not os.path.isdir(support_module_dir):
        print("Cant find supports modules dir %s" % support_module_dir)
        return False

    assemblies_path = os.path.join(path, "assets", "melonloader", "etc", "support")
    common.prepare_dir(assemblies_path)
    for path in os.listdir(support_module_dir):
        if not os.path.isfile(os.path.join(support_module_dir, path)):
            continue

        if not path.endswith(".dll"):
            continue

        shutil.copyfile(os.path.join(support_module_dir, path), os.path.join(assemblies_path, path))

    return True
