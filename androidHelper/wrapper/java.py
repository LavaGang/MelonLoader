import os
import subprocess


def java_run(*command):
    return subprocess.run(["java", "-jar", "-Duser.language=en", "-Dfile.encoding=UTF8"] + list(command))
