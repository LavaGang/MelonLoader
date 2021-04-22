import install_to_apk
import wrapper.keytool
import wrapper.adb
import helpers
import os
import sys


def full_install(id):
    if not wrapper.adb.id_exists(id):
        helpers.error("Cannot find %s" % id)

    apk_file = os.path.join(helpers.Settings.file_path, wrapper.adb.pull_apk(id))
    apk_file_modded = os.path.join(helpers.Settings.file_path, "%s-ml.apk" % helpers.file_name(apk_file))

    # install_to_apk.install_apk(apk_file, apk_file_modded)

    wrapper.keytool.keytool_generate(helpers.Settings.keystore_password())


def main():
    if len(sys.argv) != 2:
        helpers.error("usage \"py %s <package name>\"" % os.path.basename(__file__))

    full_install(sys.argv[1])


if __name__ == '__main__':
    main()
