#!/usr/bin/env bash

echo -e "\e[92mRestoring the .NET local tools...\e[0m"
dotnet tool restore

echo -e "\e[92mRunning the build script...\e[0m"
dotnet tool run dotnet-cake build.cake "$@"
