#!/bin/bash

BASE_DIR=$(realpath "$(dirname -- "$(readlink -f -- "${BASH_SOURCE}")")/../../")
source "${BASE_DIR}/Bootstrap/tools/.env"

POSITIONAL=()
while [[ $# -gt 0 ]]; do
  key="$1"

  case $key in
    -p|--platform)
      PLATFORM="$(echo "$2" | tr '[:upper:]' '[:lower:]')"
      shift
      shift
      ;;
    -DCMAKE_CXX_COMPILER=*|-DCMAKE_C_COMPILER=*)
      shift
      ;;
    --build)
      MAKE_MODE=1
      POSITIONAL+=("$1")
      shift
      ;;
    *)
      POSITIONAL+=("$1")
      shift
      ;;
  esac
done

EXTRA_CMAKE_ARGS=()

if [[ -z ${MAKE_MODE} ]]; then
  case ${PLATFORM} in
    android)
      EXTRA_CMAKE_ARGS+=(
        -DCMAKE_TOOLCHAIN_FILE=$NDK/build/cmake/android.toolchain.cmake
        -DANDROID_ABI=$ABI
        -DANDROID_NATIVE_API_LEVEL=$MINSDKVERSION
        -DPLATFORM=ANDROID
      )
      ;;
  esac

  EXTRA_CMAKE_ARGS+=(-DUSING_WRAPPER=1 )
fi

ARGS=("${EXTRA_CMAKE_ARGS[@]}" "${POSITIONAL[@]}")
set -- "${ARGS[@]}"

exec cmake "$@"
