import os
import helpers
import java

apktool_path = os.path.join(helpers.Settings.bin_path, 'apktool.jar')


def write_hash(path, compressed_path):
    hash_path = os.path.join(path, "hash")

    if os.path.exists(hash_path) and not os.path.isfile(path):
        return False

    with open(hash_path, "wb") as file:
        file.write(helpers.hash_file(compressed_path))

    return True


def check_hash(path, compressed_path):
    hash_path = os.path.join(path, "hash")

    if not os.path.exists(hash_path):
        return False

    if not os.path.isfile(hash_path):
        helpers.error("%s is expected to be a hash file")

    with open(hash_path, "rb") as file:
        contents = file.read()

    return contents == helpers.hash_file(compressed_path)


def apktool_run(*command):
    return java.java_run(apktool_path, *command) == 0


def decompile(path, output, force=False):
    command = "d \"%s\" -o \"%s\"" % (path, output)

    if force:
        command = command + " -f"

    return apktool_run(command)


def build(path, output):
    return apktool_run("b", path, "-o", output)
