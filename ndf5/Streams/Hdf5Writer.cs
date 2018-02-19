using System;
using System.IO;
using ndf5.Metadata;

namespace ndf5.Streams
{

    /// <summary>
    /// Hdf5 writer writes AND reads to HDF file streams
    /// </summary>
    internal class Hdf5Writer : Hdf5Reader
    {
        private readonly IStreamResizer
            mrStreamResizer;

        public Hdf5Writer(
            Stream aBaseStream,
            ISuperBlock aSuperBlock,
            IStreamResizer aStreamResizer) : base(aBaseStream, aSuperBlock)
        {
            if (!ContainedStream.CanWrite)
                throw new ArgumentException("Cannot Write to stream");
            mrStreamResizer = aStreamResizer;
        }

        public override bool CanWrite => true;

        private void GrowIfNeeded(
            int aBytes)
        {
            if(this.Position + aBytes >= this.Length)
                SetLength(this.Position + aBytes + 1);   
        }

        public override void WriteByte(byte value)
        {
            GrowIfNeeded(1);
            ContainedStream.WriteByte(value);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            GrowIfNeeded(count);
            ContainedStream.Write(buffer, offset, count);
        }

        public override System.Threading.Tasks.Task WriteAsync(byte[] buffer, int offset, int count, System.Threading.CancellationToken cancellationToken)
        {
            GrowIfNeeded(count);
            return ContainedStream.WriteAsync(
                buffer,
                offset,
                count,
                cancellationToken);
        }

        public override void SetLength(long value)
        {
            base.SetLength(value + mrSuperBlock.BaseAddress);
        }



    }
}
