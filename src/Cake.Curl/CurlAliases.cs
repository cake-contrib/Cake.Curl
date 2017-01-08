using System;
using System.Collections.Generic;
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
        /// Downloads the file from the specified remote URL to the working directory.
        /// </summary>
        /// <param name="context">The Cake context.</param>
        /// <param name="host">The URL of the file to download.</param>
        /// <example>
        /// <code>
        /// CurlDownloadFile(new Uri("http://host/file"));
        /// </code>
        /// </example>
        [CakeMethodAlias]
        public static void CurlDownloadFile(
            this ICakeContext context,
            Uri host)
        {
            CurlDownloadFile(context, host, new CurlDownloadSettings());
        }

        /// <summary>
        /// Downloads the file from the specified remote URL.
        /// </summary>
        /// <remarks>
        /// By default, curl is going to download the file to the working directory
        /// using the same name as on the remote host.
        /// If you want to put the file into a different directory, you can set the
        /// <see cref="P:CurlDownloadSettings.WorkingDirectory"/> property accordingly.
        /// If you want to rename the downloaded file, you can do so by specifying the
        /// output path for the file at the remote URL in the
        /// <see cref="P:CurlDownloadSettings.OutputPaths"/> property.
        /// </remarks>
        /// <param name="context">The Cake context.</param>
        /// <param name="host">The URL of the file to download.</param>
        /// <param name="settings">The settings.</param>
        /// <example>
        /// <code>
        /// CurlDownloadFile(new Uri("http://host/file"), new CurlDownloadSettings
        /// {
        ///     OutputPaths = new FilePath[] { "output/path" }
        /// });
        /// </code>
        /// </example>
        [CakeMethodAlias]
        public static void CurlDownloadFile(
            this ICakeContext context,
            Uri host,
            CurlDownloadSettings settings)
        {
            var runner = new CurlDownloadRunner(
                context.FileSystem,
                context.Environment,
                context.ProcessRunner,
                context.Tools);
            runner.DownloadFile(host, settings);
        }

        /// <summary>
        /// Downloads the files from the specified remote URLs to the working directory.
        /// </summary>
        /// <param name="context">The Cake context.</param>
        /// <param name="hosts">The URLs of the files to download.</param>
        /// <example>
        /// <code>
        /// CurlDownloadFiles(new Uri[]
        /// {
        ///     new Uri("http://host/file"),
        ///     new Uri("http://host/anotherfile")
        /// });
        /// </code>
        /// </example>
        [CakeMethodAlias]
        public static void CurlDownloadFiles(
            this ICakeContext context,
            IEnumerable<Uri> hosts)
        {
            CurlDownloadFiles(context, hosts, new CurlDownloadSettings());
        }

        /// <summary>
        /// Downloads the files from the specified remote URLs.
        /// </summary>
        /// <remarks>
        /// By default, curl is going to download the files to the working directory
        /// using the same names as on the remote host.
        /// If you want to put the files into a different directory, you can set the
        /// <see cref="P:CurlDownloadSettings.WorkingDirectory"/> property accordingly.
        /// If you want to rename the downloaded files, you can do so by specifying the
        /// output paths for each of the files at the remote URLs in the
        /// <see cref="P:CurlDownloadSettings.OutputPaths"/> property.
        /// In that case, the order of the output paths is going to be matched with
        /// the order of the remote URLs.
        /// </remarks>
        /// <param name="context">The Cake context.</param>
        /// <param name="hosts">The URLs of the files to download.</param>
        /// <param name="settings">The settings.</param>
        /// <example>
        /// <code>
        /// CurlDownloadFiles(
        ///     new Uri[]
        ///     {
        ///         new Uri("http://host/file"),
        ///         new Uri("http://host/anotherfile")
        ///     },
        ///     new CurlDownloadSettings
        ///     {
        ///         OutputPaths = new FilePath[]
        ///         {
        ///             "output/path",
        ///             "another/output/path"
        ///         }
        ///     });
        /// </code>
        /// </example>
        [CakeMethodAlias]
        public static void CurlDownloadFiles(
            this ICakeContext context,
            IEnumerable<Uri> hosts,
            CurlDownloadSettings settings)
        {
            var runner = new CurlDownloadRunner(
                context.FileSystem,
                context.Environment,
                context.ProcessRunner,
                context.Tools);
            runner.DownloadFiles(hosts, settings);
        }

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
            var runner = new CurlUploadRunner(
                context.FileSystem,
                context.Environment,
                context.ProcessRunner,
                context.Tools);
            runner.UploadFile(filePath, host, settings);
        }
    }
}
