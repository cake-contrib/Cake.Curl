using System;
using System.Collections.Generic;
using System.Linq;
using Cake.Core;
using Cake.Core.IO;
using Cake.Core.Tooling;

namespace Cake.Curl
{
    /// <summary>
    /// The curl runner to download files from remote URLs.
    /// </summary>
    public sealed class CurlDownloadRunner : Tool<CurlSettings>
    {
        private readonly ICakeEnvironment _environment;

        /// <summary>
        /// Initializes a new instance of the <see cref="CurlDownloadRunner"/> class.
        /// </summary>
        /// <param name="fileSystem">The file system.</param>
        /// <param name="environment">The environment.</param>
        /// <param name="processRunner">The process runner.</param>
        /// <param name="tools">The tool locator.</param>
        public CurlDownloadRunner(
            IFileSystem fileSystem,
            ICakeEnvironment environment,
            IProcessRunner processRunner,
            IToolLocator tools)
            : base(fileSystem, environment, processRunner, tools)
        {
            _environment = environment;
        }

        /// <summary>
        /// Downloads the files from the specified remote URLs.
        /// </summary>
        /// <param name="hosts">The URLs of the files to download.</param>
        /// <param name="settings">The settings.</param>
        public void DownloadFiles(
            IEnumerable<Uri> hosts,
            CurlDownloadSettings settings)
        {
            if (hosts == null)
            {
                throw new ArgumentNullException(nameof(hosts));
            }

            if (!hosts.Any())
            {
                throw new ArgumentException("Hosts cannot be empty", nameof(hosts));
            }

            if (settings == null)
            {
                throw new ArgumentNullException(nameof(settings));
            }

            Run(settings, GetArguments(hosts, settings));
        }

        /// <summary>
        /// Gets the possible names of the tool executable.
        /// </summary>
        /// <returns>The tool executable name.</returns>
        protected override IEnumerable<string> GetToolExecutableNames()
        {
            return new[] { "curl" };
        }

        /// <summary>
        /// Gets the name of the tool.
        /// </summary>
        /// <returns>The name of the tool.</returns>
        protected override string GetToolName()
        {
            return "curl";
        }

        private ProcessArgumentBuilder GetArguments(
            IEnumerable<Uri> hosts,
            CurlDownloadSettings settings)
        {
            var arguments = new ProcessArgumentBuilder();

            if (settings.Username != null)
            {
                arguments.Append("--user");
                arguments.AppendQuoted($"{settings.Username}:{settings.Password}");
            }

            if (settings.OutputPaths != null)
            {
                foreach (var arg in JoinHostsAndOutputPaths(hosts, settings.OutputPaths))
                {
                    arguments.Append(arg);
                }
            }
            else
            {
                foreach (var host in hosts)
                {
                    arguments.AppendSwitch("-O", host.AbsoluteUri);
                }
            }

            return arguments;
        }

        private IEnumerable<string> JoinHostsAndOutputPaths(
            IEnumerable<Uri> hosts,
            IEnumerable<FilePath> paths)
        {
            return hosts.Zip(paths, (host, path) =>
                $"-o \"{path.MakeAbsolute(_environment).FullPath}\" {host.AbsoluteUri}");
        }
    }
}
