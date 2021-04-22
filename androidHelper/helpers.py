import os
import hashlib

base_dir = os.path.dirname(os.path.realpath(__file__))
file_path = os.path.join(base_dir, "build")
bin_path = os.path.join(base_dir, "bin")

supported_abi = ["arm64-v8a"]


def error(message):
    print("Error: %s" % message)
    exit(1)


def file_name(path):
    return os.path.splitext(os.path.basename(path))[0]


def prepare_dir(path):
    if os.path.isdir(path):
        return

    if os.path.exists(path):
        error("%s exists but is not folder" % path)

    os.makedirs(name=path, exist_ok=True)


def hash_file(path):
    if not os.path.isfile(path):
        return None

    h = hashlib.sha1()

    with open(path, 'rb') as file:
        chunk = 0
        while chunk != b'':
            chunk = file.read(1024)
            h.update(chunk)

    # return the hex representation of digest
    return h.digest()


def check_abi_support(path):
    dest_dirs = os.listdir(os.path.join(path, "lib"))

    support_count = 0

    for abi in dest_dirs:
        if abi in supported_abi:
            support_count += 1
            continue

        print("WARNING: unsupported abi %s" % abi)

    return support_count > 0
