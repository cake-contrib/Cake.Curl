# Cake.Curl

[![NuGet](https://img.shields.io/nuget/v/Cake.Curl.svg)](https://www.nuget.org/packages/Cake.Curl)
[![Build](https://ci.appveyor.com/api/projects/status/bswuobfgg35c1pxg?svg=true)](https://ci.appveyor.com/project/ecampidoglio/cake-curl)

Cake.Curl is an add-in for [Cake](http://cakebuild.net/) that allows to transer
files to a remote URL using [curl](https://curl.haxx.se).

## Examples

Uploading a local file to a remote HTTP server:

```csharp
#addin Cake.Curl

Task("Upload")
    .Does(() =>
{
    CurlUploadFile("some/file.txt", new Uri("http://host/path"));
});
```
Uploading a local file to a remote FTPS server using credentials:

```csharp
#addin Cake.Curl

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
