using System;
using System.IO;
using System.Runtime.CompilerServices;

using ndf5.Metadata;

namespace ndf5.Streams
{
    public class Hdf5Reader : StreamContainer
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

        public long? ReadOffset()
        {
            return ReadFeild(mrSuperBlock.SizeOfOffsets);
        }

        public long? ReadLength()
        {
            return ReadFeild(mrSuperBlock.SizeOfLengths);
        }

        private long? ReadFeild(byte aSize)
        {
            byte[]
                fBuffer = new byte[aSize];
            Read(fBuffer, 0, aSize);
            switch (aSize)
            {
                case 2:
                    ushort
                        fShort = (ushort)(
                            (fBuffer[0]) +
                            (fBuffer[1] << 8));
                    if (fShort == ushort.MaxValue)
                        return null;
                    return fShort;
                case 4:
                    uint fUint = (uint)(
                            (fBuffer[0]) + 
                            (fBuffer[1] << 8) +
                            (fBuffer[2] << 16) + 
                            (fBuffer[3] << 24));
                    if (fUint == uint.MaxValue)
                        return null;
                    return fUint;
                case 8:
                    ulong fLow = (ulong)(
                            (fBuffer[0]) |
                            (fBuffer[1] << 8) |
                            (fBuffer[2] << 16) |
                            (fBuffer[3] << 24));
                    ulong fHigh = 
                        ((ulong)fBuffer[4] << 32) |
                        ((ulong)fBuffer[5] << 40) |
                        ((ulong)fBuffer[6] << 48) |
                        ((ulong)fBuffer[7] << 56);
                    ulong
                        fUlong = (fHigh | (uint.MaxValue & fLow));
                    if (fUlong == ulong.MaxValue)
                        return null;
                    if (fUlong > (ulong)long.MaxValue)
                        throw new Exception("Unsupported Value");
                        return (long)fUlong;
                default:
                    throw new Exception("Unsupported Size");
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

        public override void WriteByte(byte value)
        {
            throw new NotSupportedException($"{nameof(Hdf5Reader)} is read only)");
        }

        public override System.Threading.Tasks.Task WriteAsync(byte[] buffer, int offset, int count, System.Threading.CancellationToken cancellationToken)
        {
            throw new NotSupportedException($"{nameof(Hdf5Reader)} is read only)");
        }

        protected override void OnDone(bool aDisposed)
        {
            //Nothing to do
        }
    }
}
