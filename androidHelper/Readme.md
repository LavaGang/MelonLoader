# Basics of using this installer for android
This is a basic installer for getting started in android. Its not refined but it works.

# Required tools
- Unity - needs to be the same version of the game
- JDK
- adb
- java
- python 3

Make sure to update config.json to the correct paths.

# Scripts
## full_installer.py
### WARNING: Don't run twice on the same package
usage: `py full_installer.py <package name>`

description: If you're just want it to work, run this and it will go through all the installation steps.


## install_to_apk.py
usage: `py install_to_apk.py <path to apk>`

description: Installs MelonLoader on an apk. 

## deploy.py
usage: `py deploy.py <path to apk>`

description:
Installs target apk

## sign.py
usage: `py sign.py <path to apk>`

description: Tool that helps with the signing process. Keep in mind that the key is not secure and is only used to make installation simpler.

## start.py
usage: `py start.py <package name>`

description: Starts a unity game on target device.

## update.py
usage: `py update.py <path to extracted apk>`

description: Installs MelonLoader to an extracted apk. It will then install the updated apk on the device.
If you used `full_installer.py` on a package already, you can find the extracted dir in `build\<package name>`.

## distribute.py (developer only)
usage: `py distribute.py`

description:
Simple way to create a distributable installer.
