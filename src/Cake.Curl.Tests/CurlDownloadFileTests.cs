using System;
using System.Collections.Generic;
using Cake.Core;
using Cake.Core.IO;
using Cake.Curl.Tests.Fixtures;
using Cake.Testing;
using Xunit;

namespace Cake.Curl.Tests
{
    public sealed class CurlDownloadFileTests
    {
        public sealed class TheExecutable
        {
            [Fact]
            public void Should_Throw_If_Host_Is_Null()
            {
                // Given
                var fixture = new CurlDownloadFileFixture();
                fixture.Host = null;

                // When
                var result = Record.Exception(() => fixture.Run());

                // Then
                Assert.IsType<ArgumentNullException>(result);
                Assert.Equal("host", ((ArgumentNullException)result).ParamName);
            }

            [Fact]
            public void Should_Throw_If_Settings_Is_Null()
            {
                // Given
                var fixture = new CurlDownloadFileFixture();
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
                var fixture = new CurlDownloadFileFixture();
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
                var fixture = new CurlDownloadFileFixture
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
                var fixture = new CurlDownloadFileFixture
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
                var fixture = new CurlDownloadFileFixture();

                // When
                var result = fixture.Run();

                // Then
                Assert.Equal("/Working/tools/curl", result.Path.FullPath);
            }

            [Fact]
            public void Should_Set_The_Remote_Name_Switch_And_The_Url_Of_The_Host_As_Arguments()
            {
                // Given
                var fixture = new CurlDownloadFileFixture
                {
                    Host = new Uri("protocol://host/path")
                };

                // When
                var result = fixture.Run();

                // Then
                Assert.Contains("-O protocol://host/path", result.Args);
            }

            [Fact]
            public void Should_Set_The_Absolute_Output_File_Path_In_Quotes_And_The_Url_Of_The_Host_As_Arguments()
            {
                // Given
                var fixture = new CurlDownloadFileFixture
                {
                    Host = new Uri("protocol://host/path"),
                    Settings =
                    {
                        OutputPaths = new FilePath[] { "output/file" }
                    }
                };

                // When
                var result = fixture.Run();

                // Then
                Assert.Contains(
                    "-o \"/Working/output/file\" protocol://host/path",
                    result.Args);
            }

            [Fact]
            public void Should_Set_The_User_Credentials_In_Quotes_As_Argument()
            {
                // Given
                var fixture = new CurlDownloadFileFixture
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
                var fixture = new CurlDownloadFileFixture
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
                var fixture = new CurlDownloadFileFixture
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
                var fixture = new CurlDownloadFileFixture
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
                var fixture = new CurlDownloadFileFixture
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
                var fixture = new CurlDownloadFileFixture
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
                var fixture = new CurlDownloadFileFixture
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
                var fixture = new CurlDownloadFileFixture
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
                var fixture = new CurlDownloadFileFixture
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
                var fixture = new CurlDownloadFileFixture
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
                var fixture = new CurlDownloadFileFixture
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
                var fixture = new CurlDownloadFileFixture
                {
                    Settings = { Retry = 1000 }
                };

                // When
                var result = fixture.Run();

                // Then
                Assert.Contains("--retry 1000", result.Args);
            }

            [Fact]
            public void Should_Not_Set_The_Retry_Option_As_Argument()
            {
                // Given
                var fixture = new CurlDownloadFileFixture
                {
                    Settings = { Retry = 0 }
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
                var fixture = new CurlDownloadFileFixture
                {
                    Settings = { RetryDelay = 1000 }
                };

                // When
                var result = fixture.Run();

                // Then
                Assert.Contains("--retry-delay 1000", result.Args);
            }

            [Fact]
            public void Should_Not_Set_The_RetryDelay_Option_As_Argument()
            {
                // Given
                var fixture = new CurlDownloadFileFixture
                {
                    Settings = { RetryDelay = 0 }
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
                var fixture = new CurlDownloadFileFixture
                {
                    Settings = { RetryMaxTime = 1000 }
                };

                // When
                var result = fixture.Run();

                // Then
                Assert.Contains("--retry-max-time 1000", result.Args);
            }

            [Fact]
            public void Should_Not_Set_The_RetryMaxTme_Option_As_Argument()
            {
                // Given
                var fixture = new CurlDownloadFileFixture
                {
                    Settings = { RetryMaxTime = 0 }
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
                var fixture = new CurlDownloadFileFixture
                {
                    Settings = { RetryConnRefused = true }
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
                var fixture = new CurlDownloadFileFixture
                {
                    Settings = { RetryConnRefused = false }
                };

                // When
                var result = fixture.Run();

                // Then
                Assert.DoesNotContain("--retry-connrefused", result.Args);
            }
        }
    }
}
