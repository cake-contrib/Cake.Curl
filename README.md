# Cake.Curl

| Package | Windows | Linux |
| :---: | :------: | :-------: |
| [![NuGet](https://img.shields.io/nuget/v/Cake.Curl.svg)](https://www.nuget.org/packages/Cake.Curl) | [![AppVeyor](https://img.shields.io/appveyor/ci/ecampidoglio/cake-curl/master.svg)](https://ci.appveyor.com/project/ecampidoglio/cake-curl) | [![Travis CI](https://img.shields.io/travis/ecampidoglio/Cake.Curl/master.svg)](https://travis-ci.org/ecampidoglio/Cake.Curl) |

Cake.Curl is a cross-platform add-in for [Cake](http://cakebuild.net/) that allows to transfer files to and from remote URLs using [curl](https://curl.haxx.se).

## Cross-platform

Cake.Curl targets the [.NET Standard 1.6](https://docs.microsoft.com/en-us/dotnet/articles/standard/library) and the [.NET Framework 4.6](https://docs.microsoft.com/en-us/dotnet/framework/migration-guide/versions-and-dependencies). As such, it will run on Linux, macOS and Windows.

## Prerequisites

In order to use Cake.Curl, you will need to have a copy of the [curl executable for your OS](https://curl.haxx.se/download.html). It doesn't have to be in a specific location; as long as it's included in your `PATH` environment variable, [Cake will find it](http://cakebuild.net/docs/tools/tool-resolution).

## Usage

The purpose of this add-in is to expose the functionality of [curl](https://curl.haxx.se) to the Cake DSL by being a very thin wrapper around its command line interface; this means that you can use Cake.Curl in the [same way](https://curl.haxx.se/docs/manual.html) as you would normally use [curl](https://curl.haxx.se), only with a different interface.

Here are a few examples of how some common [usage scenarios](https://curl.haxx.se/docs/manual.html) would look like in a Cake script.

First of all, you need to import Cake.Curl in your build script by using the [`add-in`](http://cakebuild.net/docs/fundamentals/preprocessor-directives) directive:

```csharp
#addin Cake.Curl
```

### Downloading Files

Downloading a text file from a remote HTTP server onto the working directory:

```csharp
Task("Download")
    .Does(() =>
{
    CurlDownloadFile(new Uri("http://host/file.txt"));
});
```

Downloading a sequence of text files numbered between _1_ and _10_ from a remote HTTP server onto the working directory:

```csharp
Task("Download")
    .Does(() =>
{
    CurlDownloadFile(new Uri("http://host/file[1-10].txt"));
});
```

Downloading a text file from a remote HTTP server onto the working directory and giving it a _different name_:

```csharp
Task("Download")
    .Does(() =>
{
    CurlDownloadFile(
        new Uri("http://host/file.txt"),
        new CurlDownloadSettings
        {
            OutputPaths = new FilePath[] { "renamed.txt" }
        });
});
```

Downloading multiple files _concurrently_ from different servers onto the working directory:

```csharp
Task("Download")
    .Does(() =>
{
    CurlDownloadFiles(new[]
    {
        new Uri("ftp://host/file.txt"),
        new Uri("ftp://anotherhost/anotherfile.txt"),
        new Uri("http://yetanotherhost/yetanotherfile.txt")
    }
});
```

Downloading multiple files into _specific paths_:

```csharp
Task("Download")
    .Does(() =>
{
    CurlDownloadFiles(
        new[]
        {
            new Uri("ftp://host/file.txt"),
            new Uri("http://anotherhost/anotherfile.txt"),
        }
        new CurlDownloadSettings
        {
            OutputPaths = new FilePath[]
            {
                "some/path/file.txt",
                "some/other/path/anotherfile.txt"
            }
        });
});
```

### Uploading Files

Uploading a local text file to a remote HTTP server:

```csharp
Task("Upload")
    .Does(() =>
{
    CurlUploadFile("some/file.txt", new Uri("http://host/path"));
});
```

Uploading a local text file to a remote FTPS server using credentials:

```csharp
Task("Upload")
    .Does(() =>
{
    CurlUploadFile(
        "some/file.txt",
        new Uri("ftps://host/path"),
        new CurlSettings
        {
            Username = "username",
            Password = "password"
        });
});
```

### Custom HTTP Headers

Transferring a file using a [custom HTTP header](https://curl.haxx.se/docs/manpage.html#-H) in the request:

```csharp
Task("Upload")
    .Does(() =>
{
    CurlUploadFile(
        "some/file.txt",
        new Uri("http://host/path"),
        new CurlSettings
        {
            Headers = new Dictionary<string, string>
            {
                ["X-SomeHeader"] = "SomeValue"
            }
        });
});
```

## Additional Resources

You can find more information about how to use Cake.Curl in the official documentation for these projects:

- [Cake](http://cakebuild.net/docs)
- [curl](https://curl.haxx.se/docs)

You can also see Cake.Curl in action in the following videos:

- [Building and Deploying Applications with Cake](https://www.pluralsight.com/courses/cake-applications-deploying-building) (Pluralsight Course)
- [Cake + .NET Core = Write Once, Build Anywhere (@41:30)](https://youtu.be/FKbykwvB_MU?t=41m30s) (NDC London 2018 Conference Talk)
