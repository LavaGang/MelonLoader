import sys
import os
import helpers
import wrapper.keytool
import wrapper.apksigner

keystore_path = os.path.join(helpers.Settings.file_path, "sign.jks")


def sign(apk_path):
    if not os.path.isfile(apk_path):
        helpers.error("\"%s\" is not a file.")

    apk_path = os.path.realpath(apk_path)

    if not os.path.isfile(keystore_path) and not wrapper.keytool.keytool_generate(
            "static_sign", helpers.Settings.keystore_password(), keystore_path):
        helpers.error("Failed to generate keystore")

    if not wrapper.apksigner.sign_apk(apk_path, keystore_path, helpers.Settings.keystore_password()):
        helpers.error("Failed to sign %s" % apk_path)


def main():
    if len(sys.argv) != 2:
        helpers.error("usage \"py %s <path to apk>\"" % os.path.basename(__file__))

    apk_path = sys.argv[1]
    sign(apk_path)


if __name__ == '__main__':
    main()
