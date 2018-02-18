using System;
using System.IO;

using ndf5.Streams;

namespace ndf5.Streams
{
    /// <summary>
    /// Arbitrates a single stream across multiple request acesses;
    /// </summary>
    internal class SingleStreamProvider : IStreamProvider
    {
        public SingleStreamProvider()
        {
        }

        public Stream GetStream(StreamRequestArguments aArguments)
        {
            throw new NotImplementedException();
        }
    }
}
 