using System;
using System.Collections.Generic;
using Cake.Core;
using Cake.Core.IO;
using Cake.Core.Tooling;

namespace Cake.Curl
{
    /// <summary>
    /// The curl runner to upload files to a remote URL.
    /// </summary>
    public sealed class CurlUploadRunner : Tool<CurlSettings>
    {
        private readonly ICakeEnvironment _environment;

        /// <summary>
        /// Initializes a new instance of the <see cref="CurlUploadRunner"/> class.
        /// </summary>
        /// <param name="fileSystem">The file system.</param>
        /// <param name="environment">The environment.</param>
        /// <param name="processRunner">The process runner.</param>
        /// <param name="tools">The tool locator.</param>
        public CurlUploadRunner(
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

            Run(settings, GetArguments(filePath, host, settings));
        }

        /// <summary>
        /// Gets the possible names of the tool executable.
        /// </summary>
        /// <returns>The tool executable name.</returns>
        protected override IEnumerable<string> GetToolExecutableNames()
        {
            return new[] { "curl", "curl.exe" };
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
            FilePath filePath,
            Uri host,
            CurlSettings settings)
        {
            var arguments = new ProcessArgumentBuilder();
            arguments.AppendSettings(settings);

            arguments.AppendSwitchQuoted(
                "--upload-file",
                filePath.GetAbsolutePath(_environment));

            arguments.AppendSwitch("--url", host.AbsoluteUri);

            return arguments;
        }
    }
}
