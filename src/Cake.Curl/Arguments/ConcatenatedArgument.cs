using System.Collections.Generic;
using System.Linq;
using Cake.Core.IO;

namespace Cake.Curl.Arguments
{
    /// <summary>
    /// Represents a composite argument made up of
    /// a concatenated sequence of arguments.
    /// </summary>
    internal class ConcatenatedArgument : IProcessArgument
    {
        private readonly IEnumerable<IProcessArgument> _arguments;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConcatenatedArgument"/> class.
        /// </summary>
        /// <param name="arguments">The sequence of arguments to concatenate.</param>
        public ConcatenatedArgument(params IProcessArgument[] arguments)
        {
            _arguments = arguments;
        }

        /// <inheritdoc />
        public string Render()
        {
            return string.Concat(_arguments.Select(arg => arg.Render()));
        }

        /// <inheritdoc />
        public string RenderSafe()
        {
            return string.Concat(_arguments.Select(arg => arg.RenderSafe()));
        }
    }
}
