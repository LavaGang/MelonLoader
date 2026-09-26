#!/bin/bash
cd .
dotnet build -p:Platform="$3" -p:ForceRID="$2-$3" -p:Version="$1" -c $4
