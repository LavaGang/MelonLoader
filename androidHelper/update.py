import sys
import os
from helper import common
import deploy
import wrapper.keytool
import wrapper.apksigner
import wrapper.apktool
import sign
import prepare.support
import prepare.injection
import prepare.bootstrap
import prepare.mono
import prepare.unity
import prepare.melonloader
import prepare.support_module
import prepare.il2cpp_assembly_generation
import prepare.xamarin
import prepare.static_settings

from variants import paths

keystore_path = os.path.join(common.Settings.file_path, "sign.jks")


def update(output_path):
    if not os.path.isdir(output_path):
        common.error("\"%s\" is not a file.")

    output_path = os.path.realpath(output_path)
    build_output_path = output_path + "-ml.apk"

    unity_version = prepare.unity.get_unity_version(output_path)
    if unity_version is None:
        common.error("Failed to detect unity version")

    print("Detected Unity %s" % unity_version)

    if not common.check_abi_support(output_path):
        common.error("No supported ABIs found. Supported ABIs are [%s]." % ", ".join(common.Settings.supported_abi))

    if not prepare.support.disassemble_apk():
        common.error("Failed to disassemble support apk.")

    if not prepare.xamarin.disassemble_apk():
        common.error("Failed to disassemble xamarin apk.")

    if not prepare.support.install_java(output_path):
        common.error("Failed to install java code from support code.")

    if not prepare.support.install_native(output_path):
        common.error("Failed to install native code from support code.")

    if not prepare.support.install_permissions(output_path):
        common.error("Failed to install permissions")

    if not prepare.support.install_assets(output_path):
        common.error("Failed to install assets")

    if not prepare.xamarin.install_native(output_path):
        common.error("Failed to install native libs from xamarin.")

    if not prepare.injection.install_injection(output_path):
        common.error("Failed to inject into java")

    if not prepare.bootstrap.install_bootstrap(output_path):
        common.error("Failed to install %s" % paths.Paths.bootstrap_file)

    if not prepare.mono.install_mono(output_path):
        common.error("Failed to install mono assemblies")

    if not prepare.mono.install_mono_native(output_path):
        common.error("Failed to install native mono assemblies")

    if not prepare.unity.install_unity_assemblies(output_path):
        common.error("Failed to install unity assemblies")

    if not common.Settings.safe_mode() and not prepare.unity.install_native_original_unity_assemblies(output_path):
        print("WARNING: Failed to install unstripped unity assemblies. Some engine code may not be available.")

    if not prepare.melonloader.install_melonloader(output_path):
        common.error("Failed to install melonloader assembly")

    if not prepare.support_module.install_support_modules(output_path):
        common.error("Failed to install support modules")

    if not prepare.il2cpp_assembly_generation.install_il2cpp_gen(output_path):
        common.error("Failed to install il2cpp assembly generator")

    if not prepare.static_settings.install_settings(output_path):
        common.error("Failed to apply static settings")

    if not wrapper.apktool.build(output_path, build_output_path):
        common.error("Failed to build.")

    sign.sign(build_output_path)
    deploy.deploy_apk(build_output_path)


def main():
    if len(sys.argv) != 2:
        common.error("usage \"py %s <path to extracted apk>\"" % os.path.basename(__file__))

    apk_path = sys.argv[1]
    update(apk_path)


if __name__ == '__main__':
    main()
