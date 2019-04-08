using System;
using System.Collections.Generic;
using Cake.Core.Tooling;

namespace Cake.Curl
{
    /// <summary>
    /// Contains the settings used by curl.
    /// </summary>
    public class CurlSettings : ToolSettings
    {
        /// <summary>
        /// Gets or sets a value indicating whether to enable verbose output.
        /// </summary>
        public bool Verbose { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to display a simple progress bar
        /// instead of the standard progress meter.
        /// </summary>
        /// <remarks>
        /// By default, curl displays a progress meter during its operations. This meter
        /// contains information such as the amount of transferred data, transfer speed
        /// and the estimated time left. When the <see cref="P:ProgressBar"/> property
        /// is set to <see langword="true"/>, curl displays a single line of <em>#</em>
        /// characters instead. This may be preferable when running the build script
        /// in a terminal where we want to keep the amount of output to a minimum.
        /// </remarks>
        public bool ProgressBar { get; set; }

        /// <summary>
        /// Gets or sets the username to use when authenticating to the remote host.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Gets or sets the password to use when authenticating to the remote host.
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets the header to use when communicating to the remote host.
        /// </summary>
        public IDictionary<string, string> Headers { get; set; }

        /// <summary>
        /// Gets or sets the command to use in the request.
        /// </summary>
        /// <remarks>
        /// The set of commands that are considered valid for a particular request
        /// depends on the protocol used to talk to the remote service.
        /// For an HTTP request any valid HTTP method—such as <code>PUT</code>, <code>GET</code>,
        /// <code>POST</code> and <code>DELETE</code>—will do. Other protocols might have
        /// their own set of commands; for example, FTP has <code>LIST</code>
        /// while WebDAV supports <code>COPY</code>.
        /// Note that specifying the command using this option will simply set
        /// the word in the request and won't change curl's behavior in any way.
        /// If curl already has a dedicated option to perform a particular operation,
        /// it's almost always better to use that option instead of specifying the command directly,
        /// since that will make sure that curl behaves accordingly.
        /// For example, in order to send an HTTP <code>HEAD</code> request, setting the <code>"HEAD"</code>
        /// command in the request isn't enough. In that case, the dedicated <code>-I --head</code> option
        /// is a far better choice.
        /// </remarks>
        public string RequestCommand { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to follow HTTP redirects.
        /// </summary>
        /// <remarks>
        /// If the remote service responds with a 3xx status code and this flag
        /// is set to <see langword="true"/>, curl will redo the request
        /// to the URL found in the <code>Location</code> response header.
        /// Note that if the remote service responded with a <code>301</code> (<em>Moved Permanently</em>),
        /// <code>302</code> (<em>Found</em>) or <code>303</code> (<em>See Other</em>) status code,
        /// curl will redo the request using the <code>GET</code> method, even if the original request
        /// was using another method (like for example <code>PUT</code> or <code>POST</code>).
        /// For all other 3xx status codes, curl will redo the request using the same method
        /// as the one specified in the original request.
        /// </remarks>
        public bool FollowRedirects { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether HTTP errors should result in a non-zero exit code.
        /// </summary>
        /// <remarks>
        /// When HTTP servers fail to fulfill a request, they will often return an HTML page with the error information to the client.
        /// In situations like these, curl's default behavior is to send the content of the error page to <code>stdout</code> and exit with a zero exit code, indicating success.
        /// While this may be fine at the command line, it's quite problematic in scripts where it can lead to false positives.
        /// Setting this property to <see langword="true"/>, tells curl to treat HTTP errors (4xx and 5xx) as failed operations
        /// and to exit with status code <code>22</code> instead of <code>0</code>.
        /// Cake will then pick up the non-successful status code and turn it into a runtime exception.
        /// Note that, according to <a href="https://curl.haxx.se/docs/manpage.html#-f">curl's documentation</a>, this method is not completely fail-safe
        /// and there are occasions where non-successful response codes (especially those related to authentication, like <code>401</code>
        /// and <code>407</code>) will slip through.
        /// </remarks>
        public bool Fail { get; set; }

        /// <summary>
        /// Gets or sets the number of times curl should retry a failed operation before giving up.
        /// Setting this property to <em>zero</em> means no retries. This is the default value.
        /// <seealso cref="RetryDelaySeconds"/>
        /// <seealso cref="RetryMaxTimeSeconds"/>
        /// <seealso cref="RetryOnConnectionRefused"/>
        /// </summary>
        /// <remarks>
        /// According to <a href="https://curl.haxx.se/docs/manpage.html#--retry">curl's documentation</a>,
        /// the kinds of errors that qualify for a retry are:
        /// <list type="bullet">
        /// <item>
        /// <description>Timeouts</description>
        /// </item>
        /// <item>
        /// <description>FTP 4xx responses</description>
        /// </item>
        /// <item>
        /// <description>HTTP <code>408</code> or 5xx responses</description>
        /// </item>
        /// </list>
        /// This option can be further tweaked with <see cref="RetryDelaySeconds"/> and <see cref="RetryMaxTimeSeconds"/>.
        /// </remarks>
        public uint RetryCount { get; set; }

        /// <summary>
        /// Gets or sets the amount of time curl should wait before retrying a failed operation.
        /// Setting this property to <em>zero</em> means using curl's own exponential wait algorithm. This is the default value.
        /// <seealso cref="RetryCount"/>
        /// </summary>
        /// <remarks>
        /// According to <a href="https://curl.haxx.se/docs/manpage.html#--retry">the documentation</a>, curl's default wait
        /// algorithm is to wait <em>one</em> second after the first failure and then double the previous waiting time
        /// for all subsequent retries until it reaches <em>ten</em> minutes; after that, <em>ten</em> minutes is going to be
        /// used for all retries until the operation either succeeds, is cancelled by the user or the <see cref="RetryMaxTimeSeconds"/>
        /// is reached.
        /// </remarks>
        public uint RetryDelaySeconds { get; set; }

        /// <summary>
        /// Gets or sets the maximum amount of time during which curl should retry a failed operation.
        /// Setting this property to <em>zero</em> means retrying without a time limit. This is the default value.
        /// <seealso cref="RetryCount"/>
        /// <seealso cref="MaxTime"/>
        /// </summary>
        public uint RetryMaxTimeSeconds { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether curl should consider <code>ECONNREFUSED</code> as a failed operation
        /// before retrying. The default value is <see langword="false"/>.
        /// <seealso cref="RetryCount"/>
        /// </summary>
        public bool RetryOnConnectionRefused { get; set; }

        /// <summary>
        /// Gets or sets the maximum time in seconds that you allow the whole operation to take.
        /// </summary>
        /// <remarks>
        /// This is useful for preventing your batch jobs from hanging for hours due to slow networks or links going down.
        /// Since curl version 7.32.0, this option accepts decimal values, but the actual timeout will decrease in accuracy as the specified timeout increases in decimal precision.
        /// See also <a href="https://curl.haxx.se/docs/manpage.html#--connect-timeout">--connect-timeout</a>.
        /// </remarks>
        public TimeSpan? MaxTime { get; set; }

        /// <summary>
        /// Gets or sets the maximum time in seconds that you allow curl's connection to take.
        /// </summary>
        /// <remarks>
        /// This only limits the connection phase, so if curl connects within the given period it will continue - if not it will exit.
        /// Since curl version 7.32.0, this option accepts decimal values.
        /// See also <a href="https://curl.haxx.se/docs/manpage.html#-m">-m, --max-time</a>.
        /// </remarks>
        public TimeSpan? ConnectionTimeout { get; set; }
    }
}
