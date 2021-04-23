import os
import helpers
import shutil

unity_resources_file = os.path.join("assets", "bin", "Data", "unity default resources")
il2cpp_base_dir = os.path.join("Editor", "Data", "PlaybackEngines", "AndroidPlayer", "Variations", "il2cpp")
native_assemblies_dir = os.path.join("Release", "Libs")

native_assemblies = ["libunity.so", "libmain.so"]


def get_unity_version(path):
    res_file = os.path.join(path, unity_resources_file)
    if not os.path.isfile(res_file):
        return None

    blank_count = 0
    version = b""
    with open(res_file, "rb") as file:
        file.seek(0x0)

        limit = 128
        i = 0
        while i < limit:
            i += 1
            char = file.read(1)

            if int(char[0]) == 0x0:
                blank_count += 1

                if len(version) > 0:
                    break

            if blank_count < 4:
                continue

            if int(char[0]) == 0x35 or int(char[0]) == 0x32 or len(version) > 0:
                version += char
            elif int(char[0]) != 0x0:
                blank_count = 0

    if i >= limit:
        return None

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