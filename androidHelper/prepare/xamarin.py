import os
from helper import common
import wrapper.apktool
import shutil
import xml.etree.ElementTree as ET
import collections
from variants import paths


def clean():
    if os.path.exists(paths.Paths.xamarin_apk_dest):
        os.remove(paths.Paths.xamarin_apk_dest)


def disassemble_apk():
    if not wrapper.apktool.check_hash(paths.Paths.xamarin_apk_dest, paths.Paths.xamarin_apk_path):
        print("%s hash changed" % paths.Paths.xamarin_apk_path)
        if not wrapper.apktool.decompile(paths.Paths.xamarin_apk_path, paths.Paths.xamarin_apk_dest, force=True):
            return False
        wrapper.apktool.write_hash(paths.Paths.xamarin_apk_dest, paths.Paths.xamarin_apk_path)

    return True


def install_native(path):
    if not os.path.isdir(path):
        return False

    copy_libs = [
        "libxamarin-app.so"
    ]

    source_dirs = os.listdir(os.path.join(paths.Paths.xamarin_apk_dest, "lib"))
    dest_dirs = os.listdir(os.path.join(path, "lib"))

    for dir in dest_dirs:
        if dir not in source_dirs:
            print("Failed to find native libs for ABI: %s" % dir)
            continue

        for lib_name in copy_libs:
            source = os.path.join(os.path.join(paths.Paths.xamarin_apk_dest, "lib"), dir, lib_name)
            dest = os.path.join(os.path.join(path, "lib"), dir, lib_name)

            if not os.path.isfile(source):
                print("Failed to find %s" % source)
                return False

            shutil.copyfile(source, dest)

    return True
