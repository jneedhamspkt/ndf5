using System;
using ndf5.Streams;
using ndf5.Metadata;
namespace ndf5.Infrastructure.BTrees
{
    /// <summary>
    /// Base Class for btrees in this libarary
    /// </summary>
    public abstract class BTree
    {
        private readonly IStreamProvider 
            mrStreamProvider;

        private readonly ISuperBlock
            mrSuperBlock;

        private readonly IStreamSpaceAllocator
            mrStreamSpaceAllocator;

        protected BTree(
            IStreamProvider aStreamProvider,
            ISuperBlock aSuperBlock,
            IStreamSpaceAllocator aAllocator)
        {
            mrStreamProvider = aStreamProvider;
            mrSuperBlock = aSuperBlock;
            mrStreamSpaceAllocator = aAllocator;
        }

        internal protected long Allocate(long aBytes) => 
            mrStreamSpaceAllocator.Allocate(aBytes);

        internal protected void Free(long Address) => 
            mrStreamSpaceAllocator.Free(Address);

        internal protected Hdf5Reader GetReader() => new Hdf5Reader(
            mrStreamProvider.GetReadonlyStream(),
            mrSuperBlock);

        internal protected Hdf5Reader GetWriter() => new Hdf5Writer(
            mrStreamProvider.GetStream(new StreamRequestArguments(true)),
            mrSuperBlock);
        
    }
}
