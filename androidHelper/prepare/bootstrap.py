import os
from helper import common
import shutil
from variants import paths


def install_bootstrap(path):
    lib_dir = os.path.join(path, "lib")
    dest_dirs = os.listdir(lib_dir)

    for abi in common.Settings.supported_abi:
        if abi not in dest_dirs:
            continue

        install_bootstrap_abi(lib_dir, abi)

    return True


# TODO: Fix for multiple ABI
def install_bootstrap_abi(path, abi):
    shutil.copyfile(os.path.join(paths.Paths.bootstrap_build_dir, paths.Paths.bootstrap_file), os.path.join(path, abi, paths.Paths.bootstrap_file))
