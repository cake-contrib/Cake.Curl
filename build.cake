#tool nuget:?package=coveralls.io&version=1.4.2

#addin nuget:?package=Cake.Coverlet&version=2.2.1
#addin nuget:?package=Cake.Coveralls&version=0.9.0

#load build/package.cake
#load build/paths.cake
#load build/version.cake

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");

Setup<PackageMetadata>(context =>
{
    var package = new PackageMetadata(
        outputDirectory: Argument("packageOutputDirectory", "dist"),
        version: Argument("packageVersion", "0.1.0"),
        name: "Cake.Curl",
        extension: "nupkg");

    Information($"Package metadata\n{package}");

    return package;
});

Task("Clean")
    .Does<PackageMetadata>(package =>
{
    CleanDirectory(package.OutputDirectory);
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
    .Does<PackageMetadata>(package =>
{
    if (package.Version == "0.1.0")
    {
        package.Version = GetVersionFromProjectFile(Context, Paths.ProjectFile);
        Information($"Determined version {package.Version} from the project file");
    }
    else
    {
        SetVersionToProjectFile(Context, Paths.ProjectFile, package.Version);
        Information($"Assigned version {package.Version} to the project file");
    }
});

Task("Set-Build-Number")
    .WithCriteria(BuildSystem.IsRunningOnAppVeyor)
    .IsDependentOn("Version")
    .Does<PackageMetadata>(package =>
{
    AppVeyor.UpdateBuildVersion(
        $"{package.Version}+{AppVeyor.Environment.Build.Number}");
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
        .WithFilter("[xunit.*]*")
        .WithFilter("[Cake.Curl.*Tests]*"));
});

Task("Package")
    .IsDependentOn("Restore-Packages")
    .IsDependentOn("Version")
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
    .IsDependentOn("Version")
    .IsDependentOn("Test")
    .IsDependentOn("Package")
    .IsDependentOn("Publish-Build-Artifact")
    .IsDependentOn("Publish-Code-Coverage-Report")
    .IsDependentOn("Set-Build-Number");

Task("Deploy")
    .IsDependentOn("Version")
    .IsDependentOn("Upload-Package");

Task("Default")
    .IsDependentOn("Test");

RunTarget(target);
