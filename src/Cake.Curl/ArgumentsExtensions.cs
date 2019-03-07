using Cake.Core;
using Cake.Core.IO;
using Cake.Core.IO.Arguments;
using Cake.Curl.Arguments;

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
                    new NameValueArgument(
                        settings.Username,
                        ":",
                        new SecretArgument(new TextArgument(settings.Password))));
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
        }
    }
}
