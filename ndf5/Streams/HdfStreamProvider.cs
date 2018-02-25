using System;
using ndf5.Metadata;
namespace ndf5.Streams
{
    public class HdfStreamProvider
    {
        private readonly IStreamProvider 
            mrStreamProvider;

        private readonly ISuperBlock
            mrSuperBlock;
        
        public HdfStreamProvider(
            IStreamProvider aStreamProvider, 
            ISuperBlock aSuperBlock)
        {
            mrStreamProvider = aStreamProvider;
            mrSuperBlock = aSuperBlock;
        }

        internal protected Hdf5Reader GetReader() => new Hdf5Reader(
            mrStreamProvider.GetReadonlyStream(),
            mrSuperBlock);

        internal protected Hdf5Reader GetWriter() => new Hdf5Writer(
            mrStreamProvider.GetStream(new StreamRequestArguments(true)),
            mrSuperBlock);
    }
}
