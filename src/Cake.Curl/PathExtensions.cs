using Cake.Core;
using Cake.Core.IO;

namespace Cake.Curl
{
    internal static class PathExtensions
    {
        internal static string GetAbsolutePath(this FilePath path, ICakeEnvironment environment)
            => path.MakeAbsolute(environment).FullPath;
    }
}
