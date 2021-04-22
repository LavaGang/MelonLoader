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

import helpers
import wrapper.apktool

from helpers import error as error


def validate_path(path):
    return os.path.isfile(path)


def clean(path):
    os.remove(path)


def install_apk(apk_path):
    output_path = os.path.join(helpers.Settings.file_path, helpers.file_name(apk_path))

    if helpers.file_name(apk_path) == prepare.support.support_dirname:
        error("apk cannot be named %s.apk" % (helpers.file_name(apk_path)))

    helpers.prepare_dir(helpers.Settings.file_path)

    if not wrapper.apktool.check_hash(output_path, apk_path):
        print("%s hash changed" % apk_path)
        if not wrapper.apktool.decompile(apk_path, output_path, force=True):
            error("Failed to disassemble.")
        wrapper.apktool.write_hash(output_path, apk_path)

    if not helpers.check_abi_support(output_path):
        error("No supported ABIs found. Supported ABIs are [%s]." % ", ".join(helpers.Settings.supported_abi))

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
        error("Failed to install %s" % prepare.bootstrap.bootstrap_file)

    if not prepare.mono.install_mono(output_path):
        error("Failed to install mono assemblies")

    if not prepare.mono.install_mono_native(output_path):
        error("Failed to install native mono assemblies")

    if not prepare.unity.install_unity_assemblies(output_path):
        error("Failed to install unity assemblies")

    if helpers.Settings.unity_unstripped() and not prepare.unity.install_native_original_unity_assemblies(output_path):
        error("Failed to install unity unstripped native assemblies")

    if not prepare.melonloader.install_melonloader(output_path):
        error("Failed to install melonloader assembly")

    if not prepare.support_module.install_support_modules(output_path):
        error("Failed to install support modules")

    if not prepare.il2cpp_assembly_generation.install_il2cpp_gen(output_path):
        error("Failed to install il2cpp assembly generator")


def main():
    if len(sys.argv) != 2:
        error("usage \"py install_to_apk.py <path to apk>\"")

    apk_path = sys.argv[1]
    if not validate_path(apk_path):
        error("\"%s\" is not a file.")

    install_apk(os.path.realpath(apk_path))


if __name__ == '__main__':
    main()
