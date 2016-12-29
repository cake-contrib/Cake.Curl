using System;
using Cake.Core.IO;
using Cake.Testing.Fixtures;

namespace Cake.Curl.Tests.Fixtures
{
    internal sealed class CurlUploadFileFixture : ToolFixture<CurlSettings>
    {
        public CurlUploadFileFixture()
            : base("curl")
        {
            FilePath = "file/to/upload";
            Host = new Uri("protocol://host/path");
        }

        public FilePath FilePath { get; set; }

        public Uri Host { get; set; }

        protected override void RunTool()
        {
            var tool = new CurlUploadRunner(
                FileSystem,
                Environment,
                ProcessRunner,
                Tools);
            tool.UploadFile(FilePath, Host, Settings);
        }
    }
}
