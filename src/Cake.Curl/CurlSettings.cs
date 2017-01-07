using Cake.Core.Tooling;

namespace Cake.Curl
{
    /// <summary>
    /// Contains the settings used by curl.
    /// </summary>
    public class CurlSettings : ToolSettings
    {
        /// <summary>
        /// Gets or sets a value indicating whether to enable verbose output.
        /// </summary>
        public bool Verbose { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to display a simple progress bar
        /// instead of the standard progress meter.
        /// </summary>
        /// <remarks>
        /// By default, curl displays a progress meter during its operations. This meter
        /// contains information such as the amount of transferred data, transfer speed
        /// and the estimated time left. When the <see cref="P:ProgressBar"/> property
        /// is set to <see langword="true"/>, curl displays a single line of <em>#</em>
        /// characters instead. This may be preferrable when running the build script
        /// in a terminal where we want to keep the amount of output to a minimum.
        /// </remarks>
        public bool ProgressBar { get; set; }

        /// <summary>
        /// Gets or sets the username to use when authenticating to the remote host.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Gets or sets the password to use when authenticating to the remote host.
        /// </summary>
        public string Password { get; set; }
    }
}
