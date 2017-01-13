#!/bin/bash

SCRIPT_DIR=$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )
NUGET_EXE=$SCRIPT_DIR/NuGet/nuget.exe
ARGUMENTS="$@"

mono $NUGET_EXE $ARGUMENTS
