using Cake.Core.IO;
using Cake.Core.Tooling;
using Cake.Testing.Fixtures;

namespace Cake.Curl.Tests.Fixtures
{
    /// <summary>
    /// A base class for tool fixtures where the tool's arguments
    /// have been rendered safely.
    /// </summary>
    /// <typeparam name="TToolSettings">
    /// The type used to represent the tool's settings.
    /// </typeparam>
    internal abstract class SafeToolFixture<TToolSettings>
        : ToolFixture<TToolSettings, SafeToolFixtureResult>
        where TToolSettings : ToolSettings, new()
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SafeToolFixture{TToolSettings}"/> class.
        /// </summary>
        /// <param name="toolFilename">The tool file name.</param>
        protected SafeToolFixture(string toolFilename)
            : base(toolFilename)
        {
        }

        /// <inheritdoc />
        protected sealed override SafeToolFixtureResult CreateResult(
            FilePath path,
            ProcessSettings process)
        {
            return new SafeToolFixtureResult(path, process);
        }
    }
}
