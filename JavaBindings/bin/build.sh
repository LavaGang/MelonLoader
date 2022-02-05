#!/bin/bash

BASE_DIR=$(realpath "$(dirname -- "$(readlink -f -- "${BASH_SOURCE}")")/../")
source "${BASE_DIR}/bin/.env"
export PATH=$PATH:$ANDROID_SDK_BASE

cd $BASE_DIR

rm -rf "${BASE_DIR}/build/dex"
mkdir -p "${BASE_DIR}/build/dex"
mkdir -p "${BASE_DIR}/build/JavaBindings"


javac \
  -cp "${ANDROID_PLATFORM}/android.jar:/home/akiva/Downloads/dexpatcher-annotation-1.8.0-beta1.jar" \
  -d "${BASE_DIR}/build/JavaBindings" \
  $(find "${BASE_DIR}/src" -name "*.java")

d8 \
  --debug \
  --output "${BASE_DIR}/build/dex" \
  --lib "${ANDROID_PLATFORM}/android.jar" \
  --min-api 23 \
  --classpath "${BASE_DIR}/build/JavaBindings" \
  $(find "${BASE_DIR}/build/JavaBindings" -name "*.class")

#I=0
#for FILE in ${BASE_DIR}/build/dex/*.dex; do
#  NEW_NAME="JavaBindings_$((I + 1)).dex"
#  if [[ $I -eq 0 ]]; then
#    NEW_NAME="JavaBindings.dex"
#  fi
#
#  mv $FILE "$(dirname -- $FILE)/${NEW_NAME}"
#  I=$((I + 1))
#done