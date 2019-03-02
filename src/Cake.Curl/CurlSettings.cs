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
        /// Gets or sets the number of retries if a transient error is returned when curl performs a transfer, it will retry this number of times before giving up.
        /// </summary>
        /// <remarks>
        /// Setting the number to zero makes curl do no retries (which is the default).
        /// Transient error means either: a timeout, an FTP 4xx response code or an HTTP 408 or 5xx response code.
        /// When curl is about to retry a transfer, it will first wait one second and then for all forthcoming retries
        /// it will double the waiting time until it reaches 10 minutes which then will be the delay between the rest of the retries.
        /// By using <a href="https://curl.haxx.se/docs/manpage.html#--retry-delay">--retry-delay</a> you disable this exponential backoff algorithm.
        /// See also <a href="https://curl.haxx.se/docs/manpage.html#--retry-max-time">--retry-max-time</a> to limit the total time allowed for retries.
        /// </remarks>
        public uint RetryCount { get; set; }

        /// <summary>
        /// Gets or sets the amount of time in seconds before each retry when a transfer has failed with a transient error
        /// (it changes the default backoff time algorithm between retries).
        /// </summary>
        /// <remarks>
        /// This option is only interesting if <a href="https://curl.haxx.se/docs/manpage.html#--retry">--retry</a> is also used.
        /// Setting this delay to zero will make curl use the default backoff time.
        /// </remarks>
        public uint RetryDelaySeconds { get; set; }

        /// <summary>
        /// Gets or sets the maximum time in seconds for retries to be done.
        /// </summary>
        /// <remarks>
        /// The retry timer is reset before the first transfer attempt.
        /// Retries will be done as usual (see <a href="https://curl.haxx.se/docs/manpage.html#--retry">--retry</a>) as long as the timer hasn't reached this given limit.
        /// Notice that if the timer hasn't reached the limit, the request will be made and while performing, it may take longer than this given time period.
        /// To limit a single request´s maximum time, use <a href="https://curl.haxx.se/docs/manpage.html#-m">--max-time</a>.
        /// Set this option to zero to not timeout retries.
        /// </remarks>
        public uint RetryMaxTimeSeconds { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether curl should consider <code>ECONNREFUSED</code> (in addition to the other conditions) as a transient error
        /// for <a href="https://curl.haxx.se/docs/manpage.html#--retry">--retry</a>.
        /// </summary>
        /// <remarks>
        /// This option is used together with <a href="https://curl.haxx.se/docs/manpage.html#--retry">--retry</a>.
        /// </remarks>
        public bool RetryOnConnectionRefused { get; set; }
    }
}
