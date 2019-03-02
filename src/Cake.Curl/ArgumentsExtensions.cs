using Cake.Core;
using Cake.Core.IO;
using static System.FormattableString;

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

            if (settings.Headers != null)
            {
                foreach (var item in settings.Headers)
                {
                    arguments.AppendSwitchQuoted(
                        "--header",
                        $"{item.Key}:{item.Value}");
                }
            }

            if (settings.RequestCommand != null)
            {
                arguments.AppendSwitchQuoted(
                    "--request",
                    settings.RequestCommand.ToUpperInvariant());
            }

            if (settings.FollowRedirects)
            {
                arguments.Append("--location");
            }

            if (settings.Fail)
            {
                arguments.Append("--fail");
            }

            if (settings.Retry > 0)
            {
                arguments.AppendSwitch("--retry", Invariant($"{settings.Retry}"));
            }

            if (settings.RetryDelay > 0)
            {
                arguments.AppendSwitch("--retry-delay", Invariant($"{settings.RetryDelay}"));
            }

            if (settings.RetryMaxTime > 0)
            {
                arguments.AppendSwitch("--retry-max-time", Invariant($"{settings.RetryMaxTime}"));
            }

            if (settings.RetryConnRefused)
            {
                arguments.Append("--retry-connrefused");
            }
        }
    }
}
