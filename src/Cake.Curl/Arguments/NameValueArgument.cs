using Cake.Core.IO;
using Cake.Core.IO.Arguments;

namespace Cake.Curl.Arguments
{
    /// <summary>
    /// Represents an argument that consists of
    /// a name and a value separated by a character.
    /// </summary>
    internal class NameValueArgument : ConcatenatedArgument
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NameValueArgument"/> class.
        /// </summary>
        /// <param name="name">The name part of the argument.</param>
        /// <param name="separator">The separator between the name and the value.</param>
        /// <param name="value">The textual value part of the argument.</param>
        public NameValueArgument(string name, string separator, string value)
            : this(name, separator, new TextArgument(value))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NameValueArgument"/> class.
        /// </summary>
        /// <param name="name">The name part of the argument.</param>
        /// <param name="separator">The separator between the name and the value.</param>
        /// <param name="value">The value part of the argument.</param>
        public NameValueArgument(string name, string separator, IProcessArgument value)
            : base(new TextArgument(name), new TextArgument(separator), value)
        {
        }
    }
}
