using System.Collections.Generic;
 using System.Linq;
 using Cake.Core;
using Cake.Core.IO;

namespace Cake.Curl
{
    internal static class PathExtensions
    {
        internal static IEnumerable<string> GetAbsolutePaths(
            this IEnumerable<FilePath> paths,
            ICakeEnvironment environment)
            => paths.Select(p => p.GetAbsolutePath(environment));

        internal static string GetAbsolutePath(this FilePath path, ICakeEnvironment environment)
            => path.MakeAbsolute(environment).FullPath;
    }
}
