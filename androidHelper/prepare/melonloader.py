import os
import helpers
import shutil

melonloader_file = "MelonLoader.dll"
melonloader_build_dir = os.path.join(helpers.Settings.base_dir, "..", "Output", "Debug", "Android", "MelonLoader", "net35")


def install_melonloader(path):
    asm = os.path.join(melonloader_build_dir, melonloader_file)

    if not os.path.exists(asm):
        print("Cannot find %s" % asm)
        return False

    if not os.path.isfile(asm):
        print("%s is not a file" % asm)
        return False

    shutil.copyfile(asm, os.path.join(path, "assets", "melonloader", "etc", melonloader_file))
    return True
