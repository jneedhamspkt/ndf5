using System;
using System.IO;
using System.Threading;

using ndf5.Streams;

namespace ndf5.Streams
{
    /// <summary>
    /// Arbitrates a single stream across multiple request acesses;
    /// </summary>
    internal class SingleStreamProvider : IStreamProvider
    {
        private readonly
            Stream mrSourceStream;

        public SingleStreamProvider(Stream aSourceStream)
        {
            mrSourceStream = aSourceStream;
        }  

        public Stream GetStream(StreamRequestArguments aArguments)
        {
            if (aArguments.AccessTimeout.HasValue)
            {
                TimeSpan
                    fTimeOut = aArguments.AccessTimeout.Value;
                if (!Monitor.TryEnter(mrSourceStream, fTimeOut))
                    throw new TimeoutException($"Could not get stream within {fTimeOut}");
            }
            else
            {
                Monitor.Enter(mrSourceStream);
            }
            return new WrappedStream(mrSourceStream);
        }

        private class WrappedStream : StreamContainer
        {
            private readonly object
                mrLockSource;

            public WrappedStream(
                Stream aContainedStream) : base(aContainedStream)
            {
                mrLockSource = aContainedStream;
            }

            protected override void OnDone(bool aDisposed)
            {
                Monitor.Exit(mrLockSource);
            }
        }
    }
}
 