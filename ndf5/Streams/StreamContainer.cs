using System.IO;

namespace ndf5.Streams
{
    /// <summary>
    /// Class that provides access to the source file / stream, but won't allow 
    /// the caller to dispose contained stream
    /// </summary>
    internal abstract class StreamContainer : Stream
    {
        private Stream
            ContainedStream;

        public StreamContainer(Stream aContainedStream)
        {
            ContainedStream = aContainedStream;
        }

        public override bool CanRead => ContainedStream.CanRead;

        public override bool CanSeek => ContainedStream.CanSeek;

        public override bool CanWrite => ContainedStream.CanWrite;

        public override long Length => ContainedStream.Length;

        public override long Position
        {
            get => ContainedStream.Position;
            set => ContainedStream.Position = value;
        }

        public override void Flush()
        {
            ContainedStream.Flush();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return ContainedStream.Read(buffer, offset, count);
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return ContainedStream.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            ContainedStream.SetLength(value);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            ContainedStream.Write(buffer, offset, count);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            OnDone(disposing);
        }

        protected abstract void OnDone(bool aDisposed);
    }
}
