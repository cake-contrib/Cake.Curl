#tool nuget:?package=coveralls.io&version=1.4.2

#addin nuget:?package=Cake.Coverlet&version=2.2.1
#addin nuget:?package=Cake.Coveralls&version=0.9.0

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

    if (FileExists(Paths.CodeCoverageReportFile))
    {
        DeleteFile(Paths.CodeCoverageReportFile);
    }
});

Task("Restore-Packages")
    .Does(() =>
{
    DotNetCoreRestore(Paths.SolutionFile.FullPath);
});

Task("Compile")
    .IsDependentOn("Restore-Packages")
    .Does(() =>
{
    var settings = new DotNetCoreBuildSettings
    {
        Configuration = configuration
    };

    if (IsRunningOnUnix())
    {
        settings.Framework = "netstandard2.0";
    }

    DotNetCoreBuild(Paths.ProjectFile.FullPath, settings);
});

Task("Version")
    .Does(() =>
{
    if (string.IsNullOrEmpty(packageVersion))
    {
        packageVersion = GetVersionFromProjectFile(Context, Paths.ProjectFile);
        Information($"Determined version {packageVersion} from the project file");
    }
    else
    {
        SetVersionToProjectFile(Context, Paths.ProjectFile, packageVersion);
        Information($"Assigned version {packageVersion} to the project file");
    }
});

Task("Set-Build-Version")
    .WithCriteria(BuildSystem.IsRunningOnAppVeyor)
    .IsDependentOn("Version")
    .Does(() =>
{
    AppVeyor.UpdateBuildVersion(
        $"{packageVersion}+{AppVeyor.Environment.Build.Number}");
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
        settings.Framework = "netcoreapp2.0";
    }

    DotNetCoreTest(
        Paths.TestProjectFile.FullPath,
        settings,
        new CoverletSettings
        {
            CollectCoverage = true,
            CoverletOutputFormat = CoverletOutputFormat.opencover,
            CoverletOutputDirectory = Paths.CodeCoverageReportFile.GetDirectory(),
            CoverletOutputName = Paths.CodeCoverageReportFile.GetFilename().ToString()
        }
        .WithFilter("[Cake.Curl.*Tests]*"));
});

Task("Package")
    .IsDependentOn("Restore-Packages")
    .IsDependentOn("Version")
    .Does(() =>
{
    DotNetCorePack(
        Paths.ProjectFile.FullPath,
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

Task("Publish-Code-Coverage-Report")
    .WithCriteria(BuildSystem.IsRunningOnAppVeyor)
    .WithCriteria(() => FileExists(Paths.CodeCoverageReportFile))
    .IsDependentOn("Test")
    .Does(() =>
{
    CoverallsIo(
        Paths.CodeCoverageReportFile,
        new CoverallsIoSettings
        {
            RepoToken = EnvironmentVariable("CoverallsRepoToken")
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
    .IsDependentOn("Publish-Build-Artifact")
    .IsDependentOn("Publish-Code-Coverage-Report");

Task("Deploy")
    .IsDependentOn("Version")
    .IsDependentOn("Upload-Package");

Task("Default")
    .IsDependentOn("Test");

RunTarget(target);
