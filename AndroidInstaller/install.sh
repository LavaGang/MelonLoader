#!/bin/bash

set -e

BASE_DIR=$(realpath "$(dirname -- "$(readlink -f -- "${BASH_SOURCE}")")")
source "${BASE_DIR}/.env"

bash "${BASE_DIR}/prepare-dependencies.sh"

export PATH=$PATH:$ANDROID_SDK_BASE

cd "${BASE_DIR}"

TARGET_APK=$(realpath -- "${TARGET_APK}")
OUTPUT_DIR="${BASE_DIR}/out"
MODIFIED_APK="${OUTPUT_DIR}/$(basename -- "${TARGET_APK}")"
DEPENDENCIES_DIR="${BASE_DIR}/dependencies"

rm -rf "${OUTPUT_DIR}"

mkdir -p "${OUTPUT_DIR}/source"
mkdir -p "${OUTPUT_DIR}/bindings"
mkdir -p "${OUTPUT_DIR}/patched"

cp "${TARGET_APK}" "${MODIFIED_APK}"

echo "unpacking dex"

# Patch APK dex
unzip -jo "${MODIFIED_APK}" "*.dex" -d "${OUTPUT_DIR}/source"
echo "${OUTPUT_DIR}/patched" "${OUTPUT_DIR}/source" "${DEPENDENCIES_DIR}/dex"
bin/dexpatcher --output "${OUTPUT_DIR}/patched" "${OUTPUT_DIR}/source" "${DEPENDENCIES_DIR}/dex"

echo "loading additional files"

# Prepare managed files
mkdir -p "${OUTPUT_DIR}/patched/assets/melonloader/etc/managed/"
cp ${DEPENDENCIES_DIR}/mono/bcl/*.dll "${OUTPUT_DIR}/patched/assets/melonloader/etc/managed/"

# Prepare MelonLoader core
mkdir -p "${OUTPUT_DIR}/patched/assets/melonloader/etc/managed/etc/"
cp ${DEPENDENCIES_DIR}/core/* "${OUTPUT_DIR}/patched/assets/melonloader/etc/"

# Prepare support modules
mkdir -p "${OUTPUT_DIR}/patched/assets/melonloader/etc/support/"
cp ${DEPENDENCIES_DIR}/support_modules/* "${OUTPUT_DIR}/patched/assets/melonloader/etc/support/"

# Prepare assembly generation managed
mkdir -p "${OUTPUT_DIR}/patched/assets/melonloader/etc/assembly_generation/managed/"
cp ${DEPENDENCIES_DIR}/assembly_generation/* "${OUTPUT_DIR}/patched/assets/melonloader/etc/assembly_generation/managed/"

# Prepare assembly generation unity files
UNITY_NATIVE_LIBS_PATH="${UNITY_INSTALLATION_BASE}/Editor/Data/PlaybackEngines/AndroidPlayer/Variations/il2cpp/Development/Libs"
UNITY_MANAGED_LIBS_PATH="${UNITY_INSTALLATION_BASE}/Editor/Data/PlaybackEngines/AndroidPlayer/Variations/il2cpp/Managed"
mkdir -p "${OUTPUT_DIR}/patched/assets/melonloader/etc/assembly_generation/unity/"
cp "${UNITY_MANAGED_LIBS_PATH}"/*.dll "${OUTPUT_DIR}/patched/assets/melonloader/etc/assembly_generation/unity/"

# Install native binaries
mkdir -p "${OUTPUT_DIR}/patched/lib/arm64-v8a/"
cp "${DEPENDENCIES_DIR}"/native/*.so "${OUTPUT_DIR}/patched/lib/arm64-v8a/"
cp "${UNITY_NATIVE_LIBS_PATH}/arm64-v8a/libunity.so" "${OUTPUT_DIR}/patched/lib/arm64-v8a/"

echo "repacking"

# Re-pack APK
zip -d "${MODIFIED_APK}" "*.dex" "META-INF/*" "lib/*/libunity.so"
(cd "${OUTPUT_DIR}/patched/" && zip -ur $MODIFIED_APK .)

echo "signing"

jarsigner -verbose -sigalg SHA1withRSA -digestalg SHA1 -keystore "${BASE_DIR}/bin/dev.keystore" -storepass "123456" "${MODIFIED_APK}" alias_name
zipalign 4 "${MODIFIED_APK}" "${MODIFIED_APK}.align"
rm "${MODIFIED_APK}" && mv "${MODIFIED_APK}.align" "${MODIFIED_APK}"
