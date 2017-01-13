#!/usr/bin/env bash

##########################################################################
# This is the Cake bootstrapper script for Linux and OS X.
# This file was downloaded from https://github.com/cake-build/resources
# and modified to run on .NET Core instead of Mono
##########################################################################

# Define directories.
SCRIPT_DIR=$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )
TOOLS_DIR=$SCRIPT_DIR/tools
NUGET_DIR=$TOOLS_DIR/NuGet
NUGET_EXE=$NUGET_DIR/nuget.exe
NUGET_SHIM=$TOOLS_DIR/nuget.exe.sh
NUGET=$TOOLS_DIR/nuget.exe
CAKE_DLL=$TOOLS_DIR/Cake.CoreCLR/Cake.dll

# Define default arguments.
SCRIPT="build.cake"
TARGET="Default"
CONFIGURATION="Release"
VERBOSITY="verbose"
DRYRUN=
SHOW_VERSION=false
SCRIPT_ARGUMENTS=()

# Parse arguments.
for i in "$@"; do
    case $1 in
        -s|--script) SCRIPT="$2"; shift ;;
        -t|--target) TARGET="$2"; shift ;;
        -c|--configuration) CONFIGURATION="$2"; shift ;;
        -v|--verbosity) VERBOSITY="$2"; shift ;;
        -d|--dryrun) DRYRUN="-dryrun" ;;
        --version) SHOW_VERSION=true ;;
        --) shift; SCRIPT_ARGUMENTS+=("$@"); break ;;
        *) SCRIPT_ARGUMENTS+=("$1") ;;
    esac
    shift
done

# Make sure the tools folder exist.
if [ ! -d "$TOOLS_DIR" ]; then
  mkdir "$TOOLS_DIR"
fi

# Make sure the NuGet folder exist.
if [ ! -d "$NUGET_DIR" ]; then
    mkdir "$NUGET_DIR"
fi

# Download NuGet if it does not exist.
if [ ! -f "$NUGET_EXE" ]; then
    echo "Downloading NuGet..."
    curl -Lsfo "$NUGET_EXE" https://dist.nuget.org/win-x86-commandline/v3.4.4/nuget.exe
    if [ $? -ne 0 ]; then
        echo "An error occured while downloading nuget.exe."
        exit 1
    fi
fi

# Rename a copy of the NuGet shim to the NuGet executable.
if [ ! -f "$NUGET" ]; then
    cp "$NUGET_SHIM" "$NUGET"
fi

# Install Cake.CoreCRL from NuGet.
if [ ! -f "$CAKE_DLL" ]; then
    "$NUGET" install Cake.CoreCLR -ExcludeVersion -OutputDirectory "$TOOLS_DIR"
    if [ $? -ne 0 ]; then
        echo "Could not install the Cake.CoreCLR package from NuGet"
        exit 1
    fi
fi

# Make sure that Cake has been installed.
if [ ! -f "$CAKE_DLL" ]; then
    echo "Could not find Cake at '$CAKE_DLL'."
    exit 1
fi

# Start Cake
if $SHOW_VERSION; then
    dotnet "$CAKE_DLL" -version
else
    dotnet "$CAKE_DLL" $SCRIPT -verbosity=$VERBOSITY -configuration=$CONFIGURATION -target=$TARGET $DRYRUN "${SCRIPT_ARGUMENTS[@]}"
fi
