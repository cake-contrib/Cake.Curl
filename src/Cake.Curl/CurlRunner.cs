using System;
using System.Collections.Generic;
using Cake.Core;
using Cake.Core.IO;
using Cake.Core.Tooling;

namespace Cake.Curl
{
    /// <summary>
    /// The curl runner.
    /// </summary>
    public sealed class CurlRunner : Tool<CurlSettings>
    {
        private readonly ICakeEnvironment _environment;

        /// <summary>
        /// Initializes a new instance of the <see cref="CurlRunner"/> class.
        /// </summary>
        /// <param name="fileSystem">The file system.</param>
        /// <param name="environment">The environment.</param>
        /// <param name="processRunner">The process runner.</param>
        /// <param name="tools">The tool locator.</param>
        public CurlRunner(
            IFileSystem fileSystem,
            ICakeEnvironment environment,
            IProcessRunner processRunner,
            IToolLocator tools)
            : base(fileSystem, environment, processRunner, tools)
        {
            _environment = environment;
        }

        /// <summary>
        /// Uploads a file to a remote host using the specified URL.
        /// </summary>
        /// <param name="filePath">The path to the file to upload.</param>
        /// <param name="host">The URL to the remote host.</param>
        /// <param name="settings">The settings.</param>
        public void UploadFile(
            FilePath filePath,
            Uri host,
            CurlSettings settings)
        {
            if (filePath == null)
            {
                throw new ArgumentNullException(nameof(filePath));
            }

            if (host == null)
            {
                throw new ArgumentNullException(nameof(host));
            }

            if (settings == null)
            {
                throw new ArgumentNullException(nameof(settings));
            }

            Run(settings, GetUploadArguments(filePath, host, settings));
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

        private ProcessArgumentBuilder GetUploadArguments(
            FilePath filePath,
            Uri host,
            CurlSettings settings)
        {
            var arguments = new ProcessArgumentBuilder();

            arguments.Append("--upload-file");
            arguments.AppendQuoted(filePath.FullPath);

            arguments.Append("--url");
            arguments.Append(host.AbsoluteUri);

            if (settings.Username != null)
            {
                arguments.Append("--user");
                arguments.AppendQuoted($"{settings.Username}:{settings.Password}");
            }

            return arguments;
        }
    }
}
