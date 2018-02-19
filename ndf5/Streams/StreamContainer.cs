using System.IO;

namespace ndf5.Streams
{
    /// <summary>
    /// Class that provides access to the source file / stream, but won't allow 
    /// the caller to dispose contained stream
    /// </summary>
    internal abstract class StreamContainer : Stream
    {
        protected Stream
            ContainedStream;

        public StreamContainer(
            Stream aContainedStream)
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

        public override int ReadByte()
        {
            return ContainedStream.ReadByte();
        }

        public override int ReadTimeout
        {
            get
            {
                return ContainedStream.ReadTimeout;
            }
            set
            {
                ContainedStream.ReadTimeout = value;
            }
        }

        public override System.Threading.Tasks.Task<int> ReadAsync(
            byte[] buffer,
            int offset,
            int count,
            System.Threading.CancellationToken cancellationToken)
        {
            return ContainedStream.ReadAsync(buffer, offset, count, cancellationToken);
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return ContainedStream.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            ContainedStream.SetLength(value);
        }



        public override bool CanTimeout
        {
            get
            {
                return ContainedStream.CanTimeout;
            }
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            ContainedStream.Write(buffer, offset, count);
        }

        public override void WriteByte(byte value)
        {
            ContainedStream.WriteByte(value);
        }

        public override int WriteTimeout
        {
            get
            {
                return ContainedStream.WriteTimeout;
            }
            set
            {
                ContainedStream.WriteTimeout = value;
            }
        }

        public override System.Threading.Tasks.Task WriteAsync(
            byte[] buffer,
            int offset,
            int count, 
            System.Threading.CancellationToken cancellationToken)
        {
            return ContainedStream.WriteAsync(buffer, offset, count, cancellationToken);
        }

        public override System.Threading.Tasks.Task CopyToAsync(
            Stream destination,
            int bufferSize,
            System.Threading.CancellationToken cancellationToken)
        {
            return ContainedStream.CopyToAsync(destination, bufferSize, cancellationToken);
        }

        public override System.Threading.Tasks.Task FlushAsync(
            System.Threading.CancellationToken cancellationToken)
        {
            return ContainedStream.FlushAsync(cancellationToken);
        }

        public override string ToString()
        {
            return $"{nameof(StreamContainer)}: {nameof(ContainedStream)} = {ContainedStream}";
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            OnDone(disposing);
        }

        protected abstract void OnDone(bool aDisposed);


    }
}
