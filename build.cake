#tool nuget:?package=coveralls.io&version=1.4.2

#addin nuget:?package=Cake.Coverlet&version=2.2.1
#addin nuget:?package=Cake.Coveralls&version=0.9.0

#load build/package.cake
#load build/paths.cake
#load build/vcs.cake
#load build/version.cake

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");

Setup<PackageMetadata>(context =>
{
    var package = new PackageMetadata(
        outputDirectory: Argument("packageOutputDirectory", "dist"),
        version: GetVersionFromProjectFile(context, Paths.ProjectFile),
        name: "Cake.Curl",
        extension: "nupkg");

    Information($"Package metadata\n{package}");

    return package;
});

Task("Clean")
    .Does<PackageMetadata>(package =>
{
    CleanDirectory(package.OutputDirectory);
    CleanDirectory(Paths.CodeCoverageDirectory);
    CleanDirectories("**/bin");
    CleanDirectories("**/obj");
});

Task("Compile")
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

Task("Set-Build-Number")
    .WithCriteria(BuildSystem.IsRunningOnAppVeyor)
    .Does<PackageMetadata>(package =>
{
    AppVeyor.UpdateBuildVersion(
        $"{package.Version}+{AppVeyor.Environment.Build.Number}");
});

Task("Test")
    .Does(() =>
{
    var settings = new DotNetCoreTestSettings
    {
        Configuration = configuration
    };

    if (IsRunningOnUnix())
    {
        settings.Framework = "netcoreapp2.1";
    }

    DotNetCoreTest(
        Paths.TestProjectFile.FullPath,
        settings,
        new CoverletSettings
        {
            CollectCoverage = true,
            CoverletOutputFormat = CoverletOutputFormat.opencover,
            CoverletOutputDirectory = Paths.CodeCoverageDirectory,
            CoverletOutputName = "coverage"
        }
        .WithFilter("[xunit.*]*")
        .WithFilter("[Cake.Curl.*Tests]*"));
});

Task("Package")
    .IsDependentOn("Test")
    .Does<PackageMetadata>(package =>
{
    DotNetCorePack(
        Paths.ProjectFile.FullPath,
        new DotNetCorePackSettings
        {
            OutputDirectory = package.OutputDirectory,
            Configuration = configuration
        });
});

Task("Publish-Build-Artifact")
    .WithCriteria(BuildSystem.IsRunningOnAppVeyor)
    .IsDependentOn("Package")
    .Does<PackageMetadata>(package =>
{
    AppVeyor.UploadArtifact(
        package.FullPath,
        new AppVeyorUploadArtifactsSettings
        {
            DeploymentName = "NuGet",
            ArtifactType = AppVeyorUploadArtifactType.NuGetPackage
        });
});

Task("Publish-Code-Coverage-Report")
    .WithCriteria(BuildSystem.IsRunningOnAppVeyor)
    .WithCriteria(() => DirectoryExists(Paths.CodeCoverageDirectory))
    .IsDependentOn("Test")
    .Does(() =>
{
    foreach (var coverageReport in GetFiles($"{Paths.CodeCoverageDirectory}/**/*.xml"))
    {
        CoverallsIo(
            coverageReport,
            new CoverallsIoSettings
            {
                RepoToken = EnvironmentVariable("CoverallsRepoToken")
            });
    }
});

Task("Upload-Package")
    .WithCriteria(
        LatestCommitHasVersionTag(Context),
        "The latest commit doesn't have a version tag")
    .IsDependentOn("Package")
    .Does<PackageMetadata>(package =>
{
    NuGetPush(
        package.FullPath,
        new NuGetPushSettings
        {
            Source = "https://www.nuget.org/api/v2/package",
            ApiKey = EnvironmentVariable("NuGetApiKey")
        }
    );
});

Task("Build")
    .IsDependentOn("Test")
    .IsDependentOn("Package")
    .IsDependentOn("Publish-Build-Artifact")
    .IsDependentOn("Publish-Code-Coverage-Report")
    .IsDependentOn("Set-Build-Number");

Task("Deploy")
    .IsDependentOn("Upload-Package");

Task("Default")
    .IsDependentOn("Test");

RunTarget(target);
