using System;
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
            public void Should_Set_The_Path_Of_The_File_To_Upload_In_Quotes_As_Argument()
            {
                // Given
                var fixture = new CurlUploadFileFixture();

                // When
                var result = fixture.Run();

                // Then
                Assert.Contains(
                    $"--upload-file \"{fixture.FilePath.FullPath}\"",
                    result.Args);
            }

            [Fact]
            public void Should_Set_The_Url_Of_The_Host_As_Argument()
            {
                // Given
                var fixture = new CurlUploadFileFixture();

                // When
                var result = fixture.Run();

                // Then
                Assert.Contains(
                    $"--url {fixture.Host.AbsoluteUri}",
                    result.Args);
            }

            [Fact]
            public void Should_Set_The_User_Credentials_In_Quotes_As_Argument()
            {
                // Given
                var fixture = new CurlUploadFileFixture
                {
                    Settings = new CurlSettings
                    {
                        Username = "user",
                        Password = "password"
                    }
                };

                // When
                var result = fixture.Run();

                // Then
                Assert.Contains(
                    $"--user \"{fixture.Settings.Username}:{fixture.Settings.Password}\"",
                    result.Args);
            }

            [Fact]
            public void Should_Not_Set_The_User_Credentials_As_Argument_If_Username_Is_Null()
            {
                // Given
                var fixture = new CurlUploadFileFixture
                {
                    Settings = new CurlSettings
                    {
                        Username = null,
                        Password = "password"
                    }
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
        }
    }
}
