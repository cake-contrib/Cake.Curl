#load build/paths.cake
#load build/version.cake

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");
var packageOutputDirectory = Argument("packageOutputDirectory", "dist");
var packageVersion = Argument("packageVersion", string.Empty);

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

Task("Version")
    .Does(() =>
{
    if (string.IsNullOrEmpty(packageVersion))
    {
        packageVersion = GetVersionFromProjectFile(Paths.ProjectDirectory);
        Information($"Determined version {packageVersion} from the project file");
    }
    else
    {
        SetVersionToProjectFile(packageVersion, Paths.ProjectDirectory);
        Information($"Assigned version {packageVersion} to the project file");
    }
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
    .IsDependentOn("Version")
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

Task("Publish-Package")
    .WithCriteria(BuildSystem.IsRunningOnAppVeyor)
    .IsDependentOn("Package")
    .Does(() =>
{
    AppVeyor.UploadArtifact(
        $"{packageOutputDirectory}/Cake.Curl.{packageVersion}.nupkg",
        new AppVeyorUploadArtifactsSettings
        {
            ArtifactType = AppVeyorUploadArtifactType.NuGetPackage
        });
});

Task("Default")
    .IsDependentOn("Test")
    .IsDependentOn("Package");

RunTarget(target);
