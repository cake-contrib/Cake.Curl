using Cake.Core.IO.Arguments;

namespace Cake.Curl.Arguments
{
    /// <summary>
    /// Represents an argument that consists of
    /// a name and a secret value separated by a character.
    /// </summary>
    internal class NameSecretValueArgument : ConcatenatedArgument
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NameSecretValueArgument"/> class.
        /// </summary>
        /// <param name="name">The name part of the argument.</param>
        /// <param name="separator">The separator between the name and the value.</param>
        /// <param name="secretValue">The secret value part of the argument.</param>
        public NameSecretValueArgument(
            string name,
            string separator,
            string secretValue)
            : base(
                new TextArgument(name),
                new TextArgument(separator),
                new SecretArgument(new TextArgument(secretValue)))
        {
        }
    }
}
