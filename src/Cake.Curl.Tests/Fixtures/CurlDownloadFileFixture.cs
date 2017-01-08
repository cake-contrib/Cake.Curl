using System;
using Cake.Testing.Fixtures;

namespace Cake.Curl.Tests.Fixtures
{
    internal sealed class CurlDownloadFileFixture
        : ToolFixture<CurlDownloadSettings>
    {
        public CurlDownloadFileFixture()
            : base("curl")
        {
            Host = new Uri("protocol://host/path");
        }

        public Uri Host { get; set; }

        protected override void RunTool()
        {
            var tool = new CurlDownloadRunner(
                FileSystem,
                Environment,
                ProcessRunner,
                Tools);
            tool.DownloadFile(Host, Settings);
        }
    }
}
