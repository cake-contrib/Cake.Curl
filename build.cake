#load build/paths.cake
#load build/version.cake

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");
var packageOutputDirectory = Argument("packageOutputDirectory", "dist");
var packageVersion = Argument("packageVersion", string.Empty);
var packageFilePath = Argument("packageFilePath", string.Empty);

Task("Clean")
    .Does(() =>
{
    CleanDirectory(packageOutputDirectory);
    CleanDirectories("**/bin");
    CleanDirectories("**/obj");
});

Task("Restore-Packages")
    .Does(() =>
{
    DotNetCoreRestore();
});

Task("Compile")
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

Task("Set-Build-Version")
    .WithCriteria(BuildSystem.IsRunningOnAppVeyor)
    .IsDependentOn("Version")
    .Does(() =>
{
    AppVeyor.UpdateBuildVersion(
        $"{packageVersion}+{BuildSystem.AppVeyor.Environment.Build.Number}");
});

Task("Test")
    .IsDependentOn("Restore-Packages")
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
    .IsDependentOn("Restore-Packages")
    .IsDependentOn("Version")
    .Does(() =>
{
    DotNetCorePack(
        Paths.ProjectDirectory,
        new DotNetCorePackSettings
        {
            OutputDirectory = packageOutputDirectory,
            Configuration = configuration
        });
});

Task("Publish-Build-Artifact")
    .WithCriteria(BuildSystem.IsRunningOnAppVeyor)
    .IsDependentOn("Package")
    .Does(() =>
{
    AppVeyor.UploadArtifact(
        $"{packageOutputDirectory}/Cake.Curl.{packageVersion}.nupkg",
        new AppVeyorUploadArtifactsSettings
        {
            DeploymentName = "NuGet",
            ArtifactType = AppVeyorUploadArtifactType.NuGetPackage
        });
});

Task("Upload-Package")
    .Does(() =>
{
    NuGetPush(
        packageFilePath,
        new NuGetPushSettings
        {
            Source = "https://www.nuget.org/api/v2/package",
            ApiKey = EnvironmentVariable("NuGetApiKey")
        }
    );
});

Task("Build")
    .IsDependentOn("Version")
    .IsDependentOn("Set-Build-Version")
    .IsDependentOn("Test")
    .IsDependentOn("Package")
    .IsDependentOn("Publish-Build-Artifact");

Task("Deploy")
    .IsDependentOn("Version")
    .IsDependentOn("Upload-Package");

Task("Default")
    .IsDependentOn("Test");

RunTarget(target);
