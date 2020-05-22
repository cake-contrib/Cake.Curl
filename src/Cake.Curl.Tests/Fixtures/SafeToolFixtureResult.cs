using Cake.Core.Diagnostics;
using Cake.Core.IO;
using Cake.Testing.Fixtures;

namespace Cake.Curl.Tests.Fixtures
{
    /// <summary>
    /// Represents a tool fixture result
    /// where the arguments have been rendered safely.
    /// </summary>
    internal class SafeToolFixtureResult : ToolFixtureResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SafeToolFixtureResult"/> class.
        /// </summary>
        /// <param name="path">The tool path.</param>
        /// <param name="process">The process settings.</param>
        public SafeToolFixtureResult(FilePath path, ProcessSettings process)
            : base(path, process)
        {
            SafeArgs = process.Arguments.RenderSafe();
        }

        /// <summary>
        /// Gets the arguments specified in the <see cref="ProcessSettings"/>
        /// rendered in a safe manner.
        /// </summary>
        /// <remarks>
        /// Cake will render the arguments in a safe manner
        /// before sending them to the <see cref="ICakeLog"/>.
        /// This is to avoid having sensitive information like passwords
        /// end up in a log file.
        /// </remarks>
        public string SafeArgs { get; }
    }
}
