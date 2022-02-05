#!/bin/bash

TEMP_MONO_BCL=$(realpath "$HOME/CLionProjects/melonloader-mono/sdks/out/android-bcl/monodroid")
TEMP_MONO_BINARIES=$(realpath "$HOME/CLionProjects/melonloader-mono/sdks/out/android-arm64-v8a-debug/lib")

PROJECT_DIR=$(realpath "$(dirname -- "$(readlink -f -- "${BASH_SOURCE}")")/../")
BASE_DIR=$(realpath "$(dirname -- "$(readlink -f -- "${BASH_SOURCE}")")")
OUTPUT_BASE="${BASE_DIR}/dependencies"

MELONLOADER_SUB_PATH="Output/Debug/Android/MelonLoader/net4.7.2"
SUPPORTMODULES_SUB_PATH="Output/Debug/Android/MelonLoader/Dependencies/SupportModules/net4.7.2"
IL2CPP_GEN_SUB_PATH="Output/Debug/AnyCPU/MelonLoader/Dependencies/Il2CppAssemblyGenerator/net4.7.2"
BOOTSTRAP_COMPILE_SUB_PATH="Bootstrap/.cmake/android/debug"

rm -rf "${OUTPUT_BASE}"

mkdir -p "${OUTPUT_BASE}/dex"
rsync -Ia "${PROJECT_DIR}/JavaBindings/build/dex/" "${OUTPUT_BASE}/dex"

mkdir -p "${OUTPUT_BASE}/mono/bcl/"
cp $TEMP_MONO_BCL/*.dll "${OUTPUT_BASE}/mono/bcl/"

mkdir -p "${OUTPUT_BASE}/core/"
cp "${PROJECT_DIR}/${MELONLOADER_SUB_PATH}/MelonLoader.dll" "${OUTPUT_BASE}/core/"

mkdir -p "${OUTPUT_BASE}/support_modules/"
cp ${PROJECT_DIR}/${SUPPORTMODULES_SUB_PATH}/*.dll "${OUTPUT_BASE}/support_modules/"

mkdir -p "${OUTPUT_BASE}/assembly_generation/"
cp ${PROJECT_DIR}/${IL2CPP_GEN_SUB_PATH}/*.dll "${OUTPUT_BASE}/assembly_generation/"
cp "${PROJECT_DIR}/external/Il2CppAssemblyUnhollower/AssemblyUnhollower/bin/Debug/net4.7.2/AssemblyUnhollower.dll" "${OUTPUT_BASE}/assembly_generation/"
cp "${PROJECT_DIR}/external/Il2CppAssemblyUnhollower/AssemblyUnhollower/bin/Debug/net4.7.2/Iced.dll" "${OUTPUT_BASE}/assembly_generation/"
cp "${PROJECT_DIR}/external/Il2CppAssemblyUnhollower/UnhollowerBaseLib/bin/Debug/net4.7.2/UnhollowerBaseLib.dll" "${OUTPUT_BASE}/assembly_generation/"
cp "${PROJECT_DIR}/external/Il2CppAssemblyUnhollower/UnhollowerRuntimeLib/bin/Debug/net4.7.2/UnhollowerRuntimeLib.dll" "${OUTPUT_BASE}/assembly_generation/"

mkdir -p "${OUTPUT_BASE}/native/"
cp "${PROJECT_DIR}/${BOOTSTRAP_COMPILE_SUB_PATH}/libBootstrap.so" "${OUTPUT_BASE}/native/"
cp "${PROJECT_DIR}/${BOOTSTRAP_COMPILE_SUB_PATH}/funchook/libfunchook.so" "${OUTPUT_BASE}/native/"
cp "${PROJECT_DIR}/${BOOTSTRAP_COMPILE_SUB_PATH}/capstone/libcapstone.so" "${OUTPUT_BASE}/native/"
cp ${TEMP_MONO_BINARIES}/*.so "${OUTPUT_BASE}/native/"
