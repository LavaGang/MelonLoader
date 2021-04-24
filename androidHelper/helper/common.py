import os
import hashlib
import json


class Settings:
    _config_read = False

    _visual_studio_path = ""
    _unity_editor_path = ""
    _unity_unstripped = False
    _keytool_path = ""
    _keystore_password = ""
    _apksigner_path = ""

    base_dir = os.path.dirname(os.path.realpath(os.path.join(__file__, "..")))
    file_path = os.path.join(base_dir, "build")
    bin_path = os.path.join(base_dir, "bin")
    config_path = os.path.join(base_dir, "config.json")
    supported_abi = ["arm64-v8a"]

    @staticmethod
    def visual_studio_path():
        Settings.load_config()
        return Settings._visual_studio_path

    @staticmethod
    def unity_editor_path():
        Settings.load_config()
        return Settings._unity_editor_path

    @staticmethod
    def unity_unstripped():
        Settings.load_config()
        return Settings._unity_unstripped

    @staticmethod
    def keytool_path():
        Settings.load_config()
        return Settings._keytool_path

    @staticmethod
    def keystore_password():
        Settings.load_config()
        return Settings._keystore_password

    @staticmethod
    def apksigner_path():
        Settings.load_config()
        return Settings._apksigner_path

    @staticmethod
    def load_config():
        if Settings._config_read:
            return

        with open(Settings.config_path) as f:
            data = json.load(f)

        Settings._visual_studio_path = os.path.realpath(data['VisualStudioBase'])
        Settings._unity_editor_path = os.path.realpath(data['UnityEditorBase'])
        Settings._unity_unstripped = data['UnityUnstripped']
        Settings._keytool_path = data['KeytoolPath']
        Settings._keystore_password = data['KeystorePassword']
        Settings._apksigner_path = data['ApkSignerPath']

        Settings._config_read = True


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
        if abi in Settings.supported_abi:
            support_count += 1
            continue

        print("WARNING: unsupported abi %s" % abi)

    return support_count > 0
