using System;
using System.Collections.Generic;
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

            [Fact]
            public void Should_Set_The_Verbose_Argument()
            {
                // Given
                var fixture = new CurlDownloadMultipleFilesFixture
                {
                    Settings = { Verbose = true }
                };

                // When
                var result = fixture.Run();

                // Then
                Assert.Contains("--verbose", result.Args);
            }

            [Fact]
            public void Should_Set_The_Progress_Bar_Argument()
            {
                // Given
                var fixture = new CurlDownloadMultipleFilesFixture
                {
                    Settings = { ProgressBar = true }
                };

                // When
                var result = fixture.Run();

                // Then
                Assert.Contains("--progress-bar", result.Args);
            }

            [Fact]
            public void Should_Set_The_Headers_In_Quotes_As_Argument()
            {
                // Given
                var fixture = new CurlDownloadMultipleFilesFixture
                {
                    Settings =
                    {
                        Headers = new Dictionary<string, string>
                        {
                            ["name"] = "value"
                        }
                    }
                };

                // When
                var result = fixture.Run();

                // Then
                Assert.Contains("--header \"name:value\"", result.Args);
            }

            [Fact]
            public void Should_Set_Multiple_Headers_In_Quotes_As_Argument()
            {
                // Given
                var fixture = new CurlDownloadMultipleFilesFixture
                {
                    Settings =
                    {
                        Headers = new Dictionary<string, string>
                        {
                            ["name1"] = "value1",
                            ["name2"] = "value2",
                            ["name3"] = "value3"
                        }
                    }
                };

                // When
                var result = fixture.Run();

                // Then
                Assert.Contains("--header \"name1:value1\" --header \"name2:value2\" --header \"name3:value3\"", result.Args);
            }

            [Fact]
            public void Should_Set_The_Request_Command_In_Quotes_And_Upper_Case_As_Argument()
            {
                // Given
                var fixture = new CurlDownloadMultipleFilesFixture
                {
                    Settings = { RequestCommand = "Command" }
                };

                // When
                var result = fixture.Run();

                // Then
                Assert.Contains("--request \"COMMAND\"", result.Args);
            }

            [Fact]
            public void Should_Set_The_Location_Option_As_Argument()
            {
                // Given
                var fixture = new CurlDownloadMultipleFilesFixture
                {
                    Settings = { FollowRedirects = true }
                };

                // When
                var result = fixture.Run();

                // Then
                Assert.Contains("--location", result.Args);
            }

            [Fact]
            public void Should_Not_Set_The_Location_Option_As_Argument()
            {
                // Given
                var fixture = new CurlDownloadMultipleFilesFixture
                {
                    Settings = { FollowRedirects = false }
                };

                // When
                var result = fixture.Run();

                // Then
                Assert.DoesNotContain("--location", result.Args);
            }

            [Fact]
            public void Should_Set_The_Fail_Option_As_Argument()
            {
                // Given
                var fixture = new CurlDownloadMultipleFilesFixture
                {
                    Settings = { Fail = true }
                };

                // When
                var result = fixture.Run();

                // Then
                Assert.Contains("--fail", result.Args);
            }

            [Fact]
            public void Should_Not_Set_The_Fail_Option_As_Argument()
            {
                // Given
                var fixture = new CurlDownloadMultipleFilesFixture
                {
                    Settings = { Fail = false }
                };

                // When
                var result = fixture.Run();

                // Then
                Assert.DoesNotContain("--fail", result.Args);
            }

            [Fact]
            public void Should_Set_The_Retry_Option_As_Argument()
            {
                // Given
                var fixture = new CurlDownloadMultipleFilesFixture
                {
                    Settings = { RetryCount = 3 }
                };

                // When
                var result = fixture.Run();

                // Then
                Assert.Contains("--retry 3", result.Args);
            }

            [Fact]
            public void Should_Not_Set_The_Retry_Option_As_Argument()
            {
                // Given
                var fixture = new CurlDownloadMultipleFilesFixture
                {
                    Settings = { RetryCount = 0 }
                };

                // When
                var result = fixture.Run();

                // Then
                Assert.DoesNotContain("--retry", result.Args);
            }

            [Fact]
            public void Should_Set_The_RetryDelay_Option_As_Argument()
            {
                // Given
                var fixture = new CurlDownloadMultipleFilesFixture
                {
                    Settings = { RetryDelaySeconds = 30 }
                };

                // When
                var result = fixture.Run();

                // Then
                Assert.Contains("--retry-delay 30", result.Args);
            }

            [Fact]
            public void Should_Not_Set_The_RetryDelay_Option_As_Argument()
            {
                // Given
                var fixture = new CurlDownloadMultipleFilesFixture
                {
                    Settings = { RetryDelaySeconds = 0 }
                };

                // When
                var result = fixture.Run();

                // Then
                Assert.DoesNotContain("--retry-delay", result.Args);
            }

            [Fact]
            public void Should_Set_The_RetryMaxTime_Option_As_Argument()
            {
                // Given
                var fixture = new CurlDownloadMultipleFilesFixture
                {
                    Settings = { RetryMaxTimeSeconds = 300 }
                };

                // When
                var result = fixture.Run();

                // Then
                Assert.Contains("--retry-max-time 300", result.Args);
            }

            [Fact]
            public void Should_Not_Set_The_RetryMaxTme_Option_As_Argument()
            {
                // Given
                var fixture = new CurlDownloadMultipleFilesFixture
                {
                    Settings = { RetryMaxTimeSeconds = 0 }
                };

                // When
                var result = fixture.Run();

                // Then
                Assert.DoesNotContain("--retry-max-time", result.Args);
            }

            [Fact]
            public void Should_Set_The_RetryConnRefused_Option_As_Argument()
            {
                // Given
                var fixture = new CurlDownloadMultipleFilesFixture
                {
                    Settings = { RetryOnConnectionRefused = true }
                };

                // When
                var result = fixture.Run();

                // Then
                Assert.Contains("--retry-connrefused", result.Args);
            }

            [Fact]
            public void Should_Not_Set_The_RetryConnRefused_Option_As_Argument()
            {
                // Given
                var fixture = new CurlDownloadMultipleFilesFixture
                {
                    Settings = { RetryOnConnectionRefused = false }
                };

                // When
                var result = fixture.Run();

                // Then
                Assert.DoesNotContain("--retry-connrefused", result.Args);
            }
        }
    }
}
