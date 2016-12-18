using System;
using Cake.Core;
using Cake.Core.Annotations;
using Cake.Core.IO;

namespace Cake.Curl
{
    /// <summary>
    /// <para>
    /// Contains aliases related to <see href="https://curl.haxx.se">curl</see>.
    /// </para>
    /// <para>
    /// In order to use the commands for this add-in, a version of curl will need
    /// to be installed on the machine where the Cake script is being executed.
    /// While curl is usually included in most Unix-based operating systems, you
    /// will likely have to install it yourself on Windows.
    /// The Windows version of curl is also part of <see href="https://www.cygwin.com">Cygwin</see>.
    /// </para>
    /// <code>
    /// #addin Cake.Curl
    /// </code>
    /// </summary>
    [CakeAliasCategory("Curl")]
    public static class CurlAliases
    {
        /// <summary>
        /// Uploads the specified file to a remote host using the specified URL.
        /// </summary>
        /// <param name="context">The Cake context.</param>
        /// <param name="filePath">The path of the file to upload.</param>
        /// <param name="host">The URL of the host to upload the file to.</param>
        /// <example>
        /// <code>
        /// CurlUploadFile("file.txt", new Uri("http://host/path"));
        /// </code>
        /// </example>
        [CakeMethodAlias]
        public static void CurlUploadFile(
            this ICakeContext context,
            FilePath filePath,
            Uri host)
        {
            CurlUploadFile(context, filePath, host, new CurlSettings());
        }

        /// <summary>
        /// Uploads the specified file to a remote host using the specified URL.
        /// </summary>
        /// <param name="context">The Cake context.</param>
        /// <param name="filePath">The path of the file to upload.</param>
        /// <param name="host">The URL of the host to upload the file to.</param>
        /// <param name="settings">The settings.</param>
        /// <example>
        /// <code>
        /// CurlUploadFile("file.txt", new Uri("ftps://host/path"), new CurlSettings
        /// {
        ///     Username = "username",
        ///     Password = "password"
        /// });
        /// </code>
        /// </example>
        [CakeMethodAlias]
        public static void CurlUploadFile(
            this ICakeContext context,
            FilePath filePath,
            Uri host,
            CurlSettings settings)
        {
            var runner = new CurlRunner(
                context.FileSystem,
                context.Environment,
                context.ProcessRunner,
                context.Tools);
            runner.UploadFile(filePath, host, settings);
        }
    }
}
