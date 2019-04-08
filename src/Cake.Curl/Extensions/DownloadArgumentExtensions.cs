using System;
using System.Collections.Generic;
using System.Linq;
using Cake.Core;
using Cake.Core.IO;

namespace Cake.Curl.Extensions
{
    internal static class DownloadArgumentExtensions
    {
        internal static void AppendDownloadToCurrentDirectory(
            this ProcessArgumentBuilder arguments,
            IEnumerable<Uri> hosts)
        {
            foreach (var host in hosts)
            {
                arguments.AppendSwitch("-O", host.AbsoluteUri);
            }
        }

        internal static void AppendDownloadToSpecificPaths(
            this ProcessArgumentBuilder arguments,
            IEnumerable<Uri> hosts,
            IEnumerable<string> filePaths)
        {
            foreach (var arg in JoinHostsAndOutputPaths(hosts, filePaths))
            {
                arguments.Append(arg);
            }
        }

        private static IEnumerable<string> JoinHostsAndOutputPaths(
            IEnumerable<Uri> hosts,
            IEnumerable<string> filePaths)
        {
            return hosts.Zip(
                filePaths,
                (host, filePath) => $"-o \"{filePath}\" {host.AbsoluteUri}");
        }
    }
}
