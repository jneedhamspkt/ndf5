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
        private readonly Stream 
            mrSourceStream;

        private readonly StreamInfo
            mrStreamInfo;

        public SingleStreamProvider(
            Stream aSourceStream,
            StreamInfo aStreamInfo)
        {
            mrSourceStream = aSourceStream;
            mrStreamInfo = aStreamInfo;
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
            return new WrappedStream(mrSourceStream, mrStreamInfo);
        }

        private class WrappedStream : StreamContainer
        {
            private readonly object
                mrLockSource;

            public WrappedStream(
                Stream aContainedStream,
                StreamInfo aStreamInfo) : base(aContainedStream, aStreamInfo.Start)
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
 