import os
import helpers
import wrapper.apktool
import shutil
import xml.etree.ElementTree as ET

support_dirname = 'support'

support_apk_path = os.path.join(helpers.Settings.base_dir, '..', 'APKBindings', 'app', 'build', 'outputs', 'apk', 'debug', 'app-debug.apk')
support_apk_dest = os.path.join(helpers.Settings.file_path, support_dirname)


def clean():
    if os.path.exists(support_apk_dest):
        os.remove(support_apk_dest)


def disassemble_apk():
    if not wrapper.apktool.check_hash(support_apk_dest, support_apk_path):
        print("%s hash changed" % support_apk_path)
        if not wrapper.apktool.decompile(support_apk_path, support_apk_dest, force=True):
            return False
        wrapper.apktool.write_hash(support_apk_dest, support_apk_path)

    return True


def get_permissions(path):
    if not os.path.isfile(path):
        return None

    tree = ET.parse(path)
    root = tree.getroot()

    out = []

    for child in root.findall("uses-permission"):
        likely_key = None

        for key in child.attrib:
            if not key.endswith("name"):
                continue

            likely_key = key
            break

        out.append((likely_key, child.attrib.get(likely_key)))

    return out


def add_permissions(path, permissions):
    if not os.path.isfile(path):
        return False

    tree = ET.parse(path)
    root = tree.getroot()

    for (key, perm) in permissions:
        sub = ET.SubElement(root, "uses-permission")
        sub.attrib[key] = perm

    with open(path, "wb") as f:
        f.write(ET.tostring(root))

    return True


def copy_sub_path_m(dest_dir, mutual_path):
    copy_sub_path(dest_dir, mutual_path, mutual_path)


def copy_sub_path(dest_dir, source, dest):
    source = os.path.join(support_apk_dest, source)
    dest = os.path.join(dest_dir, dest)

    if not os.path.isdir(source):
        print("Error: cannot copy")
        return

    shutil.copytree(source, dest, dirs_exist_ok=True)


def install_java(path):
    if not os.path.isdir(path):
        return False

    copy_sub_path_m(path, "smali")
    copy_sub_path(path, "smali_classes2", "smali")

    return True


def install_native(path):
    if not os.path.isdir(path):
        return False

    copy_paths = []

    source_dirs = os.listdir(os.path.join(support_apk_dest, "lib"))
    dest_dirs = os.listdir(os.path.join(path, "lib"))

    for dir in dest_dirs:
        if dir not in source_dirs:
            print("Failed to find native libs for ABI: %s" % dir)
            continue

        copy_sub_path_m(path, os.path.join("lib", dir))

    return True


def install_assets(path):
    if not os.path.isdir(os.path.join(support_apk_dest, "assets")):
        return True

    helpers.prepare_dir(os.path.join(path, "assets"))

    copy_sub_path_m(path, "assets")

    return True


def install_permissions(path):
    if not os.path.isdir(path):
        return False

    dest_perms_file = os.path.join(path, "AndroidManifest.xml")

    source_perms = get_permissions(os.path.join(support_apk_dest, "AndroidManifest.xml"))
    dest_perms = get_permissions(dest_perms_file)

    if source_perms is None or dest_perms is None:
        return False

    missing_perms = []

    mapped_dest = map((lambda el : el[1]), dest_perms)

    for (key, perm) in source_perms:
        if perm in mapped_dest:
            continue

        missing_perms.append((key, perm))

    return add_permissions(dest_perms_file, missing_perms)

