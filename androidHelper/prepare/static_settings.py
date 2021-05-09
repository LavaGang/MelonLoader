import os
from helper import common
import shutil
from variants import paths
import struct


# settings struct def
#
# boolean - Safe Mode
#
class Settings:
    safe_mode = False

    def serialize(self):
        return struct.pack("?", self.safe_mode)


def generate_settings():
    settings = Settings()

    settings.safe_mode = common.Settings.safe_mode()

    return settings


def install_settings(path):
    output = os.path.join(path, paths.Paths.static_settings_output_dir, paths.Paths.static_settings_filename)

    if not os.path.isdir(os.path.dirname(output)):
        os.makedirs(os.path.dirname(output))

    settings = generate_settings().serialize()

    with open(output, "wb") as f:
        f.write(settings)

    return True
