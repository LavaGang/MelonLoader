import os
from helper import common
import shutil
from variants import paths


def install_melonloader(path):
    asm = os.path.join(paths.Paths.melonloader_build_dir, paths.Paths.melonloader_file)

    if not os.path.exists(asm):
        print("Cannot find %s" % asm)
        return False

    if not os.path.isfile(asm):
        print("%s is not a file" % asm)
        return False

    shutil.copyfile(asm, os.path.join(path, paths.Paths.melonloader_dest, paths.Paths.melonloader_file))
    return True
