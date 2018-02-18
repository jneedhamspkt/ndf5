using System;
using System.IO;

namespace ndf5.Streams
{
    /// <summary>
    /// Extesnions for the <see cref="IStreamProvider"/> interface
    /// </summary>
    internal static class StreamProvider
    {
        /// <summary>
        /// Gets a readonly stream.  Provided stream should be disposed 
        /// as soon as possible.
        /// </summary>
        /// <returns>A temporary, readonly stream.</returns>
        /// <param name="aProvider">A stream provider.</param>
        /// <param name="aTimeout">An access timeout (optional).</param>
        internal static Stream GetReadonlyStream(
            this IStreamProvider aProvider,
            TimeSpan? aTimeout = null)
        {
            return aProvider.GetStream(new StreamRequestArguments(
                aWriteDesired: false,
                aAccessTimeout: aTimeout));
        }
    }
}
