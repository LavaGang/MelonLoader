import os
import helpers
import shutil

bootstrap_file = "libBootstrap.so"
bootstrap_build_dir = os.path.join(helpers.Settings.base_dir, "..", "Output", "Debug", "ARM64", "Bootstrap")


def install_bootstrap(path):
    lib_dir = os.path.join(path, "lib")
    dest_dirs = os.listdir(lib_dir)

    for abi in helpers.Settings.supported_abi:
        if abi not in dest_dirs:
            continue

        install_bootstrap_abi(lib_dir, abi)

    return True


# TODO: Fix for multiple ABI
def install_bootstrap_abi(path, abi):
    shutil.copyfile(os.path.join(bootstrap_build_dir, bootstrap_file), os.path.join(path, abi, bootstrap_file))
