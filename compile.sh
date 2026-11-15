#!/bin/bash
cd .
DNPARAMS="-p:XWinCache=\"$HOME/.local/share/xwin-sdk/\" -p:Platform=\"$3\" -p:ForceRID=\"$2-$3\" -p:Version=\"$1\" -c $4"
dotnet clean $DNPARAMS
dotnet build $DNPARAMS
