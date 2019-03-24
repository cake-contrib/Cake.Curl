using System;
using System.Collections.Generic;
using System.Globalization;
using Cake.Core;
using Cake.Curl.Tests.Fixtures;
using Cake.Testing;
using Xunit;

namespace Cake.Curl.Tests
{
    public sealed class CurlUploadFileTests
    {
        public sealed class TheExecutable
        {
            [Fact]
            public void Should_Throw_If_File_Path_Is_Null()
            {
                // Given
                var fixture = new CurlUploadFileFixture();
                fixture.FilePath = null;

                // When
                var result = Record.Exception(() => fixture.Run());

                // Then
                Assert.IsType<ArgumentNullException>(result);
                Assert.Equal("filePath", ((ArgumentNullException)result).ParamName);
            }

            [Fact]
            public void Should_Throw_If_Host_Is_Null()
            {
                // Given
                var fixture = new CurlUploadFileFixture();
                fixture.Host = null;

                // When
                var result = Record.Exception(() => fixture.Run());

                // Then
                Assert.IsType<ArgumentNullException>(result);
                Assert.Equal("host", ((ArgumentNullException)result).ParamName);
            }

            [Fact]
            public void Should_Throw_If_Settings_Are_Null()
            {
                // Given
                var fixture = new CurlUploadFileFixture();
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
                var fixture = new CurlUploadFileFixture();
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
                var fixture = new CurlUploadFileFixture
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
                var fixture = new CurlUploadFileFixture
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
                var fixture = new CurlUploadFileFixture();

                // When
                var result = fixture.Run();

                // Then
                Assert.Equal("/Working/tools/curl", result.Path.FullPath);
            }

            [Fact]
            public void Should_Set_The_Absolute_Path_Of_The_File_To_Upload_In_Quotes_As_Argument()
            {
                // Given
                var fixture = new CurlUploadFileFixture
                {
                    FilePath = "file/to/upload"
                };

                // When
                var result = fixture.Run();

                // Then
                Assert.Contains("--upload-file \"/Working/file/to/upload\"", result.Args);
            }

            [Fact]
            public void Should_Set_The_Url_Of_The_Host_As_Argument()
            {
                // Given
                var fixture = new CurlUploadFileFixture
                {
                    Host = new Uri("protocol://host/path")
                };

                // When
                var result = fixture.Run();

                // Then
                Assert.Contains("--url protocol://host/path", result.Args);
            }

            [Fact]
            public void Should_Set_The_User_Credentials_In_Quotes_As_Argument()
            {
                // Given
                var fixture = new CurlUploadFileFixture
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
                var fixture = new CurlUploadFileFixture
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
                var fixture = new CurlUploadFileFixture
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
                var fixture = new CurlUploadFileFixture
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
                var fixture = new CurlUploadFileFixture
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
                var fixture = new CurlUploadFileFixture
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
                var fixture = new CurlUploadFileFixture
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
                var fixture = new CurlUploadFileFixture
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
                var fixture = new CurlUploadFileFixture
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
                var fixture = new CurlUploadFileFixture
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
                var fixture = new CurlUploadFileFixture
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
                var fixture = new CurlUploadFileFixture
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
                var fixture = new CurlUploadFileFixture
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
                var fixture = new CurlUploadFileFixture
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
                var fixture = new CurlUploadFileFixture
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
                var fixture = new CurlUploadFileFixture
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
                var fixture = new CurlUploadFileFixture
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
                var fixture = new CurlUploadFileFixture
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
                var fixture = new CurlUploadFileFixture
                {
                    Settings = { RetryOnConnectionRefused = false }
                };

                // When
                var result = fixture.Run();

                // Then
                Assert.DoesNotContain("--retry-connrefused", result.Args);
            }

            [Theory]
            [MemberData(nameof(CurlTimeSpanTestData.TimeSpanData), MemberType = typeof(CurlTimeSpanTestData))]
            public void Should_Set_The_MaxTime_Option_As_Argument(TimeSpan maxTime)
            {
                // Given
                var fixture = new CurlUploadFileFixture
                {
                    Settings = { MaxTime = maxTime }
                };

                // When
                var result = fixture.Run();

                // Then
                Assert.Contains(
                    $"--max-time {maxTime.TotalSeconds.ToString(CultureInfo.CurrentCulture)}",
                    result.Args);
            }

            [Fact]
            public void Should_Not_Set_The_MaxTime_Option_As_Argument()
            {
                // Given
                var fixture = new CurlUploadFileFixture
                {
                    Settings = { MaxTime = null }
                };

                // When
                var result = fixture.Run();

                // Then
                Assert.DoesNotContain("--max-time", result.Args);
            }

            [Theory]
            [MemberData(nameof(CurlTimeSpanTestData.TimeSpanData), MemberType = typeof(CurlTimeSpanTestData))]
            public void Should_Set_The_ConnectTimeout_Option_As_Argument(TimeSpan connectionTimeout)
            {
                // Given
                var fixture = new CurlUploadFileFixture
                {
                    Settings = { ConnectionTimeout = connectionTimeout }
                };

                // When
                var result = fixture.Run();

                // Then
                Assert.Contains(
                    $"--connect-timeout {connectionTimeout.TotalSeconds.ToString(CultureInfo.CurrentCulture)}",
                    result.Args);
            }

            [Fact]
            public void Should_Not_Set_The_ConnectTimeout_Option_As_Argument()
            {
                // Given
                var fixture = new CurlUploadFileFixture
                {
                    Settings = { ConnectionTimeout = null }
                };

                // When
                var result = fixture.Run();

                // Then
                Assert.DoesNotContain("--connect-timeout", result.Args);
            }
        }
    }
}
