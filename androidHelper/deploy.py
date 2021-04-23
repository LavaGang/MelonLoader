import wrapper.adb
import sys
import helpers
import os


def deploy_apk(apk_path):
    if not os.path.isfile(apk_path):
        helpers.error("\"%s\" is not a file.")

    apk_path = os.path.realpath(apk_path)
    if not wrapper.adb.install_apk(apk_path, safe=False):
        helpers.error("Failed to install %s" % apk_path)


def main():
    if len(sys.argv) != 2:
        helpers.error("usage \"py %s <path to apk>\"" % os.path.basename(__file__))

    apk_path = sys.argv[1]
    deploy_apk(apk_path)


if __name__ == '__main__':
    main()
