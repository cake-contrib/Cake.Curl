#!/usr/bin/env bash
########################################################################################
# This is a Cake bootstrapper script for Linux and macOS using .NET Core 2.0
# Inspired by
# https://adamhathcock.blog/2017/07/12/net-core-on-circle-ci-2-0-using-docker-and-cake/
########################################################################################

SCRIPT_DIR=$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )
CAKE_VERSION=0.30.0
TOOLS_DIR=$SCRIPT_DIR/tools
TOOLS_PROJ=$TOOLS_DIR/tools.csproj
CAKE_DLL=$TOOLS_DIR/Cake.CoreCLR.$CAKE_VERSION/cake.coreclr/$CAKE_VERSION/Cake.dll

# Make sure the tools folder exist
if [ ! -d "$TOOLS_DIR" ]; then
  mkdir "$TOOLS_DIR"
fi

# Install Cake.CoreCLR
if [ ! -f "$CAKE_DLL" ]; then
    echo -e "\e[92mInstalling Cake $CAKE_VERSION for .NET Core in '$TOOLS_DIR'\e[0m"
    echo "<Project Sdk=\"Microsoft.NET.Sdk\"><PropertyGroup><OutputType>Exe</OutputType><TargetFramework>netcoreapp2.0</TargetFramework></PropertyGroup></Project>" > $TOOLS_PROJ
    dotnet add $TOOLS_PROJ package Cake.CoreCLR -v $CAKE_VERSION --package-directory $TOOLS_DIR/Cake.CoreCLR.$CAKE_VERSION
fi

# Run the build script
exec dotnet "$CAKE_DLL" --verbosity=Verbose "$@"
