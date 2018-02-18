using System.IO;
namespace ndf5.Streams
{
    internal interface IStreamProvider
    {
        /// <summary>
        /// Gets a stream to access the file, Provided stream should be disposed 
        /// as soon as possible
        /// </summary>
        /// <returns>The stream.</returns>
        /// <param name="aArguments">Arguments expressing preferences as to how this should be done </param>
        Stream GetStream(StreamRequestArguments aArguments);
    }
}
