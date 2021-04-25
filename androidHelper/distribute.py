import helper.get_tools
import os
import helper.common
import json
import glob
import shutil
import zipfile
from helper import common

import prepare.support
import prepare.injection
import prepare.bootstrap
import prepare.mono
import prepare.unity
import prepare.melonloader
import prepare.support_module
import prepare.il2cpp_assembly_generation

from variants import paths

save_map = helper.get_tools.default_save_map + [
    (os.path.join(helper.common.Settings.bin_path, os.path.basename(helper.common.Settings.apksigner_path())),
     helper.common.Settings.apksigner_path(), "ApkSignerPath"),
]

output_dir = os.path.join(helper.common.Settings.file_path, "dist")
glob_pattern = [
    "distribute.py"
]

assemblies_base_dir = os.path.join(output_dir, "bin", "precompiled")


def zip_content():
    shutil.make_archive(os.path.join(os.getcwd(), "melonloader-android"), 'zip', root_dir=output_dir, base_dir=".")


def copy_binaries():
    java_dir = os.path.join(assemblies_base_dir, "apk_extentions")

    if not os.path.isdir(assemblies_base_dir):
        os.makedirs(assemblies_base_dir)

    if not os.path.isdir(java_dir):
        os.makedirs(java_dir)

    for abi in common.Settings.supported_abi:
        path = os.path.join(assemblies_base_dir, "lib", abi)
        if not os.path.isdir(path):
            os.makedirs(path)

        path = os.path.join(java_dir, "lib", abi)
        if not os.path.isdir(path):
            os.makedirs(path)

    paths.Paths.il2cpp_gen_assemblies_path = os.path.join("runtime", "assembly_generation")
    paths.Paths.melonloader_dest = os.path.join("runtime")
    paths.Paths.mono_assemblies_target = os.path.join("runtime", "managed", "mono")
    paths.Paths.support_module_dest = os.path.join("runtime", "support")

    shutil.copyfile(paths.Paths.support_apk_path, os.path.join(assemblies_base_dir, os.path.basename(paths.Paths.support_apk_path)))
    shutil.move(os.path.join(assemblies_base_dir, os.path.basename(paths.Paths.support_apk_path)), os.path.join(assemblies_base_dir, "apk_extensions.apk"))

    if not prepare.bootstrap.install_bootstrap(assemblies_base_dir):
        common.error("Failed to install %s" % paths.Paths.bootstrap_file)

    if not prepare.mono.install_mono(assemblies_base_dir):
        common.error("Failed to install mono assemblies")

    if not prepare.mono.install_mono_native(assemblies_base_dir):
        common.error("Failed to install native mono assemblies")

    if not prepare.melonloader.install_melonloader(assemblies_base_dir):
        common.error("Failed to install melonloader assembly")

    if not prepare.support_module.install_support_modules(assemblies_base_dir):
        common.error("Failed to install support modules")

    if not prepare.il2cpp_assembly_generation.install_il2cpp_gen(assemblies_base_dir):
        common.error("Failed to install il2cpp assembly generator")


def main():
    shutil.rmtree(output_dir)

    # get all tools
    m_save_map = []
    for el in save_map:
        m_save_map.append((os.path.join(output_dir, os.path.relpath(el[0]).lstrip(helper.common.Settings.base_dir)), el[1]))

    helper.get_tools.get_tools(m_save_map)

    save_config_name = os.path.join(
        output_dir,
        os.path.realpath(os.path.dirname(helper.common.Settings.config_path)).lstrip(helper.common.Settings.base_dir),
        os.path.basename(helper.common.Settings.config_path)
    )

    # update json
    with open(helper.common.Settings.config_path, "r") as f:
        config = json.load(f)

    for el in save_map:
        if len(el) < 3:
            continue

        save, _, update = el
        if update not in config:
            continue

        config[update] = os.path.relpath(os.path.realpath(save))

    with open(save_config_name, "w") as f:
        json.dump(config, f, indent=4)

    # copy python files
    found_files = glob.glob("**/*.py") + glob.glob("*.py") + glob.glob("variants/release/*.py")
    for file in found_files:
        if not os.path.isdir(os.path.dirname(os.path.join(output_dir, file))):
            os.makedirs(os.path.dirname(os.path.join(output_dir, file)))
        shutil.copyfile(os.path.join(helper.common.Settings.base_dir, file), os.path.join(output_dir, file))

    for pattern in glob_pattern:
        rem = glob.glob(pattern)
        for file in rem:
            os.remove(os.path.join(output_dir, file))

    if os.path.isdir(os.path.join(output_dir, "dist")):
        shutil.rmtree(os.path.join(output_dir, "dist"))

    # fix variants
    with open(os.path.join(output_dir, "variants", "__init__.py"), "r") as f:
        buffer = f.read()

    buffer = buffer.replace(" .dev ", " .release ")

    with open(os.path.join(output_dir, "variants", "__init__.py"), "w") as f:
        f.write(buffer)

    copy_binaries()
    shutil.copyfile(os.path.join(common.Settings.base_dir, "Readme.md"), os.path.join(output_dir, "Readme.md"))

    print("Zipping up")
    zip_content()


if __name__ == '__main__':
    main()
