import os
import helpers
import shutil
import mmap


unity_exception_label = b".method public final declared-synchronized uncaughtException"
unity_class_file_prefixs = "abcdefghijklmnopqrstuvwxyz"
unity_java_file_dir = os.path.join("com", "unity3d", "player")
smali_directories = ["smali", "smali_assets", "smali_classes2"]
il2cpp_base_dir = os.path.join("Editor", "Data", "PlaybackEngines", "AndroidPlayer", "Variations", "il2cpp")
native_assemblies_dir = os.path.join("Release", "Libs")

native_assemblies = ["libunity.so", "libmain.so"]


class Cache:
    unity_exception_file = None


def is_unity_exception_file(smali_file):
    with open(smali_file, "rb") as file:
        with mmap.mmap(file.fileno(), 0, access=mmap.ACCESS_READ) as s:
            return s.find(unity_exception_label) != -1


def get_exception_file(path):
    if Cache.unity_exception_file is not None:
        return Cache.unity_exception_file

    for smali_dir in smali_directories:
        unity_player_base_path = os.path.join(path, smali_dir, unity_java_file_dir)

        if not os.path.isdir(unity_player_base_path):
            continue

        for letter in list(unity_class_file_prefixs):
            smali_file = os.path.join(unity_player_base_path, letter + ".smali")

            if not os.path.isfile(smali_file):
                continue

            if is_unity_exception_file(smali_file):
                Cache.unity_exception_file = smali_file
                return smali_file

    return None


def find_line_endings(s):
    line_endings = b"\n"

    s.seek(0)

    if s.find(b"\r\n") >= 0:
        line_endings = b"\r\n"

    return line_endings


def get_unity_version(path):
    res_file = get_exception_file(path)
    if not os.path.isfile(res_file):
        return None

    version_prefix = b"Unity version"

    with open(res_file, "rb") as file:
        with mmap.mmap(file.fileno(), 0, access=mmap.ACCESS_READ) as s:
            s.seek(0)
            line_endings = find_line_endings(s)

            found_pos = s.find(version_prefix)
            if found_pos == -1:
                return None
            s.seek(found_pos)

            found_pos = s.find(b"const-string")
            if found_pos == -1:
                return None
            s.seek(found_pos)

            start_pos = s.find(b"\"")
            if start_pos == -1:
                return None
            start_pos += 1

            s.seek(start_pos)

            end_pos = s.find(b"\"")
            if end_pos == -1:
                return None

        file.seek(start_pos)
        version = file.read(end_pos - start_pos)

    return version.decode('utf-8')


def check_editor(path):
    version = get_unity_version(path)
    return os.path.isdir(os.path.join(helpers.Settings.unity_editor_path(), version))


def install_unity_assemblies(path):
    version = get_unity_version(path)

    if not check_editor(path):
        if version is not None:
            print("Failed to find unity editor %s" % version)
        else:
            print("Failed to detect unity version for %s" % path)
        return False

    managed_dir = os.path.join(helpers.Settings.unity_editor_path(), get_unity_version(path), il2cpp_base_dir, "Managed")
    if not os.path.isdir(managed_dir):
        print("Cannot find unity managed dir %s" % managed_dir)
        return False

    assemblies_path = os.path.join(path, "assets", "melonloader", "etc", "assembly_generation", "unity")
    helpers.prepare_dir(assemblies_path)
    for path in os.listdir(managed_dir):
        if not os.path.isfile(os.path.join(managed_dir, path)):
            continue

        if not path.endswith(".dll"):
            continue

        shutil.copyfile(os.path.join(managed_dir, path), os.path.join(assemblies_path, path))

    return True


def install_native_original_unity_assemblies(path):
    version = get_unity_version(path)

    if not check_editor(path):
        if version is not None:
            print("Failed to find unity editor %s" % version)
        else:
            print("Failed to detect unity version for %s" % path)
        return False

    libs_dir = os.path.join(helpers.Settings.unity_editor_path(), get_unity_version(path), il2cpp_base_dir, native_assemblies_dir)
    dest_dir = os.path.join(path, "lib")

    libs_dir_sub = os.listdir(libs_dir)
    dest_dir_sub = os.listdir(dest_dir)

    abi_count = 0

    for abi in helpers.Settings.supported_abi:
        if abi not in dest_dir_sub:
            continue

        if abi not in libs_dir_sub:
            print("WARNING: missing ABI %s in unity" % abi)
            continue

        abi_dir = os.path.join(libs_dir, abi)
        if not os.path.isdir(abi_dir):
            print("WARNING: %s exists but is not a folder" % abi_dir)

        abi_count += 1
        install_unity_abi(libs_dir, dest_dir, abi)

    return abi_count > 0


def install_unity_abi(src, path, abi):
    unity_abi_dir = os.path.join(src, abi)
    dest_abi_dir = os.path.join(path, abi)

    for value in native_assemblies:
        shutil.copyfile(os.path.join(unity_abi_dir, value), os.path.join(dest_abi_dir, value))