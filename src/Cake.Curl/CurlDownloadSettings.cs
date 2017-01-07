using System.Collections.Generic;
using Cake.Core.IO;

namespace Cake.Curl
{
    /// <summary>
    /// Contains the settings used by curl when downloading
    /// one or more files from remote URLs.
    /// </summary>
    public sealed class CurlDownloadSettings : CurlSettings
    {
        /// <summary>
        /// Gets or sets the sequence of local <see cref="FilePath"/>
        /// where to save the downloaded files.
        /// </summary>
        /// <remarks>
        /// When downloading multiple files, curl will match the order in which
        /// the remote URLs are specified with the order of the paths in this property.
        /// </remarks>
        public IEnumerable<FilePath> OutputPaths { get; set; }
    }
}
