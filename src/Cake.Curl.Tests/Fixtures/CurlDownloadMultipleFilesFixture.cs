using System;
using System.Collections.Generic;
using Cake.Testing.Fixtures;

namespace Cake.Curl.Tests.Fixtures
{
    internal sealed class CurlDownloadMultipleFilesFixture
        : ToolFixture<CurlDownloadSettings>
    {
        public CurlDownloadMultipleFilesFixture()
            : base("curl")
        {
            Hosts = new[]
            {
                new Uri("protocol://host/path"),
                new Uri("protocol://anotherhost/path")
            };
        }

        public IEnumerable<Uri> Hosts { get; set; }

        protected override void RunTool()
        {
            var tool = new CurlDownloadRunner(
                FileSystem,
                Environment,
                ProcessRunner,
                Tools);
            tool.DownloadFiles(Hosts, Settings);
        }
    }
}
