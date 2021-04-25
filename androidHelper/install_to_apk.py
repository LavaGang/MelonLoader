import os
import sys

import prepare.support
import prepare.injection
import prepare.bootstrap
import prepare.mono
import prepare.unity
import prepare.melonloader
import prepare.support_module
import prepare.il2cpp_assembly_generation
from variants import paths

from helper import common
import wrapper.apktool

from helper.common import error as error


def validate_path(path):
    return os.path.isfile(path)


def clean(path):
    os.remove(path)


def install_apk(apk_path, build_output_path):
    if not validate_path(apk_path):
        error("\"%s\" is not a file.")

    output_path = os.path.join(common.Settings.file_path, common.file_name(apk_path))

    if common.file_name(apk_path) == paths.Paths.support_dirname:
        error("apk cannot be named %s.apk" % (common.file_name(apk_path)))

    common.prepare_dir(common.Settings.file_path)

    if not wrapper.apktool.check_hash(output_path, apk_path):
        print("%s hash changed" % apk_path)
        if not wrapper.apktool.decompile(apk_path, output_path, force=True):
            error("Failed to disassemble.")
        wrapper.apktool.write_hash(output_path, apk_path)

    if not common.check_abi_support(output_path):
        error("No supported ABIs found. Supported ABIs are [%s]." % ", ".join(common.Settings.supported_abi))

    if not prepare.support.disassemble_apk():
        error("Failed to disassemble support apk.")

    if not prepare.support.install_java(output_path):
        error("Failed to install java code from support code.")

    if not prepare.support.install_native(output_path):
        error("Failed to install native code from support code.")

    if not prepare.support.install_permissions(output_path):
        error("Failed to install permissions")

    if not prepare.support.install_assets(output_path):
        error("Failed to install assets")

    if not prepare.injection.install_injection(output_path):
        error("Failed to inject into java")

    if not prepare.bootstrap.install_bootstrap(output_path):
        error("Failed to install %s" % paths.Paths.bootstrap_file)

    if not prepare.mono.install_mono(output_path):
        error("Failed to install mono assemblies")

    if not prepare.mono.install_mono_native(output_path):
        error("Failed to install native mono assemblies")

    if not prepare.unity.install_unity_assemblies(output_path):
        error("Failed to install unity assemblies")

    if common.Settings.unity_unstripped() and not prepare.unity.install_native_original_unity_assemblies(output_path):
        print("WARNING: Failed to install unstripped unity assemblies. Some engine code may not be available.")

    if not prepare.melonloader.install_melonloader(output_path):
        error("Failed to install melonloader assembly")

    if not prepare.support_module.install_support_modules(output_path):
        error("Failed to install support modules")

    if not prepare.il2cpp_assembly_generation.install_il2cpp_gen(output_path):
        error("Failed to install il2cpp assembly generator")

    if not wrapper.apktool.build(output_path, build_output_path):
        error("Failed to build.")


def main():
    if len(sys.argv) != 2:
        common.error("usage \"py %s <path to apk>\"" % os.path.basename(__file__))

    apk_path = sys.argv[1]
    if not validate_path(apk_path):
        error("\"%s\" is not a file.")

    install_apk(os.path.realpath(apk_path), os.path.join(os.getcwd(), common.file_name(apk_path) + "-ml.apk"))


if __name__ == '__main__':
    main()
