using Cake.Core;
using Cake.Core.IO;

namespace Cake.Curl
{
    internal static class ArgumentsExtensions
    {
        internal static void AppendSettings(
            this ProcessArgumentBuilder arguments,
            CurlSettings settings)
        {
            if (settings.Verbose)
            {
                arguments.Append("--verbose");
            }

            if (settings.ProgressBar)
            {
                arguments.Append("--progress-bar");
            }

            if (settings.Username != null)
            {
                arguments.AppendSwitchQuoted(
                    "--user",
                    $"{settings.Username}:{settings.Password}");
            }
        }
    }
}
