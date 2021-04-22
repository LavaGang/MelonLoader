import os
import helpers


def adb_run(cmd):
    return os.system("adb %s" % cmd)


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
    adb_run("pull \"%s\" \"%s\"" % (path, os.path.join(helpers.Settings.file_path, apk_name)))

    return apk_name
