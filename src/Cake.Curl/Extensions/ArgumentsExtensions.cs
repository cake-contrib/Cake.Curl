using System.Globalization;
using Cake.Core;
using Cake.Core.IO;
using Cake.Curl.Arguments;

namespace Cake.Curl.Extensions
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
                    new NameSecretValueArgument(settings.Username, ":", settings.Password));
            }

            if (settings.Headers != null)
            {
                foreach (var item in settings.Headers)
                {
                    arguments.AppendSwitchQuoted(
                        "--header",
                        new NameValueArgument(item.Key, ":", item.Value));
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

            if (settings.RetryCount > 0)
            {
                arguments.AppendSwitch("--retry", settings.RetryCount.ToString());
            }

            if (settings.RetryDelaySeconds > 0)
            {
                arguments.AppendSwitch("--retry-delay", settings.RetryDelaySeconds.ToString());
            }

            if (settings.RetryMaxTimeSeconds > 0)
            {
                arguments.AppendSwitch("--retry-max-time", settings.RetryMaxTimeSeconds.ToString());
            }

            if (settings.RetryOnConnectionRefused)
            {
                arguments.Append("--retry-connrefused");
            }

            if (settings.MaxTime.HasValue)
            {
                arguments.AppendSwitch(
                    "--max-time",
                    settings.MaxTime.Value.TotalSeconds.ToString(CultureInfo.CurrentCulture));
            }

            if (settings.ConnectionTimeout.HasValue)
            {
                arguments.AppendSwitch(
                    "--connect-timeout",
                    settings.ConnectionTimeout.Value.TotalSeconds.ToString(CultureInfo.CurrentCulture));
            }
        }
    }
}
