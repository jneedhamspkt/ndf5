using System;
using System.IO;
using System.Threading;

using ndf5.Streams;

namespace ndf5.Streams
{
    /// <summary>
    /// Arbitrates a single stream across multiple request acesses;
    /// </summary>
    public class SimpleSingleStreamProvider : 
        IStreamProvider, IDisposable
    {
        private readonly Stream 
            mrSourceStream;


        public SimpleSingleStreamProvider(
            Stream aSourceStream)
        {
            mrSourceStream = aSourceStream;
        }

        public void Dispose()
        {
            mrSourceStream.Dispose();
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
 