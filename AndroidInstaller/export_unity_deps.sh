#!/bin/bash

set -e

BASE_DIR=$(realpath "$(dirname -- "$(readlink -f -- "${BASH_SOURCE}")")")
source "${BASE_DIR}/.env"

OUTPUT_BASE="${BASE_DIR}/unity_dependencies"
OUTPUT_ZIP="${BASE_DIR}/out/unity_libs.zip"

UNITY_NATIVE_LIBS_PATH="${UNITY_INSTALLATION_BASE}/Editor/Data/PlaybackEngines/AndroidPlayer/Variations/il2cpp/Development/Libs"
UNITY_MANAGED_LIBS_PATH="${UNITY_INSTALLATION_BASE}/Editor/Data/PlaybackEngines/AndroidPlayer/Variations/il2cpp/Managed"

rm -rf "${OUTPUT_BASE}"
rm -rf "${OUTPUT_ZIP}"

mkdir -p "${OUTPUT_BASE}/native"
mkdir -p "${OUTPUT_BASE}/managed"
mkdir -p "${BASE_DIR}/out"

rsync -Ia "${UNITY_NATIVE_LIBS_PATH}/" "${OUTPUT_BASE}/native"
rsync -Ia "${UNITY_MANAGED_LIBS_PATH}"/*.dll "${OUTPUT_BASE}/managed"

(cd "${OUTPUT_BASE}" && zip -r "${OUTPUT_ZIP}" .)