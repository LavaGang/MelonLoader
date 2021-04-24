import install_to_apk
import sign
import deploy
import wrapper.keytool
import wrapper.adb
from helper import common
import os
import sys


def full_install(id):
    if not wrapper.adb.id_exists(id):
        common.error("Cannot find %s" % id)

    apk_file = os.path.join(common.Settings.file_path, wrapper.adb.pull_apk(id))
    apk_file_modded = os.path.join(common.Settings.file_path, "%s-ml.apk" % common.file_name(apk_file))

    install_to_apk.install_apk(apk_file, apk_file_modded)

    sign.sign(apk_file_modded)
    deploy.deploy_apk(apk_file_modded)


def main():
    if len(sys.argv) != 2:
        common.error("usage \"py %s <package name>\"" % os.path.basename(__file__))

    full_install(sys.argv[1])


if __name__ == '__main__':
    main()
