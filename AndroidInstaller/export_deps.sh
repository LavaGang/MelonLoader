#!/bin/bash

set -e

BASE_DIR=$(realpath "$(dirname -- "$(readlink -f -- "${BASH_SOURCE}")")")
source "${BASE_DIR}/.env"

bash "${BASE_DIR}/prepare-dependencies.sh"

OUTPUT_BASE="${BASE_DIR}/dependencies"
OUTPUT_ZIP="${BASE_DIR}/out/dependencies.zip"

(cd "${OUTPUT_BASE}" && zip -r "${OUTPUT_ZIP}" .)
