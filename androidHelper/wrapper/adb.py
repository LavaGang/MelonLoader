import os
import helpers
import subprocess


def adb_run(*cmd, capture_output=False):
    return subprocess.run(["adb"] + list(cmd), capture_output=capture_output)


def adb_shell(cmd):
    return os.popen("adb shell \"%s\"" % cmd.replace("\"", "\\\"")).read()


def find_apk(id):
    if not id_exists(id):
        helpers.error("%s is not a package" % id)

    path = adb_shell("pm path %s" % id)
    path = path.removeprefix("package:").rstrip("\n\s")
    return path


def id_exists(id):
    res = adb_shell("pm list packages | grep \"%s\"" % id)
    res = res.removeprefix("package:").rstrip("\n\s")
    return res == id


def pull_apk(id):
    apk_name = "%s.apk" % id
    path = find_apk(id)
    adb_run("pull", path, os.path.join(helpers.Settings.file_path, apk_name))

    return apk_name


def install_apk(apk_path, safe=True):
    if not os.path.isfile(apk_path):
        print("Cannot find file %s" % apk_path)
        return False

    res = adb_run("install", "-d", "-r", "-t", apk_path, capture_output=(not safe))

    if res.returncode == 0:
        print(res.stdout.decode('utf-8'))
        return True

    if safe:
        print(res.stderr.decode('utf-8'))
        return False

    token_start = b"Failure ["
    msg = res.stderr
    loc = msg.find(token_start) + len(token_start)
    loc = msg.find(b" ", loc) + 1
    start = msg.find(b" ", loc) + 1
    end = msg.find(b" ", start)

    package = msg[start:end].decode('utf-8')

    if not id_exists(package):
        print(msg.decode("utf-8"))
        return False

    if not uninstall_apk(package):
        return False

    return adb_run("install", "-d", "-r", "-t", apk_path).returncode == 0


# TODO: add -k
def uninstall_apk(id):
    if not id_exists(id):
        print("Package %s does not exist" % id)
        return False

    return adb_run("uninstall", id).returncode == 0


def stop_apk(id):
    if not id_exists(id):
        print("Package %s does not exist" % id)
        return False

    return adb_run("shell", "am", "force-stop", id)


def start_apk(id):
    if not id_exists(id):
        print("Package %s does not exist" % id)
        return False

    return adb_run("shell", "am", "start", "-n", "%s/com.unity3d.player.UnityPlayerActivity" % id)
