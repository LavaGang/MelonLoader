#!/bin/bash

BASE_DIR=$(realpath "$(dirname -- "$(readlink -f -- "${BASH_SOURCE}")")/../")
source "${BASE_DIR}/bin/.env"
export PATH=$PATH:$ANDROID_SDK_BASE

cd $BASE_DIR

rm -rf "${BASE_DIR}/build"
mkdir -p "${BASE_DIR}/build/dex"
mkdir -p "${BASE_DIR}/build/JavaBindings"
mkdir -p "${BASE_DIR}/build/bundle_modules"

cp "${ANDROID_PLATFORM}/android.jar" "${BASE_DIR}/build/android_sdk.jar"

# https://stackoverflow.com/a/17841619
function join_by { local IFS="$1"; shift; echo "$*"; }

for BHAPTICS_MODULE in "${BASE_DIR}/../external/BHapticsHapticLibrary/samples/unity-plugin/Assets/Bhaptics/SDK/Plugins/Android"/*.aar; do
  unzip -p $BHAPTICS_MODULE classes.jar > "${BASE_DIR}/build/bundle_modules/$(basename $BHAPTICS_MODULE .aar).jar"
done

COMPILE_TIME_DEPS=(
  "${BASE_DIR}/build/android_sdk.jar"
  "${BASE_DIR}/deps/dexpatcher_annotations.jar"
  "${BASE_DIR}/deps/unity.jar"
  "${BASE_DIR}/build/bundle_modules/bhaptics_unity_bridge.jar"
  "${BASE_DIR}/build/bundle_modules/bhaptics_manager.jar"
)

javac \
  -cp "$(join_by ":" "${COMPILE_TIME_DEPS[@]}")" \
  -d "${BASE_DIR}/build/JavaBindings" \
  $(find "${BASE_DIR}/src" -name "*.java")

for BUNDLE_MODULE in "${BASE_DIR}/build/bundle_modules"/*.jar; do
  unzip -qo "${BUNDLE_MODULE}" -d "${BASE_DIR}/build/JavaBindings/"
done

d8 \
  --debug \
  --output "${BASE_DIR}/build/dex" \
  --lib "${BASE_DIR}/build/android_sdk.jar" \
  --min-api 23 \
  --classpath "${BASE_DIR}/build/JavaBindings" \
  $(find "${BASE_DIR}/build/JavaBindings" -name "*.class")