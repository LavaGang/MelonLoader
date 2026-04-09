#!/bin/bash
cd .
dotnet restore -r $2-$3
dotnet build --no-restore -p:Platform="$3" -p:ForceRID="$2-$3" -p:Version="$1" -c $4
