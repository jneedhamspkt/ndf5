using System;
namespace ndf5.Infrastructure
{
    /// <summary>
    /// Interface for Geting and freeing memory in a stream
    /// </summary>
    public interface IHeap
    {
        /// <summary>
        /// Absoulte address where objects in this heap reference from
        /// </summary>
        /// <value>The base address.</value>
        long BaseAddress { get; }

        /// <summary>
        /// The Current size in bytes of this heap
        /// </summary>
        /// <value>The size.</value>
        long Size { get; }

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
