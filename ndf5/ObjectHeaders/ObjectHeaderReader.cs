using System;
using System.Collections.Generic;
using System.Linq;

using ndf5.Streams;
namespace ndf5.ObjectHeaders
{
    /// <summary>
    /// Implementation of <see cref="IObjectHeaderReader"/> using IHdfStreamProvider 
    /// as the data source
    /// </summary>
    public class ObjectHeaderReader : IObjectHeaderReader
    {
        private IHdfStreamProvider
            mrStreamProvider;

        private static readonly IReadOnlyList<byte>
            msrV2Header = System.Text.Encoding.ASCII.GetBytes("OHDR");

        /// <summary>
        /// Initializes a new instance of the <see cref="T:ndf5.ObjectHeaders.ObjectHeaderReader"/> class.
        /// </summary>
        /// <param name="aStreamProvider">An <see cref="T:ndf5.Streams.IHdfStreamProvider"/> stream provider.</param>
        public ObjectHeaderReader(
            IHdfStreamProvider aStreamProvider)
        {
            mrStreamProvider = aStreamProvider;
        }

        public IObjectHeader ReadHeaderAt(long aHeaderLocation)
        {
            using(Hdf5Reader fReader = mrStreamProvider.GetReader())
            {
                const int
                    fcHeadBytes = 4;
                byte[]
                    fHead = new byte[fcHeadBytes];
                if (fcHeadBytes != fReader.Read(fHead, 0, fcHeadBytes))
                    throw new System.IO.EndOfStreamException(
                        $"Could not read header at 0x{aHeaderLocation:x16}");
                if(fHead[0] == 1 && fHead[1] == 0)
                {
                    return V1ObjectHeader.Read(fHead, fReader);
                }
                if(fHead.Zip(msrV2Header, (arg1, arg2) => arg1 == arg2).All(a=>a))
                {
                    return V2ObjectHeader.Read(fReader);
                }
            }
            throw new Exceptions.CorruptObjectHeader("Could not determine Header version");
        }




    }
}
