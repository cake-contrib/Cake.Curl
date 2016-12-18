#load build/paths.cake

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");
var packageOutputDirectory = Argument("packageOutputDirectory", "dist");

Task("Clean")
    .Does(() =>
{
    CleanDirectory(packageOutputDirectory);
    CleanDirectories("**/bin");
    CleanDirectories("**/obj");
    DeleteFiles("**/project.lock.json");
});

Task("Restore-Packages")
    .Does(() =>
{
    DotNetCoreRestore();
});

Task("Build")
    .IsDependentOn("Restore-Packages")
    .Does(() =>
{
    DotNetCoreBuild(
        Paths.ProjectDirectory,
        new DotNetCoreBuildSettings
        {
            Configuration = configuration
        });
});

Task("Test")
    .IsDependentOn("Build")
    .Does(() =>
{
    var settings = new DotNetCoreTestSettings
    {
        Configuration = configuration
    };

    if (IsRunningOnUnix())
    {
        settings.Framework = "netcoreapp1.0";
    }

    DotNetCoreTest(Paths.TestsDirectory, settings);
});

Task("Package")
    .IsDependentOn("Build")
    .Does(() =>
{
    DotNetCorePack(
        Paths.ProjectDirectory,
        new DotNetCorePackSettings
        {
            OutputDirectory = packageOutputDirectory,
            NoBuild = true
        });
});

Task("Default")
    .IsDependentOn("Test")
    .IsDependentOn("Package");

RunTarget(target);
