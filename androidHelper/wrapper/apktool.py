import os
import helpers

apktool_path = os.path.join(helpers.bin_path, 'apktool.jar')


def apktool_run(command):
    print(command)
    return os.system("java -jar -Duser.language=en -Dfile.encoding=UTF8 \"%s\" %s" % (apktool_path, command)) == 0


def decompile(path, output, force=False):
    command = "d \"%s\" -o \"%s\"" % (path, output)

    if force:
        command = command + " -f"

    print(command)
    return apktool_run(command)


def build(path, output):
    return apktool_run("b \"%s\" -o \"%s\"" % (path, output))

