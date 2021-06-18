import urllib.request
import os
import helper.common
import shutil

default_save_map = [
    (os.path.join(helper.common.Settings.bin_path, "apktool.jar"), "https://bitbucket.org/iBotPeaches/apktool/downloads/apktool_2.5.0.jar")
]


def get_tools(save_map=default_save_map):
    for save, url in save_map:
        if os.path.isfile(save):
            continue

        if not os.path.isdir(os.path.dirname(save)):
            os.makedirs(os.path.dirname(save))

        if os.path.isfile(url):
            print("Copying %s" % os.path.basename(url))
            shutil.copyfile(url, save)
        else:
            print("Downloading %s" % os.path.basename(save))
            with urllib.request.urlopen(url) as r:
                with open(save, "wb") as f:
                    f.write(r.read())

