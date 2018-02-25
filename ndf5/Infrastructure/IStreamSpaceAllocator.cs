using System;
namespace ndf5.Infrastructure
{
    /// <summary>
    /// Interface for Geting and freeing memory in a stream
    /// </summary>
    public interface IStreamSpaceAllocator
    {
        /// <summary>
        /// Allocate the specified aBytes.
        /// </summary>
        /// <returns>The allocated address of the bytes</returns>
        /// <param name="aBytes">Number of bytes requested</param>
        long Allocate(long aBytes);

        /// <summary>
        /// Free the specified Address.
        /// </summary>
        /// <param name="Address">Address of the memory to free</param>
        void Free(long Address);
    }
}
