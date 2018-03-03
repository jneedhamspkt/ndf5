using System;
namespace ndf5.Infrastructure.SymbolTable
{
    /// <summary>
    /// The cache type is determined from the object header. It also determines
    /// the format for the scratch-pad space
    /// </summary>
    /// <remarks>
    /// Annotaitons here come from" <see cref="https://support.hdfgroup.org/HDF5/doc/H5.format.html#SymbolTableEntry"/>
    /// </remarks>
    public enum CacheType
    {
        /// <summary>
        /// No data is cached by the group entry. This is guaranteed to be the 
        /// case when an object header has a link count greater than one.
        /// </summary>
        NoCache = 0,
        /// <summary>
        ///  Group object header metadata is cached in the scratch-pad space.
        ///  This implies that the symbol table entry refers to another group.
        /// </summary>
        Cache = 1,
        /// <summary>
        /// The entry is a symbolic link. The first four bytes of the 
        /// scratch-pad space are the offset into the local heap for the link
        /// value. The object header address will be undefined.
        /// </summary>
        SymbolicLink = 2
    }
}
