<#
.SYNOPSIS
This is a Powershell script to bootstrap a Cake build.

.DESCRIPTION
This Powershell script will install the Cake .NET local tool
and execute your Cake build script with the parameters you provide.

.PARAMETER Script
The build script to execute.
.PARAMETER Target
The build script target to run.
.PARAMETER Configuration
The build configuration to use.
.PARAMETER PackageOutputDirectory
The path of the directory where to put the packages produced by the build script.
.PARAMETER Verbosity
Specifies the amount of information to be displayed.

.LINK
https://cakebuild.net
#>

[CmdletBinding()]
Param(
    [string]$Script = "build.cake",
    [string]$Target = "Default",
    [string]$Configuration = "Debug",
    [string]$PackageOutputDirectory = "dist",
    [ValidateSet("Quiet", "Minimal", "Normal", "Verbose", "Diagnostic")]
    [string]$Verbosity = "Verbose"
)

$CakeVersion = "1.0.0"
$PSScriptRoot = Split-Path $MyInvocation.MyCommand.Path -Parent
$ToolsDirectory = Join-Path $PSScriptRoot "tools"
$CakeTool = Join-Path $ToolsDirectory "dotnet-cake.exe"

Write-Host "Installing the Cake .NET local tool..."
dotnet tool install Cake.Tool --version $CakeVersion --tool-path $ToolsDirectory

Write-Host "Running the build script..."
& "$CakeTool" $Script @(
    "--target=$Target",
    "--configuration=$Configuration",
    "--packageOutputDirectory=$PackageOutputDirectory",
    "--verbosity=$Verbosity"
)
exit $LASTEXITCODE
