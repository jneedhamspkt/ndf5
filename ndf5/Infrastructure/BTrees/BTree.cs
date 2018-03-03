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
        private readonly IHdfStreamProvider 
            mrStreamProvider;

        private readonly IHeap
            mrStreamSpaceAllocator;

        protected BTree(
            IHdfStreamProvider aStreamProvider,
            IHeap aAllocator)
        {
            mrStreamProvider = aStreamProvider;
            mrStreamSpaceAllocator = aAllocator;
        }

        internal protected long Allocate(long aBytes) => 
            mrStreamSpaceAllocator.Allocate(aBytes);

        internal protected void Free(long Address) => 
            mrStreamSpaceAllocator.Free(Address);

        internal protected IHdfStreamProvider
            HdfStreamProvider => mrStreamProvider;
        
    }
}
