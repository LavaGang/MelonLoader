#!/bin/bash

CONFIGURATION="${4^}"
RID="$2-$3"
OUTPUT_PATH="Output/$CONFIGURATION/$RID"

DNPARAMS="-p:XWinCache=\"$HOME/.local/share/xwin-sdk/\" -p:Platform=\"$3\" -p:ForceRID=\"$RID\" -p:Version=\"$1\" -c $CONFIGURATION"

cd .
dotnet clean $DNPARAMS
dotnet build $DNPARAMS || exit 1

if [[ "$5" == "-g" ]]; then
    DESTINATION="${6/#\~/$HOME}"
    mkdir -p "$DESTINATION"
    cp -rf "$OUTPUT_PATH/." "$DESTINATION/"
fi
