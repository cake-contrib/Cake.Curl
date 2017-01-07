using System;
using Cake.Core;
using Cake.Core.IO;
using Cake.Curl.Tests.Fixtures;
using Cake.Testing;
using Xunit;

namespace Cake.Curl.Tests
{
    public sealed class CurlDownloadMultipleFilesTests
    {
        public sealed class TheExecutable
        {
            [Fact]
            public void Should_Throw_If_Hosts_Is_Null()
            {
                // Given
                var fixture = new CurlDownloadMultipleFilesFixture();
                fixture.Hosts = null;

                // When
                var result = Record.Exception(() => fixture.Run());

                // Then
                Assert.IsType<ArgumentNullException>(result);
                Assert.Equal("hosts", ((ArgumentNullException)result).ParamName);
            }

            [Fact]
            public void Should_Throw_If_Hosts_Is_Empty()
            {
                // Given
                var fixture = new CurlDownloadMultipleFilesFixture();
                fixture.Hosts = new Uri[0];

                // When
                var result = Record.Exception(() => fixture.Run());

                // Then
                Assert.IsType<ArgumentException>(result);
                Assert.Equal("hosts", ((ArgumentException)result).ParamName);
                Assert.Contains("Hosts cannot be empty", ((ArgumentException)result).Message);
            }

            [Fact]
            public void Should_Throw_If_Settings_Is_Null()
            {
                // Given
                var fixture = new CurlDownloadMultipleFilesFixture();
                fixture.Settings = null;

                // When
                var result = Record.Exception(() => fixture.Run());

                // Then
                Assert.IsType<ArgumentNullException>(result);
                Assert.Equal("settings", ((ArgumentNullException)result).ParamName);
            }

            [Fact]
            public void Should_Throw_If_Curl_Executable_Was_Not_Found()
            {
                // Given
                var fixture = new CurlDownloadMultipleFilesFixture();
                fixture.GivenDefaultToolDoNotExist();

                // When
                var result = Record.Exception(() => fixture.Run());

                // Then
                Assert.IsType<CakeException>(result);
                Assert.Equal("curl: Could not locate executable.", result.Message);
            }

            [Theory]
            [InlineData("/bin/curl", "/bin/curl")]
            [InlineData("./tools/curl", "/Working/tools/curl")]
            public void Should_Use_Curl_Runner_From_Tool_Path_If_Provided(
                string toolPath,
                string expected)
            {
                // Given
                var fixture = new CurlDownloadMultipleFilesFixture
                {
                    Settings = { ToolPath = toolPath }
                };
                fixture.GivenSettingsToolPathExist();

                // When
                var result = fixture.Run();

                // Then
                Assert.Equal(expected, result.Path.FullPath);
            }

#if NETFX
            [Theory]
            [InlineData(@"C:\bin\curl.exe", "C:/bin/curl.exe")]
            [InlineData(@".\tools\curl.exe", "/Working/tools/curl.exe")]
            public void Should_Use_Curl_Runner_From_Tool_Path_If_Provided_On_Windows(
                string toolPath,
                string expected)
            {
                // Given
                var fixture = new CurlDownloadMultipleFilesFixture
                {
                    Settings = { ToolPath = toolPath }
                };
                fixture.GivenSettingsToolPathExist();

                // When
                var result = fixture.Run();

                // Then
                Assert.Equal(expected, result.Path.FullPath);
            }
#endif

            [Fact]
            public void Should_Find_Curl_Runner_If_Tool_Path_Not_Provided()
            {
                // Given
                var fixture = new CurlDownloadMultipleFilesFixture();

                // When
                var result = fixture.Run();

                // Then
                Assert.Equal("/Working/tools/curl", result.Path.FullPath);
            }

            [Fact]
            public void Should_Set_The_Remote_Name_Switches_And_The_Urls_Of_The_Hosts_As_Arguments()
            {
                // Given
                var fixture = new CurlDownloadMultipleFilesFixture
                {
                    Hosts = new[]
                    {
                        new Uri("protocol://host/path"),
                        new Uri("protocol://anotherhost/path")
                    }
                };

                // When
                var result = fixture.Run();

                // Then
                Assert.Contains("-O protocol://host/path", result.Args);
                Assert.Contains("-O protocol://anotherhost/path", result.Args);
            }

            [Fact]
            public void Should_Set_The_Absolute_Output_File_Paths_In_Quotes_And_The_Urls_Of_The_Hosts_As_Arguments()
            {
                // Given
                var fixture = new CurlDownloadMultipleFilesFixture
                {
                    Hosts = new[]
                    {
                        new Uri("protocol://host/path"),
                        new Uri("protocol://anotherhost/path")
                    },
                    Settings =
                    {
                        OutputPaths = new FilePath[]
                        {
                            "output/file",
                            "output/anotherfile"
                        }
                    }
                };

                // When
                var result = fixture.Run();

                // Then
                Assert.Contains(
                    "-o \"/Working/output/file\" protocol://host/path",
                    result.Args);
                Assert.Contains(
                    "-o \"/Working/output/anotherfile\" protocol://anotherhost/path",
                    result.Args);
            }

            [Fact]
            public void Should_Set_The_User_Credentials_In_Quotes_As_Argument()
            {
                // Given
                var fixture = new CurlDownloadMultipleFilesFixture
                {
                    Settings = { Username = "user", Password = "password" }
                };

                // When
                var result = fixture.Run();

                // Then
                Assert.Contains("--user \"user:password\"", result.Args);
            }

            [Fact]
            public void Should_Not_Set_The_User_Credentials_As_Argument_If_Username_Is_Null()
            {
                // Given
                var fixture = new CurlDownloadMultipleFilesFixture
                {
                    Settings = { Username = null, Password = "password" }
                };

                // When
                var result = fixture.Run();

                // Then
                Assert.DoesNotContain($"--user", result.Args);
            }
        }
    }
}