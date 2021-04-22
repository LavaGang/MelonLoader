import os
import helpers
from . import java

apksigner_path = helpers.Settings.apksigner_path()


def apksigner_run(*cmd):
    return java.java_run(apksigner_path, *cmd).returncode == 0


def sign_apk(path, keystore, keypass):
    if not os.path.isfile(path):
        print("%s is not a file" % path)
        return False

    print("Signing", path)

    res = apksigner_run("sign", "--v1-signing-enabled", "true", "--v2-signing-enabled", "true", "--ks", keystore, "--ks-pass", "pass:%s" % keypass, path)
    if res:
        print("Signed", path)

    return res
