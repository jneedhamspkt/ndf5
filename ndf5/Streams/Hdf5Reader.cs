using System;
using System.IO;
using ndf5.Metadata;

namespace ndf5.Streams
{
    internal class Hdf5Reader : StreamContainer
    {
        internal protected readonly ISuperBlock
            mrSuperBlock;


        public Hdf5Reader(
            Stream aBaseStream, 
            ISuperBlock aSuperBlock) : base(aBaseStream)
        {
            mrSuperBlock = aSuperBlock;
        }

        public override bool CanWrite => false;

        public override long Length => mrSuperBlock.EndOfFileAddress - 1 - mrSuperBlock.BaseAddress;

        public override long Position {
            get => ContainedStream.Position - mrSuperBlock.BaseAddress;
            set => ContainedStream.Position = mrSuperBlock.BaseAddress + value; }


        public override long Seek(long offset, SeekOrigin origin)
        {
            switch (origin)
            {
                case SeekOrigin.Begin:
                    return ContainedStream.Seek(
                        offset + mrSuperBlock.BaseAddress,
                        SeekOrigin.Begin);
                case SeekOrigin.Current:
                    return ContainedStream.Seek(offset, SeekOrigin.Current);
                case SeekOrigin.End:
                    return ContainedStream.Seek(
                        offset + mrSuperBlock.EndOfFileAddress,
                        SeekOrigin.Begin);
                default: throw new InvalidOperationException();
            }
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException($"{nameof(Hdf5Reader)} is read only)");
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException($"{nameof(Hdf5Reader)} is read only)");
        }

        protected override void OnDone(bool aDisposed)
        {
            //Nothing to do
        }
    }
}
