from helper import common
import subprocess

keytool_path = common.Settings.keytool_path()
default_options = {
    "cn": "",
    "ou": "",
    "o": "",
    "c": ""
}


def keytool_run(*args):
    return subprocess.run([keytool_path] + list(args)) == 0


def keytool_generate(label, password, output, dname=None, validity=10000):
    if dname is None:
        dname = default_options

    return keytool_run(
        "-genkey",
        "-keystore",
        output,
        "-alias",
        label,
        "-storepass",
        password,
        "-keypass",
        password,
        "-keyalg",
        "RSA",
        "-keysize",
        "2048",
        "-validity",
        validity,
        "-dname",
        "cn=%s, ou=%s, o=%s, c=%s" % (dname["cn"], dname["ou"], dname["o"], dname["c"])
    ) == 0
