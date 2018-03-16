using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;

using ndf5.Metadata;

namespace ndf5.Streams
{
    public class Hdf5Reader : IDisposable
    {
        internal protected readonly ISuperBlock
            SuperBlock;

        internal protected readonly Stream
            Source;


        private readonly bool
            mrManageStreamLifetime;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:ndf5.Streams.Hdf5Reader"/> class.
        /// </summary>
        /// <param name="aBaseStream">Stream to read from</param>
        /// <param name="aSuperBlock">A super block.</param>
        /// <param name="aManageStreamLifetime">If set to <c>true</c> Disposing this will dispos <c>aBaseStream</c>.</param>
        public Hdf5Reader(
            Stream aBaseStream,
            ISuperBlock aSuperBlock,
            bool aManageStreamLifetime = true)
        {
            if (!aBaseStream.CanRead)
                throw new ArgumentException("Cannot read the stream");
            Source = aBaseStream;
            SuperBlock = aSuperBlock;
            mrManageStreamLifetime = aManageStreamLifetime;
        }

        ~Hdf5Reader()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            Dispose(true);
        }

        internal protected void Dispose(bool aIsDisposing)
        {
            if (!aIsDisposing)
                return;
            if(mrManageStreamLifetime)
                Source.Dispose();
        }

        public virtual Length Length => 
            new Length((ulong)(SuperBlock.EndOfFileAddress - new Length(1) - SuperBlock.BaseAddress));

        public virtual Offset Position 
        {
            get => new Offset((ulong)Source.Position) - SuperBlock.BaseAddress;
            set => Seek(value); 
        }

        public virtual Offset Seek(Offset aOffset)
        {
            ulong 
                fOffsetAddr = (ulong)(SuperBlock.BaseAddress + aOffset);
            return new Offset((ulong)Source.Seek(
                (long) fOffsetAddr,
                SeekOrigin.Begin));
        }

        public virtual long Seek(long aOffset, SeekOrigin origin)
        {
            ulong
                fOffsetAddr;
            switch (origin)
            {
                case SeekOrigin.Begin:
                    fOffsetAddr = (ulong)(SuperBlock.BaseAddress + new Length((ulong)aOffset));
                    return Source.Seek(
                        (long)fOffsetAddr,
                        SeekOrigin.Begin);
                case SeekOrigin.Current:
                    return Source.Seek(aOffset, SeekOrigin.Current);
                case SeekOrigin.End:
                    fOffsetAddr = (ulong)(SuperBlock.EndOfFileAddress + new Length((ulong)aOffset));
                    return Source.Seek(
                        (long)fOffsetAddr,
                        SeekOrigin.Begin);
                default: throw new InvalidOperationException();
            }
        }

        /// <summary>
        /// Read the stream and write it to thee specified buffer, offset and count.
        /// </summary>
        /// <returns>The read.</returns>
        /// <param name="buffer">Buffer.</param>
        /// <param name="offset">Offset.</param>
        /// <param name="count">Count.</param>
        internal int Read(byte[] buffer, int offset, int count)
        {   
            return Source.Read(buffer, offset, count);
        }

        /// <summary>
        /// Read an identical number of bytes as are in <c>aSignature</c>
        /// to verify identical data;
        /// </summary>
        /// <param name="aSignature">Signature to look for.</param>
        internal void VerifySignature(
            IReadOnlyList<byte> aSignature)
        {
            int
                fBytes = aSignature.Count;
            byte[]
                fSource = new byte[fBytes];
            if (fBytes != Source.Read(fSource, 0, fBytes))
                throw new EndOfStreamException();
            for (int i = 0; i < fBytes; ++i)
            {
                if (fSource[i] != aSignature[i])
                {
                    throw new InvalidDataException($"Expected: {String.Join(", ", aSignature)} "
                                                  + $"but got: {String.Join(", ", fBytes)} ");
                }
            }
        }

        public long SizeOfLegths => SuperBlock.SizeOfLengths;

        public long SizeOfOffset => SuperBlock.SizeOfOffsets;

        public Offset ReadOffset() 
        {
            return ReadFeild<Offset>(SuperBlock.SizeOfOffsets, a => new Offset(a));
        }

        public Length ReadLength()
        {
            return ReadFeild<Length>(SuperBlock.SizeOfLengths, a => new Length(a)) ;
        }

        private tFeild ReadFeild<tFeild>(
            byte aSize,
            Func<ulong,tFeild> aConstruct) where tFeild : UnsignedNumber
        {
            byte[]
                fBuffer = new byte[aSize];
            Source.Read(fBuffer, 0, aSize);
            switch (aSize)
            {
                case 2:
                    ushort
                        fShort = (ushort)(
                            (fBuffer[0]) +
                            (fBuffer[1] << 8));
                    if (fShort == ushort.MaxValue)
                        return null;
                    return aConstruct(fShort);
                case 4:
                    uint fUint = (uint)(
                            (fBuffer[0]) + 
                            (fBuffer[1] << 8) +
                            (fBuffer[2] << 16) + 
                            (fBuffer[3] << 24));
                    if (fUint == uint.MaxValue)
                        return null;
                    return aConstruct(fUint);
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
                    return aConstruct(fUlong);
                default:
                    throw new Exception("Unsupported Size");
            }
        }

        internal byte ReadByte()
        {
            return (byte) Source.ReadByte();
        }

        /// <summary>
        /// Reads the a Little Endian 16 bit integer
        /// </summary>
        /// <returns>The read 16 bit number</returns>
        public ushort ReadUInt16()
        {
            byte[]
                fBuffer = new byte[2];
            Source.Read(fBuffer, 0, 2);
            ushort
                fShort = (ushort)(
                    (fBuffer[0]) +
                    (fBuffer[1] << 8));
            return fShort;
        }

        /// <summary>
        /// Reads the a Little Endian 32 bit integer
        /// </summary>
        /// <returns>The read 32 bit number</returns>
        public uint ReadUInt32()
        {
            byte[]
                fBuffer = new byte[4];
            Source.Read(fBuffer, 0, 4);
            uint fUint = (uint)(
                (fBuffer[0]) +
                (fBuffer[1] << 8) +
                (fBuffer[2] << 16) +
                (fBuffer[3] << 24));
            return fUint;
        }

        /// <summary>
        /// Reads the a Little Endian 64 bit integer
        /// </summary>
        /// <returns>The read 64 bit number</returns>
        public ulong ReadUInt64()
        {
            byte[]
                fBuffer = new byte[8];
            Source.Read(fBuffer, 0, 8);
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
            return fUlong;
        }



    }
}
