import wrapper.keytool
import wrapper.adb
from helper import common
import os
import sys


def start(id):
    if not wrapper.adb.stop_apk(id):
        common.error("Failed to stop %s" % id)

    if not wrapper.adb.start_apk(id):
        common.error("Failed to start %s" % id)


def main():
    if len(sys.argv) != 2:
        common.error("usage \"py %s <package name>\"" % os.path.basename(__file__))

    start(sys.argv[1])


if __name__ == '__main__':
    main()
