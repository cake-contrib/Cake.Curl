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
    .IsDependentOn("Package");

RunTarget(target);
