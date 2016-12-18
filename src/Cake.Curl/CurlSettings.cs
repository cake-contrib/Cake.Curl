using Cake.Core.Tooling;

namespace Cake.Curl
{
    /// <summary>
    /// Contains the settings used by curl.
    /// </summary>
    public sealed class CurlSettings : ToolSettings
    {
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
