import os
from helper import common
import shutil
from variants import paths


def install_support_modules(path):
    if not os.path.isdir(paths.Paths.support_module_dir):
        print("Cant find supports modules dir %s" % paths.Paths.support_module_dir)
        return False

    assemblies_path = os.path.join(path, paths.Paths.support_module_dest)
    common.prepare_dir(assemblies_path)
    for path in os.listdir(paths.Paths.support_module_dir):
        if not os.path.isfile(os.path.join(paths.Paths.support_module_dir, path)):
            continue

        if not path.endswith(".dll"):
            continue

        shutil.copyfile(os.path.join(paths.Paths.support_module_dir, path), os.path.join(assemblies_path, path))

    return True
