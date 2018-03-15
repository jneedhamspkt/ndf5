using System;
using System.IO;
using ndf5.Metadata;

namespace ndf5.Streams
{

    /// <summary>
    /// Hdf5 writer writes AND reads to HDF file streams
    /// </summary>
    public class Hdf5Writer : Hdf5Reader
    {
        public Hdf5Writer(
            Stream aBaseStream,
            ISuperBlock aSuperBlock) : base(aBaseStream, aSuperBlock)
        {
            if (!Source.CanWrite)
                throw new ArgumentException("Cannot Write to stream");
        }

        private void GrowIfNeeded(
            int aBytes)
        {
            if (this.Position + aBytes >= this.Length)
            {
                SetLength(this.Position + aBytes + 1);
            }
        }

        public void WriteByte(byte value)
        {
            GrowIfNeeded(1);
            Source.WriteByte(value);
        }

        public void Write(byte[] buffer, int offset, int count)
        {
            GrowIfNeeded(count);
            Source.Write(buffer, offset, count);
        }

        public void SetLength(long value)
        {
            Source.SetLength(value + (long)SuperBlock.BaseAddress);
        }



    }
}
